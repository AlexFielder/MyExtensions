using Inventor;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;
using MyExtensionsContracts;
using log4net.Config;
using log4net;
using System.Collections;
using System.Linq;
using System.Windows.Interop;
using System.Configuration;
using System.Reflection;
using System.Xml;
using System.Collections.Specialized;

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
        #region Fields
        public static Configuration MyAppConfig = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
        public bool _LinearMarkingMenu = true;
        public bool _RadialMarkingMenu = true;
        public string GlobalSettingsFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
        private static readonly ILog log = LogManager.GetLogger(typeof(MyExtensionsServer));
        private static MyLog4NetFileHelper logHelper = new MyLog4NetFileHelper();

        private Dictionary<string, IVButtonPlugin> _ButtonPlugins;
        private Dictionary<string, IVContextPlugin> _ContextPlugins;
        private Dictionary<string, IVDockableWindow> _DockablePlugins;
        private Dictionary<string, IVUIPlugin> _UIPlugins;

        private bool InventorAppHasQuit = false;
        private string m_addInClassIDString = string.Empty;

        private ApplicationEvents m_AppEvents = null;
        private InteractionEvents m_interactionEvents = null;
        private UserInterfaceEvents m_uiEvents = null;
        private UserInputEvents m_UserInputEvents = null;

        private string pluginsPath = string.Empty;
        private System.Reflection.Assembly thisAssembly = System.Reflection.Assembly.GetExecutingAssembly();
        private string thisAssemblyPath = string.Empty;
        #endregion

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

        /// <summary>
        /// Do initializations in it such as caching the application object, registering event handlers, and adding ribbon buttons.
        /// </summary>
        /// <param name="siteObj">The entry object for the addin.</param>
        /// <param name="loaded1stTime">Indicating whether the addin is loaded for the 1st time.</param>
        public void Activate(ApplicationAddInSite siteObj, bool loaded1stTime)
        {
            GuidAttribute addInCLS = default(GuidAttribute);
            addInCLS = (GuidAttribute)System.Attribute.GetCustomAttribute(typeof(MyExtensionsServer), typeof(GuidAttribute));
            m_addInClassIDString = "{" + addInCLS.Value + "}";

            thisAssemblyPath = System.IO.Path.GetDirectoryName(thisAssembly.Location);
            logHelper.Init();
            logHelper.AddConsoleLogging();
            //the next line works but we want a rolling log.
            logHelper.AddFileLogging(System.IO.Path.Combine(thisAssemblyPath, "MyExtensionsServer.log"));
            logHelper.AddFileLogging(@"C:\Logs\MyLogFile.txt", log4net.Core.Level.All, true);
            logHelper.AddRollingFileLogging(@"C:\Logs\RollingFileLog.txt", log4net.Core.Level.All, true);
            log.Debug("Loading My Extension server");



            pluginsPath = thisAssemblyPath + @"\Plugins";

            MyExtensionAddinGlobal.InventorApp = siteObj.Application;
            //can't create these until the Global object is created.
            m_uiEvents = MyExtensionAddinGlobal.InventorApp.UserInterfaceManager.UserInterfaceEvents;
            m_UserInputEvents = MyExtensionAddinGlobal.InventorApp.CommandManager.UserInputEvents;
            m_AppEvents = MyExtensionAddinGlobal.InventorApp.ApplicationEvents;
            //not needed here I think:
            //m_interactionEvents = MyExtensionAddinGlobal.InventorApp.CommandManager.CreateInteractionEvents();

            //m_AppEvents.OnOpenDocument += M_AppEventsDocumentOpened;
            m_AppEvents.OnActivateDocument += M_AppEventsDocumentActivated;
            //m_AppEvents.OnSaveDocument += M_AppEventsDocumentSaved;
            m_AppEvents.OnQuit += M_AppEventsAppQuit;
            m_UserInputEvents.OnLinearMarkingMenu += M_UserInputEvents_OnLinearMarkingMenu;
            //m_UserInputEvents.OnLinearMarkingMenu += UserInputEvents_OnLinearMarkingMenu(OnLinearMarkingMenu);

            try
            {
                MyExtensionAddinGlobal.GetAddinClassId(this.GetType());

                Icon icon1 = new Icon(this.GetType(), "Resources.addin.ico"); //Change it if necessary but make sure it's embedded.
                                                                              //default settings button.
                InventorButton button1 = new InventorButton(
                    "Settings", "MyExtensionsServer.Button_" + Guid.NewGuid().ToString(), "Change the settings", "Change the settings",
                    icon1, icon1,
                    CommandTypesEnum.kShapeEditCmdType, ButtonDisplayEnum.kDisplayTextInLearningMode);
                button1.SetBehavior(true, true, true);
                button1.Execute = MyExtensionServerButtonActions.Button1_Execute;




                if (loaded1stTime == true)
                {

                    //Main Settings button
                    UserInterfaceManager uiMan = MyExtensionAddinGlobal.InventorApp.UserInterfaceManager;
                    if (uiMan.InterfaceStyle == InterfaceStyleEnum.kRibbonInterface) //kClassicInterface support can be added if necessary.
                    {
                        List<string> settingsRibbons = GetRibbonsForPlugin(EnvironmentRibbonName.All.ToString());
                        AddButtonToRibbon(button1, uiMan, settingsRibbons);
                        //instantiate the pluginloader for buttons ONLY:
                        MyIVPluginLoader<IVButtonPlugin> loader = new MyIVPluginLoader<IVButtonPlugin>(pluginsPath);
                        //loader.InitPlugins();
                        _ButtonPlugins = new Dictionary<string, IVButtonPlugin>();
                        IEnumerable<IVButtonPlugin> plugins = loader.Plugins;
                        if (plugins != null)
                        {
                            //IEnumerable<IVButtonPlugin> plugins = (IEnumerable<IVButtonPlugin>)loader.Plugins;
                            //IEnumerable<IVButtonPlugin> plugins = loader.Plugins.Select(m => m.Value).ToList();

                            foreach (IVButtonPlugin item in plugins)
                            {
                                //get the configuration settings for this plugin
                                Configuration pluginConfig = loadPluginConfig(item.Path);

                                //KeyValuePair<string, string> pluginKVP;


                                var xmlDoc = new XmlDocument();
                                xmlDoc.Load(MyAppConfig.FilePath);
                                AppSettingsSection appSettings = pluginConfig.AppSettings;
                                ClientSettingsSection customSettings = (ClientSettingsSection)pluginConfig.GetSection("userSettings/" + item.PluginSettingsPrefix + ".Properties.Settings");
                                if (customSettings != null)
                                {
                                    ApplicationSettingsBase settings = Properties.Settings.Default;

                                    foreach (SettingElement setting in customSettings.Settings)
                                    {
                                        if (setting.Name.StartsWith(item.PluginSettingsPrefix))
                                        {
                                            //the following works but we don't seem to pull the new setting through in the config editor form when Inventor loads.
                                            if (settings.Properties.Cast<SettingsProperty>().All(s => s.Name != setting.Name))
                                            {
                                                settings.Add(setting.Name, setting.Value.ValueXml.InnerText);
                                            };
                                            var settingsNode = xmlDoc.SelectSingleNode("//MyExtensions.Properties.Settings");
                                            var nodeSettings = xmlDoc.SelectNodes("//MyExtensions.Properties.Settings");
                                            if (settingsNode.ChildNodes.Cast<XmlNode>().All(s => s.Attributes["name"].Value != setting.Name))
                                            {
                                                var nodeRegion = xmlDoc.CreateElement("setting");
                                                nodeRegion.SetAttribute("name", setting.Name);
                                                nodeRegion.SetAttribute("serializeAs", "String");
                                                xmlDoc.SelectSingleNode("//userSettings/MyExtensions.Properties.Settings").AppendChild(nodeRegion);
                                                var valueNode = xmlDoc.CreateElement("value");
                                                valueNode.InnerText = setting.Value.ValueXml.InnerText;
                                                nodeRegion.AppendChild(valueNode);
                                            }
                                        }
                                    }

                                    xmlDoc.Save(MyAppConfig.FilePath);
                                    ConfigurationManager.RefreshSection("//userSettings/MyExtensions.Properties.Settings");
                                    Properties.Settings.Default.Reload();
                                }

                                //pass each plugin our original logHelper and some other objects for later use.
                                item.LogFileHelper = logHelper;
                                item.InventorApp = MyExtensionAddinGlobal.InventorApp;
                                item.ParentSettingsFilePath = MyExtensionsServer.MyAppConfig.FilePath;
                                //store the information for possible use later
                                _ButtonPlugins.Add(item.ButtonInternalName, item);
                                List<string> ribbons = GetRibbonsForPlugin(item.EnvironmentRibbonName.ToString());
                                AddButtonToRibbon(item, uiMan, ribbons, out ButtonDefinition buttonDefinition);
                                item.ThisButtonDefinition = buttonDefinition;
                                log.Info("Added button: " + item.ButtonDisplayName + " to ribbon.");
                            }
                        }

                        //speculative addition:
                        MyIVPluginLoader<IVButtonCollection> buttonLoader = new MyIVPluginLoader<MyExtensionsContracts.IVButtonCollection>(pluginsPath);
                        if (buttonLoader != null)
                        {

                        }

                        MyIVPluginLoader<IVDockableWindow> windowLoader = new MyIVPluginLoader<IVDockableWindow>(pluginsPath);
                        if (windowLoader != null)
                        {
                            _DockablePlugins = new Dictionary<string, IVDockableWindow>();

                            IEnumerable<IVDockableWindow> dockablePlugins = windowLoader.Plugins;
                            //IEnumerable<IVDockableWindow> dockablePlugins = (IEnumerable<IVDockableWindow>)windowLoader.Plugins;
                            //IEnumerable<IVDockableWindow> dockablePlugins = loader.Plugins.Select(m => m.Value).ToList();
                            if (dockablePlugins != null)
                            {
                                foreach (IVDockableWindow item in dockablePlugins)
                                {
                                    item.LogFileHelper = logHelper;
                                    item.InventorApp = MyExtensionAddinGlobal.InventorApp;

                                    //var wpfWindow = item.ChildWindow;

                                    //// Could be a good idea to set the owner for this window
                                    //// especially if used as modeless
                                    //var helper = new WindowInteropHelper(wpfWindow);
                                    //helper.Owner = new IntPtr(AddinGlobal.InventorApp.MainFrameHWND);

                                    item.CreateNew();

                                    //store the information for possible use later
                                    _DockablePlugins.Add(item.WindowInternalName, item);
                                    MyExtensionAddinGlobal.DockableList.Add(item.Window);

                                    //item.Window = new IVDockableWindow.Window();
                                    //AddDockables(item);
                                    log.Info("Added button: " + item.WindowTitle + " to available Dockable Windows.");
                                }
                            }
                        }
                        MyIVPluginLoader<IVUIPlugin> UIPluginLoader = new MyIVPluginLoader<IVUIPlugin>(pluginsPath);
                        if (UIPluginLoader != null)
                        {
                            _UIPlugins = new Dictionary<string, IVUIPlugin>();
                            IEnumerable<IVUIPlugin> uiPlugins = UIPluginLoader.Plugins;

                            foreach (IVUIPlugin item in uiPlugins)
                            {
                                item.LogFileHelper = logHelper;
                                item.InventorApp = MyExtensionAddinGlobal.InventorApp;
                                item.GenericAppEventHandler = MyExtensionAddinGlobal.InventorApp.ApplicationEvents;
                                item.GenericUserInteractionEventHandler = MyExtensionAddinGlobal.InventorApp.UserInterfaceManager.UserInterfaceEvents;
                                item.GenericUserInputEventHandler = MyExtensionAddinGlobal.InventorApp.CommandManager.UserInputEvents;
                                item.GenericFileAccessEventHandler = MyExtensionAddinGlobal.InventorApp.FileAccessEvents;
                                _UIPlugins.Add(item.CommandName, item);
                                log.Info("Added UIPlugin: " + item.CommandName + " to handle some fancy UI things.");
                            }
                        }
                    }


                }
            }
            catch (Exception e)
            {
                //turned off message display
                //MessageBox.Show(e.ToString());
                updatestatusbar("You might need to check the log file(s)!");
                log.Error(e.Message, e);
                //log.Error(e.ToString());
            }
            finally
            {
                log.Debug("Successfully activated!");
            }

            // TODO: Add more initialization code below.

        }

        public void Deactivate()
        {
            // This method is called by Inventor when the AddIn is unloaded.
            // The AddIn will be unloaded either manually by the user or
            // when the Inventor session is terminated

            // TODO: Add ApplicationAddInServer.Deactivate implementation
            foreach (InventorButton button in MyExtensionAddinGlobal.ButtonList)
            {
                if (button.ButtonDef != null) Marshal.FinalReleaseComObject(button.ButtonDef);
            }
            MyExtensionAddinGlobal.ButtonList = null;

            foreach (DockableWindow window in MyExtensionAddinGlobal.DockableList)
            {
                if (window?.Title != string.Empty) Marshal.FinalReleaseComObject(window);
            }
            MyExtensionAddinGlobal.DockableList = null;

            if (MyExtensionAddinGlobal.RibbonPanel != null) Marshal.FinalReleaseComObject(MyExtensionAddinGlobal.RibbonPanel);
            if (!InventorAppHasQuit)
            {
                if (MyExtensionAddinGlobal.InventorApp != null) Marshal.FinalReleaseComObject(MyExtensionAddinGlobal.InventorApp);
            }

            // Release objects.
            m_inventorApplication = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        /// Overrides the AddButtonToRibbon whilst allowing us to specify a panel name
        /// </summary>
        /// <param name="button1"></param>
        /// <param name="uiMan"></param>
        /// <param name="RibbonName"></param>
        /// <param name="PanelName"></param>
        private static void AddButtonToRibbon(InventorButton button1, UserInterfaceManager uiMan, string RibbonName, string PanelName)
        {
            Inventor.Ribbon ribbon = uiMan.Ribbons[RibbonName];
            RibbonTab tab = null;
            try
            {
                tab = ribbon.RibbonTabs["id_MyExtensionsServer"]; //Change it if necessary.
            }
            catch
            {
                //should never hit this but it's here just in case.
                tab = ribbon.RibbonTabs.Add("My Extensions", "id_MyExtensionsServer", Guid.NewGuid().ToString());
            }
            MyExtensionAddinGlobal.RibbonPanelId = Guid.NewGuid().ToString();
            MyExtensionAddinGlobal.RibbonPanel = tab.RibbonPanels.Add(PanelName, "MyExtensionsServer." + PanelName + Guid.NewGuid().ToString(),
                MyExtensionAddinGlobal.RibbonPanelId, String.Empty, true);

            CommandControls cmdCtrls = MyExtensionAddinGlobal.RibbonPanel.CommandControls;
            cmdCtrls.AddButton(button1.ButtonDef, button1.DisplayBigIcon, button1.DisplayText, "", button1.InsertBeforeTarget);
        }

        /// <summary>
        /// Lets us add a button to the Ribbon.
        /// </summary>
        /// <param name="button1"></param>
        /// <param name="uiMan"></param>
        /// <param name="RibbonName"></param>
        private static void AddButtonToRibbon(InventorButton button1, UserInterfaceManager uiMan, string RibbonName)
        {
            Inventor.Ribbon ribbon = uiMan.Ribbons[RibbonName];
            RibbonTab tab = null;
            try
            {
                tab = ribbon.RibbonTabs["id_MyExtensionsServer"]; //Change it if necessary.
            }
            catch
            {
                tab = ribbon.RibbonTabs.Add("My Extensions", "id_MyExtensionsServer", Guid.NewGuid().ToString());
            }
            MyExtensionAddinGlobal.RibbonPanelId = "{96148ae0-4f0f-406d-9fdb-27fa565ae058}";
            MyExtensionAddinGlobal.RibbonPanel = tab.RibbonPanels.Add("My Extensions", "MyExtensionsServer.RibbonPanel_" + Guid.NewGuid().ToString(),
                MyExtensionAddinGlobal.RibbonPanelId, String.Empty, true);

            CommandControls cmdCtrls = MyExtensionAddinGlobal.RibbonPanel.CommandControls;
            cmdCtrls.AddButton(button1.ButtonDef, button1.DisplayBigIcon, button1.DisplayText, "", button1.InsertBeforeTarget);
        }

        private static Icon GetICOResource(string icoResourceName, System.Reflection.Assembly assembly)
        {
            System.IO.Stream stream = assembly.GetManifestResourceStream(icoResourceName);
            System.Drawing.Icon ico = new System.Drawing.Icon(stream);
            return ico;
            //return PictureDispConverter.ToIPictureDisp(ico);
        }

        /// <summary>
        /// Allows us to set a group of ribbons to apply our plugins to.
        /// </summary>
        /// <param name="groupName">One of several allowed inputs: All, Main, Part, Assembly, Drawing, Presentation, ZeroDoc, Modelling</param>
        /// <returns></returns>
        private static List<string> GetRibbonsForPlugin(string groupName)
        {
            List<string> ribbons = new List<string>();
            switch (groupName)
            {
                case "All":
                    ribbons.Add("ZeroDoc");
                    ribbons.Add("Part");
                    ribbons.Add("Assembly");
                    ribbons.Add("Drawing");
                    ribbons.Add("Presentation");
                    break;
                case "Main":
                    ribbons.Add("ZeroDoc");
                    ribbons.Add("Part");
                    ribbons.Add("Assembly");
                    ribbons.Add("Drawing");
                    break;
                case "Part":
                    ribbons.Add("Part");
                    break;
                case "Assembly":
                    ribbons.Add("Assembly");
                    break;
                case "Drawing":
                    ribbons.Add("Drawing");
                    break;
                case "Presentation":
                    ribbons.Add("Presentation");
                    break;
                case "ZeroDoc":
                    ribbons.Add("ZeroDoc");
                    break;
                case "Modelling":
                    ribbons.Add("Part");
                    ribbons.Add("Assembly");
                    break;
                default:
                    break;
            }

            return ribbons;
        }

        /// <summary>
        /// The non-plugin version of the same name.
        /// </summary>
        /// <param name="button1"></param>
        /// <param name="uiMan"></param>
        /// <param name="ribbons"></param>
        private void AddButtonToRibbon(InventorButton button1, UserInterfaceManager uiMan, List<string> RibbonNames)
        {
            foreach (string item in RibbonNames)
            {
                AddButtonToRibbon(button1, uiMan, item);
            }
        }

        private void AddDockables(IVDockableWindow IVDock)
        {
            try
            {
                //need to decide if this is going to just *attempt* to display a web page or a windows form/WPF dialogue.
                // myDockForm dock = new myDockForm();
                // myDockWPF dock = new myDockWPF();
                // thinking I need to push this down into the dockable addins themselves...
                DockableWindows DWin = MyExtensionAddinGlobal.InventorApp.UserInterfaceManager.DockableWindows;
                DockableWindow newDock = DWin.Add(m_addInClassIDString, IVDock.WindowInternalName, IVDock.WindowTitle);
                newDock.Title = IVDock.WindowTitle;
                newDock.Visible = IVDock.Visible;
                newDock.ShowTitleBar = IVDock.ShowTitleBar;

                //this works but the browser disappears - my guess is you can't add a webbrowser to Inventor unless it's part of a 
                // modeless form or something?
                //System.Windows.Window ChildWindow = IVDock.ChildWindow;

                //newDock.AddChild(IVDock.windowHwnd.EnsureHandle());
                //newDock.DockingState = IVDock.DockingState;
                //newDock.DisabledDockingStates = IVDock.DisableDockingStates;
                //this next line adds whatever we want to the dockable windows list, currently it doesn't like aWebBrowser.
                MyExtensionAddinGlobal.DockableList.Add(newDock);
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }
        }

        private WebBrowserDialog aWebBrowser()
        {
            WebBrowserDialog newBrowser = MyExtensionAddinGlobal.InventorApp.WebBrowserDialogs.Add("MyBrowser");
            newBrowser.WindowState = WindowsSizeEnum.kNormalWindow;
            newBrowser.Navigate("https://google.com");
            return newBrowser;
        }

        private string GetAppSetting(Configuration config, string key)
        {
            KeyValueConfigurationElement element = config.AppSettings.Settings[key];
            if (element != null)
            {
                string value = element.Value;
                if (!string.IsNullOrEmpty(value))
                    return value;
            }
            return string.Empty;
        }

        private Configuration loadPluginConfig(string path)
        {
            Configuration config = null;
            config = ConfigurationManager.OpenExeConfiguration(path);
            return config;
        }

        private void M_AppEventsAppQuit(EventTimingEnum BeforeOrAfter, NameValueMap Context, out HandlingCodeEnum HandlingCode)
        {
            if (BeforeOrAfter == EventTimingEnum.kBefore)
            {
                InventorAppHasQuit = true;
            }
            HandlingCode = HandlingCodeEnum.kEventNotHandled;
            return;
        }

        private void M_AppEventsDocumentActivated(_Document DocumentObject, EventTimingEnum BeforeOrAfter, NameValueMap Context, out HandlingCodeEnum HandlingCode)
        {
            if (DocumentObject is PartDocument)
            {
                MyExtensionAddinGlobal.currentPartDoc = (PartDocument)DocumentObject;
                MyExtensionAddinGlobal.currentCompDef = MyExtensionAddinGlobal.currentPartDoc.ComponentDefinition;
            }
            else if (DocumentObject is AssemblyDocument)
            {
                MyExtensionAddinGlobal.currentAssyDoc = (AssemblyDocument)DocumentObject;
                MyExtensionAddinGlobal.currentCompDef = MyExtensionAddinGlobal.currentAssyDoc.ComponentDefinition;
            }
            else if (DocumentObject is DrawingDocument)
            {
                MyExtensionAddinGlobal.currentDwgDoc = (DrawingDocument)DocumentObject;
            }
            else if (DocumentObject is PresentationDocument)
            {
                MyExtensionAddinGlobal.currentPresDoc = (PresentationDocument)DocumentObject;
            }
            HandlingCode = HandlingCodeEnum.kEventNotHandled;
            //HandlingCode = UpdateiProperties(BeforeOrAfter);
        }

        private void updatestatusbar(string Message)
        {
            MyExtensionAddinGlobal.InventorApp.StatusBarText = Message;
        }

        private void updatestatusbar(double percent, string Message = "")
        {
            MyExtensionAddinGlobal.InventorApp.StatusBarText = Message + " (" + percent.ToString("P1") + ")";
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
