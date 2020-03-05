using System;
using kwd.Rdf.Std;
using kwd.RdfSeed.Core.Nodes;

namespace kwd.RdfSeed.TypedNodes
{
    /// <summary>
    /// String data with language specifier.
    /// </summary>
    public class Text
    {
	    /// <summary>
	    /// Create a literal with language node as a <see cref="Node{Text}"/>.
	    /// </summary>
	    public class TextNodeMap : NodeMap<Text>
	    {
		    /// <summary>
		    /// The Value type for Text
		    /// (http://www.w3.org/1999/02/22-rdf-syntax-ns#langString)
		    /// </summary>
		    public static readonly string ValueType = XMLSchema.LangString;

		    /// <inheritdoc />
		    public TextNodeMap() : base(ValueType){}

		    /// <inheritdoc />
		    public override Node<Text> Create(Text value)
            => new Node<Text>(this, value, value.ValueString);

		    /// <inheritdoc />
		    public override Node Create(ReadOnlySpan<char> valueString)
			    => Create(Parse(valueString));
	    }

        /// <summary>Create new <see cref="Text"/>.</summary>
        public Text(string value, string language)
        {
            Value = value;
            Language = language.StartsWith('@')? language.Substring(1) : language;
        }

        /// <summary>
        /// Split a language string to create new <see cref="Text"/> object.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public static Text Parse(ReadOnlySpan<char> data)
        {
            var idx = data.LastIndexOf('@');
            if(idx < 1 || idx == data.Length-1) 
                throw new ArgumentException(nameof(data));

            return new Text(new string(data.Slice(0, idx)), 
                new string(data.Slice(idx+1)));
        }

        /// <summary>The text value</summary>
        public readonly string Value;

        /// <summary>The text language</summary>
        public readonly string Language;

        /// <summary>
        /// internal value string for this text data.
        /// </summary>
        public string ValueString =>Value + "@" + Language;
    }
}