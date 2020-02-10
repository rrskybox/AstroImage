using AstroMath;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;


namespace AstroImage
{
    public partial class FormAstroDisplay : Form
    {

        public FormAstroDisplay(FitsFile af, double targetRA, double targetDec, int zoom, string targetName)
        {
            InitializeComponent();
            Image fit = AstroDisplay.FitsToTargetImage(af, targetRA, targetDec, zoom);
            fitsPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.Text = targetName;
            fitsPictureBox.Image = fit;
            return;
        }

        public FormAstroDisplay(string fitsFileName, double targetRA, double targetDec, int zoom, string targetName)
        {
            InitializeComponent();
            string[] fileList = { fitsFileName };
            Image[] fit = AstroDisplay.FitsFilesToTargetImages(fileList, targetRA, targetDec, zoom);

            fitsPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.Text = targetName;
            fitsPictureBox.Image = fit[0];
            return;
        }

    }




    //    public void Blink(Image baseImage)
    //    {
    //        fitsPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
    //        for (int iap = 0; iap < ImageList.Length; iap++)
    //        {
    //            // blinkPictureBox.Image = blinkList[iap];
    //            fitsPictureBox.Image = ImageList[iap];
    //            Show();
    //            Application.DoEvents();
    //            System.Threading.Thread.Sleep(1000);
    //        }
    //        fitsPictureBox.Image = baseImage;
    //        Show();
    //        Application.DoEvents();

    //        return;
    //    }
    //}



}


