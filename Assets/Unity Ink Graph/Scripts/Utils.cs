using System;
using System.IO;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using UnityEngine;

namespace UnityInkGraph {
    public static class Utils {
        /// <summary>
        /// Gets the value of a field. If it's null, initialized the field first.
        /// </summary>
        /// <param name="field">The field to get or set.</param>
        /// <param name="assignFunc">The function to use if the field is null.</param>
        /// <typeparam name="T">The type of field.</typeparam>
        /// <returns>The initialized field.</returns>
        public static T GetOrSet<T>(ref T field, Func<T> assignFunc) {
            if (field == null || field.Equals(null)) {
                field = assignFunc();
            }

            return field;
        }

        /// <summary>
        ///     Returns true if the strings is null or an empty string.
        /// </summary>
        /// <param name="source">The string to test.</param>
        /// <returns>True if the string is null or empty, otherwise false.</returns>
        [ContractAnnotation("source:null => true")]
        public static bool IsNullOrEmpty(this string source) {
            return string.IsNullOrEmpty(source);
        }
        
        /// <summary>
        ///     A convenience extension version of Regex.Replace.
        /// </summary>
        /// <param name="input">The string to search for a match.</param>
        /// <param name="pattern">The regular expression pattern to match.</param>
        /// <param name="replacement">The replacement string.</param>
        /// <param name="options">A bitwise combination of the enumeration values that provide options for matching.</param>
        /// <returns>
        ///     A new string that is identical to the input string, except that the replacement string takes the place of each
        ///     matched string.
        /// </returns>
        public static string ReplaceRegex(this string input, string pattern, string replacement, RegexOptions options = RegexOptions.None) {
            return Regex.Replace(input, pattern, replacement, options);
        }

        /// <summary>
        ///     A convenience extension version of Regex.Split.
        /// </summary>
        /// <param name="input">The string to search for a match.</param>
        /// <param name="pattern">The regular expression pattern to match.</param>
        /// <param name="options">A bitwise combination of the enumeration values that provide options for matching.</param>
        /// <returns>
        ///     A new array of strings split based on the string and pattern given.
        /// </returns>
        public static string[] SplitRegex(this string input, string pattern, RegexOptions options = RegexOptions.None) {
            return Regex.Split(input, pattern, options);
        }

        /// <summary>
        ///     Replaces a string's old value with a new value using the string comparison type.
        /// </summary>
        /// <param name="originalString">The string to run the search/replace on.</param>
        /// <param name="oldValue">The old value to find.</param>
        /// <param name="newValue">The new value to replace.</param>
        /// <param name="comparisonType">The type of comparison to use.</param>
        /// <returns></returns>
        public static string Replace(this string originalString, string oldValue, string newValue, StringComparison comparisonType) {
            int startIndex = 0;
            while (true) {
                startIndex = originalString.IndexOf(oldValue, startIndex, comparisonType);
                if (startIndex == -1) {
                    break;
                }
                originalString = originalString.Substring(0, startIndex) + newValue + originalString.Substring(startIndex + oldValue.Length);
                startIndex += newValue.Length;
            }

            return originalString;
        }

        /// <summary>
        /// Does 'File.WriteAllText', but makes sure there are folders to write to first.
        /// </summary>
        /// <param name="path">The path to write text to.</param>
        /// <param name="contents">The contents to write to the file.</param>
        public static void WriteAllText(string path, string contents) {
            CreateFoldersFor(path);
            File.WriteAllText(path, contents);
        }

        private static void CreateFoldersFor(string path) {
            if (path.IsNullOrEmpty()) {
                Debug.LogWarning("Can't make a directory for an empty path.");
                return;
            }

            string folder = Path.GetDirectoryName(path);

            if (folder.IsNullOrEmpty() || Directory.Exists(folder)) {
                return;
            }

            Directory.CreateDirectory(folder);
        }

    }
}