using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System.UserProfile;

namespace UBingWallpaper
{
    public class BingUtils
    {
        public static readonly string BING_IMG_URL_JSON = "http://www.bing.com/HPImageArchive.aspx?format=js&idx=0&n=1&mkt=";
        public static readonly string BING_URL = "http://www.bing.com";
        public static readonly string BING_RESOLUTION = "_1920x1080.jpg";
#if DEBUG
        public static string LOCAL_IMAGE_FILE_JPG = System.IO.Path.GetTempPath() + "bing-wallpaper-test.jpg";
        public static string LOCAL_CONFIGURATION_FILE_JSON = System.IO.Path.GetTempPath() + "bing-wallpaper-test.json";
        public static string BING_LOG = System.IO.Path.GetTempPath() + "bing-wallpaper-test.log";

#else
        public static string LOCAL_IMAGE_FILE_JPG = System.IO.Path.GetTempPath() + "bing-wallpaper.jpg";
        public static string LOCAL_CONFIGURATION_FILE_JSON = System.IO.Path.GetTempPath() + "bing-wallpaper.json";
        public static string BING_LOG = System.IO.Path.GetTempPath() + "bing-wallpaper.log";
#endif
        public static object BING_MTX = new object();

        public static bool WriteInBingWallpaperLog(string message)
        {
            bool result = false;
            try
            {
                lock (BING_MTX)
                {
                    File.AppendAllText(BING_LOG, string.Format("[{0}] {1}\n", DateTime.Now.ToString("dd/MM/YY HH:mm:ss"), message));
                }
            }
            catch (Exception ex)
            {
                lock (BING_MTX)
                {
                    File.AppendAllText(BING_LOG, string.Format("[{0}] {1}\n", DateTime.Now.ToString("dd/MM/YY HH:mm:ss"), ex.Message) );
                }
            }
            return result;
        }
        
        private static BingObject DownloadBingConfigFile(string location)
        {
            BingObject newBingObject = null, actualBingObject = null;
            string json = string.Empty;
            try
            {
                string url = string.Format("{0}{1}", BING_IMG_URL_JSON, location);
                if (Utils.IsInternetAvailable())
                {
                    var myClient = new HttpClient();
                    var myRequest = new HttpRequestMessage(HttpMethod.Get, url);
                    var response = myClient.SendAsync(myRequest).Result.Content.ReadAsStringAsync().Result;
                    newBingObject = JsonConvert.DeserializeObject<BingObject>(response);
                    if (File.Exists(LOCAL_CONFIGURATION_FILE_JSON))
                    {
                        string fileJson = File.ReadAllText(LOCAL_CONFIGURATION_FILE_JSON);
                        actualBingObject = JsonConvert.DeserializeObject<BingObject>(fileJson);
                        newBingObject.config = actualBingObject.config;
                        File.Delete(LOCAL_CONFIGURATION_FILE_JSON);
                    }
                    File.WriteAllText(LOCAL_CONFIGURATION_FILE_JSON, JsonConvert.SerializeObject(newBingObject));
                }
                else
                {
                    throw new WebException("Internet connection is not available");
                }
            }
            catch (Exception ex)
            {
                //Fallo en la red, obtenemos el temporal
                WriteInBingWallpaperLog(ex.Message);
            }
            return newBingObject;
        }

        private static void DownloadImageFile(string url)
        {
            try
            {
                if (Utils.IsInternetAvailable())
                {
                    WebClient webClient = new WebClient();
                    webClient.DownloadFile(url, LOCAL_IMAGE_FILE_JPG);
                }
            }
            catch { }
        }

        public static BingObject ReadConfig()
        {
            BingObject result = null;
            if (File.Exists(LOCAL_CONFIGURATION_FILE_JSON))
            {
                string fileJson = File.ReadAllText(LOCAL_CONFIGURATION_FILE_JSON);
                result = JsonConvert.DeserializeObject<BingObject>(fileJson);
            }
            return result;
        }

        public static BingObject GetWallpaperFromBing(string location)
        {
            BingObject result = null;
            try
            {
                result = DownloadBingConfigFile(location);
                if (result != null)
                {
                    DownloadImageFile(string.Format("{0}{1}{2}", BING_URL, result?.images?.FirstOrDefault()?.urlbase, BING_RESOLUTION));
                }
            }
            catch(Exception ex) {
                WriteInBingWallpaperLog(ex.Message);
            }
            return result;
        }

        public static async Task<bool> SetBingWallpaperAsync()
        {
            bool success = false;
            if (UserProfilePersonalizationSettings.IsSupported())
            {
                StorageFile file = await StorageFile.GetFileFromPathAsync(LOCAL_IMAGE_FILE_JPG);
                StorageFile copyLocalFile = await file.CopyAsync(ApplicationData.Current.LocalFolder, file.Name, NameCollisionOption.ReplaceExisting);
                success = await UserProfilePersonalizationSettings.Current.TrySetWallpaperImageAsync(copyLocalFile);
            }
            return success;
        }

        public static async Task<bool> SetBingWallpaperLockScreenAsync()
        {
            bool success = false;
            if (UserProfilePersonalizationSettings.IsSupported())
            {
                StorageFile file = await StorageFile.GetFileFromPathAsync(LOCAL_IMAGE_FILE_JPG);
                StorageFile copyLocalFile = await file.CopyAsync(ApplicationData.Current.LocalFolder, file.Name, NameCollisionOption.ReplaceExisting);
                success = await UserProfilePersonalizationSettings.Current.TrySetLockScreenImageAsync(copyLocalFile);
            }
            return success;
        }
    }
}
