using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Linq;
namespace CacheClearer
{
    //Handles deleting items in the cache for applications
    //In separate class due to the fact it is accessed by multiple pages
    public class cleanCache
    {
        public static void cleanAppCache(string guid)
        {
            string path = @"\Applications\Data\" + guid + @"\Data\Cache\";
            List<WP7RootToolsSDK.File> files = getFilesInSubFolders(path);

            foreach (WP7RootToolsSDK.File file in files)
            {
                    WP7RootToolsSDK.FileSystem.DeleteFile(file.Path);
                    System.Diagnostics.Debug.WriteLine("Deleted " + file.Path);
                
            }
        }

        //moved from DetailsPage.xaml.cs
        public static List<WP7RootToolsSDK.File> getFilesInSubFolders(string path)
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
