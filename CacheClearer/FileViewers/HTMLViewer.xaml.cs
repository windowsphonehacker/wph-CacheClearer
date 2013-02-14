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
using System.Text;

namespace CacheClearer.FileViewers
{
    public partial class HTMLViewer : PhoneApplicationPage
    {
        public HTMLViewer()
        {
            InitializeComponent();
        }
        string path = "";
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (NavigationContext.QueryString.TryGetValue("path", out path))
            {
                byte[] contents = WP7RootToolsSDK.FileSystem.ReadFile(path);
                string html = Encoding.Unicode.GetString(contents, 0, contents.Length);
                webBrowser1.NavigateToString(html);
            }
            else
            {
                NavigationService.GoBack();
            }
        }
    }
}