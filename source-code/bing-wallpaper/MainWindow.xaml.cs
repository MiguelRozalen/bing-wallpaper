using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace bing_wallpaper
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static string bing_image_file = null;
        private static BingObject bing_object = null;
        public MainWindow()
        {
            SetBingWallpaper("es-ES");
            //SetBingWallpaper();
            InitializeComponent();
            DataContext = this;

            string info = bing_object?.images?.FirstOrDefault()?.copyright;
            Text_copyright = Regex.Match(info, @"\(([^)]*)\)").Groups[1].Value;
            Text_title = info.Replace(Regex.Match(info, @"\(([^)]*)\)").Groups[0].Value, "");
        }

        public void SetBingWallpaper(string location = "en-US")
        {
            try
            {
                bing_image_file = BingUtils.GetWallpaperFromBing(location, ref bing_object);
                if (bing_image_file != null)
                {
                    Utils.SetWallpaper(bing_image_file);
                }
            }
            catch (System.Net.WebException)
            {
                Thread.Sleep(60*1000*5);
                SetBingWallpaper(location);
            }
        }

        private void image_Loaded(object sender, RoutedEventArgs e)
        {
            Image img = sender as Image;
            img.Stretch = Stretch.Fill;
            img.Source = new BitmapImage(new Uri(bing_image_file, UriKind.RelativeOrAbsolute));
        }

        private string text_title;
        private string text_copyright;
        public string Text_title
        {
            get{ return text_title; }
            set
            {
                text_title = value;
                RaisePropertyChanged("text_title");
            }
        }
        public string Text_copyright
        {
            get{ return text_copyright; }
            set
            {
                text_copyright = value;
                RaisePropertyChanged("text_copyright");
            }
        }
        private void RaisePropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
