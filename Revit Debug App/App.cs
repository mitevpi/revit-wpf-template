#region Namespaces

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Media.Imaging;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

#endregion

namespace RevitTemplate
{
    class App : IExternalApplication
    {
        // class instance
        public static App ThisApp = null;

        // ModelessForm instance
        private Ui _mMyForm;

        public Result OnStartup(UIControlledApplication a)
        {
            _mMyForm = null; // no dialog needed yet; the command will bring it
            ThisApp = this; // static access to this application instance

            // Method to add Tab and Panel 
            RibbonPanel panel = RibbonPanel(a);
            string thisAssemblyPath = Assembly.GetExecutingAssembly().Location;
            PushButton button =
                panel.AddItem(
                        new PushButtonData("Revit Template", "Revit Template", thisAssemblyPath, "RevitTemplate.EntryCommand")) as
                    PushButton;

            button.ToolTip = "Visual interface for debugging applications.";
            Uri uriImage = new Uri("pack://application:,,,/RevitTemplate;component/Resources/code-small.png");
            BitmapImage largeImage = new BitmapImage(uriImage);
            button.LargeImage = largeImage;

            a.ApplicationClosing += a_ApplicationClosing; //Set Application to Idling
            a.Idling += a_Idling;

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }

        //   The external command invokes this on the end-user's request
        public void ShowForm(UIApplication uiapp)
        {
            // If we do not have a dialog yet, create and show it
            if (_mMyForm == null || _mMyForm != null) // || m_MyForm.IsDisposed
            {
                //EXTERNAL EVENTS WITH ARGUMENTS
                EventHandlerWithStringArg evStr = new EventHandlerWithStringArg();
                EventHandlerWithWpfArg eDatabaseStore = new EventHandlerWithWpfArg();

                // The dialog becomes the owner responsible for disposing the objects given to it.
                _mMyForm = new Ui(uiapp, evStr, eDatabaseStore);
                _mMyForm.Show();
            }
        }

        #region Idling & Closing

        //*****************************a_Idling()*****************************
        void a_Idling(object sender, Autodesk.Revit.UI.Events.IdlingEventArgs e)
        {
        }

        //*****************************a_ApplicationClosing()*****************************
        void a_ApplicationClosing(object sender, Autodesk.Revit.UI.Events.ApplicationClosingEventArgs e)
        {
            //System.Windows.Application.Current.Shutdown();
            //System.Environment.Exit(1);
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
            catch
            {
            }

            // Try to create ribbon panel.
            try
            {
                RibbonPanel panel = a.CreateRibbonPanel(tab, "Develop");
            }
            catch
            {
            }

            // Search existing tab for your panel.
            List<RibbonPanel> panels = a.GetRibbonPanels(tab);
            foreach (RibbonPanel p in panels)
            {
                if (p.Name == "Develop")
                {
                    ribbonPanel = p;
                }
            }

            //return panel 
            return ribbonPanel;
        }

        #endregion
    }


    #region Method-Specific External Events

    public class EventHandlerWithStringArg : RevitEventWrapper<string>
    {
        public override void Execute(UIApplication uiApp, string args)
        {
            // Do your processing here with "args"
            TaskDialog.Show("External Event", args);
        }
    }

    public class EventHandlerWithWpfArg : RevitEventWrapper<Ui>
    {
        public override void Execute(UIApplication uiApp, Ui ui)
        {
            // SETUP
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;

            // METHODS
            if (ui.CbDocumentData.IsChecked == true)
            {
                Methods.DocumentInfo(ui, doc);
            }

            if (ui.CbSheetData.IsChecked == true)
            {
                Methods.SheetTakeoff(ui, doc);
            }

            Methods.WallInfo(ui, doc);
        }
    }

    #endregion
}