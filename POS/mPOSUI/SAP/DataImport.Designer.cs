namespace POS
{
    partial class DataImport
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.lblImportProgress = new System.Windows.Forms.Label();
            this.ImportProgressBar = new System.Windows.Forms.ProgressBar();
            this.lblImportDate = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(57, 37);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 17);
            this.label1.TabIndex = 7;
            this.label1.Text = "Import Date:";
            // 
            // lblImportProgress
            // 
            this.lblImportProgress.AutoSize = true;
            this.lblImportProgress.Location = new System.Drawing.Point(57, 78);
            this.lblImportProgress.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblImportProgress.Name = "lblImportProgress";
            this.lblImportProgress.Size = new System.Drawing.Size(270, 17);
            this.lblImportProgress.TabIndex = 8;
            this.lblImportProgress.Text = "Importing Data From SAP. Please Wait.....";
            // 
            // ImportProgressBar
            // 
            this.ImportProgressBar.Location = new System.Drawing.Point(61, 128);
            this.ImportProgressBar.Margin = new System.Windows.Forms.Padding(4);
            this.ImportProgressBar.Maximum = 9;
            this.ImportProgressBar.Name = "ImportProgressBar";
            this.ImportProgressBar.Size = new System.Drawing.Size(456, 28);
            this.ImportProgressBar.Step = 1;
            this.ImportProgressBar.TabIndex = 9;
            // 
            // lblImportDate
            // 
            this.lblImportDate.AutoSize = true;
            this.lblImportDate.Location = new System.Drawing.Point(147, 37);
            this.lblImportDate.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblImportDate.Name = "lblImportDate";
            this.lblImportDate.Size = new System.Drawing.Size(0, 17);
            this.lblImportDate.TabIndex = 10;
            // 
            // DataImport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(548, 214);
            this.Controls.Add(this.lblImportDate);
            this.Controls.Add(this.ImportProgressBar);
            this.Controls.Add(this.lblImportProgress);
            this.Controls.Add(this.label1);
            this.Name = "DataImport";
            this.Text = "Data Import";
            this.Load += new System.EventHandler(this.DataImport_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblImportProgress;
        private System.Windows.Forms.ProgressBar ImportProgressBar;
        private System.Windows.Forms.Label lblImportDate;
    }
}