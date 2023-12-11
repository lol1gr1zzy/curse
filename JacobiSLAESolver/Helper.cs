using JacobiSLAESolver.Models;

namespace JacobiSLAESolver
{
    internal static class Helper
    {
        internal static string SerializeSystem(Matrix a, Matrix b, int roundOrder = -1)
        {
            string[] lines = new string[a.Rows];

            for (int i = 0; i < a.Rows; ++i)
            {
                string[] values = new string[a.Columns + 1];

                for (int j = 0;  j < a.Columns; ++j)
                    values[j] = roundOrder < 0 ? a[i, j].ToString() : Math.Round(a[i, j], roundOrder).ToString();

                values[a.Columns] = roundOrder < 0 ? b[i, 0].ToString() : Math.Round(b[i, 0], roundOrder).ToString();
                lines[i] = string.Join(' ', values);
            }

            return string.Join('\n', lines);
        }
        internal static Matrix DeserializeSystem(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return new Matrix(3, 3, 1);

            string[] lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            string[][] splitedLines = lines.Select(x => x.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries)).ToArray();

            if (splitedLines.Length < 1 || splitedLines.Length != splitedLines.First().Length - 1 ||
                !splitedLines.All(x => x.Length == splitedLines.First().Length))
                return new Matrix(3, 3, 1);

            Matrix system = new Matrix(splitedLines.Length, splitedLines.Length, 1);
            for (int i = 0; i < system.Rows; ++i)
            {
                for (int j = 0; j < system.Columns; ++j)
                    if (double.TryParse(splitedLines[i][j], out double value) &&
                        !double.IsNaN(value) && !double.IsInfinity(value))
                        system[i, j] = value;

                if (double.TryParse(splitedLines[i].Last(), out double extValue) &&
                    !double.IsNaN(extValue) && !double.IsInfinity(extValue))
                    system.SetExtensionElement(i, 0, extValue);
            }

            return system;
        }
    }
}