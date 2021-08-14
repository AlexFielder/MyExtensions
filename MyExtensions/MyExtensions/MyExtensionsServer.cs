using Inventor;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MyExtensions
{
    /// <summary>
    /// This is the primary AddIn Server class that implements the ApplicationAddInServer interface
    /// that all Inventor AddIns are required to implement. The communication between Inventor and
    /// the AddIn is via the methods on this interface.
    /// </summary>
    [GuidAttribute("fd207621-7782-482d-b938-8c3c2442bb98")]
    public class MyExtensionsServer : Inventor.ApplicationAddInServer
    {

        // Inventor application object.
        private Inventor.Application m_inventorApplication;

        public MyExtensionsServer()
        {
        }

        #region ApplicationAddInServer Members

        /// <summary>
        /// Allows us to add buttons to the ribbon
        /// </summary>
        /// <param name="buttonPlugin"></param>
        /// <param name="uiMan"></param>
        /// <param name="RibbonNames"></param>
        public static void AddButtonToRibbon(IVButtonPlugin buttonPlugin, UserInterfaceManager uiMan, List<string> RibbonNames, out ButtonDefinition buttonDefinition)
        {
            buttonDefinition = null;
            try
            {
                Icon icon1 = GetICOResource(buttonPlugin.DefaultResourceName, System.Reflection.Assembly.LoadFrom(buttonPlugin.Path));
                InventorButton button1;
                if (buttonPlugin.CanHaveProgressiveToolTip.Value)
                {
                    button1 = new InventorButton(buttonPlugin.Path, buttonPlugin.ButtonDisplayName, buttonPlugin.ButtonInternalName, buttonPlugin.Description,
                        buttonPlugin.ToolTip, icon1, icon1, buttonPlugin.ProgressiveToolTipImagePath, CommandTypesEnum.kShapeEditCmdType, ButtonDisplayEnum.kDisplayTextInLearningMode);
                    button1.ButtonDef.ProgressiveToolTip.Description = buttonPlugin.ProgressiveToolTipDescription;
                    button1.ButtonDef.ProgressiveToolTip.ExpandedDescription = buttonPlugin.ProgressiveToolTipExpandedDescription;
                    button1.ButtonDef.ProgressiveToolTip.IsProgressive = true;
                    button1.ButtonDef.ProgressiveToolTip.Title = buttonPlugin.ProgressiveToolTipTitle;
                    //IPictureDisp progImage;
                    //progImage = LoadPicture();
                }
                else
                {
                    button1 = new InventorButton(
                        buttonPlugin.ButtonDisplayName, buttonPlugin.ButtonInternalName, buttonPlugin.Description, buttonPlugin.ToolTip,
                        icon1, icon1, CommandTypesEnum.kShapeEditCmdType, ButtonDisplayEnum.kDisplayTextInLearningMode);
                }
                if (buttonPlugin.IncludesProgressiveToolTipHelp)
                {
                    button1.ButtonDef.OnHelp += new ButtonDefinitionSink_OnHelpEventHandler(buttonPlugin.ButtonDefinition_OnHelp);
                }
                buttonDefinition = button1.ButtonDef;
                button1.Execute = buttonPlugin.Execute;
                foreach (string item in RibbonNames)
                {
                    AddButtonToRibbon(button1, uiMan, item, buttonPlugin.RibbonPanelName);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
            }
        }

        public void Activate(Inventor.ApplicationAddInSite addInSiteObject, bool firstTime)
        {
            // This method is called by Inventor when it loads the addin.
            // The AddInSiteObject provides access to the Inventor Application object.
            // The FirstTime flag indicates if the addin is loaded for the first time.

            // Initialize AddIn members.
            m_inventorApplication = addInSiteObject.Application;

            // TODO: Add ApplicationAddInServer.Activate implementation.
            // e.g. event initialization, command creation etc.
        }

        public void Deactivate()
        {
            // This method is called by Inventor when the AddIn is unloaded.
            // The AddIn will be unloaded either manually by the user or
            // when the Inventor session is terminated

            // TODO: Add ApplicationAddInServer.Deactivate implementation

            // Release objects.
            m_inventorApplication = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public void ExecuteCommand(int commandID)
        {
            // Note:this method is now obsolete, you should use the 
            // ControlDefinition functionality for implementing commands.
        }

        public object Automation
        {
            // This property is provided to allow the AddIn to expose an API 
            // of its own to other programs. Typically, this  would be done by
            // implementing the AddIn's API interface in a class and returning 
            // that class object through this property.

            get
            {
                // TODO: Add ApplicationAddInServer.Automation getter implementation
                return null;
            }
        }

        #endregion

    }
}
