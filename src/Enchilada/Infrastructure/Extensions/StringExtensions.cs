namespace Enchilada.Infrastructure.Extensions
{
    using System;
    using System.Linq;
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
            return operand.Contains( "/" )
                ? operand.Substring( 0, operand.LastIndexOf( '/' ) )
                : string.Empty;
        }

        public static string RemovePath( this string operand )
        {
            return operand.Split( '/' ).LastOrDefault();
        }

        public static string CombineAsUriWith( this string operand, string rightHandSide )
        {
            if ( rightHandSide.IsNullOrEmpty() )
                return operand;

            return string.Format( "{0}/{1}", operand.TrimEnd( '/' ), rightHandSide.Trim( '/' ) );
        }

        public static string GetFilenameFromPath( this string operand )
        {
            return operand.Contains( "/" ) ? operand.Substring( operand.LastIndexOf( '/' ) + 1 ) : operand;
        }

        public static string GetDirectoryNameFromPath( this string operand )
        {
            return operand.Contains( "/" ) ? operand.Substring( 0, operand.LastIndexOf( '/' ) + 1 ) : string.Empty;
        }

        public static string EnsureTrailingSlash( this string operand )
        {
            return operand.EndsWith( "/" ) ? operand : operand + "/";
        }
    }
}