using System.Collections.ObjectModel;

namespace JacobiSLAESolver.Models
{
    internal class SolutionResult
    {
        internal Matrix Coefficients { get; private set; }
        internal Matrix Constants { get; private set; }
        internal Matrix Solution
        {
            get => IntermediateSolutions.LastOrDefault();
        }

        internal Collection<double> MathErrors { get; private set; }
        internal Collection<Matrix> IntermediateSolutions { get; private set; }

        internal SolutionResult()
        {
            MathErrors = new Collection<double>();
            IntermediateSolutions = new Collection<Matrix>();
        }

        internal void SetSystem(Matrix coefficients, Matrix constants)
        {
            Coefficients = coefficients.Copy();
            Constants = constants.Copy();
        }
        internal void Clear()
        {
            MathErrors.Clear();
            IntermediateSolutions.Clear();
        }
    }
}