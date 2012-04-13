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
                List<WP7RootToolsSDK.File> fileList = getFilesInSubFolders(@"\Applications\Data\" + appGuid + @"\Data\Cache\");
                foreach (WP7RootToolsSDK.File file in fileList)
                {
                    filesBox.Items.Add(new FileListItem(file));
                }
            }
            else
            {
                NavigationService.GoBack();
            }
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
                return File.Name + " - " + readableFileSize(File.Size);
            }
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

        private void filesBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (filesBox.SelectedIndex == -1)
                return;

            FileListItem item = (FileListItem)filesBox.SelectedItem;
            MessageBox.Show("Path: "+item.File.Path+"\nSize: "+readableFileSize(item.File.Size), item.File.Name, MessageBoxButton.OK);
        }
        public static string readableFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            int order = 0;
            while (bytes >= 1024 && order + 1 < sizes.Length)
            {
                order++;
                bytes = bytes / 1024;
            }
            return String.Format("{0:0.##} {1}", bytes, sizes[order]);
        }
    }
}