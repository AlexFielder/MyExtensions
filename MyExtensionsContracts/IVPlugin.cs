using Inventor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExtensionsContracts
{
    public interface IVPlugin
    {
        MyLog4NetFileHelper LogFileHelper { get; set; }
        string CommandName { get; }
        string Description { get; }
        string DefaultResourceName { get; }
        EnvironmentRibbonName EnvironmentRibbonName { get; }
        Application InventorApp { get; set; }

        void Execute();
        bool InsertBeforeTarget { get; }
        string Path { get; }
        string TargetControlName { get; }
        string ToolTip { get; }
        string PluginSettingsPrefix { get; }
        string PluginSettingsPrefixVar { get; set; }
        string ParentSettingsFilePath { get; set; }
    }
}
