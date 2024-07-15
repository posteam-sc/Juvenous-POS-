namespace POS
{
    partial class PackageUsedHistoryExport
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PackageUsedHistoryExport));
            this.label1 = new System.Windows.Forms.Label();
            this.lblExportDate = new System.Windows.Forms.Label();
            this.lblExportProgress = new System.Windows.Forms.Label();
            this.ExportProgressBar = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(57, 37);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 17);
            this.label1.TabIndex = 12;
            this.label1.Text = "Export Date:";
            // 
            // lblExportDate
            // 
            this.lblExportDate.AutoSize = true;
            this.lblExportDate.Location = new System.Drawing.Point(147, 37);
            this.lblExportDate.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblExportDate.Name = "lblExportDate";
            this.lblExportDate.Size = new System.Drawing.Size(12, 17);
            this.lblExportDate.TabIndex = 13;
            this.lblExportDate.Text = " ";
            // 
            // lblExportProgress
            // 
            this.lblExportProgress.AutoSize = true;
            this.lblExportProgress.Location = new System.Drawing.Point(57, 78);
            this.lblExportProgress.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblExportProgress.Name = "lblExportProgress";
            this.lblExportProgress.Size = new System.Drawing.Size(251, 17);
            this.lblExportProgress.TabIndex = 14;
            this.lblExportProgress.Text = "Exporting Data to SAP. Please Wait.....";
            // 
            // ExportProgressBar
            // 
            this.ExportProgressBar.Location = new System.Drawing.Point(61, 128);
            this.ExportProgressBar.Margin = new System.Windows.Forms.Padding(4);
            this.ExportProgressBar.Maximum = 9;
            this.ExportProgressBar.Name = "ExportProgressBar";
            this.ExportProgressBar.Size = new System.Drawing.Size(456, 28);
            this.ExportProgressBar.Step = 1;
            this.ExportProgressBar.TabIndex = 15;
            // 
            // PackageUsedHistoryExport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(548, 214);
            this.Controls.Add(this.ExportProgressBar);
            this.Controls.Add(this.lblExportProgress);
            this.Controls.Add(this.lblExportDate);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PackageUsedHistoryExport";
            this.Text = "Package Used Export";
            this.Load += new System.EventHandler(this.PackageUsedHistoryExport_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblExportDate;
        private System.Windows.Forms.Label lblExportProgress;
        private System.Windows.Forms.ProgressBar ExportProgressBar;
    }
}