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
using System.Windows.Threading;

namespace bing_wallpaper
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static string bingImageFile = null;
        private static BingObject bingObject = null;
        private static string defaultLanguage = null;
        private static string defaultPeriod = null;
        private static bool defaultRunAtStartup = false;
        private static bool defaultSetWallpaper = false;
        private static bool defaultSetLockScreen = false;
        
        public MainWindow()
        {
            CloseSameProcesses();

            InitializeComponent();
            this.DataContext = this;          

            bingObject = BingUtils.ReadConfig();
            InitializeSettingsTab();

            SetBingWallpaper(defaultLanguage);

            Minimize();
        }

        private static void CloseSameProcesses()
        {
            Process currentProcess = Process.GetCurrentProcess();
            Process[] processes = Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location));
            if (processes.Length > 1)
            {
                for (int i = 0; i < processes.Length - 1; i++)
                {
                    processes[i].Kill();
                }
            }
        }

        private void InitializeSettingsTab()
        {
            if (bingObject?.config != null)
            {
                defaultLanguage = bingObject.config.languageOptions.Name;
                defaultPeriod = bingObject.config.period;
                defaultRunAtStartup = bingObject.config.runAtStartup;
                defaultSetWallpaper = bingObject.config.setWallpaper;
                defaultSetLockScreen = bingObject.config.setLockScreen;
            }
            else
            {
                defaultLanguage = CultureInfo.CurrentCulture.Name;
                defaultPeriod = "Every Day";
                defaultRunAtStartup = true;
                defaultSetWallpaper = true;
                defaultSetLockScreen = false;
            }

            cmbLocation.SelectedIndex = LanguageOptions.IndexOf(LanguageOptions.Where(p => p.Name == defaultLanguage).FirstOrDefault());
            cmbExecution.SelectedIndex = ExecutionPeriods.IndexOf(defaultPeriod);
            RunAtStartup = defaultRunAtStartup;
            SetWallpaper = defaultSetWallpaper;
            SetLockScreen = defaultSetLockScreen;

            RunAtStartup_Box.IsChecked = defaultRunAtStartup;
            SetWallpaper_Box.IsChecked = defaultSetWallpaper;
            SetLockScreen_Box.IsChecked = defaultSetLockScreen;
        }

        #region Properties

        private string textTitle;
        private string textCopyright;
        private bool runAtStartup;
        private bool setWallpaper;
        private bool setLockScreen;
        private List<CultureInfo> languageOptions;

        public List<CultureInfo> LanguageOptions
        {
            get
            {
                if (languageOptions == null)
                {
                    CultureInfo[] ci = CultureInfo.GetCultures(CultureTypes.AllCultures & ~CultureTypes.NeutralCultures).ToArray();
                    languageOptions = new List<CultureInfo>();
                    languageOptions.AddRange(ci.Where(p => p.Name.Length == 5).ToArray());
                    languageOptions = languageOptions.OrderBy(p => p.EnglishName).ToList();
                }
                return languageOptions;
            }
            set
            {
                languageOptions = value;
                RaisePropertyChanged("LanguageOptions");
            }
        }

        public List<string> executionPeriods = new List<string>() { "Every Day", "Every Week", "Every Month", "Every 3 Months", "Never More" };
        public List<string> ExecutionPeriods
        {
            get { return executionPeriods; }
            set
            {
                executionPeriods = value;
                RaisePropertyChanged("ExecutionPeriods");
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

        public bool SetWallpaper
        {
            get { return setWallpaper; }
            set
            {
                setWallpaper = value;
                RaisePropertyChanged("SetWallpaper");
            }
        }


        public string TextTitle
        {
            get { return textTitle; }
            set
            {
                textTitle = value;
                RaisePropertyChanged("TextTitle");
            }
        }

        public string TextCopyright
        {
            get { return textCopyright; }
            set
            {
                textCopyright = value;
                RaisePropertyChanged("TextCopyright");
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
            bingImageFile = BingUtils.GetWallpaperFromBing(location, ref bingObject);
            Utils.SetWallpaper(bingImageFile);
            if (bingObject != null && bingObject.config != null && bingObject.config.setLockScreen)
            {
                Utils.SetLockScreen(bingImageFile);
            }

            if (bingObject != null && bingObject.images != null)
            {
                string info = bingObject.images.FirstOrDefault().copyright;
                if (info != null)
                {
                    TextCopyright = Regex.Match(info, @"\(([^)]*)\)").Groups[1].Value;
                    TextTitle = info.Replace(Regex.Match(info, @"\(([^)]*)\)").Groups[0].Value, "");
                }
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

        #region GenericEventHandlers
        private void RaisePropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        #endregion

        #region WindowEvents
        private void Window_StateChanged(object sender, EventArgs e)
        {
            switch (this.WindowState)
            {
                case WindowState.Minimized:
                    Minimize();
                    break;
            }
        }
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            Minimize();
        }
        #endregion

        #region ImageEvents
        private void LoadImage(object sender, RoutedEventArgs e)
        {
            Image img = sender as Image;
            img.Stretch = Stretch.Fill;
            img.Source = new BitmapImage(new Uri(bingImageFile, UriKind.RelativeOrAbsolute));
        }
        #endregion

        #region Menu & Tooltip Events
        public void ActionDoubleClickCommandTaskBarIcon()
        {
            if (this.WindowState == WindowState.Minimized)
            {
                MaximizeOrNormal();
            }
            else
            {
                Minimize();
            }
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
        #endregion

        #region TabEvents
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.WindowState != WindowState.Minimized)
            {
                TabControl tab = sender as TabControl;
                TabItem item = tab?.SelectedItem as TabItem;

                if (item != null && e.Source.GetType() == typeof(TabControl))
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
        #endregion

        #region HyperLinkEvents
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
        #endregion

        #region ButonClickEvents
        private void Button_Click_Reset(object sender, RoutedEventArgs e)
        {
            bingObject.config = null;
            InitializeSettingsTab();
            MessageBox.Show("Preferences will be reset! Please click on 'Apply' button to save changes.", "Bing Wallpaper", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.None);
        }

        private void Button_Click_Apply(object sender, RoutedEventArgs e)
        {
            bingObject.config = new BingConfig()
            {
                languageOptions = cmbLocation.SelectedItem as CultureInfo,
                period = cmbExecution.SelectedItem as string,
                runAtStartup = RunAtStartup,
                setLockScreen = SetLockScreen,
                setWallpaper = SetWallpaper,
            };

            if (File.Exists(BingUtils.LOCAL_CONFIGURATION_FILE_JSON))
            {
                File.Delete(BingUtils.LOCAL_CONFIGURATION_FILE_JSON);
            }
            File.AppendAllText(BingUtils.LOCAL_CONFIGURATION_FILE_JSON, JsonConvert.SerializeObject(bingObject));

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "BingWallpaperServiceManager.exe";
            startInfo.Arguments = string.Format("-n {0} -d {1} -r {2} -p {3}", ("Bing Wallpaper").Replace(' ', '%') , ("Bing Wallpaper Desktop Windows").Replace(' ', '%'), runAtStartup.ToString(), (cmbExecution.SelectedItem as string).Replace(' ','%'));

            try
            {
                Process.Start(startInfo).WaitForExit();

                MessageBox.Show("Preferences have been saved sucessfully! Bing Wallpaper should be restarted for commit changes.", "Bing Wallpaper", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.None);
                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            }
            catch {
                MessageBox.Show("It was an error saving the information, please try again.", "Bing Wallpaper", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.None);
            }
        }
#endregion

    }
}
