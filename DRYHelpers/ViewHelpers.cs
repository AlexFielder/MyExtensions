using Inventor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRYHelpers
{
    public static class ViewHelpers
    {
        /// <summary>
        /// Uses the built-in Look at command to set the front view on the Viewcube
        /// </summary>
        /// <param name="InventorApp"></param>
        /// <param name="partDoc"></param>
        /// <param name="selectedWorkPlane"></param>
        public static void SetFrontViewToWorkplane(Inventor.Application InventorApp, PartDocument partDoc, WorkPlane selectedWorkPlane)
        {
            partDoc.SelectSet.Clear();
            partDoc.SelectSet.Select(selectedWorkPlane);
            SelectSet selSet = partDoc.SelectSet;
            ControlDefinition LookAt = InventorApp.CommandManager.ControlDefinitions["AppLookAtCmd"];
            LookAt.Execute();
            selSet.Clear();
            View activeView = InventorApp.ActiveView;
            activeView.SetCurrentAsFront();
        }
    }
}
