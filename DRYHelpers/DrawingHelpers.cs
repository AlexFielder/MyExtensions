using Inventor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRYHelpers
{
    public static class DrawingHelpers
    {
        /// <summary>
        /// Copied from page 23 of this file: https://forums.autodesk.com/autodesk/attachments/autodesk/120/58687/1/MA316-5,ConfiguratorTips.pdf
        /// </summary>
        /// <param name="InventorApp"></param>
        /// <param name="drawView"></param>
        /// <param name="ViewName"></param>
        public static void AutoRetrieveDim(Inventor.Application InventorApp, DrawingView drawView, string ViewName)
        {
            Document viewDoc = (Document)drawView.ReferencedDocumentDescriptor.ReferencedDocument;
            Sheet sh = drawView.Parent;
            TransientGeometry tg = InventorApp.TransientGeometry;
            ObjectCollection dimsToRetrieve = viewDoc.AttributeManager.FindObjects("GetDim", "View", ViewName);
            if (dimsToRetrieve.Count > 0)
            {
                sh.DrawingDimensions.GeneralDimensions.Retrieve(drawView, dimsToRetrieve);
            }
        }

        /// <summary>
        /// Copied from page 25 of this file: https://forums.autodesk.com/autodesk/attachments/autodesk/120/58687/1/MA316-5,ConfiguratorTips.pdf
        /// </summary>
        /// <param name="DrawView"></param>
        /// <param name="WorkPoint1"></param>
        /// <param name="WorkPoint2"></param>
        /// <param name="TextPosition"></param>
        /// <param name="AlignmentType"></param>
        /// <returns></returns>
        public static GeneralDimension DimToWorkPoints(DrawingView DrawView, WorkPoint WorkPoint1, WorkPoint WorkPoint2, Point2d TextPosition, DimensionTypeEnum AlignmentType)
        {
            try
            {
                Sheet sheet = DrawView.Parent;
                Centermark[] marks = new Centermark[1];
                double[] endfaceParams = new double[1];
                marks[0].Visible = false;
                marks[1] = sheet.Centermarks.AddByWorkFeature(WorkPoint2, DrawView);
                marks[1].Visible = false;
                GeometryIntent intent1 = sheet.CreateGeometryIntent(marks[0]);
                GeometryIntent intent2 = sheet.CreateGeometryIntent(marks[1]);
                GeneralDimension genDim = (GeneralDimension)sheet.DrawingDimensions.GeneralDimensions.AddLinear(TextPosition, intent1, intent2, AlignmentType);
                return genDim;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
