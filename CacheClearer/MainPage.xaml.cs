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
using Microsoft.Phone.Scheduler;

namespace CacheClearer
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            if (!WP7RootToolsSDK.Environment.HasRootAccess())
            {
                MessageBox.Show("No root access. Please allow through Root Tools");
                return;
            }

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);

        }


        private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If selected index is -1 (no selection) do nothing
            if (listBox1.SelectedIndex == -1)
                return;

            AppListItemViewModel item = (AppListItemViewModel)listBox1.SelectedItem;
            // Navigate to the new page
            //GlobalLoading.Instance.IsLoading = true;
            NavigationService.Navigate(new Uri("/DetailsPage.xaml?appguid=" + item.LineTwo, UriKind.Relative));

            // Reset selected index to -1 (no selection)
            listBox1.SelectedIndex = -1;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This will permanently erase cache files from your phone.", "Warning", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
            {
                return;
            }
            GlobalLoading.Instance.IsLoading = true;
            int saved = 0;

            foreach (AppListItemViewModel item in listBox1.Items)
            {
                System.Diagnostics.Debug.WriteLine(item);
                saved += cleanCache.cleanAppCache(item.LineTwo);
            }
            GlobalLoading.Instance.IsLoading = false;
            MessageBox.Show("Cache cleaned.\n\nYou saved " + Utils.readableFileSize(saved) + " of storage space.");
        }
        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
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

    }
}