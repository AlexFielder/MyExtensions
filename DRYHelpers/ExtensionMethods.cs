using Inventor;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRYHelpers
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Allows us to convert an Excel address in the int between 1 & 16384
        /// from here: https://stackoverflow.com/a/182924/572634
        /// </summary>
        /// <param name="excelAddress"></param>
        /// <param name="columnNumber"></param>
        /// <returns>Returns a string of the form A, AA, AAA etc.</returns>
        public static string GetExcelColumnName(this string excelAddress, int columnNumber)
        {
            int dividend = columnNumber;
            string columnName = String.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (int)((dividend - modulo) / 26);
            }

            return columnName;
        }


        /// <summary>
        /// Allows us to convert an Excel column name to the corresponding number.
        /// From here: http://www.c-sharpcorner.com/forums/how-can-i-get-the-column-name-as-integer-value#divReplyBox_79935
        /// </summary>
        /// <param name="straddress"></param>
        /// <returns></returns>
        public static int ExcelColumnNameToNumber(this string straddress)
        {
            char[] delimiterChars = { '$' }; //parse by $ 

            string[] words = straddress.Split(delimiterChars); //return each parsed string 

            straddress = words[1];

            if (string.IsNullOrEmpty(straddress)) throw new ArgumentNullException("columnName");
            char[] characters = straddress.ToUpperInvariant().ToCharArray();
            int sum = 0;
            for (int i = 0; i < characters.Length; i++)
            {
                sum *= 26;
                sum += (characters[i] - 'A' + 1);
            }
            return sum;  // in this example, sum would be "1" representing the column # where Customer Name resides 
        }

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
    }

}
