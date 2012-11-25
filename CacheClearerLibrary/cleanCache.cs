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
        public static int cleanIECache()
        {
            return cleanCache.cleanAppPath(@"\Windows\Profiles\guest\Temporary Internet Files\Content.IE5");
        }

        public static int cleanOfficeCache()
        {
            return cleanCache.cleanAppPath(@"\Application Data\Microsoft\Office Mobile\Cache");
        }

        public static int cleanMapsCache()
        {
            return cleanCache.cleanAppPath(@"\Application Data\Maps\Cache");
        }

        public static int cleanAppCache(string guid)
        {
            string path = @"\Applications\Data\" + guid + @"\Data\Cache";
            return cleanAppPath(path);
        }

        public static int cleanAppPath(string path)
        {
            int totalSize = 0;


            foreach (WP7RootToolsSDK.File file in getFilesInSubFolders(path))
            {
                try
                {
                    WP7RootToolsSDK.FileSystem.DeleteFile(file.Path);
                    Homebrew.IO.File.SetAttributes(file.Path, new Homebrew.IO.FileAttributes(Homebrew.IO.FileAttributesEnum.WriteThrough));
                    System.Diagnostics.Debug.WriteLine("Deleted " + file.Path);
                    totalSize += (int)file.Size;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Could not delete file " + file.Path + ": " + ex.Message);
                }
            }
            System.Diagnostics.Debug.WriteLine("Removing empty cache folders...");
            foreach (WP7RootToolsSDK.Folder folder in getFoldersInSubFolders(path))
            {
                try
                {
                    WP7RootToolsSDK.FileSystem.DeleteFolder(folder.Path);
                    System.Diagnostics.Debug.WriteLine("Deleted folder " + folder.Path);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Could not delete folder " + folder.Path + ": " + ex.Message);
                }
            }
            try
            {
                System.Diagnostics.Debug.WriteLine("Deleted app's main cache folder " + path);
                WP7RootToolsSDK.FileSystem.DeleteFolder(path);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Could not delete app's main cach folder " + path + ": " + ex.Message);
            }
            return totalSize;
        }

        //moved from DetailsPage.xaml.cs
        public static List<WP7RootToolsSDK.File> getFilesInSubFolders(string path)
        {
            List<WP7RootToolsSDK.File> fileList = new List<WP7RootToolsSDK.File>();

            try
            {
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
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            return fileList;
        }

        public static List<WP7RootToolsSDK.Folder> getFoldersInSubFolders(string path)
        {
            List<WP7RootToolsSDK.Folder> folderList = new List<WP7RootToolsSDK.Folder>();

            try
            {
                WP7RootToolsSDK.Folder folder = WP7RootToolsSDK.FileSystem.GetFolder(path);
                foreach (WP7RootToolsSDK.FileSystemEntry item in folder.GetSubItems())
                {
                    if (item.IsFolder)
                    {
                        folderList.AddRange(getFoldersInSubFolders(item.Path));
                        folderList.Add((WP7RootToolsSDK.Folder)item);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            return folderList;
        }
        public static uint getTotalCacheSize()
        {
            uint cacheSize = 0;
            WP7RootToolsSDK.Folder folder = WP7RootToolsSDK.FileSystem.GetFolder("\\Applications\\Data\\");
            List<WP7RootToolsSDK.FileSystemEntry> apps = folder.GetSubItems();
            foreach (WP7RootToolsSDK.FileSystemEntry app in apps)
            {
                if (app.IsFolder)
                {
                    List<WP7RootToolsSDK.FileSystemEntry> items = ((WP7RootToolsSDK.Folder)app).GetSubItems();
                    System.Diagnostics.Debug.WriteLine(app.Name);
                    String cachePath = app.Path + "\\Data\\Cache\\";
                    if (WP7RootToolsSDK.FileSystem.FileExists(cachePath))
                    {
                        try
                        {
                            foreach (WP7RootToolsSDK.File file in getFilesInSubFolders(cachePath))
                            {
                                cacheSize += file.Size;
                            }
                        }
                        catch
                        { //this fails for some reason, sometimes. not sure why.
                        }
                    }

                    System.Diagnostics.Debug.WriteLine("");
                }
            }
            return cacheSize;
        }
        public static int clearAll()
        {
            int cleared = 0;
            WP7RootToolsSDK.Folder folder = WP7RootToolsSDK.FileSystem.GetFolder("\\Applications\\Data\\");
            List<WP7RootToolsSDK.FileSystemEntry> apps = folder.GetSubItems();
            foreach (WP7RootToolsSDK.FileSystemEntry app in apps)
            {
                if (app.IsFolder)
                {
                    System.Diagnostics.Debug.WriteLine(app.Name);
                    cleared += cleanAppCache(app.Name);
                }
            }

            cleared += cleanIECache();
            cleared += cleanOfficeCache();
            cleared += cleanMapsCache();

            return cleared;
        }
    }
}
