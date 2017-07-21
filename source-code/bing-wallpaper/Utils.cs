using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;

namespace bing_wallpaper
{
    public class Utils
    {
        private const int SPI_SETDESKWALLPAPER = 0x14;
        private const int SPIF_UPDATEINIFILE = 0x1;
        private const int SPIF_SENDWININICHANGE = 0x2;
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        public static bool SetWallpaper(string local_file_uri)
        {
            bool result = false;
            try
            {
                SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, local_file_uri, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
            }
            catch { }
            return result;
        }
    }
}
