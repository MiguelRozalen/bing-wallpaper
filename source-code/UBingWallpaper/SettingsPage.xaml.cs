using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace UBingWallpaper
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {

        private static string defaultLanguage = null;
        private static string defaultPeriod = null;
        private static bool defaultRunAtStartup = false;
        private static bool defaultSetWallpaper = false;
        private static bool defaultSetLockScreen = false;
       
        #region Atributes

        public List<string> executionPeriods = new List<string>() { "Every Day", "Every Week", "Every Month", "Every 3 Months", "Never More" };
        public List<string> ExecutionPeriods
        {
            get { return executionPeriods; }
            set
            {
                executionPeriods = value;
            }
        }

        private bool runAtStartup;
        public bool RunAtStartup
        {
            get { return runAtStartup; }
            set
            {
                runAtStartup = value;
            }
        }

        private bool setLockScreen;
        public bool SetLockScreen
        {
            get { return setLockScreen; }
            set
            {
                setLockScreen = value;
            }
        }

        private bool setWallpaper;
        public bool SetWallpaper
        {
            get { return setWallpaper; }
            set
            {
                setWallpaper = value;
            }
        }

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
            }
        }
        #endregion


        public SettingsPage()
        {
            BingObject bingObject = BingUtils.ReadConfig();
            this.InitializeComponent();
            this.DataContext = this;

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
                defaultSetLockScreen = true;
            }

            cmbLocation.ItemsSource = LanguageOptions;
            cmbExecution.ItemsSource = ExecutionPeriods;
                        
            cmbLocation.SelectedIndex = LanguageOptions.IndexOf(LanguageOptions.Where(p => p.Name == defaultLanguage).ToList().FirstOrDefault());
            cmbExecution.SelectedIndex = executionPeriods.IndexOf(defaultPeriod);
                                   
            RunAtStartup_Box.IsChecked = defaultRunAtStartup;
            SetWallpaper_Box.IsChecked = defaultSetWallpaper;
            SetLockScreen_Box.IsChecked = defaultSetLockScreen;
        }

        private void Button_Click_Apply(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_Reset(object sender, RoutedEventArgs e)
        {

        }
    }
}
