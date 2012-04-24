using System.Windows;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using System.Linq;
using System.IO.IsolatedStorage;
namespace TileUpdaterAgent
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        private static volatile bool _classInitialized;

        /// <remarks>
        /// ScheduledAgent constructor, initializes the UnhandledException handler
        /// </remarks>
        public ScheduledAgent()
        {
            if (!_classInitialized)
            {
                _classInitialized = true;
                // Subscribe to the managed exception handler
                Deployment.Current.Dispatcher.BeginInvoke(delegate
                {
                    Application.Current.UnhandledException += ScheduledAgent_UnhandledException;
                });
            }
        }

        /// Code to execute on Unhandled Exceptions
        private void ScheduledAgent_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        /// <summary>
        /// Agent that runs a scheduled task
        /// </summary>
        /// <param name="task">
        /// The invoked task
        /// </param>
        /// <remarks>
        /// This method is called when a periodic or resource intensive task is invoked
        /// </remarks>
        protected override void OnInvoke(ScheduledTask task)
        {
            System.Diagnostics.Debug.WriteLine("Launching the agent...");

            if (getSetting("clean"))
            {
                //Clean cache
                CacheClearer.cleanCache.clearAll();
            }

            if (getSetting("updatetile"))
            {
                uint bytes = CacheClearer.cleanCache.getTotalCacheSize();

                // Execute periodic task actions here.
                ShellTile TileToFind = ShellTile.ActiveTiles.First();
                if (TileToFind != null)
                {
                    StandardTileData NewTileData = new StandardTileData
                    {
                        BackContent = CacheClearer.Utils.readableFileSize(bytes),
                        BackTitle = "Cache Size"
                    };
                    TileToFind.Update(NewTileData);
                }
            }


            NotifyComplete();
        }
        bool getSetting(string key)
        {
            bool val = false;

            IsolatedStorageSettings.ApplicationSettings.TryGetValue(key, out val);

            return val;
        }
    }
}