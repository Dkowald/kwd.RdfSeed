using System;
using System.Runtime.Serialization;
using kwd.Rdf.Std;
using kwd.RdfSeed.Core;
using kwd.RdfSeed.Core.Nodes;
using kwd.RdfSeed.Core.Nodes.Builtin;
using kwd.RdfSeed.TypedNodes;

namespace kwd.RdfSeed
{
    // ReSharper disable MemberCanBePrivate.Global
    // ReSharper disable once UnusedType.Global

    /// <summary>
    /// Helpers for debugging
    /// </summary>
    public static class RdfDebug
    {
        private static readonly Lazy<ObjectIDGenerator> ObjectId = 
            new Lazy<ObjectIDGenerator>(()=> new ObjectIDGenerator());

        private static string Id(object? o) => 
            o is null ? "null" : $"({ObjectId.Value.GetId(o, out _)})";

        /// <summary>Debug print a <see cref="Node"/>.</summary>
        /// <remarks>
        /// Use a short hand syntax for well known dataTypes
        /// https://www.w3.org/TR/n-quads/
        /// </remarks>
        public static string Print(Node? node, bool includeId = true)
        {
	        if (node is null) return "null";

            var txt = string.Empty;

            if (node is UriNode)
                txt = $"<{node.ValueString}>";

            if (node is BlankNode blank)
                txt = $"_:{blank.Label} [blank]";

            if (node is Node<Text> text)
                txt = $"{text.Value.Value}@{text.Value.Language} [text]";
            
            if (node is Node<bool>)
                txt = $"{node.ValueString} [bool]";

            if (node is Node<byte[]>)
                txt = " ... [binary]";

            if (node is Node<DateTime>)
                txt = $"{node.ValueString} [datetime]";

            if (node is Node<double>)
                txt = $"{node.ValueString} [double]";

            if (node is Node<int> intNode)
                txt = $"{intNode.Value} [int]";

            if (node.ValueType.DataType == XMLSchema.String)
                txt = $"{node.ValueString} [literal]";

            if (ObjectNodeMap.IsObjectType(node.ValueType.DataType))
                txt = $"{node.ValueType.Native.Name} [object]";
            
            if(txt == string.Empty)
                txt = $"{node.ValueString}^^<{node.ValueType.DataType}>";

            return includeId ? txt + Id(node) : txt;
        }

        /// <summary>Debug print a <see cref="Quad"/>.</summary>
        public static string Print(Quad? quad)
        {
	        if (quad is null) return "null";

            return $"{Print(quad.Subject, false)} " +
                   $"{Print(quad.Predicate, false)} " +
                   $"{Print(quad.Object, false)} .";
        }
    }
}