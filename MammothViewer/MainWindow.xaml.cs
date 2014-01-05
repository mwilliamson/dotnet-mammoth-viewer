using CefSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
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

namespace MammothViewer
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            var settings = new CefSharp.Settings
            {
                PackLoadingDisabled = true,
            };
            CEF.Initialize(settings);
            InitializeComponent();
            WebView.PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs eventArgs)
        {
            if (eventArgs.PropertyName == "IsBrowserInitialized")
            {
                OnBrowserInitialised();
            }
        }

        private void OnBrowserInitialised()
        {
            WebView.LoadHtml(ReadResource("MammothViewer.index.html"));
        }

        private string ReadResource(string resourceName)
        {
            var currentAssembly = Assembly.GetExecutingAssembly();
            using (var stream = currentAssembly.GetManifestResourceStream(resourceName))
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
