using JacobiSLAESolver.Models;
using JacobiSLAESolver.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Collections.ObjectModel;

namespace JacobiSLAESolver
{
    public partial class Form1 : Form
    {
        #region Constants

        private const int OrderLimit = 8;                   //Предел размерности системы
        private const int LimitForIterationsLimit = 1000000;//Предел для предела итераций
        private const double AccuracyLimit = 1e-8;          //Предел для |ε|
        private const int RoundOrder = 8;                   //Число знаков округления для отображения

        private const int LeftLayoutOffset = 5;             //Смещение всех динамических элементов
        private const int TopLayoutOffset = 60;             //Смещение всех динамических элементов

        private const int StandartElementWidth = 70;        //Стандартная ширина динамического элемента
        private const int StandartElementHeight = 23;       //Стандартная высота динамического элемента
        private const int StandartElementMargin = 3;        //Расстояние между динамическими элементами

        private readonly Color NormalColor = Color.White;   //Цвет TextBox при нормальной работе
        private readonly Color InvalidColor = Color.Red;    //Цвет TextBox при неверных данных

        #endregion

        #region Main variables

        private int Order                       //Порядок системы (число переменных)
        {
            get => A == null ? 0 : A.Rows;
        }
        private double Accuracy;                //Точность, до которой необходимо применять метод
        private int IterationsLimit;            //Предел итераций

        private Matrix A;                       //Матрица коэффициентов
        private Matrix B;                       //Матрица констант
        private SolutionResult SolutionResult;  //Хранит промежуточные данные вычислений

        private DataContext DataContext;        //Предоставляет доступ к MS SQL DB

        #endregion

