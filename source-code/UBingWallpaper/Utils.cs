using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System.UserProfile;

namespace UBingWallpaper
{
    public class Utils
    {
        private const int SPI_SETDESKWALLPAPER = 0x14;
        private const int SPIF_UPDATEINIFILE = 0x1;
        private const int SPIF_SENDWININICHANGE = 0x2;
        [DllImport("user32.dll", CharSet = CharSet.Ansi)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        public static bool SetWallpaper(string local_file_uri)
        {
            bool result = false;
            try
            {
                SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, local_file_uri, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
                result = true;
            }
            catch { }
            return result;
        }

        public static bool SetLockScreen(string local_file_uri)
        {
            bool result = false;
            try
            {
                throw new NotImplementedException();
            }
            catch { }
            return result;
        }


        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int description, int reservedValue);

        public static bool IsInternetAvailable()
        {
            int description;
            return InternetGetConnectedState(out description, 0);
        }        
    }
}