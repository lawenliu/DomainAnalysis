namespace DomainAnalysis.Resources.Dialogs
{
    partial class SummaryShow
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
            this.quality = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.component = new System.Windows.Forms.Label();
            this.summaryBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(166, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Summary between Quality:";
            // 
            // quality
            // 
            this.quality.AutoSize = true;
            this.quality.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.quality.Location = new System.Drawing.Point(184, 30);
            this.quality.Name = "quality";
            this.quality.Size = new System.Drawing.Size(47, 15);
            this.quality.TabIndex = 1;
            this.quality.Text = "label2";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(271, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "and Component:";
            // 
            // component
            // 
            this.component.AutoSize = true;
            this.component.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.component.Location = new System.Drawing.Point(392, 31);
            this.component.Name = "component";
            this.component.Size = new System.Drawing.Size(47, 15);
            this.component.TabIndex = 3;
            this.component.Text = "label3";
            // 
            // summaryBox
            // 
            this.summaryBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.summaryBox.Location = new System.Drawing.Point(15, 69);
            this.summaryBox.Multiline = true;
            this.summaryBox.Name = "summaryBox";
            this.summaryBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.summaryBox.Size = new System.Drawing.Size(590, 198);
            this.summaryBox.TabIndex = 4;
            // 
            // SummaryShow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(617, 283);
            this.Controls.Add(this.summaryBox);
            this.Controls.Add(this.component);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.quality);
            this.Controls.Add(this.label1);
            this.Name = "SummaryShow";
            this.Text = "SummaryShow";
            this.Load += new System.EventHandler(this.SummaryShow_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label quality;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label component;
        private System.Windows.Forms.TextBox summaryBox;
    }
}