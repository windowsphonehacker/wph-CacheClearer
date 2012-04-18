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

namespace CacheClearer
{
    //Merge with Utils.cs or just keep here?
    public class FileTypes
    {
        public enum FileType { Image = 0, CacheIndex, Html, Other }
        public static FileType getFileType(string extension)
        {
            switch (extension)
            {
                case "jpg":
                case "jpeg":
                case "png":
                case "bmp":
                case "gif":
                    return FileType.Image;
                    
                case "html":
                case "htm":
                    return FileType.Html;
                    
                case "dat":
                    return FileType.CacheIndex;
                    
                default:
                    return FileType.Other;
            }
            
        }
    }
}
