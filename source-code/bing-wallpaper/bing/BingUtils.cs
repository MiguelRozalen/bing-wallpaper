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
        public static string LOCAL_CONFIGURATION_FILE_JSON = Environment.GetEnvironmentVariable("temp") + "\\bing-wallpaper.json";
        
        private static BingObject Step1_DownloadBingConfigFile(string location)
        {
            BingObject newBingObject = null, actualBingObject = null;
            string json = string.Empty;
            try
            {
                string url = string.Format("{0}{1}", BING_IMG_URL_JSON, location);
                if (Utils.IsInternetAvailable())
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    using (Stream stream = response.GetResponseStream())
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        json = reader.ReadToEnd();
                        newBingObject = JsonConvert.DeserializeObject<BingObject>(json);

                        if (File.Exists(LOCAL_CONFIGURATION_FILE_JSON))
                        {
                            string fileJson = File.ReadAllText(LOCAL_CONFIGURATION_FILE_JSON);
                            actualBingObject = JsonConvert.DeserializeObject<BingObject>(fileJson);
                            newBingObject.config = actualBingObject.config;
                            File.Delete(LOCAL_CONFIGURATION_FILE_JSON);
                        }
                        File.AppendAllText(LOCAL_CONFIGURATION_FILE_JSON, JsonConvert.SerializeObject(newBingObject));
                    }
                }
                else
                {
                    throw new WebException();
                }
            }
            catch
            {
                //Fallo en la red, obtenemos el temporal
                json = File.ReadAllText(LOCAL_CONFIGURATION_FILE_JSON);
                newBingObject = JsonConvert.DeserializeObject<BingObject>(json);
            }
            return newBingObject;
        }

        private static void Step2_DownloadImageFile(string url)
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
            if (File.Exists(LOCAL_CONFIGURATION_FILE_JSON))
            {
                string fileJson = File.ReadAllText(LOCAL_CONFIGURATION_FILE_JSON);
                return JsonConvert.DeserializeObject<BingObject>(fileJson);
            }
            return null;
        }
        public static string GetWallpaperFromBing(string location, ref BingObject bingObject)
        {
            try
            {
                bingObject = Step1_DownloadBingConfigFile(location);
                string url = string.Format("{0}{1}", BING_URL, bingObject?.images?.FirstOrDefault()?.url);
                Debug.Print("La imagen está en: {0}", url);
                Step2_DownloadImageFile(url);
            }
            catch(Exception ex) {
                Debug.Print(ex.Message);
            }
            return LOCAL_IMAGE_FILE_JPG;
        }
    }
}
