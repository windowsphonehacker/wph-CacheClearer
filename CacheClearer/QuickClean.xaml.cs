using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace CacheClearer
{
    public partial class QuickClean : PhoneApplicationPage
    {
        public QuickClean()
        {
            InitializeComponent();

            if (!WP7RootToolsSDK.Environment.HasRootAccess())
            {
                MessageBox.Show("No root access. Please allow through Root Tools");
                return;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            GlobalLoading.Instance.IsLoading = true;

            int saved = cleanCache.clearAll();

            GlobalLoading.Instance.IsLoading = false;
            MessageBox.Show("Cache cleaned.\n\nYou saved " + Utils.readableFileSize(saved) + " of storage space.");
        }

        private void pinMe(object sender, EventArgs e)
        {
            Microsoft.Phone.Shell.StandardTileData d = new StandardTileData();
            d.BackgroundImage = new Uri("Background.png", UriKind.Relative);
            try
            {
                Microsoft.Phone.Shell.ShellTile.Create(new Uri("/QuickCleanTile.xaml", UriKind.Relative), d);
            }
            catch
            {
            }
        }

        private void SettingsButton_Click(object sender, EventArgs e)
        {
            // Navigate to the new page
            NavigationService.Navigate(new Uri("/SettingsPage.xaml?page=settings", UriKind.Relative));
        }
        private void AboutButton_Click(object sender, EventArgs e)
        {
            // Navigate to the new page
            NavigationService.Navigate(new Uri("/SettingsPage.xaml?page=about", UriKind.Relative));
        }

        private void AdvancedButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void PhoneApplicationPage_Loaded_1(object sender, RoutedEventArgs e)
        {
            WindowsPhoneHacker.wph.bacon("CacheClearer2");
        }
    }
}