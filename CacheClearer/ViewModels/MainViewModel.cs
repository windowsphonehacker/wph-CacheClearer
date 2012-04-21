using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;


namespace CacheClearer
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            this.Items = new ObservableCollection<AppListItemViewModel>();
        }

        /// <summary>
        /// A collection for AppListItemViewModel objects.
        /// </summary>
        public ObservableCollection<AppListItemViewModel> Items { get; private set; }

        private string _sampleProperty = "Sample Runtime Property Value";
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding
        /// </summary>
        /// <returns></returns>
        public string SampleProperty
        {
            get
            {
                return _sampleProperty;
            }
            set
            {
                if (value != _sampleProperty)
                {
                    _sampleProperty = value;
                    NotifyPropertyChanged("SampleProperty");
                }
            }
        }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates and adds a few AppListItemViewModel objects into the Items collection.
        /// </summary>
        public void LoadData()
        {
            GlobalLoading.Instance.IsLoading = true;
            WP7RootToolsSDK.Folder folder = WP7RootToolsSDK.FileSystem.GetFolder("\\Applications\\Data\\");
            List<WP7RootToolsSDK.FileSystemEntry> apps = folder.GetSubItems();
            foreach (WP7RootToolsSDK.FileSystemEntry app in apps)
            {
                if (app.IsFolder)
                {
                    List<WP7RootToolsSDK.FileSystemEntry> items = ((WP7RootToolsSDK.Folder)app).GetSubItems();
                    String appName = WP7RootToolsSDK.Applications.GetApplicationName(new Guid(app.Name));
                    System.Diagnostics.Debug.WriteLine(appName + " - " + app.Name);
                    String cachePath = app.Path + "\\Data\\Cache\\";
                    if (WP7RootToolsSDK.FileSystem.FileExists(cachePath))
                    {
                        //WP7RootToolsSDK.Folder CacheFolder = WP7RootToolsSDK.FileSystem.GetFolder(cachePath);
                        this.Items.Add(new AppListItemViewModel() { LineOne = appName, LineTwo = app.Name, LineThree = "Third line" });
                    }

                    System.Diagnostics.Debug.WriteLine("");
                }
            }
            GlobalLoading.Instance.IsLoading = false;
            this.IsDataLoaded = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}