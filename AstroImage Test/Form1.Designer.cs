namespace AstroImage_Test
{
    partial class Form1
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
            this.LoadFItsButton = new System.Windows.Forms.Button();
            this.fitsFileTextBox = new System.Windows.Forms.TextBox();
            this.FitsPictureBox = new System.Windows.Forms.PictureBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.TargetButton = new System.Windows.Forms.Button();
            this.radectextbox = new System.Windows.Forms.TextBox();
            this.imageradectextbox = new System.Windows.Forms.TextBox();
            this.fitsradectextbox = new System.Windows.Forms.TextBox();
            this.TargetXYBox = new System.Windows.Forms.TextBox();
            this.StackButton = new System.Windows.Forms.Button();
            this.SequentialBox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.FitsPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // LoadFItsButton
            // 
            this.LoadFItsButton.Location = new System.Drawing.Point(25, 10);
            this.LoadFItsButton.Name = "LoadFItsButton";
            this.LoadFItsButton.Size = new System.Drawing.Size(75, 23);
            this.LoadFItsButton.TabIndex = 0;
            this.LoadFItsButton.Text = "Load FITS";
            this.LoadFItsButton.UseVisualStyleBackColor = true;
            this.LoadFItsButton.Click += new System.EventHandler(this.LoadButton_Click);
            // 
            // fitsFileTextBox
            // 
            this.fitsFileTextBox.Location = new System.Drawing.Point(143, 12);
            this.fitsFileTextBox.Name = "fitsFileTextBox";
            this.fitsFileTextBox.Size = new System.Drawing.Size(569, 20);
            this.fitsFileTextBox.TabIndex = 1;
            // 
            // FitsPictureBox
            // 
            this.FitsPictureBox.Location = new System.Drawing.Point(25, 103);
            this.FitsPictureBox.Name = "FitsPictureBox";
            this.FitsPictureBox.Size = new System.Drawing.Size(736, 470);
            this.FitsPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.FitsPictureBox.TabIndex = 2;
            this.FitsPictureBox.TabStop = false;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Multiselect = true;
            // 
            // TargetButton
            // 
            this.TargetButton.Location = new System.Drawing.Point(25, 61);
            this.TargetButton.Name = "TargetButton";
            this.TargetButton.Size = new System.Drawing.Size(75, 23);
            this.TargetButton.TabIndex = 4;
            this.TargetButton.Text = "Target";
            this.TargetButton.UseVisualStyleBackColor = true;
            this.TargetButton.Click += new System.EventHandler(this.TargetButton_Click);
            // 
            // radectextbox
            // 
            this.radectextbox.Location = new System.Drawing.Point(143, 64);
            this.radectextbox.Name = "radectextbox";
            this.radectextbox.Size = new System.Drawing.Size(274, 20);
            this.radectextbox.TabIndex = 5;
            // 
            // imageradectextbox
            // 
            this.imageradectextbox.Location = new System.Drawing.Point(438, 38);
            this.imageradectextbox.Name = "imageradectextbox";
            this.imageradectextbox.Size = new System.Drawing.Size(274, 20);
            this.imageradectextbox.TabIndex = 6;
            // 
            // fitsradectextbox
            // 
            this.fitsradectextbox.Location = new System.Drawing.Point(143, 38);
            this.fitsradectextbox.Name = "fitsradectextbox";
            this.fitsradectextbox.Size = new System.Drawing.Size(274, 20);
            this.fitsradectextbox.TabIndex = 7;
            // 
            // TargetXYBox
            // 
            this.TargetXYBox.Location = new System.Drawing.Point(438, 64);
            this.TargetXYBox.Name = "TargetXYBox";
            this.TargetXYBox.Size = new System.Drawing.Size(274, 20);
            this.TargetXYBox.TabIndex = 8;
            // 
            // StackButton
            // 
            this.StackButton.Location = new System.Drawing.Point(718, 12);
            this.StackButton.Name = "StackButton";
            this.StackButton.Size = new System.Drawing.Size(75, 23);
            this.StackButton.TabIndex = 9;
            this.StackButton.Text = "Stack";
            this.StackButton.UseVisualStyleBackColor = true;
            this.StackButton.Click += new System.EventHandler(this.StackButton_Click);
            // 
            // SequentialBox
            // 
            this.SequentialBox.AutoSize = true;
            this.SequentialBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.SequentialBox.Location = new System.Drawing.Point(717, 41);
            this.SequentialBox.Name = "SequentialBox";
            this.SequentialBox.Size = new System.Drawing.Size(76, 17);
            this.SequentialBox.TabIndex = 10;
            this.SequentialBox.Text = "Sequential";
            this.SequentialBox.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(794, 584);
            this.Controls.Add(this.SequentialBox);
            this.Controls.Add(this.StackButton);
            this.Controls.Add(this.TargetXYBox);
            this.Controls.Add(this.fitsradectextbox);
            this.Controls.Add(this.imageradectextbox);
            this.Controls.Add(this.radectextbox);
            this.Controls.Add(this.TargetButton);
            this.Controls.Add(this.FitsPictureBox);
            this.Controls.Add(this.fitsFileTextBox);
            this.Controls.Add(this.LoadFItsButton);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.FitsPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button LoadFItsButton;
        private System.Windows.Forms.TextBox fitsFileTextBox;
        private System.Windows.Forms.PictureBox FitsPictureBox;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button TargetButton;
        private System.Windows.Forms.TextBox radectextbox;
        private System.Windows.Forms.TextBox imageradectextbox;
        private System.Windows.Forms.TextBox fitsradectextbox;
        private System.Windows.Forms.TextBox TargetXYBox;
        private System.Windows.Forms.Button StackButton;
        private System.Windows.Forms.CheckBox SequentialBox;
    }
}

