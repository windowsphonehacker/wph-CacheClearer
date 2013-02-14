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
using Microsoft.Phone.Scheduler;
using System.IO.IsolatedStorage;
namespace CacheClearer
{
    public partial class SettingsPage : PhoneApplicationPage
    {
        PeriodicTask task;
        public SettingsPage()
        {
            InitializeComponent();
#if DEBUG
            button2.Visibility = System.Windows.Visibility.Visible;
#endif

            toggleSwitch_Task.IsChecked = findAgent();
            toggleSwitch_Tile.IsChecked = getSetting("updatetile");
            toggleSwitch_Clean.IsChecked = getSetting("clean");
            toggleSwitch_Toast.IsChecked = getSetting("toast");
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string page = "about";
            NavigationContext.QueryString.TryGetValue("page", out page);
            switch (page)
            {
                case "about":
                    MainPivot.SelectedItem = AboutPivotItem;
                    break;
                case "settings":
                    MainPivot.SelectedItem = SettingsPivotItem;
                    break;
            }
        }
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Adding an agent...");
            task = ScheduledActionService.Find("TileUpdaterAgent") as PeriodicTask;
            if (task != null)
            {
                System.Diagnostics.Debug.WriteLine("Existing agent found, removing it.");
                ScheduledActionService.Remove("TileUpdaterAgent");
            }
            task = new PeriodicTask("TileUpdaterAgent");
            task.Description = "Periodically does cache cleaning functions.";

            ScheduledActionService.Add(task);
            ScheduledActionService.LaunchForTest("TileUpdaterAgent", TimeSpan.FromSeconds(10));

            System.Diagnostics.Debug.WriteLine("Agent added (hopefully)");
        }

        Boolean addAgent()
        {
            try
            {
                if (findAgent())
                    removeAgent();

                task = new PeriodicTask("TileUpdaterAgent");
                task.Description = "Periodically does cache cleaning functions";

                ScheduledActionService.Add(task);
                ScheduledActionService.LaunchForTest("TileUpdaterAgent", TimeSpan.FromSeconds(10));
                return true;
            }
            catch (Exception)
            {
                toggleSwitch_Task.IsChecked = false;
                MessageBox.Show("The task could not be enabled. You might have background tasks disabled in settings.", "Error", MessageBoxButton.OK);
                return false;
            }
        }
        void removeAgent()
        {
            try
            {
                ScheduledActionService.Remove("TileUpdaterAgent");
            }
            catch
            {
            }
        }
        bool findAgent()
        {
            task = ScheduledActionService.Find("TileUpdaterAgent") as PeriodicTask;
            if (task != null)
            {
                return true;
            }
            return false;
        }

        private void hyperlinkButton1_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Phone.Tasks.WebBrowserTask task = new Microsoft.Phone.Tasks.WebBrowserTask();
            task.Uri = new Uri("http://windowsphonehacker.com/");
            task.Show();
        }

        private void toggleSwitch_Task_Checked(object sender, RoutedEventArgs e)
        {
            if (addAgent())
            {
                toggleSwitch_Tile.Visibility = System.Windows.Visibility.Visible;
                if (!(bool)toggleSwitch_Clean.IsChecked)
                {
                    toggleSwitch_Tile.IsChecked = true;
                }
                else
                {
                    toggleSwitch_Toast.Visibility = System.Windows.Visibility.Visible;
                }
                toggleSwitch_Clean.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void toggleSwitch_Task_Unchecked(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Task unchecked.");
            removeAgent();
            toggleSwitch_Tile.Visibility = System.Windows.Visibility.Collapsed;
            toggleSwitch_Clean.Visibility = System.Windows.Visibility.Collapsed;
            toggleSwitch_Toast.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void toggleSwitch_Tile_Checked(object sender, RoutedEventArgs e)
        {
            setSetting("updatetile", true);
        }

        void setSetting(string key, bool val)
        {
            //remove if it's there
            try
            {
                IsolatedStorageSettings.ApplicationSettings.Remove(key);
            }
            catch
            {
            }
            IsolatedStorageSettings.ApplicationSettings.Add(key, val);
            IsolatedStorageSettings.ApplicationSettings.Save();
        }
        bool getSetting(string key)
        {
            bool val = false;

            IsolatedStorageSettings.ApplicationSettings.TryGetValue(key, out val);

            return val;
        }

        private void toggleSwitch_Clean_Checked(object sender, RoutedEventArgs e)
        {
            setSetting("clean", true);
            toggleSwitch_Toast.Visibility = System.Windows.Visibility.Visible;
        }

        private void toggleSwitch_Clean_Unchecked(object sender, RoutedEventArgs e)
        {
            setSetting("clean", false);
            if (!(bool)toggleSwitch_Tile.IsChecked)
            {
                toggleSwitch_Task.IsChecked = false;
            }
            toggleSwitch_Toast.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void toggleSwitch_Tile_Unchecked(object sender, RoutedEventArgs e)
        {
            setSetting("updatetile", false);
            if (!(bool)toggleSwitch_Clean.IsChecked)
            {
                toggleSwitch_Task.IsChecked = false;
            }
        }
        private void toggleSwitch_Toast_Checked(object sender, RoutedEventArgs e)
        {
            setSetting("toast", true);
        }
        private void toggleSwitch_Toast_Unchecked(object sender, RoutedEventArgs e)
        {
            setSetting("toast", false);
        }
    }
}