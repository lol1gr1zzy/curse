namespace JacobiSLAESolver
{
    partial class PrintingForm
    {
        #region Controls

        private ComboBox PrinterNamesComboBox;

        private Button UpdatePrintersButton;
        private Button StartButton;

        private PrintPreviewControl DocumentViewer;

        #endregion

        #region Windows Form Designer generated code

        private System.ComponentModel.IContainer components = null;

        private void InitializeComponent()
        {
            PrinterNamesComboBox = new ComboBox();
            StartButton = new Button();
            DocumentViewer = new PrintPreviewControl();
            UpdatePrintersButton = new Button();
            LoadingLabel = new Label();
            SuspendLayout();
            // 
            // PrinterNamesComboBox
            // 
            PrinterNamesComboBox.FormattingEnabled = true;
            PrinterNamesComboBox.Location = new Point(12, 12);
            PrinterNamesComboBox.Name = "PrinterNamesComboBox";
            PrinterNamesComboBox.Size = new Size(219, 23);
            PrinterNamesComboBox.TabIndex = 0;
            // 
            // StartButton
            // 
            StartButton.Enabled = false;
            StartButton.Location = new Point(318, 11);
            StartButton.Name = "StartButton";
            StartButton.Size = new Size(75, 23);
            StartButton.TabIndex = 1;
            StartButton.Text = "Начать";
            StartButton.UseVisualStyleBackColor = true;
            StartButton.Click += StartButton_Click;
            // 
            // DocumentViewer
            // 
            DocumentViewer.AutoZoom = false;
            DocumentViewer.Location = new Point(12, 41);
            DocumentViewer.Name = "DocumentViewer";
            DocumentViewer.Size = new Size(776, 397);
            DocumentViewer.TabIndex = 2;
            DocumentViewer.Zoom = 1D;
            // 
            // UpdatePrintersButton
            // 
            UpdatePrintersButton.Location = new Point(237, 11);
            UpdatePrintersButton.Name = "UpdatePrintersButton";
            UpdatePrintersButton.Size = new Size(75, 23);
            UpdatePrintersButton.TabIndex = 1;
            UpdatePrintersButton.Text = "Обновить";
            UpdatePrintersButton.UseVisualStyleBackColor = true;
            UpdatePrintersButton.Click += UpdatePrintersButton_Click;
            // 
            // LoadingLabel
            // 
            LoadingLabel.AutoSize = true;
            LoadingLabel.Location = new Point(399, 15);
            LoadingLabel.Name = "LoadingLabel";
            LoadingLabel.Size = new Size(64, 15);
            LoadingLabel.TabIndex = 3;
            LoadingLabel.Text = "Загрузка...";
            // 
            // PrintingForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(LoadingLabel);
            Controls.Add(DocumentViewer);
            Controls.Add(UpdatePrintersButton);
            Controls.Add(StartButton);
            Controls.Add(PrinterNamesComboBox);
            Name = "PrintingForm";
            Text = "PrintingForm";
            ResumeLayout(false);
            PerformLayout();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion

        private Label LoadingLabel;
    }
}