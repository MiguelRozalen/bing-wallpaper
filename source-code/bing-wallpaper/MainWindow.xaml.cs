using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
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
using System.Windows.Media.Animation;
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
        private static List<string> execution_periods = null;
        private static string defaultLanguage = null;
        private static string defaultPeriod = null;
        private static bool defaultRunAtStartup = false;
        private static bool defaultSetWallpaper = false;
        private static bool defaultSetLockScreen = false;

        public MainWindow()
        {
            //string defaultLanguage = "es-ES";
            bing_object = BingUtils.ReadConfig();

            if (bing_object?.config != null)
            {
                defaultLanguage = bing_object.config.languageOptions.Name;
                defaultPeriod= bing_object.config.period;
                defaultRunAtStartup = bing_object.config.runAtStartup;
                defaultSetWallpaper = bing_object.config.setWallpaper;
                defaultSetLockScreen = bing_object.config.setLockScreen;
            }
            else
            {
                defaultLanguage = CultureInfo.CurrentCulture.Name;
                defaultPeriod = "Every Day";
                defaultRunAtStartup = false;
                defaultSetWallpaper = false;
                defaultSetLockScreen = false;
            }

            SetBingWallpaper(defaultLanguage);
            
            InitializeComponent();
            this.DataContext = this;

            CultureInfo[] ci = CultureInfo.GetCultures(CultureTypes.AllCultures & ~CultureTypes.NeutralCultures).ToArray();
            languageOptions.AddRange(ci.Where(p=>p.Name.Length==5).ToArray());
            languageOptions.OrderByDescending(p => p.EnglishName);
            cmbLocation.SelectedIndex = languageOptions.IndexOf(languageOptions.Where(p => p.Name == defaultLanguage).FirstOrDefault());

            execution_periods = new List<string>()
            {
                "Every Day",
                "Every Week",
                "Every Month",
                "Every 3 Months",
                "Never More"
            };
            cmbExecution.ItemsSource = execution_periods;
            cmbExecution.SelectedIndex = execution_periods.IndexOf(defaultPeriod);

            runAtStartup = defaultRunAtStartup;
            setWallpaper = defaultSetWallpaper;
            setLockScreen = defaultSetLockScreen;

            string info = bing_object?.images?.FirstOrDefault()?.copyright;
            if (info != null)
            {
                Text_copyright = Regex.Match(info, @"\(([^)]*)\)").Groups[1].Value;
                Text_title = info.Replace(Regex.Match(info, @"\(([^)]*)\)").Groups[0].Value, "");
            }
#if !DEBUG
            Minimize();
#endif
        }

#region Properties

        private string text_title;
        private string text_copyright;
        private bool runAtStartup;
        private bool setWallpaper;
        private bool setLockScreen;
        private List<CultureInfo> languageOptions = new List<CultureInfo>();

        public List<CultureInfo> LanguageOptions
        {
            get { return languageOptions; }
            set
            {
                languageOptions = value;
                RaisePropertyChanged("LanguageOptions");
            }
        }

        public bool RunAtStartup
        {
            get { return runAtStartup; }
            set
            {
                runAtStartup = value;
                RaisePropertyChanged("RunAtStartup");
            }
        }

        public bool SetLockScreen
        {
            get { return setLockScreen; }
            set
            {
                setLockScreen = value;
                RaisePropertyChanged("SetLockScreen");
            }
        }

        public bool  SetWallpaper
        {
            get { return setWallpaper; }
            set
            {
                setWallpaper = value;
                RaisePropertyChanged("SetWallpaper");
            }
        }


        public string Text_title
        {
            get { return text_title; }
            set
            {
                text_title = value;
                RaisePropertyChanged("Text_title");
            }
        }
 
        public string Text_copyright
        {
            get { return text_copyright; }
            set
            {
                text_copyright = value;
                RaisePropertyChanged("Text_copyright");
            }
        }

        private ICommand _doubleClickCommandTaskBarIcon;
        public ICommand DoubleClickCommandTaskBarIcon
        {
            get
            {
                return _doubleClickCommandTaskBarIcon ?? (_doubleClickCommandTaskBarIcon = new CommandHandler(() => ActionDoubleClickCommandTaskBarIcon(), true));
            }
        }
#endregion

