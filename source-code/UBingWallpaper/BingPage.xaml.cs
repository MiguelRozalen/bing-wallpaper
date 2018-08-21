using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace UBingWallpaper
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class BingPage : Page
    {
        public BingPage()
        {
            this.InitializeComponent();
        }

        private void LoadImage(object sender, RoutedEventArgs e)
        {
            Image img = sender as Image;
            img.Stretch = Stretch.Fill;
            img.Source = new BitmapImage(new Uri(BingUtils.LOCAL_IMAGE_FILE_JPG, UriKind.RelativeOrAbsolute));
            img.Stretch = Stretch.UniformToFill;

            BingObject bingObject = BingUtils.ReadConfig();
            if (bingObject.images != null)
            {
                string info = bingObject.images.FirstOrDefault()?.copyright;
                if (info != null)
                {
                    copyright_window.Text = Regex.Match(info, @"\(([^)]*)\)").Groups[1].Value;
                    title_window.Text = info.Replace(Regex.Match(info, @"\(([^)]*)\)").Groups[0].Value, "");

                    System.Threading.Tasks.Task.Run(async () =>
                    {
                        await BingUtils.SetBingWallpaperAsync();
                    }).Wait();

                    if (bingObject.config != null && bingObject.config.setLockScreen)
                    {
                        System.Threading.Tasks.Task.Run(async () =>
                        {
                            await BingUtils.SetBingWallpaperLockScreenAsync();
                        }).Wait();
                    }
                }
            }
        }
    }
}
