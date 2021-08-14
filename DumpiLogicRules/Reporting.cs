using Inventor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DumpiLogicRules
{
    /// <summary>
    /// The purpose of this class is to provide feedback to the user - and also prove that we grabbed the Inventor Application object
    /// from the parent Extensions Server.
    /// </summary>
    public class Reporting
    {
        public static Application m_InventorApp = DumpiLogicRules.m_InventorApp;

        ///Updates the statusbar with a percentage so we can see how far along we are.
        public void UpdateStatusBar(double percent, string Message = "")
        {
            m_InventorApp.StatusBarText = Message + " (" + percent.ToString("P1") + ")";
        }

        public void UpdateStatusBar(string Message)
        {
            m_InventorApp.StatusBarText = Message;
        }

    }
}
