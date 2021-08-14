using Inventor;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Environment;
using Microsoft.Office.Interop.Excel;
using System.Xml;
using MyExtensions;
using System.Configuration;
using MyExtensionsContracts;

namespace DRYHelpers
{
    public static class ApplicationHelpers
    {
        public static readonly ILog log = LogManager.GetLogger(typeof(ApplicationHelpers));
        private static string m_strSettingName;
        #region "GetOrCreateInstance"
        // Acquire and release Application objects
        private static object GetInstance(string appName)
        {
            return Marshal.GetActiveObject(appName);
        }
        private static object CreateInstance(string appName)
        {
            return Activator.CreateInstance(Type.GetTypeFromProgID(appName));
        }
        public static object GetOrCreateInstance(string appName, out bool AlreadyRunning)
        {
            try
            {
                AlreadyRunning = true;
                return GetInstance(appName);
            }
            catch
            {
                AlreadyRunning = false; 
                return CreateInstance(appName);
            }
        }
        private static object GetOrCreateInstance(string appName)
        {
            try
            {
                return GetInstance(appName);
            }
            catch
            {
                return CreateInstance(appName);
            }
        }
        #endregion

        /// <summary>
        /// Allows us to easily open an existing file
        /// </summary>
        /// <param name="Filter">The filetype(s) to filter on. Examples could be: "Inventor files (*.ipt; *.iam; *.idw)|*.ipt;*.iam;*.idw"</param>
        /// <param name="title">The file open Dialogue Title.</param>
        /// <param name="InventorApp">The Parent Inventor Application object that every plugin has.</param>
        /// <param name="initialDir">The Initial Directory to search in.</param>
        /// <returns>Returns a string path to the chosen file</returns>
        public static string SelectFile(string Filter, string title, Inventor.Application InventorApp, SpecialFolder initialDir = SpecialFolder.MyComputer)
        {
            InventorApp.CreateFileDialog(out FileDialog fileBrowser);
            fileBrowser.Filter = Filter;
            fileBrowser.InitialDirectory = initialDir.ToString();
            fileBrowser.DialogTitle = title;
            String selectedFile = string.Empty;
            try
            {
                fileBrowser.ShowOpen();
                selectedFile = fileBrowser.FileName;
            }
            catch (Exception)
            {
                return string.Empty;
            }
            return selectedFile;
        }

        /// <summary>
        /// A variation of the original that accepts an initialDir that *isn't* one of the SpecialFolders
        /// </summary>
        /// <param name="Filter">The filetype(s) to filter on.</param>
        /// <param name="title">The file open Dialogue Title.</param>
        /// <param name="InventorApp">The Parent Inventor Application object that every plugin has.</param>
        /// <param name="initialDir">The Initial Directory to search in.</param>
        /// <returns>Returns a string path to the chosen file</returns>
        public static string SelectFile(string Filter, string title, Inventor.Application InventorApp, bool multiSelectEnabled, string initialDir = "")
        {
            InventorApp.CreateFileDialog(out FileDialog fileBrowser);
            fileBrowser.Filter = Filter;
            fileBrowser.InitialDirectory = initialDir.ToString();
            fileBrowser.DialogTitle = title;
            fileBrowser.MultiSelectEnabled = multiSelectEnabled;
            String selectedFile = string.Empty;
            try
            {
                fileBrowser.ShowOpen();
                selectedFile = fileBrowser.FileName;
            }
            catch (Exception)
            {
                return string.Empty;
            }
            return selectedFile;
        }

        public static string SelectFolder(string Description, string initialPath = "", SpecialFolder initialDir = SpecialFolder.MyComputer)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                dialog.Description = Description;
                if(initialPath == string.Empty)
                {
                    dialog.SelectedPath = initialDir.ToString();
                }
                else
                {
                    dialog.SelectedPath = initialPath;
                }
                
