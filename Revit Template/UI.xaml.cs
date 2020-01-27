using System;
using System.Windows;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitTemplate
{
    /// <summary>
    /// Interaction logic for UI.xaml
    /// </summary>
    public partial class Ui : Window
    {
        private readonly Document _doc;
        private readonly UIApplication _uiApp;
        private readonly UIDocument _uiDoc;
        private readonly Autodesk.Revit.ApplicationServices.Application _app;

        private EventHandlerWithStringArg _mExternalMethodStringArg;
        private EventHandlerWithWpfArg _mExternalMethodWpfArg;

        public Ui(UIApplication uiApp, EventHandlerWithStringArg evExternalMethodStringArg, EventHandlerWithWpfArg eExternalMethodWpfArg)
        {
            _uiDoc = uiApp.ActiveUIDocument;
            _doc = _uiDoc.Document;
            _app = _doc.Application;
            Closed += MainWindow_Closed;

            InitializeComponent();
            _mExternalMethodStringArg = evExternalMethodStringArg;
            _mExternalMethodWpfArg = eExternalMethodWpfArg;
        }


        private void MainWindow_Closed(object sender, EventArgs e)
        {
            Close();
        }

        private void bTest_Click(object sender, RoutedEventArgs e)
        {
            _mExternalMethodStringArg.Raise("Test String Passed");
        }

        private void BExternalMethod1_Click(object sender, RoutedEventArgs e)
        {
            _mExternalMethodWpfArg.Raise(this);
        }

        #region Non-External Project Methods

        private void BNonExternal3_Click(object sender, RoutedEventArgs e)
        {
            // the sheet takeoff + delete method won't work here because it's not in a valid Revit api context
            // and we need to do a transaction
            // Methods.SheetTakeoff(this, _doc);
            TaskDialog.Show("Non-External Method", "Non-External Method Executed Successfully");
        }

        private void BNonExternal1_Click(object sender, RoutedEventArgs e)
        {
            Methods.DocumentInfo(this, _doc);
            TaskDialog.Show("Non-External Method", "Non-External Method Executed Successfully");
        }

        private void BNonExternal2_Click(object sender, RoutedEventArgs e)
        {
            Methods.WallInfo(this, _doc);
            TaskDialog.Show("Non-External Method", "Non-External Method Executed Successfully");
        }

        #endregion
    }
}