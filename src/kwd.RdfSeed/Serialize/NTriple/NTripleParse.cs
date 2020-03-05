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
        private Node<UriOrBlank>? Subject;
        private UriNode? Predicate;
        private Node? Object;

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

            if(token.Type == NTripleTokenType.ws)return;

            if(token.Type == NTripleTokenType.comment)return;
            
            if (Subject is null)
            {
                if (token.Type == NTripleTokenType.uri)
                {
                    IsValidUri(token.Value);
                    Subject = _graph.Uri(ValueEncoder.UriUnEscape(token.Value));
                    return;
                }

                if (token.Type == NTripleTokenType.blank)
                {
                    Subject = _graph.Blank(token.Value);
                    return;
                }
                throw new Exception("Expected uri for subject");
            }

            if (Predicate is null)
            {
                if (token.Type == NTripleTokenType.uri)
                {
	                IsValidUri(token.Value);
                    Predicate = _graph.Uri(
                        ValueEncoder.UriUnEscape(token.Value));
                    return;
                }

                throw new Exception("Expected uri for predicate");
            }

            if (Object is null)
            {
                if (token.Type == NTripleTokenType.uri)
                {
                    IsValidUri(token.Value);
                    Object = _graph.Uri(token.Value);
                    return;
                }

                if (token.Type == NTripleTokenType.blank)
                {
                    Object = _graph.Blank(token.Value);
                    return;
                }

                if (_objectParts is null)
                {
                    if(token.Type != NTripleTokenType.literal)
                        throw new Exception("expected literal for object value");
                    _objectParts = new ObjectParts
                    {
                        Literal = ValueEncoder.LiteralUnEscape(token.Value)
                    };
                    return;
                }
                
                if (token.Type == NTripleTokenType.lang)
                {
	                IsValidLanguageTag(token.Value);
                    _objectParts.Lang = new string(token.Value);
                    return;
                }

                if (token.Type == NTripleTokenType.dataType)
                {
                    IsValidUri(token.Value);
                    _objectParts.ValueType = new string(token.Value);
                    return;
                }
            }

            if(token.Type != NTripleTokenType.dot)
                throw new Exception("Expected dot to end triple.");

            if (Object is null && _objectParts !=  null)
            {
                Object = _graph.NewNode(_objectParts.Literal,
                    ValueEncoder.UriUnEscape(_objectParts.ValueType),
                    _objectParts.Lang);
            }

            if(Object is null)
                throw new Exception("Expected object to be created");

            _graph.Assert(Subject, Predicate, Object);

            //reset for next triple.
            Subject = null;
            Predicate = null;
            Object = null;
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