        public Form1()
        {
            SolutionResult = new SolutionResult();

            InitializeComponent();
            InitializeVariables();      //Установить начальные значения

            OpenDBConnect();            //Подключиться к БД
        }
        private void InitializeVariables()
        {
            OrderTextBox.Text = "3";
            AccuracyTextBox.Text = "1e-7";
            IterationsLimitTextBox.Text = "1000";

            FileInfo info = new FileInfo(Resources.SQLConnectionStringFile);
            if (info.Exists && info.Length < 1 << 20)
            {
                try
                {
                    Resources.SQLConnectionString = File.ReadAllText(Resources.SQLConnectionStringFile);
                }
                catch { }
            }
        }
        private async Task OpenDBConnect()
        {
            try
            {
                DataContext dataContext = new DataContext();
                await dataContext.Database.EnsureCreatedAsync();           //Создать БД, если её ещё нет
                DataContext = dataContext;

                foreach (SLAE slae in DataContext.Systems)      //Добавление систем из БД в выпадающий список
                    SavedSystemsComboBox.Items.Add(slae.Name);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}\n\n{nameof(ex.StackTrace)}:\n{ex.StackTrace}",
                                "Ошибка чтения файла", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region Dynamic render

        //Проверяет все текстовые поля на наличие неверных данных
        private bool ExistsInvalidData
        {
            get => DynamicControls.Where(x => x is TextBox).Any(x => (x as TextBox).BackColor.Equals(InvalidColor));
        }

        #region Controls collections

        //Список динамически сгенерированных элементов для возможности их удаления
        private Collection<Control> DynamicControls = new Collection<Control>();

        private Collection<TextBox> CoefficientsControls = new Collection<TextBox>();   //Для коэффициентов
        private Collection<TextBox> ConstantsControls = new Collection<TextBox>();      //Для констант
        private Collection<TextBox> VariablesControls = new Collection<TextBox>();      //Для переменных

        //Остальное функциональное (элементы в которых данные должны обнавляться при пересчёте)
        private Collection<Control> OtherSolutionControls = new Collection<Control>();

        //Методы по индексам получают соответствующие текстовые поля
        private TextBox GetCoefficientTextBox(int row, int column)
        {
            return CoefficientsControls.FirstOrDefault(x => x.Tag is Point p && p.X == row && p.Y == column);
        }
        private TextBox GetConstantTextBox(int row)
        {
            return ConstantsControls.FirstOrDefault(x => x.Tag is int i && i == row);
        }
        private TextBox GetVariableTextBox(int row)
        {
            return VariablesControls.FirstOrDefault(x => x.Tag is int i && i == row);
        }

        //Удаляют динасически созданные элементы (только решения / все)
        private void DeleteSolutionControls()
        {
            foreach (TextBox textBox in VariablesControls)
            {
                DynamicControls.Remove(textBox);
                Controls.Remove(textBox);
            }
            VariablesControls.Clear();

            foreach (Control control in OtherSolutionControls)
            {
                DynamicControls.Remove(control);
                Controls.Remove(control);
            }
            OtherSolutionControls.Clear();
        }
        private void DeleteDynamicControls()
        {
            foreach (Control control in DynamicControls)
                Controls.Remove(control);

            DynamicControls.Clear();
            CoefficientsControls.Clear();
            ConstantsControls.Clear();
            VariablesControls.Clear();
            OtherSolutionControls.Clear();
        }

        #endregion

        //Указывает, нужно ли пересоздавать элементы, не несущие информации о текущем состоянии решения
        private bool NeedSolutionVariblesVectorLabel
        {
            get => !DynamicControls.Any(x => x.Name == "SolutionVariblesVectorLabel");
        }
        private bool NeedPrintButton
        {
            get => !DynamicControls.Any(x => x.Name == "PrintButton");
        }

        //Методы отрисовки различных динамических элементов (однообразны и просты)
        private void RenderCoefficients()
        {
            if (Order == 0)
                return;

            for (int i = 0; i < Order; ++i)
            {
                for (int j = 0; j < Order; ++j)
                {
                    TextBox textBox = new TextBox();
                    textBox.Width = StandartElementWidth;
                    textBox.Height = StandartElementHeight;
                    textBox.Left = LeftLayoutOffset + (StandartElementWidth + StandartElementMargin) * j;
                    textBox.Top = TopLayoutOffset + (StandartElementHeight + StandartElementMargin) * i;
                    textBox.Tag = new Point(i, j);
                    textBox.TextChanged += CoefficientTextChanged;
                    textBox.Text = Math.Round(A[i, j], RoundOrder).ToString();

                    DynamicControls.Add(textBox);
                    CoefficientsControls.Add(textBox);
                    Controls.Add(textBox);
                }
            }
        }
        private void RenderVariablesVectorLabel(bool isSolution)
        {
            if (Order == 0)
                return;

            Label label = new Label();
            label.Width = StandartElementWidth;
            label.Height = StandartElementHeight;
            label.Left = LeftLayoutOffset + (StandartElementWidth + StandartElementMargin) * Order;
            label.Top = TopLayoutOffset + ((StandartElementHeight + StandartElementMargin) * Order - StandartElementMargin) / 2 - StandartElementHeight / 2;
            label.Text = "X = ";

            if (isSolution)
            {
                label.Name = "SolutionVariblesVectorLabel";
                label.Left += 3 * (StandartElementWidth + StandartElementMargin);

                if (!NeedSolutionVariblesVectorLabel)
                    return;
            }

            DynamicControls.Add(label);
            Controls.Add(label);
        }
        private void RenderConstants()
        {
            if (Order == 0)
                return;

            for (int i = 0; i < Order; ++i)
            {
                TextBox textBox = new TextBox();
                textBox.Width = StandartElementWidth;
                textBox.Height = StandartElementHeight;
                textBox.Left = LeftLayoutOffset + (StandartElementWidth + StandartElementMargin) * (Order + 1);
                textBox.Top = TopLayoutOffset + (StandartElementHeight + StandartElementMargin) * i;
                textBox.Tag = i;
                textBox.TextChanged += ConstantTextChanged;
                textBox.Text = Math.Round(B[i, 0], RoundOrder).ToString();

                DynamicControls.Add(textBox);
                ConstantsControls.Add(textBox);
                Controls.Add(textBox);
            }
        }
        private void RenderVariables()
        {
            if (Order == 0 || !SolutionResult.IntermediateSolutions.Any())
                return;

            Matrix x = SolutionResult.Solution;
            for (int i = 0; i < Order; ++i)
            {
                TextBox textBox = new TextBox();
                textBox.Width = StandartElementWidth;
                textBox.Height = StandartElementHeight;
                textBox.Left = LeftLayoutOffset + (StandartElementWidth + StandartElementMargin) * (Order + 4);
                textBox.Top = TopLayoutOffset + (StandartElementHeight + StandartElementMargin) * i;
                textBox.ReadOnly = true;
                textBox.Tag = i;
                textBox.Text = Math.Round(x[i, 0], RoundOrder).ToString();

                DynamicControls.Add(textBox);
                VariablesControls.Add(textBox);
                Controls.Add(textBox);
            }
        }
        private void RenderIterationsLabel()
        {
            if (Order == 0)
                return;

            Label label = new Label();
            label.Width = 2 * StandartElementWidth;
            label.Height = StandartElementHeight;
            label.Left = LeftLayoutOffset;
            label.Top = TopLayoutOffset + (StandartElementHeight + StandartElementMargin) * Order;
            label.Text = $"Всего итераций: {SolutionResult.IntermediateSolutions.Count}";

            DynamicControls.Add(label);
            OtherSolutionControls.Add(label);
            Controls.Add(label);
        }
        private void RenderPrintButton()
        {
            if (Order == 0 || !SolutionResult.IntermediateSolutions.Any() || !NeedPrintButton)
                return;

            Button button = new Button();
            button.Name = "PrintButton";
            button.Width = StandartElementWidth;
            button.Height = StandartElementHeight;
            button.Left = LeftLayoutOffset + (StandartElementWidth + StandartElementMargin) * 2;
            button.Top = TopLayoutOffset + (StandartElementHeight + StandartElementMargin) * Order;
            button.Text = "Печать";
            button.Click += PrintButtonClick;

            DynamicControls.Add(button);
            Controls.Add(button);
        }
        private void RenderDataGridView()
        {
            if (Order == 0 || !SolutionResult.IntermediateSolutions.Any())
                return;

            DataGridView dataGridView = new DataGridView();
            dataGridView.Width = StandartElementWidth * (Order + 3);
            dataGridView.Height = StandartElementHeight * 13;
            dataGridView.Left = LeftLayoutOffset;
            dataGridView.Top = TopLayoutOffset + (StandartElementHeight + StandartElementMargin) * (Order + 1);
            dataGridView.ReadOnly = true;
            dataGridView.RowHeadersVisible = false;
            dataGridView.AllowUserToAddRows = false;
            dataGridView.AllowUserToDeleteRows = false;
            dataGridView.AllowUserToOrderColumns = false;
            dataGridView.AllowUserToResizeRows = false;

            dataGridView.Columns.Add("IterationsCount", "N");
            for (int i = 0; i < VariablesControls.Count; ++i)
                dataGridView.Columns.Add($"x{i + 1}", $"x{i + 1}");
            dataGridView.Columns.Add("Error", "|ε|_max");

            for (int i = 0; i < dataGridView.ColumnCount; ++i)
                dataGridView.Columns[i].Width = StandartElementWidth;

            for (int n = 0; n < SolutionResult.IntermediateSolutions.Count; ++n)
            {
                Collection<object> rowValues = new Collection<object>();
                Matrix x = SolutionResult.IntermediateSolutions[n];

                rowValues.Add((n + 1).ToString());
                for (int i = 0; i < Order; ++i)
                    rowValues.Add(Math.Round(x[i, 0], RoundOrder).ToString());
                rowValues.Add(Math.Round(SolutionResult.MathErrors[n], RoundOrder).ToString());

                dataGridView.Rows.Add(rowValues.ToArray());
            }

            DynamicControls.Add(dataGridView);
            OtherSolutionControls.Add(dataGridView);
            Controls.Add(dataGridView);
        }

        private void RenderSystem()
        {
            RenderCoefficients();
            RenderVariablesVectorLabel(false);
            RenderConstants();
        }
        private void RenderSolution()
        {
            RenderVariablesVectorLabel(true);
            RenderVariables();
            RenderIterationsLabel();
            RenderPrintButton();
            RenderDataGridView();
        }

        #region Dynamic controls events

        //События смены текста в динамических элементах
        private void CoefficientTextChanged(object sender, EventArgs e)
        {
            if (!(sender is TextBox textBox) || !(textBox.Tag is Point point))
                return;

            if (double.TryParse(textBox.Text, out double value) && !double.IsNaN(value) && !double.IsInfinity(value))
            {
                textBox.BackColor = NormalColor;
                A[point.X, point.Y] = Math.Round(value, RoundOrder);
            }
            else
            {
                textBox.BackColor = InvalidColor;
            }
        }
        private void ConstantTextChanged(object sender, EventArgs e)
        {
            if (!(sender is TextBox textBox) || !(textBox.Tag is int index))
                return;

            if (double.TryParse(textBox.Text, out double value) && !double.IsNaN(value) && !double.IsInfinity(value))
            {
                textBox.BackColor = NormalColor;
                B[index, 0] = Math.Round(value, RoundOrder);
            }
            else
            {
                textBox.BackColor = InvalidColor;
            }
        }

        //Событие кнопки "Печать"
        private void PrintButtonClick(object sender, EventArgs e)
        {
            DocumentProvider dp = new DocumentProvider();
            PrintingForm form = new PrintingForm();
            form.Show();

            Task.Run(() => form.DefineData(dp.GenerateDocument(SolutionResult, RoundOrder)));
        }

        #endregion

        #endregion

        #region Static controls events

        private void OpenFileButtonClick(object sender, EventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            if (dialog.ShowDialog() != CommonFileDialogResult.Ok)
                return;

            try
            {
                using (FileStream fs = new FileStream(dialog.FileName, FileMode.Open, FileAccess.Read))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        Collection<string[]> splittedLines = new Collection<string[]>();
                        while (!sr.EndOfStream)
                        {
                            string line = sr.ReadLine();
                            if (string.IsNullOrWhiteSpace(line))
                                continue;

                            splittedLines.Add(line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries));
                        }

                        if (!splittedLines.Any())
                        {
                            MessageBox.Show("Некорректные данные", "Ошибка чтения файла",
                                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }

                        int rows = splittedLines.Count;
                        int columns = splittedLines.First().Length;
                        if (rows != columns - 1 || !splittedLines.All(x => x.Length == columns))
                        {
                            MessageBox.Show("Некорректные данные", "Ошибка чтения файла",
                                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }

                        A = new Matrix(rows, rows);
                        B = new Matrix(rows, 1);

                        DeleteDynamicControls();
                        RenderSystem();

                        for (int i = 0; i < rows; ++i)
                        {
                            for (int j = 0; j < columns - 1; ++j)
                                GetCoefficientTextBox(i, j).Text = splittedLines[i][j];

                            GetConstantTextBox(i).Text = splittedLines[i].Last();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}\n\n{nameof(ex.StackTrace)}:\n{ex.StackTrace}",
                                "Ошибка чтения файла",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        private void OrderTextChanged(object sender, EventArgs e)
        {
            if (!(sender is TextBox textBox))
                return;

            if (int.TryParse(textBox.Text, out int value) && value > 0 && value <= OrderLimit)
            {
                textBox.BackColor = NormalColor;

                A = new Matrix(value, value);
                B = new Matrix(value, value);

                DeleteDynamicControls();
                RenderSystem();
            }
            else
            {
                textBox.BackColor = InvalidColor;
            }
        }
        private void AccuracyTextChanged(object sender, EventArgs e)
        {
            if (!(sender is TextBox textBox))
                return;

            if (double.TryParse(textBox.Text, out double value) && value >= AccuracyLimit &&
                !double.IsNaN(value) && !double.IsInfinity(value))
            {
                textBox.BackColor = NormalColor;
                Accuracy = value;
            }
            else
            {
                textBox.BackColor = InvalidColor;
            }
        }
        private void IterationsLimitTextChanged(object sender, EventArgs e)
        {
            if (!(sender is TextBox textBox))
                return;

            if (int.TryParse(textBox.Text, out int value) && value > 0 && value <= LimitForIterationsLimit)
            {
                textBox.BackColor = NormalColor;
                IterationsLimit = value;
            }
            else
            {
                textBox.BackColor = InvalidColor;
            }
        }

        private void SolveButtonClick(object sender, EventArgs e)
        {
            if (Order == 0 || ExistsInvalidData)
                return;

            if (Math.Round(A.Determinant, RoundOrder) == 0)
            {
                MessageBox.Show("Матрица вырождена, система имеет бесконечное число решений",
                                "Матрица вырождена", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SolutionResult.Clear();
            SolutionResult.SetSystem(A, B);

            #region Define vars

            int iterations = 0;
            Matrix a, b, x, residue;

            a = A.Copy();
            b = B.Copy();

            #endregion

            #region Transformate system for Jacobi method

            //Симметризация матрицы коэффициентов
            b = a.TransposeMatrix * b;
            a = a.TransposeMatrix * a;

            //Диагональная матрица
            Matrix d = new Matrix(Order, Order);
            for (int i = 0; i < Order; ++i)
                d[i, i] = a[i, i];

            a = d.ReverseMatrix * (d - a);
            b = d.ReverseMatrix * b;
            x = B.Copy();

            #endregion

            #region Iterations

            while (iterations < IterationsLimit)
            {
                x = a * x + b;
                ++iterations;

                residue = A * x - B;
                double maxError = Enumerable.Range(0, Order).Select(i => Math.Abs(residue[i, 0])).Max();

                SolutionResult.IntermediateSolutions.Add(x);
                SolutionResult.MathErrors.Add(maxError);

                if (maxError <= Accuracy)
                    break;
            }

            #endregion

            DeleteSolutionControls();
            RenderSolution();
        }

        //Проверяет, существует ли система с таким именем в БД
        private async Task<bool> ContainsSystem(string name)
        {
            if (DataContext != null)
                return await DataContext.Systems.AnyAsync(x => x.Name == name);
            else
                return false;
        }
        private async void LoadSystemButtonClick(object sender, EventArgs e)
        {
            if (DataContext == null || !await ContainsSystem(SavedSystemsComboBox.Text))
                return;

            SLAE slae = await DataContext.Systems.FirstAsync(x => x.Name == SavedSystemsComboBox.Text);
            A = Helper.DeserializeSystem(slae.Coefficients);
            B = A.GetExtension();
            A.RemoveExtension();

            DeleteDynamicControls();
            RenderSystem();
        }
        private async void SaveSystemButtonClick(object sender, EventArgs e)
        {
            if (DataContext == null)
                return;

            if (ExistsInvalidData || string.IsNullOrWhiteSpace(SavedSystemsComboBox.Text))
            {
                MessageBox.Show("Введены некорректные данные", "Некорректные данные",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SLAE slae = await DataContext.Systems.FirstOrDefaultAsync(x => x.Name == SavedSystemsComboBox.Text);
            if (slae != null)
            {
                DialogResult result = MessageBox.Show("Система с таким именем уже есть в базе. Заменить?",
                                                      "Обновить систему?", MessageBoxButtons.OKCancel,
                                                      MessageBoxIcon.Information);
                if (result == DialogResult.OK)
                    UpdateSystemButtonClick(sender, e);

                return;
            }

            slae = new SLAE();
            slae.Name = SavedSystemsComboBox.Text;
            slae.Coefficients = Helper.SerializeSystem(A, B);
            await DataContext.Systems.AddAsync(slae);
            await DataContext.SaveChangesAsync();

            SavedSystemsComboBox.Items.Add(slae.Name);
        }
        private async void UpdateSystemButtonClick(object sender, EventArgs e)
        {
            if (DataContext == null || !await ContainsSystem(SavedSystemsComboBox.Text))
                return;

            if (ExistsInvalidData)
            {
                MessageBox.Show("Введены некорректные данные", "Некорректные данные",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SLAE slae = await DataContext.Systems.FirstAsync(x => x.Name == SavedSystemsComboBox.Text);
            slae.Coefficients = Helper.SerializeSystem(A, B);
            await DataContext.SaveChangesAsync();
        }
        private async void DeleteSystemButtonClick(object sender, EventArgs e)
        {
            if (DataContext == null || !await ContainsSystem(SavedSystemsComboBox.Text))
                return;

            SLAE slae = await DataContext.Systems.FirstOrDefaultAsync(x => x.Name == SavedSystemsComboBox.Text);
            DataContext.Systems.Remove(slae);
            await DataContext.SaveChangesAsync();

            SavedSystemsComboBox.Items.Remove(slae.Name);
        }

        #endregion
    }
}