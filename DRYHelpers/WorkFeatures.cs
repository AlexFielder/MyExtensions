using Inventor;
using log4net;
using System;

namespace DRYHelpers
{
    public class WorkFeatures
    {
        public static readonly ILog log = LogManager.GetLogger(typeof(WorkFeatures));
        /// <summary>
        /// Utility function used by the AlignOccurrencesWithConstraints macro.
        /// Given an assembly component definition it returns the base work planes that are in
        /// the part or assembly the occurrence references.  It gets the
        /// proxies for the planes since it needs the work planes in the
        /// context of the assembly and not in the part or assembly document
        /// where they actually exist.
        /// </summary>
        /// <param name="compDefinition"></param>
        /// <param name="baseXY"></param>
        /// <param name="baseYZ"></param>
        /// <param name="baseXZ"></param>
        public static void GetPlanesFromAssembly(AssemblyComponentDefinition compDefinition, ref WorkPlane baseXY, ref WorkPlane baseYZ, ref WorkPlane baseXZ)
        {
            baseXY = compDefinition.WorkPlanes["XY Plane"];
            baseYZ = compDefinition.WorkPlanes["YZ Plane"];
            baseXZ = compDefinition.WorkPlanes["XZ Plane"];
        }

        /// <summary>
        /// Utility function used by the AlignOccurrencesWithConstraints macro.
        /// Given an occurrence it returns the base work planes that are in
        /// the part or assembly the occurrence references.  It gets the
        /// proxies for the planes since it needs the work planes in the
        /// context of the assembly and not in the part or assembly document
        /// where they actually exist.
        /// </summary>
        /// <param name="Occurrence"></param>
        /// <param name="BaseXY"></param>
        /// <param name="BaseYZ"></param>
        /// <param name="BaseXZ"></param>
        public static void GetPlanesFromPart(ComponentOccurrence Occurrence, ref WorkPlane BaseXY, ref WorkPlane BaseYZ, ref WorkPlane BaseXZ)
        {
            // Get the work planes from the definition of the occurrence.
            // These will be in the context of the part or subassembly, not
            // the top-level assembly, which is what we need to return.
            PartDocument thisDoc = null;
            object m_BaseXY = null;
            object m_BaseYZ = null;
            object m_BaseXZ = null;

            try
            {
                thisDoc = (PartDocument)Occurrence.Definition.Document;
                BaseXY = thisDoc.ComponentDefinition.WorkPlanes["XY Plane"];
                BaseYZ = thisDoc.ComponentDefinition.WorkPlanes["YZ Plane"];
                BaseXZ = thisDoc.ComponentDefinition.WorkPlanes["XZ Plane"];
                Occurrence.CreateGeometryProxy(BaseXY, out m_BaseXY);
                Occurrence.CreateGeometryProxy(BaseYZ, out m_BaseYZ);
                Occurrence.CreateGeometryProxy(BaseXZ, out m_BaseXZ);
            }
            catch (InvalidCastException ex)
            {
                log.Info("Caught possible expected, invalid document cast Exception: " + ex.Message);
                AssemblyDocument thisAssy = (AssemblyDocument)Occurrence.Definition.Document;
                AssemblyComponentDefinition thisAssyDef = thisAssy.ComponentDefinition;
                GetPlanesFromAssembly(thisAssyDef, ref BaseXY, ref BaseYZ, ref BaseXZ);

            }
            ////would need to encompass the following in a try catch to account for either parts or assemblies.
            ////BaseXY = thisDoc.ComponentDefinition.WorkPlanes["XY Plane"];
            //BaseXY = Occurrence.Definition.WorkPlanes["XY Plane"];
            //BaseYZ = Occurrence.Definition.WorkPlanes["YZ Plane"];
            //BaseXZ = Occurrence.Definition.WorkPlanes["XZ Plane"];

            // Create proxies for these planes.  This will act as the work
            // plane in the context of the top-level assembly.
            if (thisDoc != null)
            {
                BaseXY = (WorkPlane)m_BaseXY;
                BaseYZ = (WorkPlane)m_BaseYZ;
                BaseXZ = (WorkPlane)m_BaseXZ;
            }


        }
    }
}
