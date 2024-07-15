namespace POS
{
    partial class PostJson
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PostJson));
            this.Lable1 = new System.Windows.Forms.Label();
            this.lblAPIName = new System.Windows.Forms.Label();
            this.txtJson = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.SuspendLayout();
            // 
            // Lable1
            // 
            this.Lable1.AutoSize = true;
            this.Lable1.Location = new System.Drawing.Point(24, 42);
            this.Lable1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Lable1.Name = "Lable1";
            this.Lable1.Size = new System.Drawing.Size(78, 17);
            this.Lable1.TabIndex = 29;
            this.Lable1.Text = "API Name: ";
            // 
            // lblAPIName
            // 
            this.lblAPIName.AutoSize = true;
            this.lblAPIName.Location = new System.Drawing.Point(105, 42);
            this.lblAPIName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblAPIName.Name = "lblAPIName";
            this.lblAPIName.Size = new System.Drawing.Size(0, 17);
            this.lblAPIName.TabIndex = 32;
            // 
            // txtJson
            // 
            this.txtJson.Location = new System.Drawing.Point(48, 76);
            this.txtJson.Margin = new System.Windows.Forms.Padding(4);
            this.txtJson.Multiline = true;
            this.txtJson.Name = "txtJson";
            this.txtJson.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtJson.Size = new System.Drawing.Size(571, 434);
            this.txtJson.TabIndex = 33;
            // 
            // btnSave
            // 
            this.btnSave.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnSave.FlatAppearance.BorderSize = 0;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Image = global::POS.Properties.Resources.save_big;
            this.btnSave.Location = new System.Drawing.Point(245, 529);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(151, 57);
            this.btnSave.TabIndex = 34;
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.CheckPathExists = false;
            this.saveFileDialog1.DefaultExt = "txt";
            this.saveFileDialog1.Filter = "\"txt files (*.txt)|*.txt|All files (*.*)|*.*\";  ";
            this.saveFileDialog1.InitialDirectory = " @\"C:\\\"";
            this.saveFileDialog1.RestoreDirectory = true;
            this.saveFileDialog1.Title = "Save Json To A Text File";
            // 
            // PostJson
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(644, 629);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtJson);
            this.Controls.Add(this.lblAPIName);
            this.Controls.Add(this.Lable1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PostJson";
            this.Text = "Post Json";
            this.Load += new System.EventHandler(this.PostJson_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Lable1;
        private System.Windows.Forms.Label lblAPIName;
        private System.Windows.Forms.TextBox txtJson;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
    }
}