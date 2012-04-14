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
using System.Windows.Navigation;

namespace CacheClearer
{
    public partial class DetailsPage : PhoneApplicationPage
    {

        public string appGuid = "";
        public DetailsPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (NavigationContext.QueryString.TryGetValue("appguid", out appGuid))
            {
                MainPivot.Title = WP7RootToolsSDK.Applications.GetApplicationName(new Guid(appGuid));
                refreshInfos();
            }
            else
            {
                NavigationService.GoBack();
            }
        }
        private void refreshInfos()
        {
            filesBox.Items.Clear();
            cachedFilesBlock.Text = "Unknown";
            cacheSizeBlock.Text = "Unknown";
            List<WP7RootToolsSDK.File> fileList = cleanCache.getFilesInSubFolders(@"\Applications\Data\" + appGuid + @"\Data\Cache\");
            int files = 0;
            uint filesizes = 0;
            foreach (WP7RootToolsSDK.File file in fileList)
            {
                files++;
                filesizes += file.Size;
                filesBox.Items.Add(new FileListItem(file));
            }

            cachedFilesBlock.Text = files.ToString();
            cacheSizeBlock.Text = Utils.readableFileSize(filesizes);
        }
        public class FileListItem
        {
            public WP7RootToolsSDK.File File;

            public FileListItem() { }
            public FileListItem(WP7RootToolsSDK.File File)
            {
                this.File = File;
            }

            public override string ToString()
            {
                return File.Name + " - " + Utils.readableFileSize(File.Size);
            }
        }


        private void filesBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (filesBox.SelectedIndex == -1)
                return;

            FileListItem item = (FileListItem)filesBox.SelectedItem;
            if (MessageBox.Show("Path: " + item.File.Path + "\nSize: " + Utils.readableFileSize(item.File.Size) + "\n\nOpen file in default app?", item.File.Name, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                string ext = Utils.getFileExtension(item.File.Name);
                //TODO: Find a way to open in native app (ShellExecuteEx in C++) and maybe create in-app viewers for images and web pages.
                //TODO: Add fiinix credits for dllimport
                switch (ext)
                {
                    case "jpg":
                    case "jpeg":
                    case "png":
                    case "bmp":
                    case "gif":
                        CSharp___DllImport.Phone.AppLauncher.OpenPicture(item.File.Path);
                        break;
                    case "html":
                    case "htm":
                        NavigationService.Navigate(new Uri("/FileViewers/HTMLViewer.xaml?path=" + item.File.Path, UriKind.Relative));
                        break;
                    default:
                        MessageBox.Show("I don't know how to open this file", "Error", MessageBoxButton.OK);
                        break;
                }
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            cleanCache.cleanAppCache(appGuid);
            refreshInfos();
            MessageBox.Show("Cache cleared!");
        }
    }
}