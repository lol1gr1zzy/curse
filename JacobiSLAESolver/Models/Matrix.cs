namespace JacobiSLAESolver.Models
{
    public class Matrix
    {
        /// <summary>
        /// Закрывай, мне самому тут не по себе
        /// 
        /// На самом деле просто куча примитивных методов преобразования матрицы
        /// </summary>

        private static Random random = new Random();

        /// <summary>
        /// Данные матрицы
        /// </summary>
        private double[,] _Matrix;
        /// <summary>
        /// Данные расширения матрицы
        /// </summary>
        private double[,] _Extension;

        /// <summary>
        /// Определение индексатора матрицы
        /// </summary>
        /// <param name="row">Строка</param>
        /// <param name="column">Столбец</param>
        /// <returns>Значение матрицы в указанных индексах</returns>
        public double this[int row, int column]
        {
            get => _Matrix[row, column];
            set => _Matrix[row, column] = value;
        }

        /// <summary>
        /// Указывает, является ли матрица квадратной
        /// </summary>
        public bool IsSquare
        {
            get => Rows == Columns;
        }
        /// <summary>
        /// Указывает, является ли матрица расширенной
        /// </summary>
        public bool IsExtended
        {
            get => _Extension != null;
        }

        /// <summary>
        /// Число строк матрицы
        /// </summary>
        public int Rows
        {
            get => _Matrix.GetLength(0);
        }
        /// <summary>
        /// Число столбцов матрицы
        /// </summary>
        public int Columns
        {
            get => _Matrix.GetLength(1);
        }
        /// <summary>
        /// Число столбцов расширения матрицы
        /// </summary>
        public int ExtensionColumns
        {
            get => _Extension != null ? _Extension.GetLength(1) : 0;
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="rows">Число строк</param>
        /// <param name="columns">Число столбцов</param>
        /// <param name="extensionColumns">Число столбцов расширения</param>
        public Matrix(int rows, int columns, int extensionColumns = 0)
        {
            _Matrix = new double[rows, columns];

            if (extensionColumns != 0)
                _Extension = new double[rows, extensionColumns];
        }

        /// <summary>
        /// Создаёт копию матрицы
        /// </summary>
        /// <returns>Копия матрицы</returns>
        public Matrix Copy()
        {
            Matrix copy = new Matrix(Rows, Columns, ExtensionColumns);

            for (int r = 0; r < Rows; ++r)
                for (int c = 0; c < Columns; ++c)
                    copy[r, c] = this[r, c];

            if (IsExtended)
                for (int r = 0; r < Rows; ++r)
                    for (int c = 0; c < ExtensionColumns; ++c)
                        copy.SetExtensionElement(r, c, GetExtensionElement(r, c));

            return copy;
        }

        /// <summary>
        /// Получает элемент матрицы
        /// </summary>
        /// <param name="row">Строка</param>
        /// <param name="column">Столбец</param>
        /// <returns></returns>
        public double GetElement(int row, int column)
        {
            return _Matrix[row, column];
        }
        /// <summary>
        /// Задаёт элемент матрицы
        /// </summary>
        /// <param name="row">Строка</param>
        /// <param name="column">Столбец</param>
        /// <param name="value">Значение</param>
        public void SetElement(int row, int column, double value)
        {
            _Matrix[row, column] = value;
        }
        /// <summary>
        /// Получает элемент расширения матрицы
        /// </summary>
        /// <param name="row">Строка</param>
        /// <param name="column">Столбец расширения</param>
        /// <returns></returns>
        public double GetExtensionElement(int row, int column)
        {
            return _Extension[row, column];
        }
        /// <summary>
        /// Задаёт столбец расширения матрицы
        /// </summary>
        /// <param name="row">Строка</param>
        /// <param name="column">Столбец расширения</param>
        /// <param name="value">Значение</param>
        public void SetExtensionElement(int row, int column, double value)
        {
            _Extension[row, column] = value;
        }

        /// <summary>
        /// Получает расширение матрицы в виде отдельной матрицы
        /// </summary>
        /// <returns></returns>
        public Matrix GetExtension()
        {
            if (!IsExtended)
                return null;

            Matrix exMatrix = new Matrix(Rows, ExtensionColumns);

            for (int r = 0; r < Rows; ++r)
                for (int c = 0; c < ExtensionColumns; ++c)
                    exMatrix[r, c] = GetExtensionElement(r, c);

            return exMatrix;
        }
        /// <summary>
        /// Расширяет матрицу
        /// </summary>
        /// <param name="extensionColumns">Количество столбцов расширения</param>
        public void Extend(int extensionColumns)
        {
            _Extension = new double[Rows, extensionColumns];
        }
        /// <summary>
        /// Раширяет матрицу
        /// </summary>
        /// <param name="exMatrix">Матрица расширения</param>
        public void Extend(Matrix exMatrix)
        {
            if (exMatrix == null || Rows != exMatrix.Rows)
                return;

            _Extension = new double[Rows, exMatrix.Columns];

            for (int r = 0; r < Rows; ++r)
                for (int c = 0; c < ExtensionColumns; ++c)
                    SetExtensionElement(r, c, exMatrix[r, c]);
        }
        /// <summary>
        /// Удаляет расширение матрицы
        /// </summary>
        public void RemoveExtension()
        {
            _Extension = null;
        }
        /// <summary>
        /// Округляет элементы матрицы
        /// </summary>
        /// <param name="order">Число знаков после запятой</param>
        public void Round(int order)
        {
            if (order < 0)
                return;

            for (int r = 0; r < Rows; ++r)
            {
                for (int c = 0; c < Columns; ++c)
                    this[r, c] = Math.Round(this[r, c], order);

                for (int c = 0; c < ExtensionColumns; ++c)
                    SetExtensionElement(r, c, Math.Round(GetExtensionElement(r, c), order));
            }
        }
        /// <summary>
        /// Транспонирует матрицу
        /// </summary>
        public void Transpose()
        {
            if (IsExtended)
                return;

            double[,] transposedMatrix = new double[Columns, Rows];

            for (int r = 0; r < Rows; ++r)
                for (int c = 0; c < Columns; ++c)
                    transposedMatrix[c, r] = this[r, c];

            _Matrix = transposedMatrix;
        }
        /// <summary>
        /// Приводит матрицу к единичной
        /// </summary>
        public void ToUnitMatrix()
        {
            if (!IsSquare || Determinant == 0)
                return;

            RemoveZeroesFromMainDiagonal();
            ChooseMainElements();

            for (int c = 0; c < Columns; ++c)
            {
                DevideRow(c, this[c, c]);

                for (int r = c + 1; r < Rows; ++r)
                    MultiplyAndSumRow(c, r, -this[r, c]);
            }

            for (int c = Columns - 1; c > 0; --c)
                for (int r = c - 1; r > -1; --r)
                    MultiplyAndSumRow(c, r, -this[r, c]);
        }

        /// <summary>
        /// Меняет две строки матрицы местами
        /// </summary>
        /// <param name="row1">Номер строки 1</param>
        /// <param name="row2">Номер строки 2</param>
        public void ExchangeRows(int row1, int row2)
        {
            double buff;

            for (int c = 0; c < Columns; ++c)
            {
                buff = this[row1, c];
                this[row1, c] = this[row2, c];
                this[row2, c] = buff;
            }

            if (IsExtended)
                for (int c = 0; c < ExtensionColumns; ++c)
                {
                    buff = GetExtensionElement(row1, c);
                    SetExtensionElement(row1, c, GetExtensionElement(row2, c));
                    SetExtensionElement(row2, c, buff);
                }
        }
        /// <summary>
        /// Меняет два столбца матрицы местами
        /// </summary>
        /// <param name="column1">Номер столбца 1</param>
        /// <param name="column2">Номер столбца 2</param>
        public void ExchangeColumns(int column1, int column2)
        {
            double buff;
            for (int r = 0; r < Rows; ++r)
            {
                buff = this[r, column1];
                this[r, column1] = this[r, column2];
                this[r, column2] = buff;
            }
        }
        /// <summary>
        /// Меняет два столбца расширения матрицы местами
        /// </summary>
        /// <param name="column1">Номер столбца расширения 1</param>
        /// <param name="column2">Номер столбца расширения 2</param>
        public void ExchangeExtensionColumns(int column1, int column2)
        {
            double buff;
            for (int r = 0; r < Rows; ++r)
            {
                buff = GetExtensionElement(r, column1);
                SetExtensionElement(r, column1, GetExtensionElement(r, column2));
                SetExtensionElement(r, column2, buff);
            }
        }

        /// <summary>
        /// Умножает строку матрицы на заданное число
        /// </summary>
        /// <param name="row">Строка</param>
        /// <param name="value">Число</param>
        public void MultiplyRow(int row, double value)
        {
            for (int c = 0; c < Columns; ++c)
                this[row, c] *= value;

            if (IsExtended)
                for (int c = 0; c < ExtensionColumns; ++c)
                    SetExtensionElement(row, c, value * GetExtensionElement(row, c));
        }
        /// <summary>
        /// Умножает столбец матрицы на заданное число
        /// </summary>
        /// <param name="column">Столбец</param>
        /// <param name="value">Число</param>
        public void MultiplyColumn(int column, double value)
        {
            for (int r = 0; r < Rows; ++r)
                this[r, column] *= value;
        }
        /// <summary>
        /// Умножает столбец расширения матрицы на заданное число
        /// </summary>
        /// <param name="column">Столбец расширения</param>
        /// <param name="value">Число</param>
        public void MultiplyExtensionColumn(int column, double value)
        {
            for (int r = 0; r < Rows; ++r)
                SetExtensionElement(r, column, value * GetExtensionElement(r, column));
        }

        /// <summary>
        /// Умножает строку на число и прибавляет к другой строке
        /// </summary>
        /// <param name="fromRow">Прибавляемая строка</param>
        /// <param name="toRow">Строка назначения</param>
        /// <param name="multiplier">Множитель</param>
        public void MultiplyAndSumRow(int fromRow, int toRow, double multiplier)
        {
            for (int c = 0; c < Columns; ++c)
                this[toRow, c] += multiplier * this[fromRow, c];

            if (_Extension != null)
                for (int c = 0; c < ExtensionColumns; ++c)
                    SetExtensionElement(toRow, c, GetExtensionElement(toRow, c) + multiplier * GetExtensionElement(fromRow, c));
        }
        /// <summary>
        /// Умножает столбец на число и прибавляет к другому столбцу
        /// </summary>
        /// <param name="fromColumn">Прибавляемый столбец</param>
        /// <param name="toColumn">Столбец назначения</param>
        /// <param name="multiplier">Множитель</param>
        public void MultiplyAndSumColumn(int fromColumn, int toColumn, double multiplier)
        {
            for (int r = 0; r < Rows; ++r)
                this[r, toColumn] += multiplier * this[r, fromColumn];
        }
        /// <summary>
        /// Умножает столбец расширения на число и прибавляет к другому столбцу расширения
        /// </summary>
        /// <param name="fromColumn">Прибавляемый столбец расширения</param>
        /// <param name="toColumn">Столбец назначения расширения</param>
        /// <param name="multiplier">Множитель</param>
        public void MultiplyAndSumExtensionColumn(int fromColumn, int toColumn, double multiplier)
        {
            for (int r = 0; r < Rows; ++r)
                SetExtensionElement(r, toColumn, GetExtensionElement(r, toColumn) + multiplier * GetExtensionElement(r, fromColumn));
        }

        /// <summary>
        /// Делит строку на число
        /// </summary>
        /// <param name="row">Строка</param>
        /// <param name="devider">Делитель</param>
        public void DevideRow(int row, double devider)
        {
            for (int c = 0; c < Columns; ++c)
                this[row, c] /= devider;

            if (IsExtended)
                for (int c = 0; c < ExtensionColumns; ++c)
                    SetExtensionElement(row, c, GetExtensionElement(row, c) / devider);
        }
        /// <summary>
        /// Делит столбец на число
        /// </summary>
        /// <param name="column">Столбец</param>
        /// <param name="devider">Делитель</param>
        public void DevideColumn(int column, double devider)
        {
            for (int r = 0; r < Rows; ++r)
                this[r, column] /= devider;
        }
        /// <summary>
        /// Делит столбец расширения на число
        /// </summary>
        /// <param name="column">Столбец расширения</param>
        /// <param name="devider">Делитель</param>
        public void DevideExtensionColumn(int column, double devider)
        {
            for (int r = 0; r < Rows; ++r)
                SetExtensionElement(r, column, GetExtensionElement(r, column) / devider);
        }

        /// <summary>
        /// Делит строку на число и прибавляет к другой строке
        /// </summary>
        /// <param name="fromRow">Прибавляемая строка</param>
        /// <param name="toRow">Строка назначения</param>
        /// <param name="devider">Делитель</param>
        public void DevideAndSumRow(int fromRow, int toRow, double devider)
        {
            for (int c = 0; c < Columns; ++c)
                this[toRow, c] += this[fromRow, c] / devider;

            if (_Extension != null)
                for (int c = 0; c < ExtensionColumns; ++c)
                    SetExtensionElement(toRow, c, GetExtensionElement(toRow, c) + GetExtensionElement(fromRow, c) / devider);
        }
        /// <summary>
        /// Делит столбец на число и прибавляет к другому столбцу
        /// </summary>
        /// <param name="fromColumn">Прибавляемый столбец</param>
        /// <param name="toColumn">Столбец назначения</param>
        /// <param name="devider">Делитель</param>
        public void DevideAndSumColumn(int fromColumn, int toColumn, double devider)
        {
            for (int r = 0; r < Rows; ++r)
                this[r, toColumn] += this[r, fromColumn] / devider;
        }
        /// <summary>
        /// Делит столбец расширения на число и прибавляет к другому столбцу расширения
        /// </summary>
        /// <param name="fromColumn">Прибавляемый столбец расширения</param>
        /// <param name="toColumn">Столбец назначения расширения</param>
        /// <param name="devider">Делитель</param>
        public void DevideAndSumExtensionColumn(int fromColumn, int toColumn, double devider)
        {
            for (int r = 0; r < Rows; ++r)
                SetExtensionElement(r, toColumn, GetExtensionElement(r, toColumn) + GetExtensionElement(r, fromColumn) / devider);
        }

        /// <summary>
        /// Приводит матрицу к виду без нулей на главной диагонали
        /// </summary>
        /// <returns>Число перестановок</returns>
        public int RemoveZeroesFromMainDiagonal()
        {
            int exchanges = 0;

            for (int r1 = 0; r1 < Rows - 1; ++r1)
                if (this[r1, r1] == 0)
                    for (int r2 = r1 + 1; r2 < Rows; ++r2)
                        if (this[r2, r2] != 0)
                        {
                            ExchangeRows(r1, r2);
                            ++exchanges;
                        }

            return exchanges;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int ChooseMainElements()
        {
            int exchanges = 0;

            for (int c = 0; c < Columns - 1; ++c)
                for (int r = c + 1; r < Rows; ++r)
                    if (Math.Abs(this[c, c]) < Math.Abs(this[r, c]) && this[c, r] != 0)
                    {
                        ExchangeRows(c, r);
                        ++exchanges;
                    }

            return exchanges;
        }
        /// <summary>
        /// Детерминант матрицы
        /// </summary>
        public double Determinant
        {
            get
            {
                if (!IsSquare)
                    return double.NaN;

                int exchanges = 0;
                Matrix copy = Copy();
                exchanges += copy.RemoveZeroesFromMainDiagonal();
                exchanges += copy.ChooseMainElements();

                double determinant = 1;

                for (int r = 0; r < copy.Rows; ++r)
                    if (copy[r, r] == 0)
                        return 0;

                for (int c = 0; c < copy.Columns; ++c)
                {
                    for (int r = c; r < copy.Rows; ++r)
                        if (copy[r, c] != 0)
                        {
                            determinant *= copy[r, c];
                            copy.DevideRow(r, copy[r, c]);
                        }

                    for (int r = c + 1; r < copy.Rows; ++r)
                        copy.MultiplyAndSumRow(c, r, -copy[r, c]);
                }
                
                for (int r = 0; r < copy.Rows; ++r)
                    determinant *= copy[r, r];

                return determinant * Math.Pow(-1, exchanges);
            }
        }
        /// <summary>
        /// Максимальная норма матрицы
        /// </summary>
        public double NormaMax
        {
            get
            {
                List<double> rowModules = new List<double>();

                for (int r = 0; r < Rows; ++r)
                {
                    double module = 0;

                    for (int c = 0; c < Columns; ++c)
                        module += Math.Abs(this[r, c]);

                    rowModules.Add(module);
                }

                return rowModules.Max();
            }
        }
        /// <summary>
        /// Обратная матрица
        /// </summary>
        public Matrix ReverseMatrix
        {
            get
            {
                if (!IsSquare || Determinant == 0)
                    return null;

                Matrix exMatrix = Copy();
                exMatrix.Extend(UnitMatrix(exMatrix.Rows));
                exMatrix.RemoveZeroesFromMainDiagonal();
                exMatrix.ToUnitMatrix();

                return exMatrix.GetExtension();
            }
        }
        /// <summary>
        /// Транспонированная матрица
        /// </summary>
        public Matrix TransposeMatrix
        {
            get
            {
                Matrix result = Copy();
                result.Transpose();
                return result;
            }
        }

        /// <summary>
        /// Единичная матрица
        /// </summary>
        /// <param name="order">Размерность</param>
        /// <returns></returns>
        public static Matrix UnitMatrix(int order)
        {
            Matrix uMatrix = new Matrix(order, order);

            for (int r = 0; r < uMatrix.Rows; ++r)
                uMatrix[r, r] = 1;

            return uMatrix;
        }
        /// <summary>
        /// Случайная матрица
        /// </summary>
        /// <param name="rows">Число строк</param>
        /// <param name="columns">Число столбцов</param>
        /// <param name="min">Минимальный элемент</param>
        /// <param name="max">Максимальный элемент</param>
        /// <returns></returns>
        public static Matrix RandomMatrix(int rows, int columns, double min, double max)
        {
            Matrix rMatrix = new Matrix(rows, columns);

            for (int r = 0; r < rMatrix.Rows; ++r)
                for (int c = 0; c < rMatrix.Columns; ++c)
                    rMatrix[r, c] = random.NextDouble() * (max - min) + min;

            return rMatrix;
        }


        public static Matrix operator +(Matrix m1, Matrix m2)
        {
            if (m1.IsExtended || m2.IsExtended || m1.Rows != m2.Rows || m1.Columns != m2.Columns)
                return null;

            Matrix result = new Matrix(m1.Rows, m1.Columns);

            for (int r = 0; r < m1.Rows; ++r)
                for (int c = 0; c < m1.Columns; ++c)
                    result[r, c] = m1[r, c] + m2[r, c];

            return result;
        }
        public static Matrix operator -(Matrix m1, Matrix m2)
        {
            if (m1.IsExtended || m2.IsExtended || m1.Rows != m2.Rows || m1.Columns != m2.Columns)
                return null;

            Matrix result = new Matrix(m1.Rows, m1.Columns);

            for (int r = 0; r < m1.Rows; ++r)
                for (int c = 0; c < m1.Columns; ++c)
                    result[r, c] = m1[r, c] - m2[r, c];

            return result;
        }
        public static Matrix operator *(Matrix m, double multiplier)
        {
            if (m.IsExtended)
                return null;

            Matrix result = m.Copy();

            for (int r = 0; r < result.Rows; ++r)
                for (int c = 0; c < result.Columns; ++c)
                    result[r, c] *= multiplier;

            return result;
        }
        public static Matrix operator *(double multiplier, Matrix m) => m * multiplier;
        public static Matrix operator /(Matrix m, double devideValue)
        {
            if (m.IsExtended)
                return null;

            Matrix result = m.Copy();

            for (int r = 0; r < result.Rows; ++r)
                for (int c = 0; c < result.Columns; ++c)
                    result[r, c] /= devideValue;

            return result;
        }
        public static Matrix operator *(Matrix m1, Matrix m2)
        {
            if (m1.Columns != m2.Rows)
                return null;

            Matrix multiplication = new Matrix(m1.Rows, m2.Columns);

            double element;
            for (int r = 0; r < multiplication.Rows; ++r)
                for (int c = 0; c < multiplication.Columns; ++c)
                {
                    element = 0;

                    for (int rc = 0; rc < m1.Rows; ++rc)
                        element += m1[r, rc] * m2[rc, c];

                    multiplication[r, c] = element;
                }

            return multiplication;
        }
        public static bool operator ==(Matrix m1, Matrix m2)
        {
            if (m1 is null && m2 is null)
                return true;
            else if (!(m1 is null) && m2 is null || m1 is null && !(m2 is null))
                return false;

            if (m1.Rows != m2.Rows || m1.Columns != m2.Columns || m1.ExtensionColumns != m2.ExtensionColumns)
                return false;

            for (int r = 0; r < m1.Rows; ++r)
                for (int c = 0; c < m1.Columns; ++c)
                    if (m1[r, c] != m2[r, c])
                        return false;

            if (m1.IsExtended)
                for (int r = 0; r < m1.Rows; ++r)
                    for (int c = 0; c < m1.ExtensionColumns; ++c)
                        if (m1.GetExtensionElement(r, c) != m2.GetExtensionElement(r, c))
                            return false;

            return true;
        }
        public static bool operator !=(Matrix m1, Matrix m2)
        {
            return !(m1 == m2);
        }
    }
}