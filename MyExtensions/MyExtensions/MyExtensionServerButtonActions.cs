#region Namespaces
using System.Windows;
using Inventor;
using System;
using System.Windows.Forms;

#endregion

namespace MyExtensions
{
    public static class MyExtensionServerButtonActions
    {
        public static void Button1_Execute()
        {
            //TODO: add code below for the button click callback.
            //MessageBox.Show("Hello!");
            //ConfigEditor configEditor = new ConfigEditor();
            //configEditor.Show();
            ConfigEditorMk2 configEditor = new ConfigEditorMk2();
            configEditor.ShowDialog(new WindowWrapper(new IntPtr(MyExtensionAddinGlobal.InventorApp.MainFrameHWND)));
            //configEditor.Show();
        }
    }
}
