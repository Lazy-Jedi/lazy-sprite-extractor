/*
 *
 * Created By: Uee
 * Modified By: 
 *
 * Last Modified: 19 April 2019
 *
 *
 * This software is released under the terms of the
 * GNU license. See https://www.gnu.org/licenses/#GPL
 * for more information.
 *
 */

using System.Collections.Generic;
using System.Text;

namespace SpriteExtractor.Extensions
{
    public static class StringExtension
    {
        /// <summary>
        ///     Cached String Builder of String Data.
        /// </summary>
        private static readonly StringBuilder _stringBuilder = new StringBuilder();

        /// <summary>
        ///     Concat String Data using String Builder.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static string Combine(this string value, params object[] values)
        {
            if (values.Length == 0) return value;

            if (values.Length == 1 && !values[0].GetType().IsArray) return values[0].ToString();

            // Clear String Builder.
            _stringBuilder.Clear();

            // Add the Value Before Concatenating.
            _stringBuilder.Append(value);

            for (int i = 0; i < values.Length; i++)
                if (values[i].GetType().IsArray)
                    CombineArray(values[i] as object[]);
                else
                    _stringBuilder.Append(values[i]);

            return _stringBuilder.ToString();
        }


        /// <summary>
        ///     Helper Method to Assist with Combining String Data in an Array.
        /// </summary>
        /// <param name="values"></param>
        private static void CombineArray(IReadOnlyCollection<object> values)
        {
            if (values.Count == 0) return;

            foreach (string val in values) _stringBuilder.Append(val);
        }
    }
}