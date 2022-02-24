namespace AstroImage
{
    partial class FormAstroDisplay
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
            this.fitsPictureBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.fitsPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // fitsPictureBox
            // 
            this.fitsPictureBox.Location = new System.Drawing.Point(12, 12);
            this.fitsPictureBox.Name = "fitsPictureBox";
            this.fitsPictureBox.Size = new System.Drawing.Size(1044, 756);
            this.fitsPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.fitsPictureBox.TabIndex = 3;
            this.fitsPictureBox.TabStop = false;
            // 
            // FormAstroDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1068, 780);
            this.Controls.Add(this.fitsPictureBox);
            this.Name = "FormAstroDisplay";
            this.Text = "FITS Image";
            ((System.ComponentModel.ISupportInitialize)(this.fitsPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox fitsPictureBox;
    }
}