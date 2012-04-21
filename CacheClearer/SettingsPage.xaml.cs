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
            task.Description = "This demonstrates a periodic task.";

            ScheduledActionService.Add(task);
            ScheduledActionService.LaunchForTest("TileUpdaterAgent", TimeSpan.FromSeconds(10));

            System.Diagnostics.Debug.WriteLine("Agent added (hopefully)");
        }
    }
}