#region Functions
        public void SetBingWallpaper(string location)
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
                Thread.Sleep(60 * 1000 * 5);
                SetBingWallpaper(location);
            }
        }

        private void Minimize()
        {
            this.WindowState = WindowState.Minimized;
            this.Visibility = Visibility.Hidden;
            this.ShowInTaskbar = false;
            SystemCommands.MinimizeWindow(this);
        }

        private void MaximizeOrNormal()
        {
            this.WindowState = WindowState.Normal;
            this.Visibility = Visibility.Visible;
            this.ShowInTaskbar = true;
            SystemCommands.RestoreWindow(this);
        }
#endregion

#region EventHandlers
        private void RaisePropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void Window_StateChanged(object sender, EventArgs e)
        {
            switch (this.WindowState)
            {
                case WindowState.Minimized:
                    Minimize(); 
                    break;
            }
        }
        private void image_Loaded(object sender, RoutedEventArgs e)
        {
            Image img = sender as Image;
            img.Stretch = Stretch.Fill;
            img.Source = new BitmapImage(new Uri(bing_image_file, UriKind.RelativeOrAbsolute));
        }

        public void ActionDoubleClickCommandTaskBarIcon()
        {
            if(this.WindowState == WindowState.Minimized)
            {
                MaximizeOrNormal();
            }
            else
            {
                Minimize();
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            Minimize();
        }

        private void MenuItem_Click_Exit(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void MenuItem_Click_Open(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                MaximizeOrNormal();
            }
            e.Handled = true;
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {   
            if (this.WindowState != WindowState.Minimized)
            {
                TabControl tab = sender as TabControl;
                TabItem item = tab?.SelectedItem as TabItem;
                
                if (item != null && e.Source.GetType()==typeof(TabControl))
                {
                    DoubleAnimation animation_width = new DoubleAnimation(item.Width, item.Width * 0.965,
                        new Duration(new TimeSpan(0, 0, 0, 0, 230)));
                    DoubleAnimation animation_height = new DoubleAnimation(item.Height, item.Height * 0.965,
                        new Duration(new TimeSpan(0, 0, 0, 0, 230)));

                    animation_width.AutoReverse = true;
                    animation_height.AutoReverse = true;

                    Storyboard.SetTarget(animation_width, item);
                    Storyboard.SetTarget(animation_height, item);

                    Storyboard.SetTargetProperty(animation_width, new PropertyPath("Width"));
                    Storyboard.SetTargetProperty(animation_height, new PropertyPath("Height"));

                    Storyboard story = new Storyboard();
                    story.Children.Add(animation_width);
                    story.Children.Add(animation_height);
                    story.Begin();
                }
            }
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
        #endregion

        private void cmbLocation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox tab = sender as ComboBox;
            CultureInfo item = tab?.SelectedItem as CultureInfo;
            Debug.Print(item.Name);
        }

        private void Button_Click_Reset(object sender, RoutedEventArgs e)
        {
            cmbLocation.SelectedIndex = languageOptions.IndexOf(languageOptions.Where(p => p.Name == CultureInfo.CurrentCulture.Name).FirstOrDefault());
            cmbExecution.SelectedIndex = execution_periods.IndexOf("Every Day");
            startup_box.IsChecked = true;
            wallpaper_box.IsChecked = true;
            lock_box.IsChecked = false;

            MessageBox.Show("Preferences reset!", "Bing Wallpaper", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
        }

        private void Button_Click_Apply(object sender, RoutedEventArgs e)
        {
            bing_object.config = new BingConfig()
            {
                languageOptions = cmbLocation.SelectedItem as CultureInfo,
                period = cmbExecution.SelectedItem as string,
                runAtStartup = runAtStartup,
                setLockScreen = setLockScreen,
                setWallpaper = setWallpaper,
            };

            if (File.Exists(BingUtils.LOCAL_CONFIGURATION_FILE_JSON))
            {
                File.Delete(BingUtils.LOCAL_CONFIGURATION_FILE_JSON);
            }
            File.AppendAllText(BingUtils.LOCAL_CONFIGURATION_FILE_JSON, JsonConvert.SerializeObject(bing_object));

            MessageBox.Show("Preferences saved sucessfully!", "Bing Wallpaper", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
        }
    }
}
