#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

#endregion

namespace RAA_2021_MODULE_01
{
    [Transaction(TransactionMode.Manual)]
    public class Module01Challenge : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Adding for a test
            // this is a variable for the Revit application
            UIApplication uiapp = commandData.Application;

            // this is a variable for the current Revit model
            Document doc = uiapp.ActiveUIDocument.Document;

            //Set Variables
            int numFloors = 250;
            double currentElev = 0;
            int floorHeight = 15;
            int numFloorPlans = 0;
            int numCeilingPlans = 0;
            int numSheets = 0;

            //Get Titleblock
            FilteredElementCollector tbCollector = new FilteredElementCollector(doc);
            tbCollector.OfCategory(BuiltInCategory.OST_TitleBlocks);
            tbCollector.WhereElementIsElementType();
            ElementId tblockId = tbCollector.FirstElementId();

            //Get View Family Types
            FilteredElementCollector vftCollector = new FilteredElementCollector(doc);
            vftCollector.OfClass(typeof(ViewFamilyType));

            ViewFamilyType fpVPT = null;
            ViewFamilyType cpVPT = null;

            //Set the Viewtype of each collected View.
            foreach (ViewFamilyType curVFT in vftCollector)
            {
                if (curVFT.ViewFamily == ViewFamily.FloorPlan)
                {
                    fpVPT = curVFT;
                }

                else if (curVFT.ViewFamily == ViewFamily.CeilingPlan)
                {
                    cpVPT = curVFT;
                }
            }

            //Create a transaction to lock the model.
            Transaction t = new Transaction(doc);
            t.Start("Module_01_FIZZBUZZ Challenge");

            //Create a loop to go through floors.
            for (int i = 1; i <= numFloors; i++)
            {
                //Create Levels
                Level newLevel = Level.Create(doc, currentElev);
                newLevel.Name = "LEVEL " + i.ToString();

                //Increment the Elevation.
                currentElev += floorHeight;

                //Check for most restrictive case - FIZZBUZZ
                if (i % 3 == 0 && i % 5 == 0)
                {
                    //FIZZBUZZ - Create Sheet
                    ViewSheet newSheet = ViewSheet.Create(doc, tblockId);
                    newSheet.SheetNumber = i.ToString();
                    newSheet.Name = "FIZZBUZZ_" + i.ToString();

                    //BONUS - Create Floor Plan of each FIZZBUZZ
                    ViewPlan newFloorPlan = ViewPlan.Create(doc, fpVPT.Id, newLevel.Id);
                    newFloorPlan.Name = "FIZZBUZZ_" + i.ToString();

                    //BONUS - Add FIZZBUZZ Views to Sheets
                    XYZ insPoint = new XYZ(2, 1.5, 0);
                    Viewport newViewport = Viewport.Create(doc, newSheet.Id, newFloorPlan.Id, insPoint);

                    //BONUS - Count the # of times executed.
                    numSheets++;
                    numFloorPlans++;
                }
                else if (i % 3 == 0)
                {
                    //FIZZ - Create Floor Plan
                    ViewPlan newFloorPlan = ViewPlan.Create(doc, fpVPT.Id, newLevel.Id);
                    newFloorPlan.Name = "FIZZ_" + i.ToString();

                    //BONUS - Count the # of times executed.
                    numFloorPlans++;
                }
                else if (i % 5 == 0)
                {
                    //BUZZ - Create Ceiling Plan
                    ViewPlan newCeilingPlan = ViewPlan.Create(doc, cpVPT.Id, newLevel.Id);
                    newCeilingPlan.Name = "BUZZ_" + i.ToString();

                    //BONUS - Count the # of times executed.
                    numCeilingPlans++;
                }

            }

            //Make changes in Revit.
            t.Commit();
            t.Dispose();

            //Alert the user of the actions.
            TaskDialog.Show("Complete", "Created " + numFloors + " levels.");
            TaskDialog.Show("Complete", "Created the following: " + numFloorPlans + " Floor Plans, " + numCeilingPlans + " Ceiling Plans, and " + numSheets + " Sheets.");

            return Result.Succeeded;
        }
    }
}
