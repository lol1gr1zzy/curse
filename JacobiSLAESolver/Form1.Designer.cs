using System.ComponentModel;

namespace JacobiSLAESolver
{
    partial class Form1
    {
        #region Controls

        private Button OpenFileButton;

        private Label OrderLabel;
        private TextBox OrderTextBox;

        private Label AccuracyLabel;
        private TextBox AccuracyTextBox;

        private Label IterationsLimitLabel;
        private TextBox IterationsLimitTextBox;

        private Button SolveButton;

        private ComboBox SavedSystemsComboBox;

        private Button LoadSystemButton;
        private Button SaveSystemButton;
        private Button UpdateSystemButton;
        private Button DeleteSystemButton;

        #endregion

        #region Windows Form Designer generated code

        private IContainer Components = null;

        private void InitializeComponent()
        {
            OpenFileButton = new Button();
            OrderLabel = new Label();
            OrderTextBox = new TextBox();
            AccuracyLabel = new Label();
            AccuracyTextBox = new TextBox();
            IterationsLimitLabel = new Label();
            IterationsLimitTextBox = new TextBox();
            SolveButton = new Button();
            SavedSystemsComboBox = new ComboBox();
            LoadSystemButton = new Button();
            UpdateSystemButton = new Button();
            DeleteSystemButton = new Button();
            SaveSystemButton = new Button();
            SuspendLayout();
            // 
            // OpenFileButton
            // 
            OpenFileButton.Location = new Point(5, 5);
            OpenFileButton.Name = "OpenFileButton";
            OpenFileButton.Size = new Size(96, 23);
            OpenFileButton.TabIndex = 0;
            OpenFileButton.Text = "Открыть файл";
            OpenFileButton.UseVisualStyleBackColor = true;
            OpenFileButton.Click += OpenFileButtonClick;
            // 
            // SystemOrderLabel
            // 
            OrderLabel.AutoSize = true;
            OrderLabel.Location = new Point(107, 9);
            OrderLabel.Name = "SystemOrderLabel";
            OrderLabel.Size = new Size(106, 15);
            OrderLabel.TabIndex = 1;
            OrderLabel.Text = "Порядок системы";
            // 
            // SystemOrderTextBox
            // 
            OrderTextBox.Location = new Point(219, 5);
            OrderTextBox.Name = "SystemOrderTextBox";
            OrderTextBox.Size = new Size(51, 23);
            OrderTextBox.TabIndex = 2;
            OrderTextBox.TextChanged += OrderTextChanged;
            // 
            // SolutionAccuracyLabel
            // 
            AccuracyLabel.AutoSize = true;
            AccuracyLabel.Location = new Point(276, 9);
            AccuracyLabel.Name = "SolutionAccuracyLabel";
            AccuracyLabel.Size = new Size(111, 15);
            AccuracyLabel.TabIndex = 1;
            AccuracyLabel.Text = "Точность решения";
            // 
            // SolutionAccuracyTextBox
            // 
            AccuracyTextBox.Location = new Point(393, 5);
            AccuracyTextBox.Name = "SolutionAccuracyTextBox";
            AccuracyTextBox.Size = new Size(51, 23);
            AccuracyTextBox.TabIndex = 2;
            AccuracyTextBox.TextChanged += AccuracyTextChanged;
            // 
            // IterationsLimitLabel
            // 
            IterationsLimitLabel.AutoSize = true;
            IterationsLimitLabel.Location = new Point(450, 9);
            IterationsLimitLabel.Name = "IterationsLimitLabel";
            IterationsLimitLabel.Size = new Size(98, 15);
            IterationsLimitLabel.TabIndex = 1;
            IterationsLimitLabel.Text = "Лимит итераций";
            // 
            // IterationsLimitTextBox
            // 
            IterationsLimitTextBox.Location = new Point(554, 5);
            IterationsLimitTextBox.Name = "IterationsLimitTextBox";
            IterationsLimitTextBox.Size = new Size(51, 23);
            IterationsLimitTextBox.TabIndex = 2;
            IterationsLimitTextBox.TextChanged += IterationsLimitTextChanged;
            // 
            // SolveButton
            // 
            SolveButton.Location = new Point(611, 5);
            SolveButton.Name = "SolveButton";
            SolveButton.Size = new Size(79, 23);
            SolveButton.TabIndex = 0;
            SolveButton.Text = "Решить";
            SolveButton.UseVisualStyleBackColor = true;
            SolveButton.Click += SolveButtonClick;
            // 
            // SystemsComboBox
            // 
            SavedSystemsComboBox.FormattingEnabled = true;
            SavedSystemsComboBox.Location = new Point(5, 34);
            SavedSystemsComboBox.Name = "SystemsComboBox";
            SavedSystemsComboBox.Size = new Size(208, 23);
            SavedSystemsComboBox.TabIndex = 3;
            // 
            // LoadSystemButton
            // 
            LoadSystemButton.Location = new Point(219, 34);
            LoadSystemButton.Name = "LoadSystemButton";
            LoadSystemButton.Size = new Size(96, 23);
            LoadSystemButton.TabIndex = 0;
            LoadSystemButton.Text = "Загрузить";
            LoadSystemButton.UseVisualStyleBackColor = true;
            LoadSystemButton.Click += LoadSystemButtonClick;
            // 
            // UpdateSystemButton
            // 
            UpdateSystemButton.Location = new Point(423, 34);
            UpdateSystemButton.Name = "UpdateSystemButton";
            UpdateSystemButton.Size = new Size(96, 23);
            UpdateSystemButton.TabIndex = 0;
            UpdateSystemButton.Text = "Обновить";
            UpdateSystemButton.UseVisualStyleBackColor = true;
            UpdateSystemButton.Click += UpdateSystemButtonClick;
            // 
            // DeleteSystemButton
            // 
            DeleteSystemButton.Location = new Point(525, 34);
            DeleteSystemButton.Name = "DeleteSystemButton";
            DeleteSystemButton.Size = new Size(96, 23);
            DeleteSystemButton.TabIndex = 0;
            DeleteSystemButton.Text = "Удалить";
            DeleteSystemButton.UseVisualStyleBackColor = true;
            DeleteSystemButton.Click += DeleteSystemButtonClick;
            // 
            // SaveSystemButton
            // 
            SaveSystemButton.Location = new Point(321, 34);
            SaveSystemButton.Name = "SaveSystemButton";
            SaveSystemButton.Size = new Size(96, 23);
            SaveSystemButton.TabIndex = 0;
            SaveSystemButton.Text = "Сохранить";
            SaveSystemButton.UseVisualStyleBackColor = true;
            SaveSystemButton.Click += SaveSystemButtonClick;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(SavedSystemsComboBox);
            Controls.Add(IterationsLimitTextBox);
            Controls.Add(AccuracyTextBox);
            Controls.Add(OrderTextBox);
            Controls.Add(IterationsLimitLabel);
            Controls.Add(AccuracyLabel);
            Controls.Add(OrderLabel);
            Controls.Add(SolveButton);
            Controls.Add(DeleteSystemButton);
            Controls.Add(SaveSystemButton);
            Controls.Add(UpdateSystemButton);
            Controls.Add(LoadSystemButton);
            Controls.Add(OpenFileButton);
            Name = "Form1";
            Text = "Решение СЛАУ методом Якоби";
            ResumeLayout(false);
            PerformLayout();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (Components != null))
            {
                Components.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}