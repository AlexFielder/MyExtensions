using Inventor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExtensionsContracts
{
    public interface IVButtonPlugin : IVPlugin
    {
        //button specific
        bool DisplayBigIcon { get; }
        bool DisplayText { get; }
        string ButtonInternalName { get; }
        string ButtonDisplayName { get; }
        string RibbonPanelName { get; }
        bool? CanHaveProgressiveToolTip { get; }
        bool IncludesProgressiveToolTipHelp { get; }
        string ProgressiveToolTipDescription { get; set; }
        string ProgressiveToolTipExpandedDescription { get; set; }
        string ProgressiveToolTipTitle { get; set; }
        string ProgressiveToolTipImagePath { get; set; }
        ButtonDefinition ThisButtonDefinition { get; set; }
        void ButtonDefinition_OnHelp(NameValueMap Context, out HandlingCodeEnum HandlingCode);
        //void StartCommand();
        //void ExecuteCommand();
        //void StopCommand();
        //void UpdateCommandStatus();
        //void EnableInteraction();
        //void DisableInteraction();
        //void OnPreSelect(object preSelectEntity, out bool doHighlight, ObjectCollection morePreSelectEntities, SelectionDeviceEnum selectionDevice, Point modelPosition, Point2d viewPosition, View view);
        //void OnSelect(ObjectsEnumerator justSelectedEntities, SelectionDeviceEnum selectionDevice, Point modelPosition, Point2d viewPosition, View view);


    }
}
