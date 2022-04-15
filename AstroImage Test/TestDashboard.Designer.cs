namespace AstroImage_Test
{
    partial class TestDashboard
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
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
            this.LogStackButton = new System.Windows.Forms.Button();
            this.HistoChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            ((System.ComponentModel.ISupportInitialize)(this.FitsPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.HistoChart)).BeginInit();
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
            this.FitsPictureBox.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.MouseWheel_Handler);
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
            // LogStackButton
            // 
            this.LogStackButton.Location = new System.Drawing.Point(718, 38);
            this.LogStackButton.Name = "LogStackButton";
            this.LogStackButton.Size = new System.Drawing.Size(75, 23);
            this.LogStackButton.TabIndex = 10;
            this.LogStackButton.Text = "LogStack";
            this.LogStackButton.UseVisualStyleBackColor = true;
            this.LogStackButton.Click += new System.EventHandler(this.LogStackButton_Click);
            // 
            // HistoChart
            // 
            chartArea1.AxisX.Minimum = 0D;
            chartArea1.Name = "ChartArea1";
            this.HistoChart.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.HistoChart.Legends.Add(legend1);
            this.HistoChart.Location = new System.Drawing.Point(28, 593);
            this.HistoChart.Name = "HistoChart";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.HistoChart.Series.Add(series1);
            this.HistoChart.Size = new System.Drawing.Size(732, 281);
            this.HistoChart.TabIndex = 11;
            this.HistoChart.Text = "HistoChart";
            // 
            // TestDashboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(794, 902);
            this.Controls.Add(this.HistoChart);
            this.Controls.Add(this.LogStackButton);
            this.Controls.Add(this.StackButton);
            this.Controls.Add(this.TargetXYBox);
            this.Controls.Add(this.fitsradectextbox);
            this.Controls.Add(this.imageradectextbox);
            this.Controls.Add(this.radectextbox);
            this.Controls.Add(this.TargetButton);
            this.Controls.Add(this.FitsPictureBox);
            this.Controls.Add(this.fitsFileTextBox);
            this.Controls.Add(this.LoadFItsButton);
            this.Name = "TestDashboard";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.FitsPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.HistoChart)).EndInit();
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
        private System.Windows.Forms.Button LogStackButton;
        private System.Windows.Forms.DataVisualization.Charting.Chart HistoChart;
    }
}

