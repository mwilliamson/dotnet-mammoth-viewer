﻿using CefSharp;
using CefSharp.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Input;

namespace MammothViewer
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            var settings = new CefSharp.Settings
            {
                PackLoadingDisabled = false,
            };
            CEF.Initialize(settings);
            InitializeComponent();
            WebView.PropertyChanged += OnPropertyChanged;
            KeyDown += (sender, eventArgs) => HandleKeyDown(eventArgs.Key);
        }

        private void HandleKeyDown(Key key)
        {
            if (key == Key.F11)
            {
                WebView.ShowDevTools();
            }
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
            WebView.RegisterJsObject("MammothViewerBackend", new MammothViewerModel(WebView));
            WebView.LoadHtml(ReadResource("MammothViewer.index.html"));
        }

        private static string ReadResource(string resourceName)
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

        private class MammothViewerModel
        {
            private readonly WebView m_Browser;
            private IList<FileSystemWatcher> m_FileWatchers = new List<FileSystemWatcher>();

            public string Styles { get { return ReadResource("MammothViewer.style.css"); } }
            public string Js { get {
                return ReadResource("MammothViewer.mammoth.js") + ReadResource("MammothViewer.viewer.js");
            } }

            public MammothViewerModel(WebView browser)
            {
                m_Browser = browser;
            }
            public string ReadFileBytesBase64(string path)
            {
                return Convert.ToBase64String(File.ReadAllBytes(path));
            }

            public string ReadFileString(string path)
            {
                return File.ReadAllText(path, Encoding.UTF8);
            }

            public string OpenFile(string filter)
            {
                var dialog = new Microsoft.Win32.OpenFileDialog();
                dialog.Filter = filter;
                var result = dialog.ShowDialog();
                if (result == true)
                {
                    return dialog.FileName;
                }
                else
                {
                    return null;
                }
            }

            public void SetWatchedFiles(string watchedFilesJson) {
                var paths = DeserialiseJson<IList<string>>(watchedFilesJson);
                foreach (var watcher in m_FileWatchers) {
                    // TODO: disposal when application closes
                    watcher.Dispose();
                }
                m_FileWatchers = paths
                        .Where(File.Exists)
                        .Select(WatchFile)
                        .ToList();
            }

            private FileSystemWatcher WatchFile(string path)
            {
                var directory = Path.GetDirectoryName(path);
                var filename = Path.GetFileName(path);
                var watcher = new FileSystemWatcher(directory, filename);
                watcher.Changed += (x, y) => RenderOutput();
                watcher.EnableRaisingEvents = true;
                return watcher;
            }

            private void RenderOutput()
            {
                m_Browser.ExecuteScript("MammothViewer.renderOutput();");
            }

            private T DeserialiseJson<T>(string json)
            {
                var serializer = new JavaScriptSerializer();
                return serializer.Deserialize<T>(json);
            }
        }
    }
}
