namespace DomainAnalysis
{
    partial class ImportAutoDataWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImportAutoDataWindow));
            this.gbSelectData = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnSelectFolder = new System.Windows.Forms.Button();
            this.tbRawFileFolder = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnLastResult = new System.Windows.Forms.Button();
            this.gbSelectData.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbSelectData
            // 
            this.gbSelectData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbSelectData.Controls.Add(this.label2);
            this.gbSelectData.Controls.Add(this.btnSelectFolder);
            this.gbSelectData.Controls.Add(this.tbRawFileFolder);
            this.gbSelectData.Location = new System.Drawing.Point(18, 12);
            this.gbSelectData.Name = "gbSelectData";
            this.gbSelectData.Size = new System.Drawing.Size(873, 124);
            this.gbSelectData.TabIndex = 0;
            this.gbSelectData.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(23, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(336, 24);
            this.label2.TabIndex = 5;
            this.label2.Text = "Please select domain document folder:";
            // 
            // btnSelectFolder
            // 
            this.btnSelectFolder.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnSelectFolder.Location = new System.Drawing.Point(769, 77);
            this.btnSelectFolder.Name = "btnSelectFolder";
            this.btnSelectFolder.Size = new System.Drawing.Size(89, 30);
            this.btnSelectFolder.TabIndex = 4;
            this.btnSelectFolder.Text = "Select";
            this.btnSelectFolder.UseVisualStyleBackColor = true;
            this.btnSelectFolder.Click += new System.EventHandler(this.btnSelectFolder_Click);
            // 
            // tbRawFileFolder
            // 
            this.tbRawFileFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbRawFileFolder.Location = new System.Drawing.Point(21, 79);
            this.tbRawFileFolder.Name = "tbRawFileFolder";
            this.tbRawFileFolder.ReadOnly = true;
            this.tbRawFileFolder.Size = new System.Drawing.Size(717, 26);
            this.tbRawFileFolder.TabIndex = 3;
            // 
            // btnOK
            // 
            this.btnOK.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnOK.Location = new System.Drawing.Point(549, 192);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(123, 44);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "Start New Computing";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnCancel.Location = new System.Drawing.Point(195, 192);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(113, 44);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnLastResult
            // 
            this.btnLastResult.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.btnLastResult.DialogResult = System.Windows.Forms.DialogResult.Retry;
            this.btnLastResult.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnLastResult.Location = new System.Drawing.Point(373, 192);
            this.btnLastResult.Name = "btnLastResult";
            this.btnLastResult.Size = new System.Drawing.Size(108, 44);
            this.btnLastResult.TabIndex = 1;
            this.btnLastResult.Text = "Open Last Result";
            this.btnLastResult.UseVisualStyleBackColor = true;
            // 
            // ImportAutoDataWindow
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(915, 272);
            this.Controls.Add(this.btnLastResult);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.gbSelectData);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ImportAutoDataWindow";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Select Source File";
            this.Load += new System.EventHandler(this.ImportAutoDataWindow_Load);
            this.gbSelectData.ResumeLayout(false);
            this.gbSelectData.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbSelectData;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnSelectFolder;
        private System.Windows.Forms.TextBox tbRawFileFolder;
        private System.Windows.Forms.Button btnLastResult;
    }
}