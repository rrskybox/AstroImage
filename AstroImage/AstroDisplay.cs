using System.Drawing;

namespace AstroImage
{
    public class AstroDisplay
    {
        private AstroPic ap;
        private FitsFile af;

        public Image PixImage => ap.PixImage;

        public AstroDisplay(ref FitsFile adFitsFile)
        {
            ap = new AstroPic(ref adFitsFile);
            FitsToTargetImage();
        }

        public Image FitsToTargetImage()
        {
            //ImageFilter.SigmaFastFilter(af, 5, 10);
            //ap.ArcSinhStretch();
            ap.LinearStretch();
            return ap.PixImage;
       }

        public void AddCrossHair(Point target, int crossWidth, int lineWidth) => ap.AddCrossHair(target, crossWidth, lineWidth);

        public Image Zoom(int zoom) => ap.Zoom(zoom);
      
        public Image FitsToTargetImageXY(double targetX, double targetDecY, int zoom)
        {
            ap.ArcSinhStretch();
            ap.PixImage = ap.AddCrossHair(new Point((int)targetX, (int)targetDecY), 80, 8);
            return Zoom(zoom);
        }

        public static Image[] FitsFilesToTargetImages(string[] fileList, double targetRAHrs, double targetDecDeg, int zoom)
        {

            int fileCount = fileList.Length;
            //Open a fitsfile object for each
            FitsFile[] fitsList = new FitsFile[fileCount];
            AstroPic[] apList = new AstroPic[fileCount];
            Image[] blinkList = new Image[fileCount];

            for (int i = 0; i < fileCount; i++)
            {
                fitsList[i] = new FitsFile(fileList[i], true);
                if ((targetRAHrs == 0) && (targetDecDeg == 0))
                {
                    targetRAHrs = fitsList[i].ObjectRA;
                    targetDecDeg = fitsList[i].ObjectDec;
                }
                apList[i] = new AstroPic(ref fitsList[i]);
                apList[i].ArcSinhStretch();

                Point targetXY = fitsList[i].RADECtoImageXY(targetRAHrs, targetDecDeg);
                apList[i].AddCrossHair(targetXY, 80, 2);
                Size framesize = new Size(apList[i].PixImage.Width / zoom, apList[i].PixImage.Height / zoom);
                double rotation = -fitsList[i].PA;
                blinkList[i] = apList[i].RotateTranslateCrop(targetXY, rotation, framesize);
                //Size sizeUp = new Size(blinkList[i].Size.Width * zoom, blinkList[i].Size.Height * zoom);
                blinkList[i] = apList[i].Zoom(zoom);
            }

            return blinkList;
        }

    }
}