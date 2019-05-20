namespace DomainAnalysis
{
    partial class ImportManualDataWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImportManualDataWindow));
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.gbSelectData = new System.Windows.Forms.GroupBox();
            this.btnSearchTermPath = new System.Windows.Forms.Button();
            this.tbSearchTermPath = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnSelectModelFile = new System.Windows.Forms.Button();
            this.btnSelectRawFileFolder = new System.Windows.Forms.Button();
            this.tbModelFilePath = new System.Windows.Forms.TextBox();
            this.tbRawFileFolder = new System.Windows.Forms.TextBox();
            this.btnLastResult = new System.Windows.Forms.Button();
            this.gbSelectData.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnOK.Location = new System.Drawing.Point(574, 352);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(130, 44);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "Start New Computing";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnCancel.Location = new System.Drawing.Point(186, 352);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(108, 44);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // gbSelectData
            // 
            this.gbSelectData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbSelectData.Controls.Add(this.btnSearchTermPath);
            this.gbSelectData.Controls.Add(this.tbSearchTermPath);
            this.gbSelectData.Controls.Add(this.label3);
            this.gbSelectData.Controls.Add(this.label1);
            this.gbSelectData.Controls.Add(this.label2);
            this.gbSelectData.Controls.Add(this.btnSelectModelFile);
            this.gbSelectData.Controls.Add(this.btnSelectRawFileFolder);
            this.gbSelectData.Controls.Add(this.tbModelFilePath);
            this.gbSelectData.Controls.Add(this.tbRawFileFolder);
            this.gbSelectData.Location = new System.Drawing.Point(17, 11);
            this.gbSelectData.Name = "gbSelectData";
            this.gbSelectData.Size = new System.Drawing.Size(895, 317);
            this.gbSelectData.TabIndex = 2;
            this.gbSelectData.TabStop = false;
            // 
            // btnSearchTermPath
            // 
            this.btnSearchTermPath.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnSearchTermPath.Location = new System.Drawing.Point(770, 256);
            this.btnSearchTermPath.Name = "btnSearchTermPath";
            this.btnSearchTermPath.Size = new System.Drawing.Size(89, 30);
            this.btnSearchTermPath.TabIndex = 9;
            this.btnSearchTermPath.Text = "Select";
            this.btnSearchTermPath.UseVisualStyleBackColor = true;
            this.btnSearchTermPath.Click += new System.EventHandler(this.searchTermPath_Click);
            // 
            // tbSearchTermPath
            // 
            this.tbSearchTermPath.BackColor = System.Drawing.SystemColors.Control;
            this.tbSearchTermPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSearchTermPath.Location = new System.Drawing.Point(22, 265);
            this.tbSearchTermPath.Name = "tbSearchTermPath";
            this.tbSearchTermPath.Size = new System.Drawing.Size(717, 26);
            this.tbSearchTermPath.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(28, 216);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(341, 24);
            this.label3.TabIndex = 7;
            this.label3.Text = "Do you have pre-defined search terms?";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(24, 123);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(403, 24);
            this.label1.TabIndex = 6;
            this.label1.Text = "Please select the manual model file path (.xml):";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(24, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(336, 24);
            this.label2.TabIndex = 5;
            this.label2.Text = "Please select domain document folder:";
            // 
            // btnSelectModelFile
            // 
            this.btnSelectModelFile.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnSelectModelFile.Location = new System.Drawing.Point(770, 169);
            this.btnSelectModelFile.Name = "btnSelectModelFile";
            this.btnSelectModelFile.Size = new System.Drawing.Size(89, 30);
            this.btnSelectModelFile.TabIndex = 4;
            this.btnSelectModelFile.Text = "Select";
            this.btnSelectModelFile.UseVisualStyleBackColor = true;
            this.btnSelectModelFile.Click += new System.EventHandler(this.btnSelectModelFile_Click);
            // 
            // btnSelectRawFileFolder
            // 
            this.btnSelectRawFileFolder.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnSelectRawFileFolder.Location = new System.Drawing.Point(770, 62);
            this.btnSelectRawFileFolder.Name = "btnSelectRawFileFolder";
            this.btnSelectRawFileFolder.Size = new System.Drawing.Size(89, 30);
            this.btnSelectRawFileFolder.TabIndex = 4;
            this.btnSelectRawFileFolder.Text = "Select";
            this.btnSelectRawFileFolder.UseVisualStyleBackColor = true;
            this.btnSelectRawFileFolder.Click += new System.EventHandler(this.btnSelectRawFileFolder_Click);
            // 
            // tbModelFilePath
            // 
            this.tbModelFilePath.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbModelFilePath.Location = new System.Drawing.Point(22, 171);
            this.tbModelFilePath.Name = "tbModelFilePath";
            this.tbModelFilePath.ReadOnly = true;
            this.tbModelFilePath.Size = new System.Drawing.Size(717, 26);
            this.tbModelFilePath.TabIndex = 3;
            // 
            // tbRawFileFolder
            // 
            this.tbRawFileFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbRawFileFolder.Location = new System.Drawing.Point(22, 64);
            this.tbRawFileFolder.Name = "tbRawFileFolder";
            this.tbRawFileFolder.ReadOnly = true;
            this.tbRawFileFolder.Size = new System.Drawing.Size(717, 26);
            this.tbRawFileFolder.TabIndex = 3;
            // 
            // btnLastResult
            // 
            this.btnLastResult.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.btnLastResult.DialogResult = System.Windows.Forms.DialogResult.Retry;
            this.btnLastResult.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnLastResult.Location = new System.Drawing.Point(390, 352);
            this.btnLastResult.Name = "btnLastResult";
            this.btnLastResult.Size = new System.Drawing.Size(108, 44);
            this.btnLastResult.TabIndex = 1;
            this.btnLastResult.Text = "Open Last Result";
            this.btnLastResult.UseVisualStyleBackColor = true;
            // 
            // ImportManualDataWindow
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(924, 407);
            this.Controls.Add(this.gbSelectData);
            this.Controls.Add(this.btnLastResult);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ImportManualDataWindow";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Select Source File";
            this.Load += new System.EventHandler(this.ImportManualDataWindow_Load);
            this.gbSelectData.ResumeLayout(false);
            this.gbSelectData.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox gbSelectData;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnSelectRawFileFolder;
        private System.Windows.Forms.TextBox tbRawFileFolder;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSelectModelFile;
        private System.Windows.Forms.TextBox tbModelFilePath;
        private System.Windows.Forms.Button btnLastResult;
        private System.Windows.Forms.Button btnSearchTermPath;
        private System.Windows.Forms.TextBox tbSearchTermPath;
        private System.Windows.Forms.Label label3;
    }
}