using Inventor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace DRYHelpers
{
    public class iProperties
    {
        public static readonly ILog log = LogManager.GetLogger(typeof(iProperties));

        public static string GetiPropertyDisplayName(Property iProp)
	    {
		    return iProp.DisplayName;
	    }

	    public static ObjectTypeEnum GetiPropertyType(Property iProp)
	    {
		    return iProp.Type;
	    }

	    public static string GetiPropertyTypeString(Property iProp)
	    {
            string valToTest = iProp.Value.ToString();
            if (int.TryParse(valToTest, out int intResult))
            {
                return "Number";
            }

            if (double.TryParse(valToTest, out double doubleResult))
            {
                return "Number";
            }

            if (System.DateTime.TryParse(valToTest, out DateTime dateResult))
            {
                return "Date";
            }

            if (bool.TryParse(valToTest, out bool booleanResult))
            {
                return "Boolean";
            }

            //Dim currencyResult As Currency = Nothing

            //should probabyl do this last as most property values will equate to string!
            string strResult = string.Empty;
		    if (!(iProp.Value.ToString() == string.Empty)) {
			    return "String";
		    }
		    return null;
	    }
	    #region "Set iProperty Values"
	    #region "Get or Set Standard iProperty Values"

	    /// <summary>
	    /// Design Tracking Properties
	    /// </summary>
	    /// <param name="DocToUpdate"></param>
	    /// <param name="iPropertyTypeEnum"></param>
	    /// <param name="newpropertyvalue"></param>
	    /// <returns></returns>
	    public static string GetorSetStandardiProperty(
            Inventor.Document DocToUpdate, PropertiesForDesignTrackingPropertiesEnum iPropertyTypeEnum, 
            string newpropertyvalue = "", string propertyTypeStr = "") 
	    {
		    PropertySet invProjProperties = DocToUpdate.PropertySets["{32853F0F-3444-11D1-9E93-0060B03C1CA6}"];
		    string currentvalue = string.Empty;
		    if (!(newpropertyvalue == string.Empty)) {
			    invProjProperties.ItemByPropId[(int)iPropertyTypeEnum].Value = newpropertyvalue.ToString();
		    } else {
			    currentvalue = invProjProperties.ItemByPropId[(int)iPropertyTypeEnum].Value.ToString();
			    newpropertyvalue = GetiPropertyDisplayName(invProjProperties.ItemByPropId[(int)iPropertyTypeEnum]);
		    }
		    if (propertyTypeStr == string.Empty) {
			    propertyTypeStr = GetiPropertyTypeString(invProjProperties.ItemByPropId[(int)iPropertyTypeEnum]);
		    }
		    return currentvalue;
	    }

	    /// <summary>
	    /// Document Summary Properties
	    /// </summary>
	    /// <param name="DocToUpdate"></param>
	    /// <param name="iPropertyTypeEnum"></param>
	    /// <param name="newpropertyvalue"></param>
	    /// <returns></returns>
	    public static string GetorSetStandardiProperty(
            Document DocToUpdate, PropertiesForDocSummaryInformationEnum iPropertyTypeEnum, 
            string newpropertyvalue = "", string propertyTypeStr = "")
	    {
		    PropertySet invDocSummaryProperties = DocToUpdate.PropertySets["{D5CDD502-2E9C-101B-9397-08002B2CF9AE}"];
		    string currentvalue = string.Empty;
		    if (!(newpropertyvalue == string.Empty)) {
			    invDocSummaryProperties.ItemByPropId[(int)iPropertyTypeEnum].Value = newpropertyvalue.ToString();
		    } else {
                currentvalue = invDocSummaryProperties.ItemByPropId[(int)iPropertyTypeEnum].Value.ToString();
			    newpropertyvalue = GetiPropertyDisplayName(invDocSummaryProperties.ItemByPropId[(int)iPropertyTypeEnum]);
		    }
		    if (propertyTypeStr == string.Empty) {
			    propertyTypeStr = GetiPropertyTypeString(invDocSummaryProperties.ItemByPropId[(int)iPropertyTypeEnum]);
		    }
		    return currentvalue;
	    }

	    /// <summary>
	    /// Summary Properties
	    /// </summary>
	    /// <param name="DocToUpdate"></param>
	    /// <param name="iPropertyTypeEnum"></param>
	    /// <param name="newpropertyvalue"></param>
	    /// <returns></returns>
	    public static string GetorSetStandardiProperty(
            Document DocToUpdate, PropertiesForSummaryInformationEnum iPropertyTypeEnum, 
            string newpropertyvalue = "", string propertyTypeStr = "")
	    {
            PropertySet invSummaryiProperties = DocToUpdate.PropertySets["{F29F85E0-4FF9-1068-AB91-08002B27B3D9}"];
		    string currentvalue = string.Empty;
		    if (!(newpropertyvalue == string.Empty)) {
			    invSummaryiProperties.ItemByPropId[(int)iPropertyTypeEnum].Value = newpropertyvalue.ToString();
		    } else {
                currentvalue = invSummaryiProperties.ItemByPropId[(int)iPropertyTypeEnum].Value.ToString();
			    newpropertyvalue = GetiPropertyDisplayName(invSummaryiProperties.ItemByPropId[(int)iPropertyTypeEnum]);
		    }
		    if (propertyTypeStr == string.Empty) {
			    propertyTypeStr = GetiPropertyTypeString(invSummaryiProperties.ItemByPropId[(int)iPropertyTypeEnum]);
		    }
		    return currentvalue;
	    }
	    #endregion
	    #region "Get or Set Custom iProperty Values"

	    /// <summary>
	    /// This method should set or get any custom iProperty value
	    /// </summary>
	    /// <param name="Doc">the document to edit</param>
	    /// <param name="PropertyName">the iProperty name to retrieve or update</param>
	    /// <param name="PropertyValue">the optional value to assign - if empty we are retrieving a value</param>
	    /// <returns></returns>
	    public static object SetorCreateCustomiProperty(Inventor.Document Doc, string PropertyName, object PropertyValue = null)
	    {
            try
            {
                // Get the custom property set.
                PropertySet customPropSet = default(Inventor.PropertySet);
                object customproperty = null;

                customPropSet = Doc.PropertySets["Inventor User Defined Properties"];

                // Get the existing property, if it exists.
                Inventor.Property prop = null;
                bool propExists = true;
                try
                {
                    prop = customPropSet[PropertyName];
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                    propExists = false;
                }
                if ((PropertyValue != null))
                {
                    // Check to see if the property was successfully obtained.
                    if (!propExists)
                    {
                        // Failed to get the existing property so create a new one.
                        prop = customPropSet.Add(PropertyValue, PropertyName);
                    }
                    else
                    {
                        // Change the value of the existing property.
                        prop.Value = PropertyValue;
                    }
                }
                else
                {
                    customproperty = prop.Value;
                }
                return customproperty;
            }
            catch (Exception mainEx)
            {
                log.Error("Error: " + mainEx.Message + "in file: " + Doc.FullFileName, mainEx);
                return null;
            }
	    }
	    #endregion

	    #endregion
}
}
