using System;
using kwd.RdfSeed.Core;
using kwd.RdfSeed.Core.Nodes;
using kwd.RdfSeed.Core.Nodes.Builtin;
using kwd.RdfSeed.TypedNodes;

namespace kwd.RdfSeed.Serialize.NTriple
{
    /// <summary>
    /// Parser for N-triple data.
    /// </summary>
    /// <remarks>
    /// todo: provide exceptions with details.
    /// e.g bad formed literal escape.
    /// </remarks>
    public class NTripleParse
    {
        private readonly Graph _graph;
        private Node<UriOrBlank>? _subject;
        private UriNode? _predicate;
        private Node? _object;

        private class ObjectParts
        {
            public string? Literal;
            public string? ValueType;
            public string? Lang;
        }

        private ObjectParts? _objectParts;

        /// <summary>Create a <see cref="NTripleParse"/>.</summary>
        public NTripleParse(Graph graph)
        {
            _graph = graph;
        }

        /// <summary>Load triple data</summary>
        public void Load(ReadOnlySpan<char> text)
        {
            var token = NTripleTokenizer.NextToken(text);
            while (!token.IsEnd)
            {
                Next(token);
                token = NTripleTokenizer.NextToken(token.Rest);
            }
        }

        /// <summary>
        /// Handle the next <see cref="NTripleTokenizer.Token"/>
        /// </summary>
        public void Next(NTripleTokenizer.Token token)
        {
            if(token.Type == NTripleTokenType.Invalid)
                throw new Exception("Invalid token");

            if(token.Type == NTripleTokenType.Whitespace)return;

            if(token.Type == NTripleTokenType.Comment)return;
            
            if (_subject is null)
            {
                if (token.Type == NTripleTokenType.Uri)
                {
                    IsValidUri(token.Value);
                    _subject = _graph.Uri(ValueEncoder.UriUnEscape(token.Value));
                    return;
                }

                if (token.Type == NTripleTokenType.Blank)
                {
                    _subject = _graph.Blank(token.Value);
                    return;
                }
                throw new Exception("Expected uri for subject");
            }

            if (_predicate is null)
            {
                if (token.Type == NTripleTokenType.Uri)
                {
	                IsValidUri(token.Value);
                    _predicate = _graph.Uri(
                        ValueEncoder.UriUnEscape(token.Value));
                    return;
                }

                throw new Exception("Expected uri for predicate");
            }

            if (_object is null)
            {
                if (token.Type == NTripleTokenType.Uri)
                {
                    IsValidUri(token.Value);
                    _object = _graph.Uri(token.Value);
                    return;
                }

                if (token.Type == NTripleTokenType.Blank)
                {
                    _object = _graph.Blank(token.Value);
                    return;
                }

                if (_objectParts is null)
                {
                    if(token.Type != NTripleTokenType.Literal)
                        throw new Exception("expected literal for object value");
                    _objectParts = new ObjectParts
                    {
                        Literal = ValueEncoder.LiteralUnEscape(token.Value)
                    };
                    return;
                }
                
                if (token.Type == NTripleTokenType.Language)
                {
	                IsValidLanguageTag(token.Value);
                    _objectParts.Lang = new string(token.Value);
                    return;
                }

                if (token.Type == NTripleTokenType.DataType)
                {
                    IsValidUri(token.Value);
                    _objectParts.ValueType = new string(token.Value);
                    return;
                }
            }

            if(token.Type != NTripleTokenType.Dot)
                throw new Exception("Expected dot to end triple.");

            if (_object is null && _objectParts !=  null)
            {
                _object = _graph.NewNode(_objectParts.Literal,
                    ValueEncoder.UriUnEscape(_objectParts.ValueType),
                    _objectParts.Lang);
            }

            if(_object is null)
                throw new Exception("Expected object to be created");

            _graph.Assert(_subject, _predicate, _object);

            //reset for next triple.
            _subject = null;
            _predicate = null;
            _object = null;
            _objectParts = null;
        }

        private static void IsValidLanguageTag(ReadOnlySpan<char> data)
        {
	        if(!char.IsLetter(data[0]))
		        throw new Exception("Language spec must start with letter");

	        foreach (var ch in data)
	        {
		        if(!char.IsLetterOrDigit(ch) && ch != '-')
			        throw new Exception("Language spec can only contain letter, digit or '-'");
	        }
        }

        private static void IsValidUri(ReadOnlySpan<char> data)
        {
			foreach (var ch in data)
	        {
		        if(ch == '<' || ch == '>' ||
		           ch =='"' ||
		           ch == '{' || ch == '}' ||
		           ch == '^' || ch == '`' || ch == '\\' ||
		           (ch >= 0x00 && ch <= 0x20) )
			        throw new Exception($"Invalid character in URI: {ch}");
	        }

            if(data.IndexOf(':') < 0)
	            throw new Exception("Must be an absolute URI");
        }
    }
}