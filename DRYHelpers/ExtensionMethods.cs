using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
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
    }

}
