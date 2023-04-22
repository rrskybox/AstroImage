using AstroImage;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using AstroMath;
using System.IO;
using System.Windows.Forms.DataVisualization.Charting;

namespace AstroImage_Test
{
    public partial class TestDashboard : Form
    {
        public FitsFile af;
        public AstroPic testBMP;

        public double centerHoursRA;
        public double centerDegreesDec;
        public int zoomDistance;

        public TestDashboard()
        {
            InitializeComponent();
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            const double defaultRAHours = 0;
            const double defaultDecDegrees = 0;
            LoadFItsButton.BackColor = Color.Salmon;
            openFileDialog1.Filter = "FITS files (*.fit)|*.fit";
            openFileDialog1.ShowDialog();
            fitsFileTextBox.Text = openFileDialog1.FileName;

            af = new AstroImage.FitsFile(fitsFileTextBox.Text, true);

            //monochrome test
            //Bitmap monoBMP =  MonoImage.MakeMonochromeBitmap (af.fitsArray , 255);
            //fitsPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            //fitsPictureBox.Image = monoBMP;
            //return;

            double pixSize = 1;
            if (af.FocalLength != 0) pixSize = (206.265 / af.FocalLength) * af.XpixSz;
            if (af.ObjectRA == 0) af.ObjectRA = defaultRAHours;
            if (af.ObjectDec == 0) af.ObjectDec = defaultDecDegrees;
            testBMP = new AstroPic(af);
            if (af.PlateSolvePS2())
            {
                //ImageFilter.SigmaFastFilter(af, 5, 10);
                testBMP.LinearStretch();
            }
            else
                testBMP.LogStretch();

            Point target;
            target = af.RADECtoImageXY(af.ObjectRA, af.ObjectDec);

            testBMP.AddCrossHair(target, 80, 2);

            Size subSize = new Size(testBMP.PixImage.Width / 4, testBMP.PixImage.Height / 4);

            //Image subImage = af.FitsSubframe(ap.PixImage, target, subSize);
            //Size sizeUp = new Size(ap.PixImage.Size.Width / 2, ap.PixImage.Size.Height / 2);
            //Image baseImage = AstroPic.Zoom(ap.PixImage, sizeUp);
            Image baseImage = testBMP.PixImage;

            FitsPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            FitsPictureBox.Image = baseImage;

            LoadFItsButton.BackColor = Color.Green;
            return;
        }

        private void StackButton_Click(object sender, EventArgs e)
        {
            const double defaultRAHours = 0;
            const double defaultDecDegrees = 0;
            zoomDistance = 0;
            StackButton.BackColor = Color.Salmon;
            openFileDialog1.Filter = "FITS files (*.fit)|*.fit";
            openFileDialog1.ShowDialog();
            string[] fileNames = openFileDialog1.FileNames;
            if (fileNames.Length < 1) return;

            FitsFile[] fsSet = new FitsFile[fileNames.Length];
            for (int fs = 0; fs < fileNames.Length; fs++)
            {
                fitsFileTextBox.Text = Path.GetFileName(fileNames[fs]);
                Show();
                Application.DoEvents();
                fsSet[fs] = new AstroImage.FitsFile(fileNames[fs], true);
            }
            //
            //Stack fsSet
            Stack afstk = new Stack(fsSet);

            if (afstk.FitsStack.ObjectRA == 0) afstk.FitsStack.ObjectRA = defaultRAHours;
            if (afstk.FitsStack.ObjectDec == 0) afstk.FitsStack.ObjectDec = defaultDecDegrees;
            ImageFilter.SigmaFastFilter(afstk.FitsStack, 5, 10);

            testBMP = new AstroPic(afstk.FitsStack);
            testBMP.LinearStretch();

            //target cross hairs
            double pixSize = 1;
            if (afstk.FitsStack.FocalLength != 0)
                pixSize = (206.265 / afstk.FitsStack.FocalLength) * afstk.FitsStack.XpixSz;
            Point target = afstk.FitsStack.RADECtoImageXY(afstk.FitsStack.ObjectRA, afstk.FitsStack.ObjectDec);
            testBMP.AddCrossHair(target, 400, 5);

            Image baseImage = testBMP.ResizeImage(FitsPictureBox.Size, true);
            FitsPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            FitsPictureBox.Image = baseImage;

            Show();
            Application.DoEvents();

            HistoChart.Series.Clear();
            Series hisPnts = new Series();
            
            for (int i = 0; i < afstk.FitsStack.FITS_Hist.Length; i++)
                hisPnts.Points.AddXY(Math.Log10(i+1), Math.Log10(afstk.FitsStack.FITS_Hist[i]+1));
            HistoChart.Series.Add(hisPnts);

            Series hisBounds = new Series();
            hisBounds.Color = Color.Red;
            hisBounds.Points.AddXY(Math.Log10(afstk.FitsStack.HistUpperBound), Math.Log10(UInt16.MaxValue));
            hisBounds.Points.AddXY(Math.Log10(afstk.FitsStack.HistLowerBound), Math.Log10(UInt16.MaxValue));
            HistoChart.Series.Add(hisBounds);

            Show();
            Application.DoEvents();

            StackButton.BackColor = Color.Green;
            return;
        }

