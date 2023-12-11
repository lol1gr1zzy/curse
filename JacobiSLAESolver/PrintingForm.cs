using Microsoft.WindowsAPICodePack.Dialogs;
using PdfiumViewer;
using SautinSoft.Document;
using Spire.Doc;
using System.Collections.ObjectModel;
using System.Drawing.Printing;
using System.Management;

namespace JacobiSLAESolver
{
    public partial class PrintingForm : Form
    {
        private Document Document;
        private PrintDocument PrintDocument;

        private byte[] Docx;
        private byte[] Pdf;

        private Collection<string> PrinterNames;

        public PrintingForm()
        {
            InitializeComponent();

            PrinterNames = new Collection<string>();
            UpdatePrinters();
        }

        private void GenerateFiles()
        {
            //Create .docx data
            using (MemoryStream ms = new MemoryStream())
            {
                Document.SaveToStream(ms, FileFormat.Docx);
                Docx = ms.ToArray();
            }

            //Convert .docx to .pdf and generate .pdf data
            using (MemoryStream docxStream = new MemoryStream(Docx))
            {
                try
                {
                    DocumentCore document = DocumentCore.Load(docxStream, new DocxLoadOptions());
                    using (MemoryStream pdfStream = new MemoryStream())
                    {
                        document.Save(pdfStream, new PdfSaveOptions());
                        Pdf = pdfStream.ToArray();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{ex.Message}\n\n{nameof(ex.StackTrace)}:\n{ex.StackTrace}",
                                    "Ошибка конвертации файла", MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                    Pdf = new byte[0];
                }
            }

            //Activate save/print button
            this.BeginInvoke(() =>
            {
                StartButton.Enabled = true;
                LoadingLabel.Visible = false;
            });
        }
        private void RenderUI()
        {
            PrintDocument = Document.PrintDocument;

            //Загрузка предпросмотра документа
            this.BeginInvoke(() =>
            {
                DocumentViewer.Document = PrintDocument;
                DocumentViewer.Rows = Document.GetPageCount();
            });
        }
        private void UpdatePrinters()
        {
            string selectedPrinter = PrinterNamesComboBox.SelectedItem as string;

            PrinterNames.Clear();
            PrinterNamesComboBox.Items.Clear();

            ManagementScope objScope = new ManagementScope(ManagementPath.DefaultPath);
            objScope.Connect();

            SelectQuery selectQuery = new SelectQuery();
            selectQuery.QueryString = "Select * from win32_Printer";
            ManagementObjectSearcher MOS = new ManagementObjectSearcher(objScope, selectQuery);
            ManagementObjectCollection MOC = MOS.Get();
            foreach (ManagementObject mo in MOC)
                PrinterNames.Add(mo["Name"].ToString());

            PrinterNamesComboBox.Items.AddRange(PrinterNames.ToArray());
            if (!PrinterNames.Any(x => x == PrinterNamesComboBox.Text))
                PrinterNamesComboBox.Text = null;
        }

        internal void DefineData(Document document)
        {
            Document = document;

            GenerateFiles();
            RenderUI();
        }

        private void UpdatePrintersButton_Click(object sender, EventArgs e)
        {
            UpdatePrinters();
        }

        private string AddExtension(string path, string extension)
        {
            if (Path.GetExtension(path).Equals(extension, StringComparison.InvariantCultureIgnoreCase))
                return path;
            else
                return path + extension;
        }
        private void SaveAsFile()
        {
            CommonSaveFileDialog dlg = new CommonSaveFileDialog();
            dlg.Filters.Add(new CommonFileDialogFilter("Pdf documents", ".pdf"));   //Type index = 1
            dlg.Filters.Add(new CommonFileDialogFilter("Word documents", ".docx")); //Type index = 2
            dlg.DefaultFileName = "Report";

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                try
                {
                    if (dlg.SelectedFileTypeIndex == 1)
                        using (FileStream fs = new FileStream(AddExtension(dlg.FileName, ".pdf"), FileMode.Create, FileAccess.Write))
                        using (BinaryWriter bw = new BinaryWriter(fs))
                            bw.Write(Pdf);
                    else if (dlg.SelectedFileTypeIndex == 2)
                        using (FileStream fs = new FileStream(AddExtension(dlg.FileName, ".docx"), FileMode.Create, FileAccess.Write))
                        using (BinaryWriter bw = new BinaryWriter(fs))
                            bw.Write(Docx);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{ex.Message}\n\n{nameof(ex.StackTrace)}:\n{ex.StackTrace}",
                                    "Ошибка сохранения файла", MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                }
            }

        }
        private void StartButton_Click(object sender, EventArgs e)
        {
            if (!PrinterNames.Any(x => x == PrinterNamesComboBox.Text))
            {
                SaveAsFile();
                return;
            }

            try
            {
                // Create the printer settings for our printer
                PrinterSettings printerSettings = new PrinterSettings();
                printerSettings.PrinterName = PrinterNamesComboBox.Text;
                printerSettings.Copies = 1;

                // Create our page settings for the paper size selected
                PageSettings pageSettings = new PageSettings(printerSettings);
                pageSettings.Margins = new Margins(0, 0, 0, 0);

                foreach (PaperSize paperSize in printerSettings.PaperSizes)
                {
                    if (paperSize.PaperName.Equals("A4", StringComparison.InvariantCultureIgnoreCase))
                    {
                        pageSettings.PaperSize = paperSize;
                        break;
                    }
                }

                // Now print the PDF document
                using (MemoryStream ms = new MemoryStream(Pdf))
                {
                    using (PdfDocument document = PdfDocument.Load(ms))
                    {
                        using (PrintDocument printDocument = document.CreatePrintDocument())
                        {
                            printDocument.PrinterSettings = printerSettings;
                            printDocument.DefaultPageSettings = pageSettings;
                            printDocument.PrintController = new StandardPrintController();
                            printDocument.Print();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}\n\n{nameof(ex.StackTrace)}:\n{ex.StackTrace}",
                                "Ошибка печати", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}