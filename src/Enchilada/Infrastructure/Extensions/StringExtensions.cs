namespace Enchilada.Infrastructure.Extensions
{
    using System;
    using System.Text.RegularExpressions;

    public static class StringExtensions
    {
        public static bool IsNullOrEmpty( this string operand )
        {
            return string.IsNullOrEmpty( operand );
        }

        public static bool IsNullOrWhiteSpace( this string operand )
        {
            return string.IsNullOrWhiteSpace( operand );
        }


        public static string StripDoubleSlash( this string operand )
        {
            var regex = new Regex( "(?<!:)\\/\\/", RegexOptions.Compiled );

            return regex.Replace( operand, "/" );
        }

        public static string StripLeadingSlash( this string operand )
        {
            return operand.IndexOf( "/", StringComparison.Ordinal ) < 0
                ? operand
                : operand.TrimStart( '/' );
        }

        public static string RemoveFilename( this string operand )
        {
            if ( operand.Contains( "/" ) )
            {
                return operand.Substring( 0, operand.LastIndexOf( '/' ) );
            }

            return string.Empty;
        }
    }
}