using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Netherite.Utils
{
    public static class ConventionHelper
    {
        public static string ToSnakeCase<T>(this T @enum) where T : Enum
        {
            return Enum.GetName(typeof(T), @enum).ToSnakeCase();
        }

        public static T ToEnum<T>(this string n) where T : Enum
        {
            return (T)Enum.Parse(typeof(T), n.ToPascalCase());
        }

        /// <summary>
        /// Converts from "PascalCase" to "snake_case".
        /// </summary>
        /// <returns></returns>
        public static string ToSnakeCase(this string n)
        {
            var result = "";
            foreach (Match m in new Regex("([A-Z][^A-Z]*)").Matches(n))
            {
                if (m.Value.Length > 0)
                {
                    result += "_";
                    result += m.Value.ToLower();
                }
            }
            return result[1..];
        }

        /// <summary>
        /// Converts from "snake_case" to "PascalCase".
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static string ToPascalCase(this string n)
        {
            var result = "";
            foreach (Match m in new Regex("([a-z0-9]*)").Matches(n))
            {
                if (m.Value.Length > 0)
                {
                    result += m.Value[0].ToString().ToUpper();
                    result += m.Value[1..];
                }
            }
            return result;
        }
    }
}
