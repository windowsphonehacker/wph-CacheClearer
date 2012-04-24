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
    public class Utils
    {
        public static string getFileExtension(string fileName)
        {

            string extension = "";
            extension = System.IO.Path.GetExtension(fileName).Substring(1);
            /*
            char[] arr = fileName.ToCharArray();
            int index = 0;

            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] == '.')
                {
                    index = i;
                }
            }

            for (int x = index + 1; x < arr.Length; x++)
            {
                extension = extension + arr[x];
            }
            */
            return extension;

        }
        //Moved from DetailsPage.xaml.cs
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
