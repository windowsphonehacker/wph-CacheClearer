using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace CacheClearer
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            listCacheApps();
        }

        public void listCacheApps()
        {

            // FileSystem

            if (!WP7RootToolsSDK.Environment.HasRootAccess())
            {
                MessageBox.Show("No root access. Please allow through Root Tools");
                return;
            }

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
                        listBox1.Items.Add(new AppListItem(app.Name, appName));
                    }

                    System.Diagnostics.Debug.WriteLine("");
                }
            }


        }
        public class AppListItem
        {
            public string Guid;
            public string AppName;

            public AppListItem() { }
            public AppListItem(string Guid, string AppName)
            {
                this.Guid = Guid;
                this.AppName = AppName;
            }

            public override string ToString()
            {
                return AppName + " - " + Guid.ToString();
            }
        }

        private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AppListItem item = (AppListItem)listBox1.SelectedItem;
            listBox1.IsEnabled = false;
            Microsoft.Phone.Shell.ProgressIndicator pi = new Microsoft.Phone.Shell.ProgressIndicator();
            Microsoft.Phone.Shell.SystemTray.SetProgressIndicator(this, pi);
            pi.IsIndeterminate = true;
            pi.Text = "Checking cache of " + item.AppName;
            pi.IsVisible = true;
            List<WP7RootToolsSDK.File> fileList = getFilesInSubFolders("\\Applications\\Data\\" + item.Guid + "\\Data\\Cache\\");
            MessageBox.Show(fileList.Count + " files in cache!");
            listBox1.IsEnabled = true;
            pi.IsVisible = false;

        }
        public List<WP7RootToolsSDK.File> getFilesInSubFolders(string path)
        {
            List<WP7RootToolsSDK.File> fileList = new List<WP7RootToolsSDK.File>();

           // try
            //{
                WP7RootToolsSDK.Folder folder = WP7RootToolsSDK.FileSystem.GetFolder(path);
                foreach (WP7RootToolsSDK.FileSystemEntry item in folder.GetSubItems())
                {
                    if (item.IsFile)
                    {
                        fileList.Add((WP7RootToolsSDK.File)item);
                    }
                    else
                    {
                        fileList.AddRange(getFilesInSubFolders(item.Path));
                    }
                }
           // }
            //catch (Exception ex)
           // {
                //System.Diagnostics.Debug.WriteLine(ex.Message);
           // }
            return fileList;
        }
    }
}