                if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    return string.Empty;
                return dialog.SelectedPath + "\\";
            }
        }

        //public static Microsoft.Office.Interop.Excel.Application GetOrCreateInstance(string appName)
        //{
        //    try
        //    {
        //        return GetInstance(appName);
        //    }
        //    catch
        //    {
        //        return CreateInstance(appName);
        //    }
        //}
        //public static void SaveSettings(List<ConfigSettings> configFromExtension = null)
        //{
        //    var xmlDoc = new XmlDocument();
        //    xmlDoc.Load(MyExtensionsServer.MyAppConfig.FilePath);
        //    XmlNode applicationSettingsNode = xmlDoc.GetElementsByTagName("userSettings")[0];
        //    foreach (XmlNode objXmlNode in applicationSettingsNode.FirstChild.ChildNodes)
        //    {
        //        m_strSettingName = objXmlNode.Attributes[0].Value;
        //        ConfigSettings objConfigSettings = null;
        //        if (configFromExtension == null)
        //        {
        //            objConfigSettings = ConfigSettingsList.Find(GetSetting);
        //        }
        //        else
        //        {
        //            objConfigSettings = configFromExtension.Find(GetSetting);
        //        }

        //        if (objConfigSettings != null)
        //        {
        //            objXmlNode.Attributes[0].Value = objConfigSettings.SettingName;
        //            objXmlNode.Attributes[1].Value = objConfigSettings.SerializeAs;
        //            objXmlNode.FirstChild.InnerText = objConfigSettings.SettingValue;
        //        }

        //    }

        //    xmlDoc.Save(MyExtensionsServer.MyAppConfig.FilePath);
        //}

        private static List<ConfigSettings> ConfigSettingsList = new List<ConfigSettings>();

        /// <summary>
        /// Currently this doesn't take account of whether the value is a double or not. Needs testing!
        /// </summary>
        /// <param name="configSettingsList"></param>
        //public static void UpdateSettingsFromChangedList(List<ConfigSettings> configSettingsList)
        //{
        //    var xmlDoc = new XmlDocument();
        //    xmlDoc.Load(MyExtensionAddinGlobal.MyAppConfig.FilePath);
        //    //AppSettingsSection appSettings = pluginConfig.AppSettings;
        //    if (configSettingsList != null && configSettingsList.Count > 0)
        //    {
        //        ApplicationSettingsBase settings = Properties.Settings.Default;

        //        foreach (ConfigSettings item in configSettingsList)
        //        {
        //            if (settings.Properties.Cast<SettingsProperty>().All(s => s.Name != item.SettingName))
        //            {
        //                settings.Add(item.SettingName, item.SettingValue);
        //            };
        //            var settingsNode = xmlDoc.SelectSingleNode("//MyExtensions.Properties.Settings");
        //            var nodeSettings = xmlDoc.SelectNodes("//MyExtensions.Properties.Settings");
        //            if (settingsNode.ChildNodes.Cast<XmlNode>().All(s => s.Attributes["name"].Value == item.SettingName)) //specifically updates the relevant lines (I think)
        //            {
        //                var nodeRegion = xmlDoc.CreateElement("setting");
        //                nodeRegion.SetAttribute("name", item.SettingName);
        //                nodeRegion.SetAttribute("serializeAs", "String");
        //                xmlDoc.SelectSingleNode("//userSettings/MyExtensions.Properties.Settings").AppendChild(nodeRegion);
        //                var valueNode = xmlDoc.CreateElement("value");
        //                valueNode.InnerText = item.SettingValue;
        //                nodeRegion.AppendChild(valueNode);
        //            }
        //        } 
        //    }
        //    xmlDoc.Save(MyExtensionAddinGlobal.MyAppConfig.FilePath);
        //    ConfigurationManager.RefreshSection("//userSettings/MyExtensions.Properties.Settings");
        //    Properties.Settings.Default.Reload();
        //}



        private static bool GetSetting(ConfigSettings objConfigSettings)
        {
            return m_strSettingName == objConfigSettings.SettingName;
        }
    }
}
