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
using System.Collections.ObjectModel;
namespace CacheClearer
{
    public partial class DetailsPage : PhoneApplicationPage
    {
        //test
        int[] sizeByType;

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
            GlobalLoading.Instance.IsLoading = true;
            sizeByType = new int[FileTypes.FileTypeEnumSize];

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
                FileTypes.FileType type = FileTypes.getFileType(Utils.getFileExtension(file.Name));
                sizeByType[(int)type] += (int)file.Size;
            }

            cachedFilesBlock.Text = files.ToString();
            cacheSizeBlock.Text = Utils.readableFileSize(filesizes);

            updateChart();

            System.Diagnostics.Debug.WriteLine(sizeByType[0]);
            System.Diagnostics.Debug.WriteLine(sizeByType[1]);
            System.Diagnostics.Debug.WriteLine(sizeByType[2]);
            System.Diagnostics.Debug.WriteLine(sizeByType[3]);
            GlobalLoading.Instance.IsLoading = false;
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
                switch (FileTypes.getFileType(ext))
                {
                    //Image case not needed as the default handles images too.
                    /*case FileTypes.FileType.Image:
                        CSharp___DllImport.Phone.AppLauncher.OpenPicture(item.File.Path);
                        break;*/
                    case FileTypes.FileType.Html:
                        NavigationService.Navigate(new Uri("/FileViewers/HTMLViewer.xaml?path=" + item.File.Path, UriKind.Relative));
                        break;
                    default:
                        try
                        {
                            WP7RootToolsSDK.Environment.ShellExecute(item.File.Path);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("I don't know how to open this file!\nError: " + ex.Message, "Error", MessageBoxButton.OK);
                        }
                        break;
                }
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            GlobalLoading.Instance.IsLoading = true;
            var saved = cleanCache.cleanAppCache(appGuid);
            refreshInfos();
            GlobalLoading.Instance.IsLoading = false;
            MessageBox.Show("Cache cleared! You saved " + Utils.readableFileSize(saved));
        }

        /* Chart */

        public class PData
        {
            public string title { get; set; }
            public double value { get; set; }
        }
        void updateChart()
        {
            ObservableCollection<PData> data = new ObservableCollection<PData>();
            for (int i = 0; i < sizeByType.Length; i++)
            {
                if (sizeByType[i] > 0)
                    data.Add(new PData() { title = ((FileTypes.FileType)i).ToString(), value = sizeByType[i] });
            }
            pieChart.DataSource = data;
        }
    }
}