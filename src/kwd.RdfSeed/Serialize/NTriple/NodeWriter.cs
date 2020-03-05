using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using kwd.Rdf.Std;
using kwd.RdfSeed.Core;
using kwd.RdfSeed.Core.Nodes;
using kwd.RdfSeed.Core.Nodes.Builtin;
using kwd.RdfSeed.Serialize.Errors;
using kwd.RdfSeed.TypedNodes;

namespace kwd.RdfSeed.Serialize.NTriple
{
    /// <summary>
    /// Write nodes for n-triple file.
    /// </summary>
    public class NodeWriter
    {
	    private readonly Graph _g;
	    
        /// <summary>Create new <see cref="NodeWriter"/></summary>
        public NodeWriter(Graph g)
	    {
		    _g = g;
	    }

        /// <summary>Write a single <see cref="Quad"/> to output stream</summary>
        public async Task PrintQuad(Quad quad, TextWriter wr, 
	        CancellationToken cancel = default)
        {
            cancel.ThrowIfCancellationRequested();

            if(!IsPrintableValue(quad.Object))return;

	        var txt = $"{PrintLinkNode(quad.Subject)} " +
	                  $"{PrintLinkNode(quad.Predicate)} " +
	                  $"{PrintValueNode(quad.Object)} .";

            cancel.ThrowIfCancellationRequested();
            await wr.WriteLineAsync(txt);
        }

        /// <summary>
        /// Print all triples to the <paramref name="wr"/>.
        /// </summary>
        public void Print(TextWriter wr)
        {
            foreach (var quad in _g.Query)
            {
                if(!IsPrintableValue(quad.Object))continue;

                var line = 
                    $"{PrintLinkNode(quad.Subject)} " +
                    $"{PrintLinkNode(quad.Predicate)} " +
                    $"{PrintValueNode(quad.Object)} .";

                wr.WriteLine(line);
            }
        }

        private bool IsPrintableValue(Node node)
        {
            return !(node.ValueString is null) &&
                !ObjectNodeMap.IsObjectType(node.ValueType.DataType);
        }

        private string PrintLinkNode(Node<UriOrBlank> node)
        {
            if (node is BlankNode b)
                return b.Scope == _g.Id ? 
	                "_:" + b.Label :
	                throw new BlankNodeScopedToInvalidGraph();

            if (node is UriNode u)
	            return $"<{ValueEncoder.UriEscape(u.Uri)}>";

            throw new Exception($"Unknown {nameof(UriOrBlank)} node");
        }

        private string PrintValueNode(Node node)
        {
            if (node is Node<UriOrBlank> refNode)
                return PrintLinkNode(refNode);

            if (node is Node<Text> textNode)
            {
                var val = textNode.Value;
                return $"\"{ValueEncoder.LiteralEscape(val.Value)}\"" +
                       $"@{val.Language}";
            }

            //only write a node with 
            var dataType = node.ValueType.DataType == XMLSchema.String?
                "" : "^^<" + ValueEncoder.UriEscape(node.ValueType.DataType)+ ">";

            if(node.ValueString is null)
                throw new Exception("cannot print node without value string");

            return $"\"{ValueEncoder.LiteralEscape(node.ValueString)}\"{dataType}";
        }
    }
}