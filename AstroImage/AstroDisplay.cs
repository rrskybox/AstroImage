using System.Drawing;

namespace AstroImage
{
    public partial class AstroDisplay
    {
        public static Image FitsToTargetImage(FitsFile af, double targetRA, double targetDec, int zoom)
        {
            AstroPic ap = new AstroPic(af);
            ap.LinearStretch();

            if (ap.PixImage == null) return null;
            if ((targetRA == 0) && (targetDec == 0))
            {
                targetRA = af.RA;
                targetDec = af.Dec;
            }
            Point target = af.RADECtoImageXY(targetRA, targetDec);
            target.X -= 40;
            target.Y -= 2;
            ap.PixImage = ap.AddCrossHair(target, 80, 2);
            Size sizeUp = new Size(ap.PixImage.Size.Width / zoom, ap.PixImage.Size.Height / zoom);
            return AstroPic.Zoom(ap.PixImage, sizeUp);
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
                    targetRAHrs = fitsList[i].RA;
                    targetDecDeg = fitsList[i].Dec;
                }
                apList[i] = new AstroPic(fitsList[i]);
                apList[i].LinearStretch();

                Point center = fitsList[i].RADECtoImageXY(targetRAHrs, targetDecDeg);
                center.X += -40;
                center.Y += -2;
                apList[i].AddCrossHair(center, 80, 2);
                //Point center = fitsList[i].RADECtoImageXY(fitsList[i].RA, fitsList[i].Dec);
                Size framesize = new Size(apList[i].PixImage.Width / zoom, apList[i].PixImage.Height / zoom);
                double rotation = -fitsList[i].PA;
                blinkList[i] = apList[i].RotateTranslateCrop(center, rotation, framesize);
                Size sizeUp = new Size(blinkList[i].Size.Width * zoom, blinkList[i].Size.Height * zoom);
                blinkList[i] = AstroPic.Zoom(blinkList[i], sizeUp);
            }

            return blinkList;
        }
    }
}