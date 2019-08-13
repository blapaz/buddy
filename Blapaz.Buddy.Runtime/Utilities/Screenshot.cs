using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace Blapaz.Buddy.Runtime.Utilities
{
    public class Screenshot
    {
        public static string Capture(string fileName, ImageFormat fileType)
        {
            Rectangle bounds = Screen.GetBounds(Point.Empty);

            using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                }
                bitmap.Save(Path.GetTempPath() + fileName, fileType);
            }

            return fileName;
        }

        public static void Open(string fileName)
        {
            Process myProcess = new Process();
            myProcess.StartInfo.FileName = Path.GetTempPath() + fileName;
            myProcess.Start();
        }

        public static void CaptureAndOpen(string fileName, ImageFormat fileType)
        {
            Open(Capture(fileName, fileType));
        }
    }
}
