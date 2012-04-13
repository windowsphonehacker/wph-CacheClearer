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

                    System.Diagnostics.Debug.WriteLine(WP7RootToolsSDK.Applications.GetApplicationName(new Guid(app.Name)));
                    foreach (WP7RootToolsSDK.FileSystemEntry item in items)
                    {
                        System.Diagnostics.Debug.WriteLine(item.Name);
                    }
                    System.Diagnostics.Debug.WriteLine("");
                }
            }


        }
    }
}