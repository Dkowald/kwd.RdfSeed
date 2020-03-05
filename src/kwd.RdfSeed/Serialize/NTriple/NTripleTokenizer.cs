using System;

namespace kwd.RdfSeed.Serialize.NTriple
{
	/// <summary>
	/// Tokenizer to extract base data.
	/// </summary>
	public static class NTripleTokenizer
	{
		/// <summary>
		/// Create token by scanning source data.
		/// </summary>
		public static Token NextToken(ReadOnlySpan<char> data)
		{
			if (data.IsEmpty) return Token.Invalid(data);

			if (char.IsWhiteSpace(data[0])) return new Token(
				NTripleTokenType.ws, data.Slice(0, 1), data.Slice(1));

			if (data[0] == '.') return new Token(
				 NTripleTokenType.dot, data.Slice(0, 1), data.Slice(1));

			int end;

			if (data[0] == '"')
			{
				end = -1;
				var escape = false;
				for (var i = 1; i < data.Length; i++)
				{
					if (data[i] == '"' && !escape)
					{ end = i; break; }

					//skip \\
					if (data[i] == '\\' &&
						i + 1 < data.Length && data[i + 1] == '\\')
					{ i++; continue; }

					escape = data[i] == '\\';
				}

				if (end < 0) return Token.Invalid(data);

				return new Token(
						NTripleTokenType.literal,
						data.Slice(1, end - 1),
						data.Slice(end + 1)
					);
			}

			if (data[0] == '<')
			{
				end = data.IndexOf(">");
				if (end < 0) return Token.Invalid(data);

				var val = ValueEncoder.LiteralUnEscape(data.Slice(1, end - 1));
				return new Token(
						NTripleTokenType.uri, val, data.Slice(end + 1));
			}

			if (data[0] == '#')
			{
				//safe for EOL \r\n or \n

				end = data.IndexOf('\n');
				if (end < 0) end = data.Length;

				var remainder =
					end == data.Length ?
						ReadOnlySpan<char>.Empty :
						data.Slice(end + 1);

				if (data[end - 1] == '\r') end--;

				var value = end == data.Length ? 
					data.Slice(1) : data.Slice(1, end);

				return new Token(NTripleTokenType.comment,
					value, remainder);
			}

			if (data.StartsWith("_:"))
			{
				end = 2;
				if(end == data.Length || IsInvalidBlankStartChar(data[end]))
					throw new Exception(
					"Blank label cannot start with a " +
					(end == data.Length ? "no-char" : data[end].ToString()));
				
				for (end = 3; end < data.Length; end++)
				{
					var ch = data[end];
					
					if (!IsValidBlankChar(ch))
					{ end--; break; }
				}

				if (end == data.Length) end--;

				//cannot end on a .
				if (data[end] == '.') end--;

				var rest = end+1 < data.Length ? data.Slice(end+1) 
					: ReadOnlySpan<char>.Empty;

				return new Token(
					NTripleTokenType.blank,
					data.Slice(2, end-1), rest);
			}

			if (data[0] == '@')
			{
				end = IndexOfWhitespace(data);
				if (end < 0) return Token.Invalid(data);

				return new Token(
					NTripleTokenType.lang,
					data.Slice(1, end - 1),
					data.Slice(end)
					);
			}

			if (data.StartsWith("^^<"))
			{
				end = data.IndexOf('>');
				if (end < 0) return Token.Invalid(data);

				return new Token(NTripleTokenType.dataType,
					data.Slice(3, end - 3),
					data.Slice(end + 1));
			}

			return Token.Invalid(data);
		}

		/// <summary>A Token</summary>
		public ref struct Token
		{
			/// <summary>Create token representing invalid read.</summary>
			public static Token Invalid(ReadOnlySpan<char> rest) => new Token(
				NTripleTokenType.Invalid, ReadOnlySpan<char>.Empty, rest);

			/// <summary>Create new <see cref="Token"/></summary>
			public Token(NTripleTokenType type, ReadOnlySpan<char> value, ReadOnlySpan<char> rest)
			{
				Type = type;
				Value = value;
				Rest = rest;
			}

			/// <summary>The type of token</summary>
			public readonly NTripleTokenType Type;

			/// <summary>The value of <see cref="Token"/>.</summary>
			public readonly ReadOnlySpan<char> Value;

			/// <summary>
			/// Remaining text after extracting this <see cref="Token"/>.
			/// </summary>
			public readonly ReadOnlySpan<char> Rest;

			/// <summary>
			/// True if <see cref="Rest"/> is empty,
			/// or this is an invalid token.
			/// </summary>
			public bool IsEnd => Type == NTripleTokenType.Invalid && Rest.IsEmpty;
		}

		/// <summary>
		/// Valid char for Blank.
		/// </summary>
		/// <remarks>
		/// Deliberately re-ordered to short-circuit common chars
		/// </remarks>
		public static bool IsValidBlankChar(char ch)
			=> char.IsLetter(ch) ||//PN_CHARS_BASE
				char.IsDigit(ch) ||//PN_CHARS
			    ch == '_' || ch == ':' ||//PN_CHARS_U
				ch == '-' || //PN_CHARS
				ch == '.' ||//PN_CHARS (0x00B7)

				//PN_CHARS_BASE
			   (ch >= 0x00C0 && ch <= 0x00D6) ||
			   (ch >= 0x00D8 && ch <= 0x00F6) ||
			   (ch >= 0x00F8 && ch <= 0x02FF) ||
			   (ch >= 0x0370 && ch <= 0x037D) ||
			   (ch >= 0x037F && ch <= 0x1FFF) ||
			   (ch >= 0x200C && ch <= 0x200D) ||
			   (ch >= 0x2070 && ch <= 0x218F) ||
			   (ch >= 0x2C00 && ch <= 0x2FEF) ||
			   (ch >= 0x3001 && ch <= 0xD7FF) ||
			   (ch >= 0xF900 && ch <= 0xFDCD) ||
			   (ch >= 0xFDF0 && ch <= 0xFFFD) ||
			   (ch >= 0x10000 && ch <= 0xEFFFF) ||
			   //PN_CHARS
			   (ch >= 0x0300 && ch <= 0x036F) ||
			   (ch >= 0x203F && ch <= 0x2040);
		
		/// <summary>
		/// Test if char is in-valid start char for Blank node label.
		/// </summary>
		public static bool IsInvalidBlankStartChar(char ch) =>
			ch == '-' || ch == 0x00B7 || ch == '.' ||
			(ch >= 0x0300 && ch <= 0x036F) ||
			(ch >= 0x203F && ch <= 0x2040);
		
		private static int IndexOfWhitespace(ReadOnlySpan<char> data)
		{
			for (var i = 0; i < data.Length; i++)
			{ if (char.IsWhiteSpace(data[i])) return i; }

			return -1;
		}
	}
}