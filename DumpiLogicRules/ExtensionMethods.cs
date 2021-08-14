using Inventor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DRYHelpers
{
    static class ExtensionMethods
    {
        /// <summary>
        /// Extension method used to allow searching for multiple extensions.
        /// converted from here: https://stackoverflow.com/questions/3527203/getfiles-with-multiple-extentions
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="extensions"></param>
        /// <returns></returns>
        public static IEnumerable<FileInfo> GetFilesByExtensions(this DirectoryInfo dir, params string[] extensions)
        {
            if (extensions == null)
            {
                throw new ArgumentNullException("extensions");
            }
            IEnumerable<FileInfo> files = dir.EnumerateFiles("*.*", SearchOption.AllDirectories);
            //.Where(Function(s As FileInfo) s.FullName.EndsWith(My.Settings.TemplateSearchString001) OrElse s.FullName.EndsWith(My.Settings.TemplateSearchString002))
            //Return files
            return files.Where((FileInfo f) => extensions.Contains(f.Extension));
        }

        /// <summary>
        /// Extension method intended to negate the dumb mechanic that Inventor currently uses when copying Sketchblocks whereby any
        /// renamed parameters are copied and created anew.
        /// </summary>
        /// <param name="SketchBlockDefExtending">the object being extended.</param>
        /// <param name="SketchBlockToCopy">The sketchblockdefinition being copied</param>
        /// <param name="DestinationPartDocument">The destination part document.</param>
        /// <returns></returns>
        public static SketchBlockDefinition CopyTo(this SketchBlockDefinition SketchBlockDefExtending, SketchBlockDefinition SketchBlockToCopy, PartDocument DestinationPartDocument)
        {
            throw new NotImplementedException();
        }

        
        public static TResult IfNotNull<TInput, TResult>(this TInput o, Func<TInput, TResult> evaluator) where TInput : class where TResult : class
        {
            if (o == null)
                return null;
            return evaluator(o);
        }

        //<Extension>
        //Public Function IfNotNull(ObjectExtending As DisplayFormatEnum, ObjectBeingChecked As DisplayFormatEnum) As Object
        //    If ObjectBeingChecked Is Nothing Then Return Nothing
        //    Return ObjectBeingChecked
        //End Function
    }
}
