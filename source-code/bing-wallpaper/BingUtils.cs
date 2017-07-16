using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;

namespace bing_wallpaper
{
    public class BingUtils
    {
        public const string BING_IMG_URL_JSON = "http://www.bing.com/HPImageArchive.aspx?format=js&idx=0&n=1&mkt=";
        public const string BING_URL = "http://www.bing.com";
        private static string LOCAL_IMAGE_FILE_JPG = Environment.GetEnvironmentVariable("temp") + "\\bing-wallpaper.jpg";
        private static string LOCAL_CONFIGURATION_FILE_XML = Environment.GetEnvironmentVariable("temp") + "\\bing-wallpaper.xml";
        public static BingObject ACTUAL_BING_OBJECT = null;

        private static BingObject Step1_DownloadBingConfigFile(string location)
        {
            BingObject result = null;
            string json = string.Empty;
            try
            {
                string url = string.Format("{0}{1}", BING_IMG_URL_JSON, location);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    json = reader.ReadToEnd();
                    if (File.Exists(LOCAL_CONFIGURATION_FILE_XML))
                    {
                        File.Delete(LOCAL_CONFIGURATION_FILE_XML);
                    }
                    File.AppendAllText(LOCAL_CONFIGURATION_FILE_XML, json);
                }
            }
            catch
            {
                //Fallo en la red, obtenemos el temporal
                json = File.ReadAllText(LOCAL_CONFIGURATION_FILE_XML);
            }
            result = JsonConvert.DeserializeObject<BingObject>(json);
            return result;
        }

        private static void Step2_DownloadImageFile(string url)
        {
            try
            {
                WebClient webClient = new WebClient();
                if (File.Exists(LOCAL_IMAGE_FILE_JPG))
                {
                    File.Delete(LOCAL_IMAGE_FILE_JPG);
                }
                webClient.DownloadFile(url, LOCAL_IMAGE_FILE_JPG);
            }
            catch { }
        }

        public static string GetWallpaperFromBing(string location, ref BingObject bingObject)
        {
            bingObject = Step1_DownloadBingConfigFile(location);
            string url = string.Format("{0}{1}", BING_URL, bingObject?.images?.FirstOrDefault()?.url);
            Debug.Print("La imagen está en: {0}", url);
            Step2_DownloadImageFile(url);
            return LOCAL_IMAGE_FILE_JPG;
        }
    }
}
