using System;
using kwd.RdfSeed.Errors;

namespace kwd.RdfSeed.Core.Nodes.Builtin
{
	/// <summary>
	/// Builder to create <see cref="Node{T}"/>
	/// </summary>
	public class UriOrBlankMap : NodeMap<UriOrBlank>
	{
		/// <summary>
		/// Data type for the inbuilt node type.
		/// </summary>
		public const string ValueType = "inbuilt:rdfLink";

		/// <summary>Create new <see cref="UriOrBlankMap"/>.</summary>
		public UriOrBlankMap():base(ValueType){}

		/// <inheritdoc />
		public override Node<UriOrBlank> Create(UriOrBlank value)
			=> value.Uri is null ? 
				(Node<UriOrBlank>)new BlankNode(this, value) : 
				new UriNode(this, value);

		/// <inheritdoc />
		public override Node Create(ReadOnlySpan<char> valueString)
			=> New(valueString);

		/// <summary>Create new <see cref="UriNode"/></summary>
		/// <exception cref="InvalidUri"></exception>
		public UriNode New(ReadOnlySpan<char> uriValue)
			=> New(new string(uriValue));

		/// <summary>Create new <see cref="UriNode"/></summary>
		/// <exception cref="InvalidUri"></exception>
		public UriNode New(string uriValue)
			=> new UriNode(this, new UriOrBlank(uriValue));

		/// <summary>Create new <see cref="BlankNode"/></summary>
		public BlankNode NewBlank(Node<UriOrBlank> scope, string label)
			=> new BlankNode(this, new UriOrBlank(scope, label));
				    
        
		/// <summary>Create new <see cref="BlankNode"/> scoped to itself</summary>
		public BlankNode NewSelfScoped(string label)
			=>new BlankNode(this, new UriOrBlank(null, label));
	}
}