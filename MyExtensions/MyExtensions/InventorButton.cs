using System;
using System.Windows.Forms;
using System.Drawing;

using Inventor;
using MyExtensionsContracts;
using System.Reflection;
using log4net;


namespace MyExtensions
{
    public class InventorButton
    {
        #region Fields & Properties
        private static readonly ILog log = LogManager.GetLogger(typeof(MyExtensionsServer));
        private ButtonDefinition mButtonDef;
        public ButtonDefinition ButtonDef
        {
            get { return mButtonDef; }
            set { mButtonDef = value; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Signature allows creation of button with ProgressiveToolTip
        /// </summary>
        /// <param name="displayName"></param>
        /// <param name="internalName"></param>
        /// <param name="description"></param>
        /// <param name="tooltip"></param>
        /// <param name="standardIcon"></param>
        /// <param name="largeIcon"></param>
        /// <param name="ProgressiveToolTipImagePath"></param>
        /// <param name="commandType"></param>
        /// <param name="buttonDisplayType"></param>
        public InventorButton(string pluginPath, string displayName, string internalName, string description, string tooltip,
                                Icon standardIcon, Icon largeIcon, string ProgressiveToolTipImagePath,
                                CommandTypesEnum commandType, ButtonDisplayEnum buttonDisplayType)
        {
            InternalName = internalName;
            Create(pluginPath, displayName, internalName, description, tooltip, MyExtensionAddinGlobal.ClassId,
                standardIcon, largeIcon, ProgressiveToolTipImagePath, commandType, buttonDisplayType);
        }



        /// <summary>
        /// The second most comprehensive signature.
        /// </summary>
        public InventorButton(string displayName, string internalName, string description, string tooltip,
                                Icon standardIcon, Icon largeIcon,
                                CommandTypesEnum commandType, ButtonDisplayEnum buttonDisplayType)
        {
            InternalName = internalName;
            Create(displayName, internalName, description, tooltip, MyExtensionAddinGlobal.ClassId,
                standardIcon, largeIcon, commandType, buttonDisplayType);
        }

        /// <summary>
        /// The signature does not care about Command Type (always editing) and Button Display (always with text).
        /// </summary>
        public InventorButton(string displayName, string internalName, string description, string tooltip,
                                Icon standardIcon, Icon largeIcon)
        {
            Create(displayName, internalName, description, tooltip, MyExtensionAddinGlobal.ClassId,
                null, null, CommandTypesEnum.kEditMaskCmdType, ButtonDisplayEnum.kAlwaysDisplayText);
        }

        /// <summary>
        /// The signature does not care about icons.
        /// </summary>
        public InventorButton(string displayName, string internalName, string description, string tooltip,
                                CommandTypesEnum commandType, ButtonDisplayEnum buttonDisplayType)
        {
            Create(displayName, internalName, description, tooltip, MyExtensionAddinGlobal.ClassId,
                null, null, commandType, buttonDisplayType);
        }

        /// <summary>
        /// This signature only cares about display name and icons.
        /// </summary>
        /// <param name="displayName"></param>
        /// <param name="standardIcon"></param>
        /// <param name="largeIcon"></param>
        public InventorButton(string displayName, Icon standardIcon, Icon largeIcon)
        {
            Create(displayName, displayName, displayName, displayName, MyExtensionAddinGlobal.ClassId,
                standardIcon, largeIcon, CommandTypesEnum.kEditMaskCmdType, ButtonDisplayEnum.kAlwaysDisplayText);
        }

        /// <summary>
        /// The simplest signature, which can be good for prototyping.
        /// </summary>
        public InventorButton(string displayName)
        {
            Create(displayName, displayName, displayName, displayName, MyExtensionAddinGlobal.ClassId,
                    null, null, CommandTypesEnum.kEditMaskCmdType, ButtonDisplayEnum.kAlwaysDisplayText);
        }

        /// <summary>
        /// The helper method for constructors to call to avoid duplicate code.
        /// </summary>
        public void Create(
            string displayName, string internalName, string description, string tooltip,
            string clientId,
            Icon standardIcon, Icon largeIcon,
            CommandTypesEnum commandType, ButtonDisplayEnum buttonDisplayType)
        {
            if (string.IsNullOrEmpty(clientId))
                clientId = MyExtensionAddinGlobal.ClassId;

            stdole.IPictureDisp standardIconIPictureDisp = null;
            stdole.IPictureDisp largeIconIPictureDisp = null;
            if (standardIcon != null)
            {
                standardIconIPictureDisp = IconToPicture(standardIcon);
                largeIconIPictureDisp = IconToPicture(largeIcon);
            }

            mButtonDef = MyExtensionAddinGlobal.InventorApp.CommandManager.ControlDefinitions.AddButtonDefinition(
                displayName, internalName, commandType,
                clientId, description, tooltip,
                standardIconIPictureDisp, largeIconIPictureDisp, buttonDisplayType);

            mButtonDef.Enabled = true;
            mButtonDef.OnExecute += ButtonDefinition_OnExecute;

            DisplayText = true;

            MyExtensionAddinGlobal.ButtonList.Add(this);
        }

        /// <summary>
        /// Allows us to create a buttondefinition that includes a progressive tool tip.
        /// concept copied from here: http://adndevblog.typepad.com/manufacturing/2013/12/progressive-tooltip-with-button.html
        /// </summary>
        /// <param name="pluginPath"></param>
        /// <param name="displayName"></param>
        /// <param name="internalName"></param>
        /// <param name="description"></param>
        /// <param name="tooltip"></param>
        /// <param name="classId"></param>
        /// <param name="standardIcon"></param>
        /// <param name="largeIcon"></param>
        /// <param name="progressiveToolTipImagePath"></param>
        /// <param name="commandType"></param>
        /// <param name="buttonDisplayType"></param>
        private void Create(
            string pluginPath, string displayName, string internalName, string description, string tooltip,
            string classId, Icon standardIcon, Icon largeIcon, string progressiveToolTipImagePath,
            CommandTypesEnum commandType, ButtonDisplayEnum buttonDisplayType)
        {
            Create(displayName, internalName, description, tooltip, MyExtensionAddinGlobal.ClassId,
                standardIcon, largeIcon, commandType, buttonDisplayType);
            IPictureDisp progImg = (IPictureDisp)GetIPictureDispResource(progressiveToolTipImagePath, Assembly.LoadFrom(pluginPath));
            mButtonDef.ProgressiveToolTip.Image = progImg;
            //mButtonDef.ProgressiveToolTip.Image = progressiveToolTipImagePath;
        }

        #endregion

        #region Behavior 

        public bool DisplayBigIcon { get; set; }
        public bool DisplayText { get; set; }
        public bool InsertBeforeTarget { get; set; }
        public string TargetControlName { get; set; }
        public string InternalName { get; set; }

        public void SetBehavior(bool displayBigIcon, bool displayText, bool insertBeforeTarget)
        {
            DisplayBigIcon = displayBigIcon;
            DisplayText = displayText;
            InsertBeforeTarget = insertBeforeTarget;
        }

        public void CopyBehaviorFrom(InventorButton button)
        {
            DisplayBigIcon = button.DisplayBigIcon;
            DisplayText = button.DisplayText;
            InsertBeforeTarget = this.InsertBeforeTarget;
        }

        #endregion

        #region Actions

        /// <summary>
        /// The button callback method.
        /// </summary>
        /// <param name="context"></param>
        private void ButtonDefinition_OnExecute(NameValueMap context)
        {
            if (Execute != null)
                Execute();
            else
                MessageBox.Show("Nothing to execute.");
        }

        /// <summary>
        /// The button action to be assigned from anywhere outside.
        /// </summary>
        public Action Execute;

        #endregion

        #region Image Converters

        private static stdole.IPictureDisp GetIPictureDispResource(string pictureResourceName, System.Reflection.Assembly assembly)
        {
            try
            {
                System.IO.Stream stream = assembly.GetManifestResourceStream(pictureResourceName);
                Bitmap image = new Bitmap(stream);
                //Image image = new Image(stream);
                //System.Drawing.Icon ico = new System.Drawing.Icon(stream);
                return ImageConverter.ImageToPicture(image);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                return null;
            }
            //return PictureDispConverter.ToIPictureDisp(ico);
        }

        public static stdole.IPictureDisp ImageToPicture(Image image)
        {
            return ImageConverter.ImageToPicture(image);
        }

        public static stdole.IPictureDisp IconToPicture(Icon icon)
        {
            return ImageConverter.ImageToPicture(icon.ToBitmap());
        }

        public static Image PictureToImage(stdole.IPictureDisp picture)
        {
            return ImageConverter.PictureToImage(picture);
        }

        public static Icon PictureToIcon(stdole.IPictureDisp picture)
        {
            return ImageConverter.PictureToIcon(picture);
        }

        private class ImageConverter : AxHost
        {
            public ImageConverter() : base(String.Empty) { }

            public static stdole.IPictureDisp ImageToPicture(Image image)
            {
                return (stdole.IPictureDisp)GetIPictureDispFromPicture(image);
            }

            public static stdole.IPictureDisp IconToPicture(Icon icon)
            {
                return ImageToPicture(icon.ToBitmap());
            }

            public static Image PictureToImage(stdole.IPictureDisp picture)
            {
                return GetPictureFromIPicture(picture);
            }

            public static Icon PictureToIcon(stdole.IPictureDisp picture)
            {
                Bitmap bitmap = new Bitmap(PictureToImage(picture));
                return System.Drawing.Icon.FromHandle(bitmap.GetHicon());
            }
        }

        #endregion
    }
}