        private void TargetButton_Click(object sender, EventArgs e)
        {
            FitsFile af = new FitsFile();
            char[] spc = new char[2];
            spc[0] = ' ';
            spc[1] = ',';
            //radectextbox.Text = "2.6723948953,39.03134476";
            double hoursRA;
            double degreesDec;
            try
            {
                hoursRA = Convert.ToDouble(radectextbox.Text.Split(spc, StringSplitOptions.RemoveEmptyEntries)[0]);
                degreesDec = Convert.ToDouble(radectextbox.Text.Split(spc, StringSplitOptions.RemoveEmptyEntries)[1]);
            }
            catch { return; }
            Point targetXY = af.RADECtoImageXY(hoursRA, degreesDec);
            TargetXYBox.Text = targetXY.X.ToString() + " ,  " + targetXY.Y.ToString();
            //cPos.X -= 40;
            targetXY.X -= 40;
            targetXY.Y += 2;
            testBMP.AddCrossHair(targetXY, 80, 2);
            //Size sizeUp = new Size(testBMP.PixImage.Size.Width / 2, testBMP.PixImage.Size.Height / 2);
            Image baseImage = testBMP.Zoom(testBMP.PixImage.Size.Width / 2);

            FitsPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            FitsPictureBox.Image = baseImage;

            //fitsPictureBox.Image = (Image)AstroPic.AddCrossHair((Bitmap)fitsPictureBox.Image, cPos, 20, 2);
            //
            //
            //double pixSize = 1;
            //if (af.FocalLength != 0) pixSize = (206.265 / af.FocalLength) * af.XpixSz;
            //AstroImage.AstroDisplay astd = new AstroDisplay(af, hoursRA, degreesDec, pixSize );
            //astd.Show();
            Application.DoEvents();
        }

        private void MouseWheel_Handler(object sender, MouseEventArgs e)
        {
            zoomDistance += e.Delta / 3;
            Size subSize = new Size(FitsPictureBox.Size.Width + zoomDistance, FitsPictureBox.Size.Height + zoomDistance);
            Image baseImage = testBMP.ResizeImage(subSize, true);
            FitsPictureBox.Image = baseImage;
            return;
        }

        private void LogStackButton_Click(object sender, EventArgs e)
        {
            const double defaultRAHours = 0;
            const double defaultDecDegrees = 0;
            zoomDistance = 0;
            StackButton.BackColor = Color.Salmon;
            openFileDialog1.Filter = "FITS files (*.fit)|*.fit";
            openFileDialog1.ShowDialog();
            string[] fileNames = openFileDialog1.FileNames;
            if (fileNames.Length < 1) return;

            FitsFile[] fsSet = new FitsFile[fileNames.Length];
            for (int fs = 0; fs < fileNames.Length; fs++)
            {
                fitsFileTextBox.Text = Path.GetFileName(fileNames[fs]);
                Show();
                Application.DoEvents();
                fsSet[fs] = new AstroImage.FitsFile(fileNames[fs], true);
            }
            //
            //Stack fsSet
            Stack afstk = new Stack(fsSet);

            if (af.ObjectRA == 0) af.ObjectRA = defaultRAHours;
            if (af.ObjectDec == 0) af.ObjectDec = defaultDecDegrees;
            AstroImage.ImageFilter.SigmaFastFilter(af, 5, 10);

            testBMP = new AstroPic(af);
            testBMP.LogStretch();

            //target cross hairs
            Point target;
            double pixSize = 1;
            if (af.FocalLength != 0)
                pixSize = (206.265 / af.FocalLength) * af.XpixSz;
            target = af.RADECtoImageXY(af.ObjectRA, af.ObjectDec);
            testBMP.AddCrossHair(target, 400, 5);

            Image baseImage = testBMP.ResizeImage(FitsPictureBox.Size, true);
            FitsPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            FitsPictureBox.Image = baseImage;

            Show();
            Application.DoEvents();

            StackButton.BackColor = Color.Green;
            return;
        }
    }
}


