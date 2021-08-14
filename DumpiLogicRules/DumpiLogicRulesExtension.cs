using Inventor;
using log4net;
using MyExtensionsContracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DumpiLogicRules
{
    [Export(typeof(IVButtonPlugin))]
    public partial class DumpiLogicRulesExtension : IVButtonPlugin
    {
        private Assembly thisAssembly = System.Reflection.Assembly.GetExecutingAssembly();
        private static MyLog4NetFileHelper logHelper = null;
        public static readonly ILog log = LogManager.GetLogger(typeof(DumpiLogicRulesExtension));
        public static Application m_InventorApp = null;
        internal static DirtyCollection<RuleType> listofiLogicRules;

        public string ButtonDisplayName
        {
            get
            {
                return "Dump iLogic Rules";
            }
        }

        public string ButtonInternalName
        {
            get
            {
                //Assembly a = Assembly.GetCallingAssembly();
                var attribute = (GuidAttribute)thisAssembly.GetCustomAttributes(typeof(GuidAttribute), true)[0];
                string AssemblyGuid = attribute.Value;
                return Properties.Settings.Default.ButtonExtensionInternalName + AssemblyGuid;
            }
        }

        public string CommandName
        {
            get
            {
                return "DumpiLogicRulesCmd";
            }
        }

        public CommandTypesEnum CommandType
        {
            get
            {
                return CommandTypesEnum.kShapeEditCmdType;
            }
        }

        public string DefaultResourceName
        {
            get
            {
                return "DumpiLogicRulesExtension.Resources.ExportRules.ico";
            }
        }

        public string Description
        {
            get
            {
                return "Dumps iLogic Rules contained within all referenced documents to an XML file";
            }
        }

        public bool DisplayBigIcon
        {
            get
            {
                return true;
            }
        }

        public bool DisplayText
        {
            get
            {
                return true;
            }
        }

        public EnvironmentRibbonName EnvironmentRibbonName
        {
            get
            {
                return EnvironmentRibbonName.Main;
            }
        }

        public bool InsertBeforeTarget
        {
            get
            {
                return true;
            }
        }

        public MyLog4NetFileHelper LogFileHelper
        {
            get
            {
                return logHelper;
            }

            set
            {
                logHelper = value;
            }
        }

        public string Path
        {
            get
            {
                return thisAssembly.Location;
            }
        }

        public string RibbonPanelName
        {
            get
            {
                return "iLogic: " + thisAssembly.GetName().Version.ToString();
            }
        }

        public string TargetControlName
        {
            get
            {
                return "Settings";
            }
        }

        public string ToolTip
        {
            get
            {
                return "Dump iLogic Rules";
            }
        }

        public Application InventorApp
        {
            get
            {
                return m_InventorApp;
            }

            set
            {
                m_InventorApp = value;
            }
        }

        public string PluginSettingsPrefix
        {
            get
            {
                return "DumpiLogicRules";
            }
        }
        public static string m_parentSettingsFilePath;
        public string ParentSettingsFilePath
        {
            get
            {
                return m_parentSettingsFilePath;
            }
            set
            {
                m_parentSettingsFilePath = value;
            }
        }
        public static string m_pluginSettingsPrefixVar = "DumpiLogicRules";
        public string PluginSettingsPrefixVar
        {
            get
            {
                return m_pluginSettingsPrefixVar;
            }
            set
            {
                m_pluginSettingsPrefixVar = PluginSettingsPrefix;
            }
        }

        public bool? CanHaveProgressiveToolTip
        {
            get
            {
                return true;
            }
        }
        private string _proTTDescr = "This tool is designed to automate some aspects of dumping out iLogic rules to one file.";
        public string ProgressiveToolTipDescription
        {
            get
            {
                return _proTTDescr;
            }
            set
            {
                _proTTDescr = value;
            }
        }
        private string m_proTTExpDescr = "This tool is designed to automate some aspects of dumping out iLogic rules to one file.";
        public string ProgressiveToolTipExpandedDescription
        {
            get
            {
                return m_proTTExpDescr;
            }
            set
            {
                m_proTTExpDescr = value;
            }
        }
        private string m_proTTTitle = "Dump iLogic Rules Tool";
        public string ProgressiveToolTipTitle
        {
            get
            {
                return m_proTTTitle;
            }
            set
            {
                m_proTTTitle = value;
            }

        }
        private string m_proTTImagePath = "DumpiLogicRulesExtension.Resources.ExportRules.bmp"; //"Resources.RadialHolesTooltip";


        public string ProgressiveToolTipImagePath
        {
            get
            {
                return m_proTTImagePath;
            }
            set
            {
                m_proTTImagePath = value;
            }
        }

        public bool IncludesProgressiveToolTipHelp
        {
            get
            {
                return true;
            }
        }
        private static ButtonDefinition m_buttonDefinition = null;
        public ButtonDefinition ThisButtonDefinition
        {
            get
            {
                return m_buttonDefinition;
            }
            set
            {
                m_buttonDefinition = value;
            }
        }

        //public static Application InventorApp
        //{
        //    get
        //    {
        //        return m_InventorApp;
        //    }

        //    set
        //    {
        //        m_InventorApp = value;
        //    }
        //}

        public void Execute()
        {
            DumpiLogicRulesButtonActions.Button1_Execute();
        }

        public void ButtonDefinition_OnHelp(NameValueMap Context, out HandlingCodeEnum HandlingCode)
        {
            throw new NotImplementedException();
        }
    }
}
