using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventor;
using log4net;

namespace DRYHelpers
{
    public static class ClientGFX
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Parameters));

        public static void ClientGraphics3DPrimitives(string clientGraphicsCollectionId, Application InventorApp, 
            PartComponentDefinition compDef, Point origin, Point end, Line centreline, UnitVector arrowDirection)
        {
            try
            {
                Document oDoc = InventorApp.ActiveDocument;
                //  Set a reference to component definition of the active document.
                //  This assumes that a part or assembly document is active
                PartComponentDefinition oCompDef = compDef;
                //  Check to see if the test graphics data object already exists.
                //  If it does clean up by removing all associated of the client 
                //  graphics from the document. If it doesn't create it
                // TODO: On Error Resume Next Warning!!!: The statement is not translatable 
                ClientGraphics oClientGraphics = null;
                try
                {
                    oClientGraphics = oCompDef.ClientGraphicsCollection[clientGraphicsCollectionId];
                }
                catch (Exception ex)
                {
                    // TODO: On Error GoTo 0 Warning!!!: The statement is not translatable 
                    //  An existing client graphics object was successfully 
                    //  obtained so clean up
                    oClientGraphics.Delete();
                    //  Update the display to see the results
                    InventorApp.ActiveView.Update();
                    log.Debug(ex.Message, ex);
                }
                finally
                {
                    TransientGeometry oTransGeom = InventorApp.TransientGeometry;
                    //  Create the ClientGraphics object.
                    if (oClientGraphics == null)
                    {
                        oClientGraphics = oCompDef.ClientGraphicsCollection.Add(clientGraphicsCollectionId);
                    }
                    //  Create a new graphics node within the client graphics objects
                    GraphicsNode oSurfacesNode = oClientGraphics.AddNode(1);
                    TransientBRep oTransientBRep = InventorApp.TransientBRep;


                    //  Create a point representing the center of the bottom of 
                    //  the cone
                    Point coneBottom = origin;
                    //Point coneBottom = InventorApp.TransientGeometry.CreatePoint(0, 0, 0);
                    //  Create a point representing the tip of the cone
                    //Point oTop = end;
                    Point coneTop = InventorApp.TransientGeometry.CreatePoint(0, 10, 0);
                    // Create a point representing the base of the cylinder
                    Point cylinderBottom = InventorApp.TransientGeometry.CreatePoint(0, -40, 0);

                    //sort out the direction we need to point.
                    Matrix conePos = InventorApp.TransientGeometry.CreateMatrix();
                    Vector conePosVector = InventorApp.TransientGeometry.CreateVector(arrowDirection.X, arrowDirection.Y, arrowDirection.Z);
                    conePos.SetTranslation(conePosVector);
                    conePos.SetToRotateTo(coneTop.VectorTo(coneBottom), conePosVector);
                    //move the necessary points
                    coneTop.TransformBy(conePos);
                    cylinderBottom.TransformBy(conePos);
                    //  Create a transient cone body
                    //SurfaceBody oBody = oTransientBRep.CreateSolidCylinderCone(oBottom, oTop, 5, 5, 0);
                    SurfaceBody oBody = oTransientBRep.CreateSolidCylinderCone(coneBottom, coneTop, 5, 5, 0);


                    //  Reset the top point indicating the center of the top of 
                    //  the cylinder
                    //oTop = InventorApp.TransientGeometry.CreatePoint(0, -40, 0);
                    coneTop = InventorApp.TransientGeometry.CreatePoint(0, -40, 0);
                    //  Create a transient cylinder body
                    //SurfaceBody oCylBody = oTransientBRep.CreateSolidCylinderCone(oBottom, oTop, 2.5, 2.5, 2.5);
                    SurfaceBody oCylBody = oTransientBRep.CreateSolidCylinderCone(coneBottom, cylinderBottom, 2.5, 2.5, 2.5);
                    //  Union the cone and cylinder bodies
                    oTransientBRep.DoBoolean(oBody, oCylBody, BooleanTypeEnum.kBooleanTypeUnion);

                    //  Create client graphics based on the transient body
                    SurfaceGraphics oSurfaceGraphics = oSurfacesNode.AddSurfaceGraphics(oBody);
                    //  Update the view to see the resulting curves
                    InventorApp.ActiveView.Update();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
            }
        }
    }
}
