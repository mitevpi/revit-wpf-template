using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;

namespace RevitTemplate
{
    /// <summary>
    /// This is the main class which defines the Application, and inherits from Revit's
    /// IExternalApplication class.
    /// </summary>
    class App : IExternalApplication
    {
        // class instance
        public static App ThisApp;

        // ModelessForm instance
        private Ui _mMyForm;

        // Separate thread to run Ui on
        private Thread _uiThread;

        public Result OnStartup(UIControlledApplication a)
        {
            _mMyForm = null; // no dialog needed yet; the command will bring it
            ThisApp = this; // static access to this application instance

            // Method to add Tab and Panel 
            RibbonPanel panel = RibbonPanel(a);
            string thisAssemblyPath = Assembly.GetExecutingAssembly().Location;

            // BUTTON FOR THE SINGLE-THREADED WPF OPTION
            if (panel.AddItem(
                new PushButtonData("WPF Template", "WPF Template", thisAssemblyPath,
                    "RevitTemplate.EntryCommand")) is PushButton button)
            {
                // defines the tooltip displayed when the button is hovered over in Revit's ribbon
                button.ToolTip = "Visual interface for debugging applications.";
                // defines the icon for the button in Revit's ribbon - note the string formatting
                Uri uriImage = new Uri("pack://application:,,,/RevitTemplate;component/Resources/code-small.png");
                BitmapImage largeImage = new BitmapImage(uriImage);
                button.LargeImage = largeImage;
            }

            // BUTTON FOR THE MULTI-THREADED WPF OPTION
            if (panel.AddItem(
                new PushButtonData("WPF Template\nMulti-Thread", "WPF Template\nMulti-Thread", thisAssemblyPath,
                    "RevitTemplate.EntryCommandSeparateThread")) is PushButton button2)
            {
                button2.ToolTip = "Visual interface for debugging applications.";
                Uri uriImage = new Uri("pack://application:,,,/RevitTemplate;component/Resources/code-small.png");
                BitmapImage largeImage = new BitmapImage(uriImage);
                button2.LargeImage = largeImage;
            }


            // listeners/watchers for external events (if you choose to use them)
            a.ApplicationClosing += a_ApplicationClosing; //Set Application to Idling
            a.Idling += a_Idling;

            return Result.Succeeded;
        }

        /// <summary>
        /// What to do when the application is shut down.
        /// </summary>
        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }

        /// <summary>
        /// This is the method which launches the WPF window, and injects any methods that are
        /// wrapped by ExternalEventHandlers. This can be done in a number of different ways, and
        /// implementation will differ based on how the WPF is set up.
        /// </summary>
        /// <param name="uiapp">The Revit UIApplication within the add-in will operate.</param>
        public void ShowForm(UIApplication uiapp)
        {
            // If we do not have a dialog yet, create and show it
            if (_mMyForm != null && _mMyForm == null) return;
            //EXTERNAL EVENTS WITH ARGUMENTS
            EventHandlerWithStringArg evStr = new EventHandlerWithStringArg();
            EventHandlerWithWpfArg evWpf = new EventHandlerWithWpfArg();

            // The dialog becomes the owner responsible for disposing the objects given to it.
            _mMyForm = new Ui(uiapp, evStr, evWpf);
            _mMyForm.Show();
        }

        /// <summary>
        /// This is the method which launches the WPF window in a separate thread, and injects any methods that are
        /// wrapped by ExternalEventHandlers. This can be done in a number of different ways, and
        /// implementation will differ based on how the WPF is set up.
        /// </summary>
        /// <param name="uiapp">The Revit UIApplication within the add-in will operate.</param>
        public void ShowFormSeparateThread(UIApplication uiapp)
        {
            // If we do not have a thread started or has been terminated start a new one
            if (!(_uiThread is null) && _uiThread.IsAlive) return;
            //EXTERNAL EVENTS WITH ARGUMENTS
            EventHandlerWithStringArg evStr = new EventHandlerWithStringArg();
            EventHandlerWithWpfArg evWpf = new EventHandlerWithWpfArg();

            _uiThread = new Thread(() =>
            {
                SynchronizationContext.SetSynchronizationContext(
                    new DispatcherSynchronizationContext(
                        Dispatcher.CurrentDispatcher));
                // The dialog becomes the owner responsible for disposing the objects given to it.
                _mMyForm = new Ui(uiapp, evStr, evWpf);
                _mMyForm.Closed += (s, e) => Dispatcher.CurrentDispatcher.InvokeShutdown();
                _mMyForm.Show();
                Dispatcher.Run();
            });

            _uiThread.SetApartmentState(ApartmentState.STA);
            _uiThread.IsBackground = true;
            _uiThread.Start();
        }

        #region Idling & Closing

        /// <summary>
        /// What to do when the application is idling. (Ideally nothing)
        /// </summary>
        void a_Idling(object sender, IdlingEventArgs e)
        {
        }

        /// <summary>
        /// What to do when the application is closing.)
        /// </summary>
        void a_ApplicationClosing(object sender, ApplicationClosingEventArgs e)
        {
        }

        #endregion

        #region Ribbon Panel

        public RibbonPanel RibbonPanel(UIControlledApplication a)
        {
            string tab = "Template"; // Tab name
            // Empty ribbon panel 
            RibbonPanel ribbonPanel = null;
            // Try to create ribbon tab. 
            try
            {
                a.CreateRibbonTab(tab);
            }
            catch (Exception ex)
            {
                Util.HandleError(ex);
            }

            // Try to create ribbon panel.
            try
            {
                RibbonPanel panel = a.CreateRibbonPanel(tab, "Develop");
            }
            catch (Exception ex)
            {
                Util.HandleError(ex);
            }

            // Search existing tab for your panel.
            List<RibbonPanel> panels = a.GetRibbonPanels(tab);
            foreach (RibbonPanel p in panels.Where(p => p.Name == "Develop"))
            {
                ribbonPanel = p;
            }

            //return panel 
            return ribbonPanel;
        }

        #endregion
    }
}