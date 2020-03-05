using System;
using System.Collections.Generic;
using kwd.Rdf.Std;
using kwd.RdfSeed.Core;
using kwd.RdfSeed.Core.Nodes;

namespace kwd.RdfSeed.TypedNodes
{
    /// <summary>
    /// These helper provide short-cuts
    /// available when using the TypedNode builders.
    /// </summary>
    public static class TypedNodesHelpers
    {
        /// <summary>
        /// A set of common strong typed nodes.
        /// </summary>
        public static IEnumerable<NodeMap> AllNodeMappings()
        {
            yield return new BooleanNodeMap();
            yield return new ByteArrayNodeMap();
            yield return new DateTimeNodeMap();
            yield return new DoubleNodeMap();
            yield return new IntegerNodeMap();
            yield return new LiteralNodeMap();
            yield return new Text.TextNodeMap();
        }

        /// <summary>
        /// Create Literal or Text or Typed-literal.
        /// </summary>
        public static Node NewNode(this IBasicNodeFactory self,
            ReadOnlySpan<char> valueString,
            ReadOnlySpan<char> dataType,
            ReadOnlySpan<char> language)
        {
            
            if(!dataType.IsEmpty && !language.IsEmpty)
                throw new Exception("Cannot have both data type and language");

            if (!language.IsEmpty)
                return self.New(new Text(
                    new string(valueString), 
                    new string(language)));

            if (!dataType.IsEmpty)
                return self.New(valueString, dataType);

            return self.New(valueString, XMLSchema.String);
        }

        #region Text and literals
        /// <summary>
        /// Create a string literal node.
        /// </summary>
        public static Node<string> Literal(this IBasicNodeFactory self, ReadOnlySpan<char> value)
            => (Node<string>)self.New(value, XMLSchema.String);

        /// <summary>
        /// Create a text node: a string literal with language.
        /// </summary>
        public static Node<Text> Text(this IBasicNodeFactory self,
            ReadOnlySpan<char> text, ReadOnlySpan<char> language)
            => self.New(new Text(new string(text), new string(language)));

        /// <summary>
        /// Creates either a text node or literal node.
        /// </summary>
        public static Node TextOrLiteral(this IBasicNodeFactory self,
            ReadOnlySpan<char> value, ReadOnlySpan<char> language)
            => language.IsEmpty ? (Node)self.Literal(value) 
                : self.Text(value, language);

        /// <summary>
        /// Literal text node with optional language
        /// </summary>
        public static Node L(this IBasicNodeFactory self, string literal, string? language = null) 
            => self.TextOrLiteral(literal, language);
        #endregion
    }
}