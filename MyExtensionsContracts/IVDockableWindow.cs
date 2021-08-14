using Inventor;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Interop;

namespace MyExtensionsContracts
{
    public interface IVDockableWindow : IVPlugin
    {
        string WindowName { get; }
        string WindowInternalName { get; }
        string WindowTitle { get; }
        bool Visible { get; }
        bool ShowTitleBar { get; }
        //DockingStateEnum DockingState { get; } //the default
        //DockingStateEnum DisableDockingStates { get; }
        Window ChildWindow { get; set; }
        WindowInteropHelper windowHwnd { get; }
        DockableWindow Window { get; set; }
        Form FormToAdd { get; set; }
        ElementHost ElemHost { get; set; }

        void CreateNew();
        bool CanUpdate();
        void Update();
    }
}
