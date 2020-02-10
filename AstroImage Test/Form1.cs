using AstroImage;
using System;
using System.Drawing;
using System.Windows.Forms;
using AstroMath;

namespace AstroImage_Test
{
    public partial class Form1 : Form
    {
        public FitsFile af;
        public AstroPic ap;
        public double centerHoursRA;
        public double centerDegreesDec;

        public Form1()
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
            if (af.RA == 0) af.RA = defaultRAHours;
            if (af.Dec == 0) af.Dec = defaultDecDegrees;
            ap = new AstroPic(af);
            Point target;
            target = af.RADECtoImageXY(af.RA, af.Dec);
            //target = af.RADECtoImageXY(2.6733599, 39.06333); //NGC 1023 J2000 (Lorenzo)
            //target = af.RADECtoImageXY(2.673519167, 39.06290278); //0.31 center J2000
            //target = af.RADECtoImageXY(2.6733611, 39.06333);// NGC 1023 center J2000
            //target = af.RADECtoImageXY(2.6723948953, 39.03134476); //Tycho something, near NGC 1023
            //target = af.RADECtoImageXY(2.6733599,39.06333);  /NGC 1023
            //target = af.RADECtoImageXY(2.6942859,39.14737);

            target.X -= 40;
            target.Y += 2;

            ap.AddCrossHair(target, 80, 2);

            Size subSize = new Size(ap.PixImage.Width / 4, ap.PixImage.Height / 4);

            Image subImage = af.FitsSubframe(ap.PixImage, target, subSize);
            Size sizeUp = new Size(ap.PixImage.Size.Width / 2, ap.PixImage.Size.Height / 2);
            Image baseImage = AstroPic.Zoom(ap.PixImage, sizeUp);
            //Image baseImage = ap.PixImage;

            fitsPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            fitsPictureBox.Image = baseImage;

            LoadFItsButton.BackColor = Color.Green;
            return;
        }

        private void TargetButton_Click(object sender, EventArgs e)
        {
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
            ap.AddCrossHair(targetXY, 80, 2);
            Size sizeUp = new Size(ap.PixImage.Size.Width / 2, ap.PixImage.Size.Height / 2);
            Image baseImage = AstroPic.Zoom(ap.PixImage, sizeUp);

            fitsPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            fitsPictureBox.Image = baseImage;

            //fitsPictureBox.Image = (Image)AstroPic.AddCrossHair((Bitmap)fitsPictureBox.Image, cPos, 20, 2);
            //
            //
            //double pixSize = 1;
            //if (af.FocalLength != 0) pixSize = (206.265 / af.FocalLength) * af.XpixSz;
            //AstroImage.AstroDisplay astd = new AstroDisplay(af, hoursRA, degreesDec, pixSize );
            //astd.Show();
            Application.DoEvents();
        }


    }
}


