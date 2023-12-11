using JacobiSLAESolver.Models;
using Spire.Doc;
using Spire.Doc.Documents;

namespace JacobiSLAESolver.Services
{
    internal class DocumentProvider
    {
        internal Document GenerateDocument(SolutionResult solutionResult, int roundOrder)
        {
            Document document = new Document();

            #region Define styles

            ParagraphStyle formulaParagraphStyle = new ParagraphStyle(document);
            formulaParagraphStyle.Name = "FormulaStyle";
            formulaParagraphStyle.CharacterFormat.FontName = "Cambria Math";
            formulaParagraphStyle.CharacterFormat.FontSize = 14;
            document.Styles.Add(formulaParagraphStyle);

            ParagraphStyle baseTextParagraphStyle = new ParagraphStyle(document);
            baseTextParagraphStyle.Name = "BaseTextStyle";
            baseTextParagraphStyle.CharacterFormat.FontName = "Times New Roman";
            baseTextParagraphStyle.CharacterFormat.FontSize = 14;
            document.Styles.Add(baseTextParagraphStyle);

            #endregion

            Section section = document.AddSection();

            #region Source system generating

            Paragraph srcSystemParagraphLablel = section.AddParagraph();
            srcSystemParagraphLablel.ApplyStyle(baseTextParagraphStyle.Name);
            srcSystemParagraphLablel.AppendText("Исходная система уравнений:");
            srcSystemParagraphLablel.Format.AfterAutoSpacing = false;
            srcSystemParagraphLablel.Format.AfterSpacing = 10;

            Paragraph srcSystemParagraph = section.AddParagraph();
            srcSystemParagraph.ApplyStyle(formulaParagraphStyle.Name);
            for (int i = 0; i < solutionResult.Coefficients.Rows; ++i)
            {
                bool needPlus = false;
                for (int j = 0; j < solutionResult.Coefficients.Columns; ++j)
                {
                    if (solutionResult.Coefficients[i, j] == 0)
                        continue;

                    if (solutionResult.Coefficients[i, j] > 0)
                    {
                        if (needPlus)
                            srcSystemParagraph.AppendText(" + ");
                    }
                    else
                    {
                        srcSystemParagraph.AppendText(" – ");
                    }

                    string num = Math.Round(Math.Abs(solutionResult.Coefficients[i, j]), roundOrder).ToString();
                    if (num == "1")
                        num = string.Empty;

                    srcSystemParagraph.AppendText(num + "x");
                    srcSystemParagraph.AppendText((j + 1).ToString()).CharacterFormat.SubSuperScript = SubSuperScript.SubScript;
                    needPlus = true;
                }

                srcSystemParagraph.AppendText(" = ");
                srcSystemParagraph.AppendText(Math.Round(solutionResult.Constants[i, 0], roundOrder).ToString().Replace('-', '–'));
                srcSystemParagraph.AppendBreak(BreakType.LineBreak);
            }

            #endregion

            #region Transformed system generating

            Paragraph transformedSystemFormulaParagraphLablel = section.AddParagraph();
            transformedSystemFormulaParagraphLablel.ApplyStyle(baseTextParagraphStyle.Name);
            transformedSystemFormulaParagraphLablel.AppendText("Формулы преобразования для решения методом Якоби:");
            transformedSystemFormulaParagraphLablel.Format.AfterAutoSpacing = false;
            transformedSystemFormulaParagraphLablel.Format.AfterSpacing = 10;

            Paragraph transformedSystemFormulaParagraph = section.AddParagraph();
            transformedSystemFormulaParagraph.ApplyStyle(formulaParagraphStyle.Name);
            //A -> AtA, B -> AtB
            transformedSystemFormulaParagraph.AppendText("A → A");
            transformedSystemFormulaParagraph.AppendText("T").CharacterFormat.SubSuperScript = SubSuperScript.SuperScript;
            transformedSystemFormulaParagraph.AppendText("A;\tB → A");
            transformedSystemFormulaParagraph.AppendText("T").CharacterFormat.SubSuperScript = SubSuperScript.SuperScript;
            transformedSystemFormulaParagraph.AppendText("B;");
            transformedSystemFormulaParagraph.AppendBreak(BreakType.LineBreak);
            //A -> D-1 * (D - A), B = D-1 * B
            transformedSystemFormulaParagraph.AppendText("A → D");
            transformedSystemFormulaParagraph.AppendText("–1").CharacterFormat.SubSuperScript = SubSuperScript.SuperScript;
            transformedSystemFormulaParagraph.AppendText("(D – A);\tB → D");
            transformedSystemFormulaParagraph.AppendText("–1").CharacterFormat.SubSuperScript = SubSuperScript.SuperScript;
            transformedSystemFormulaParagraph.AppendText("B,");

            transformedSystemFormulaParagraph.Format.AfterAutoSpacing = false;
            transformedSystemFormulaParagraph.Format.AfterSpacing = 10;

            Paragraph transformedSystemDescriptionParagraph = section.AddParagraph();
            transformedSystemDescriptionParagraph.ApplyStyle(baseTextParagraphStyle.Name);
            transformedSystemDescriptionParagraph.AppendText("где D – матрица, составленная из диагональных коэффициентов матрицы A.");
            transformedSystemDescriptionParagraph.AppendBreak(BreakType.LineBreak);

            Matrix a = solutionResult.Coefficients.Copy();
            Matrix b = solutionResult.Constants.Copy();

            b = a.TransposeMatrix * b;
            a = a.TransposeMatrix * a;

            Matrix d = new Matrix(a.Rows, a.Columns);
            for (int c = 0; c < a.Columns; ++c)
                d[c, c] = a[c, c];

            a = d.ReverseMatrix * (d - a);
            b = d.ReverseMatrix * b;

            Paragraph transformedSystemParagraphLabel = section.AddParagraph();
            transformedSystemParagraphLabel.ApplyStyle(baseTextParagraphStyle.Name);
            transformedSystemParagraphLabel.AppendText("Приведённая система:");
            transformedSystemParagraphLabel.Format.AfterAutoSpacing = false;
            transformedSystemParagraphLabel.Format.AfterSpacing = 10;

            Paragraph transformedSystemParagraph = section.AddParagraph();
            transformedSystemParagraph.ApplyStyle(baseTextParagraphStyle.Name);
            for (int i = 0; i < a.Rows; ++i)
            {
                bool needPlus = false;
                for (int j = 0; j < a.Columns; ++j)
                {
                    if (a[i, j] == 0)
                        continue;

                    if (a[i, j] > 0)
                    {
                        if (needPlus)
                            transformedSystemParagraph.AppendText(" + ");
                    }
                    else
                    {
                        transformedSystemParagraph.AppendText(" – ");
                    }

                    string num = Math.Round(Math.Abs(a[i, j]), roundOrder).ToString();
                    if (num == "1")
                        num = string.Empty;

                    transformedSystemParagraph.AppendText(num + "x");
                    transformedSystemParagraph.AppendText((j + 1).ToString()).CharacterFormat.SubSuperScript = SubSuperScript.SubScript;
                    needPlus = true;
                }

                transformedSystemParagraph.AppendText(" = ");
                transformedSystemParagraph.AppendText(Math.Round(b[i, 0], roundOrder).ToString().Replace('-', '–'));
                transformedSystemParagraph.AppendBreak(BreakType.LineBreak);
            }

            #endregion

            #region Table generating

            Paragraph tableParagraphLablel = section.AddParagraph();
            tableParagraphLablel.ApplyStyle(baseTextParagraphStyle.Name);
            tableParagraphLablel.AppendText("Таблица последовательных приближений");
            tableParagraphLablel.Format.AfterAutoSpacing = false;
            tableParagraphLablel.Format.AfterSpacing = 10;

            Table table = section.AddTable(true);
            table.ResetCells(solutionResult.IntermediateSolutions.Count + 1, solutionResult.Constants.Rows + 2);

            TableRow headers = table.Rows[0];

            Paragraph iterationsHeaderParagraph = headers.Cells[0].AddParagraph();
            iterationsHeaderParagraph.ApplyStyle(formulaParagraphStyle.Name);
            iterationsHeaderParagraph.AppendText("N");
            iterationsHeaderParagraph.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;
            headers.Cells[0].CellFormat.VerticalAlignment = VerticalAlignment.Middle;

            for (int i = 0; i < solutionResult.Constants.Rows; ++i)
            {
                Paragraph variableHeaderParagraph = headers.Cells[i + 1].AddParagraph();
                variableHeaderParagraph.ApplyStyle(formulaParagraphStyle.Name);
                variableHeaderParagraph.AppendText("x");
                variableHeaderParagraph.AppendText((i + 1).ToString()).CharacterFormat.SubSuperScript = SubSuperScript.SubScript;
                variableHeaderParagraph.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;
                headers.Cells[i + 1].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
            }

            Paragraph errorHeaderParagraph = headers.Cells[headers.Cells.Count - 1].AddParagraph();
            errorHeaderParagraph.ApplyStyle(formulaParagraphStyle.Name);
            errorHeaderParagraph.AppendText("|ε|");
            errorHeaderParagraph.AppendText("max").CharacterFormat.SubSuperScript = SubSuperScript.SubScript;
            errorHeaderParagraph.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;
            headers.Cells[headers.Cells.Count - 1].CellFormat.VerticalAlignment = VerticalAlignment.Middle;

            for (int i = 0; i < solutionResult.IntermediateSolutions.Count; ++i)
            {
                TableRow row = table.Rows[i + 1];

                Paragraph iterationsParagraph = row.Cells[0].AddParagraph();
                iterationsParagraph.ApplyStyle(baseTextParagraphStyle.Name);
                iterationsParagraph.AppendText((i + 1).ToString());
                iterationsParagraph.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;
                row.Cells[0].CellFormat.VerticalAlignment = VerticalAlignment.Middle;

                for (int j = 0; j < solutionResult.Constants.Rows; ++j)
                {
                    Paragraph variableParagraph = row.Cells[j + 1].AddParagraph();
                    variableParagraph.ApplyStyle(baseTextParagraphStyle.Name);
                    variableParagraph.AppendText(Math.Round(solutionResult.IntermediateSolutions[i][j, 0], roundOrder).ToString().Replace('-', '–'));
                    variableParagraph.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;
                    row.Cells[j + 1].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                }

                Paragraph errorParagraph = row.Cells[row.Cells.Count - 1].AddParagraph();
                errorParagraph.ApplyStyle(baseTextParagraphStyle.Name);
                errorParagraph.AppendText(Math.Round(solutionResult.MathErrors[i], roundOrder).ToString());
                errorParagraph.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;
                row.Cells[headers.Cells.Count - 1].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
            }

            #endregion

            return document;
        }
    }
}