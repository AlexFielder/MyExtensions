using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyExtensions;
using Inventor;
using log4net;

namespace DRYHelpers
{
    /// <summary>
    /// All of these functions will work with the currently active document inside of Inventor.
    /// </summary>
    public static class Parameters
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Parameters));
        //private static dynamic m_ActiveDoc = null;
        //public static dynamic ThisDoc
        //{
        //    get
        //    {
        //        return AddinGlobal.InventorApp.ActiveDocument;
        //    }

        //    set
        //    {
        //        m_ActiveDoc = value;
        //    }
        //}
        //public static PartDocument PartDoc = null;
        //public static AssemblyDocument AssyDoc = null;
        //public static PresentationDocument PresentationDoc = null;
        //public static DrawingDocument DrawingDoc = null;

        //public static void Init()
        //{
        //    if (AddinGlobal.InventorApp.ActiveDocument.DocumentType == DocumentTypeEnum.kPartDocumentObject)
        //    {
        //        PartDoc = ThisDoc;
        //    }
        //    else if (AddinGlobal.InventorApp.ActiveDocument.DocumentType == DocumentTypeEnum.kAssemblyDocumentObject)
        //    {
        //        AssyDoc = ThisDoc;
        //    }
        //    else if (AddinGlobal.InventorApp.ActiveDocument.DocumentType == DocumentTypeEnum.kDrawingDocumentObject)
        //    {
        //        throw new NotImplementedException();
        //    }
        //    else if (AddinGlobal.InventorApp.ActiveDocument.DocumentType == DocumentTypeEnum.kPresentationDocumentObject)
        //    {
        //        throw new NotImplementedException();
        //    }
        //    else
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        #region "Parameters"

        /// <summary>
        /// Sets a string parameter value
        /// </summary>
        /// <param name="ParameterName">The name of the Parameter to Set or Create</param>
        /// <param name="ParameterValue"></param>
        public static void SetParameter(string ParameterName, string ParameterValue, UnitsTypeEnum currentUnits, dynamic DocToAffect = null)
        {
            //dynamic ThisDoc = DocToAffect;
            // Get the Parameters object. Assumes a part or assembly document is active.
            Inventor.Parameters oParameters = null;

            if (DocToAffect == null)
            {
                //oParameters = ThisDoc.ComponentDefinition.Parameters;
                oParameters = MyExtensionAddinGlobal.currentCompDef.Parameters;
            }
            else
            {
                dynamic doc = null;
                try
                {
                    doc = (PartDocument)DocToAffect;
                }
                catch (InvalidCastException ex)
                {
                    doc = (AssemblyDocument)DocToAffect;
                    log.Info(ex.Message);
                }
                oParameters = doc.ComponentDefinition.Parameters;
            }
            try
            {
                // Get the parameter named "Length".
                Inventor.Parameter oLengthParam = oParameters[ParameterName];

                // Change the equation of the parameter.
                oLengthParam.Expression = ParameterValue;
            }
            catch
            {
                // the parameter probably doesn't exist
                if (ParameterName.Contains(".")) { throw new ParameterNotStoredException(); }
                oParameters.UserParameters.AddByValue(ParameterName, ParameterValue, UnitsTypeEnum.kTextUnits);
            }
            finally
            {
                // Update the document.
                if (MyExtensionAddinGlobal.UpdateAfterEachParameterChange)
                {
                    MyExtensionAddinGlobal.InventorApp.ActiveDocument.Update();
                }
            }
        }
        /// <summary>
        /// UNTESTED 2016-05-24 AF
        /// Sets a number parameter value
        /// </summary>
        /// <param name="ParameterName">The name of the Parameter to Set or Create</param>
        /// <param name="ParameterValue"></param>
        public static void SetParameter(string ParameterName, double ParameterValue, UnitsTypeEnum currentUnits)
        {
            // Get the Parameters object. Assumes a part or assembly document is active.
            Inventor.Parameters oParameters = MyExtensionAddinGlobal.currentCompDef.Parameters;
            Inventor.UserParameter oParam;
            try
            {
                oParam = (UserParameter)oParameters[ParameterName];

                // Change the equation of the parameter.
                oParam.Expression = ParameterValue.ToString();
            }
            catch
            {
                // the parameter probably doesn't exist
                oParam = oParameters.UserParameters.AddByValue(ParameterName, ParameterValue, currentUnits);
            }
            finally
            {
                // Update the document.
                if (MyExtensionAddinGlobal.UpdateAfterEachParameterChange)
                {
                    MyExtensionAddinGlobal.InventorApp.ActiveDocument.Update();
                }
            }
        }
        /// <summary>
        /// UNTESTED 2016-05-24 AF
        /// Sets a true/false parameter value
        /// </summary>
        /// <param name="ParameterName">The name of the Parameter to Set or Create</param>
        /// <param name="ParameterValue"></param>
        public static void SetParameter(string ParameterName, bool ParameterValue)
        {
            // Get the Parameters object. Assumes a part or assembly document is active.
            Inventor.Parameters oParameters = MyExtensionAddinGlobal.currentCompDef.Parameters;
            try
            {
                // Get the parameter named "Length".
                Parameter oLengthParam = oParameters[ParameterName];

                // Change the equation of the parameter.
                oLengthParam.Expression = ParameterValue.ToString();
            }
            catch
            {
                // the parameter probably doesn't exist
                oParameters.UserParameters.AddByValue(ParameterName, ParameterValue, UnitsTypeEnum.kBooleanUnits);
            }
            finally
            {
                // Update the document.
                if (MyExtensionAddinGlobal.UpdateAfterEachParameterChange)
                {
                    MyExtensionAddinGlobal.InventorApp.ActiveDocument.Update();
                }
            }

        }
        /// <summary>
        /// UNTESTED 2016-05-24 AF
        /// Sets a Date Parameter Value
        /// </summary>
        /// <param name="ParameterName">The name of the Parameter to Set or Create</param>
        /// <param name="ParameterValue"></param>
        public static void SetParameter(string ParameterName, DateTime ParameterValue)
        {
            // Get the Parameters object. Assumes a part or assembly document is active.
            Inventor.Parameters oParameters = MyExtensionAddinGlobal.currentCompDef.Parameters;

            // Get the parameter named "Length".
            Inventor.Parameter oLengthParam = oParameters[ParameterName];

            // Change the equation of the parameter.
            //this needs some kind of formatting I guess?
            oLengthParam.Expression = ParameterValue.ToString();

            // Update the document.
            if (MyExtensionAddinGlobal.UpdateAfterEachParameterChange)
            {
                MyExtensionAddinGlobal.InventorApp.ActiveDocument.Update();
            }
        }

        public static Parameter GetParameter(string ParamName)
        {
            Inventor.Parameters oParameters = MyExtensionAddinGlobal.currentCompDef.Parameters;
            if (oParameters[ParamName].ParameterType == ParameterTypeEnum.kUserParameter)
            {
                return (Parameter)GetUserParameter(ParamName);
            }
            else if (oParameters[ParamName].ParameterType == ParameterTypeEnum.kReferenceParameter)
            {
                return (Parameter)GetReferenceParameter(ParamName);
            }
            else if (oParameters[ParamName].ParameterType == ParameterTypeEnum.kModelParameter)
            {
                return (Parameter)GetModelParameter(ParamName);
            }
            else if (oParameters[ParamName].ParameterType == ParameterTypeEnum.kDerivedParameter)
            {
                return (Parameter)GetDerivedParameter(ParamName);
            }
            else if (oParameters[ParamName].ParameterType == ParameterTypeEnum.kTableParameter)
            {
                throw new NotSupportedException();
            }
            return null;
        }

        /// <summary>
        /// Gets the object of a parameter by name
        /// </summary>
        /// <param name="ParameterName"></param>
        /// <returns></returns>
        public static UserParameter GetUserParameter(string ParameterName)
        {
            Inventor.Parameters oParameters = MyExtensionAddinGlobal.currentCompDef.Parameters;
            return (UserParameter)oParameters[ParameterName];
        }

        /// <summary>
        /// Gets the object of a reference parameter by name
        /// </summary>
        /// <param name="ParameterName"></param>
        /// <returns></returns>
        public static ReferenceParameter GetReferenceParameter(string ParameterName)
        {
            Inventor.Parameters oParameters = MyExtensionAddinGlobal.currentCompDef.Parameters;
            return (ReferenceParameter)oParameters[ParameterName];
        }

        /// <summary>
        /// Gets the object of a model parameter by name
        /// </summary>
        /// <param name="ParameterName"></param>
        /// <returns></returns>
        public static ModelParameter GetModelParameter(string ParameterName)
        {
            Inventor.Parameters oParameters = MyExtensionAddinGlobal.currentCompDef.Parameters;
            return (ModelParameter)oParameters[ParameterName];
        }

        /// <summary>
        /// Gets the object of a derived parameter by name
        /// </summary>
        /// <param name="ParameterName"></param>
        /// <returns></returns>
        public static DerivedParameter GetDerivedParameter(string ParameterName)
        {
            Inventor.Parameters oParameters = MyExtensionAddinGlobal.currentCompDef.Parameters;
            return (DerivedParameter)oParameters[ParameterName];
        }


        #endregion

    }
}
