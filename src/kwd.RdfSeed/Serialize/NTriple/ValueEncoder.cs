using System;
using System.Globalization;
using System.Text;

namespace kwd.RdfSeed.Serialize.NTriple
{
	/// <summary>
	/// Utility for encode / decode n-triple string data.
	/// </summary>
	public static class ValueEncoder
    {
		/// <summary>Escape a uri string for n-triple </summary>
		public static string UriEscape(ReadOnlySpan<char> uri)
		    => Uri.EscapeUriString(new string(uri));

		/// <summary>Un-escape a uri string </summary>
		public static string UriUnEscape(ReadOnlySpan<char> uri)
	        => Uri.UnescapeDataString(new string(uri));
		
        /// <summary>
        /// Escape special case literal chars.
        /// Per https://www.w3.org/TR/n-triples/#canonical-ntriples
        /// </summary>
        public static string LiteralEscape(string data)
        {
	        var build = new StringBuilder();
	        foreach (var ch in data)
	        {
		        var txt = ch switch
		        {
			        '\\' => "\\\\",
			        '"' => "\"",
			        '\r' => "\\r",
			        '\n' => "\\n",
			        _ => null
		        };

		        if (txt is null) build.Append(ch);
		        else build.Append(txt);
	        }
	        return build.ToString();
        }

        /// <summary>
        /// Replace escaped text with corresponding char value.
        /// </summary>
        public static string LiteralUnEscape(ReadOnlySpan<char> data)
        {
            if(!data.Contains('\\'))return new string(data);

            var build = new StringBuilder();
            for (var i = 0; i < data.Length; i++)
            {
	            if (data[i] != '\\')
	            {
		            build.Append(data[i]);
		            continue;
	            }

	            switch (data[i + 1])
	            {
		            case 't': build.Append("\t"); break;
		            case 'b': build.Append("\b"); break;
		            case 'n': build.Append("\n"); break;
		            case 'r': build.Append("\r"); break;
		            case 'f': build.Append("\f"); break;
		            case '"': build.Append("\""); break;
		            case '\'': build.Append("'"); break;
		            case '\\': build.Append("\\"); break;
		            case 'u':
			            build.Append(FromHex(data.Slice(i + 2, 4)));
			            i += 4; break;
		            case 'U':
			            build.Append(FromHex(data.Slice(i + 2, 8)));
			            i += 8; break;
		            default:
			            throw new Exception($"Cannot escape: {data[i+1]}");
	            }

	            i++;
            }

            return build.ToString();
        }
		
        private static char FromHex(ReadOnlySpan<char> data)
            => Convert.ToChar(short.Parse(data, NumberStyles.AllowHexSpecifier));
    }
}