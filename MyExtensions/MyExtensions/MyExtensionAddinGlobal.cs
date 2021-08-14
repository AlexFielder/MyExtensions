using Inventor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MyExtensions
{
    class MyExtensionAddinGlobal
    {
        public static Application InventorApp;

        public static bool UpdateAfterEachParameterChange = false;
        public static string RibbonPanelId;
        public static RibbonPanel RibbonPanel;
        public static List<InventorButton> ButtonList = new List<InventorButton>();
        //next line is uneccessary as Context commands are in fact buttons.
        //public static List<IVContextPlugin> ContextList = new List<IVContextPlugin>();
        public static List<DockableWindow> DockableList = new List<DockableWindow>();
        public static List<IVPlugin> PluginList = new List<IVPlugin>();
        public static PartDocument m_currentPartDoc = null;
        public static PartDocument currentPartDoc
        {
            get
            {
                return m_currentPartDoc;
            }
            set
            {
                m_currentPartDoc = value;
            }
        }
        public static AssemblyDocument m_currentAssyDoc = null;
        public static AssemblyDocument currentAssyDoc
        {
            get
            {
                return m_currentAssyDoc;
            }
            set
            {
                m_currentAssyDoc = value;
            }
        }
        public static DrawingDocument m_currentDwgDoc = null;
        public static DrawingDocument currentDwgDoc
        {
            get
            {
                return m_currentDwgDoc;
            }
            set
            {
                m_currentDwgDoc = value;
            }
        }
        public static PresentationDocument m_currentPresDoc = null;
        public static PresentationDocument currentPresDoc
        {
            get
            {
                return m_currentPresDoc;
            }
            set
            {
                m_currentPresDoc = value;
            }
        }
        private static dynamic m_compdef = null;
        public static dynamic currentCompDef
        {
            get
            {
                return m_compdef;
            }

            set
            {
                m_compdef = value;
            }

        }

        private static string mClassId;

        public static string ClassId
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(mClassId))
                    return MyExtensionAddinGlobal.mClassId;
                else
                    throw new Exception("The addin server id hasn't been collected yet!");
            }
            set { MyExtensionAddinGlobal.mClassId = value; }
        }

        public static void GetAddinClassId(Type t)
        {
            GuidAttribute guidAtt = (GuidAttribute)GuidAttribute.GetCustomAttribute(t, typeof(GuidAttribute));
            mClassId = "{" + guidAtt.Value + "}";
        }
    }
}
