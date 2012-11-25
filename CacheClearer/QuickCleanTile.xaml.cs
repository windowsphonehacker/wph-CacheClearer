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
    public partial class QuickCleanTile : PhoneApplicationPage
    {
        public QuickCleanTile()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded_1(object sender, RoutedEventArgs e)
        {
            GlobalLoading.Instance.IsLoading = true;

            int saved = cleanCache.clearAll();

            GlobalLoading.Instance.IsLoading = false;
            MessageBox.Show("Cache cleaned.\n\nYou saved " + Utils.readableFileSize(saved) + " of storage space.");

            //close the app
            throw new Exception();
        }
    }
}