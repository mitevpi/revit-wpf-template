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

        public static void SheetTakeoff(Ui ui, Document doc)
        {
            // get sheets
            ICollection<ViewSheet> sheets = new FilteredElementCollector(doc)
                .OfClass(typeof(ViewSheet))
                .Select(p => (ViewSheet)p).ToList();

            // report the count
            string message = $"There are {sheets.Count} Sheets in the project";
            ui.TbDebug.Text += "\n" + (DateTime.Now).ToLongTimeString() + "\t" + message;

            // delete all the sheets
            using (Transaction t = new Transaction(doc, "Delete Sheets"))
            {
                t.Start("Delete Area Lines");

                foreach (ViewSheet sheet in sheets)
                {
                    // delete the sheet and tell the user
                    // doc.Delete(sheet.Id);
                    sheet.LookupParameter("Sheet Name").SetValueString("TEST");
                    string deletedMessage = $"Deleted Sheet: {sheet.Title}";
                    ui.TbDebug.Text += "\n" + (DateTime.Now).ToLongTimeString() + "\t" + deletedMessage;
                }

                t.Commit();
                t.Dispose();
            }

            // report completion
            ui.TbDebug.Text += "\n" + (DateTime.Now).ToLongTimeString() + "\t" + "SHEETS HAVE BEEN DELETED";
        }

        public static void DocumentInfo(Ui ui, Document doc)
        {
            ui.TbDebug.Text += "\n" + (DateTime.Now).ToLongTimeString() + "\t" + doc.Title;
        }

        public static void WallInfo(Ui ui, Document doc)
        {
            ICollection<Wall> walls = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Walls).WhereElementIsNotElementType()
                .Select(p => (Wall)p).ToList();

            string message = $"There are {walls.Count} Walls in the project";

            ui.TbDebug.Text += "\n" + (DateTime.Now).ToLongTimeString() + "\t" + message;
        }

    }
}

