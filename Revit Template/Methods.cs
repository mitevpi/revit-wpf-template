using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace RevitTemplate
{
    /// <summary>
    /// Create methods here that need to be wrapped in a valid Revit Api context.
    /// Things like transactions modifying Revit Elements, etc.
    /// </summary>
    internal class Methods
    {
        /// <summary>
        /// Rename all the sheets in the project. This opens a transaction, and it MUST be executed
        /// in a "Valid Revit API Context", otherwise the add-in will crash. Because of this, we must
        /// wrap it in a ExternalEventHandler, as we do in the App.cs file in this template.
        /// </summary>
        /// <param name="ui">An instance of our UI class, which in this template is the main WPF
        /// window of the application.</param>
        /// <param name="doc">The Revit Document to rename sheets in.</param>
        public static void SheetRename(Ui ui, Document doc)
        {
            ICollection<ViewSheet> sheets = null;
            sheets = new FilteredElementCollector(doc)
                .OfClass(typeof(ViewSheet))
                .Select(p => (ViewSheet) p).ToList();

            // get sheets, report results - push the task off to another thread
            Task.Run(() =>
            {
                // report the count
                string message = $"There are {sheets.Count} Sheets in the project";
                ui.Dispatcher.Invoke(() =>
                    ui.TbDebug.Text += "\n" + (DateTime.Now).ToLongTimeString() + "\t" + message);
            });

            // rename all the sheets, but first open a transaction
            using (Transaction t = new Transaction(doc, "Rename Sheets"))
            {
                // start a transaction within the valid Revit API context
                t.Start("Rename Sheets");

                // loop over the collection of sheets
                foreach (ViewSheet sheet in sheets)
                {
                    // rename the sheets
                    bool? renamed = sheet.LookupParameter("Sheet Name")?.Set("TEST");
                    string renameMessage = $"Renamed: {sheet.Title}, Status: {renamed}";
                    ui.Dispatcher.Invoke(() =>
                        ui.TbDebug.Text += "\n" + (DateTime.Now).ToLongTimeString() + "\t" + renameMessage);
                }

                t.Commit();
                t.Dispose();
            }

            // invoke the UI dispatcher to print the results to report completion
            ui.Dispatcher.Invoke(() =>
                ui.TbDebug.Text += "\n" + (DateTime.Now).ToLongTimeString() + "\t" + "SHEETS HAVE BEEN RENAMED");
        }

        /// <summary>
        /// Print the Title of the Revit Document on the main text box of the WPF window of this application.
        /// </summary>
        /// <param name="ui">An instance of our UI class, which in this template is the main WPF
        /// window of the application.</param>
        /// <param name="doc">The Revit Document to print the Title of.</param>
        public static void DocumentInfo(Ui ui, Document doc)
        {
            ui.Dispatcher.Invoke(() => ui.TbDebug.Text += "\n" + (DateTime.Now).ToLongTimeString() + "\t" + doc.Title);
        }

        /// <summary>
        /// Count the walls in the Revit Document, and print the count
        /// on the main text box of the WPF window of this application.
        /// </summary>
        /// <param name="ui">An instance of our UI class, which in this template is the main WPF
        /// window of the application.</param>
        /// <param name="doc">The Revit Document to count the walls of.</param>
        public static void WallInfo(Ui ui, Document doc)
        {
            Task.Run(() =>
            {
                // get all walls in the document
                ICollection<Wall> walls = new FilteredElementCollector(doc)
                    .OfCategory(BuiltInCategory.OST_Walls).WhereElementIsNotElementType()
                    .Select(p => (Wall) p).ToList();

                // format the message to show the number of walls in the project
                string message = $"There are {walls.Count} Walls in the project";

                // invoke the UI dispatcher to print the results to the UI
                ui.Dispatcher.Invoke(() =>
                    ui.TbDebug.Text += "\n" + (DateTime.Now).ToLongTimeString() + "\t" + message);
            });
        }
    }
}