using System;
using System.IO;
using kwd.RdfSeed.Core.Nodes;

namespace kwd.RdfSeed.Tests.Samples.Rdf
{
    /// <summary>
    /// A custom node type for File info.
    /// </summary>
    public class FileInfoNodeMap : NodeMap<FileInfo>
    {
        /// <summary>A unique Value type string for nodes</summary>
        public const string TypeString = "app:FileInfoNode";

        /// <summary>
        /// Helper to create ValueString from <see cref="FileInfo"/>
        /// </summary>
        public static string ValueStringFor(FileInfo file)
            => file.FullName.ToLower();

        /// <summary>Create file info node.</summary>
        public FileInfoNodeMap() : base(TypeString){}

        /// <summary>Node from value string</summary>
        public override Node Create(ReadOnlySpan<char> valueString)
            => Create(new FileInfo(new string(valueString)));

        /// <summary>Node from object</summary>
        public override Node<FileInfo> Create(FileInfo value)
            => new Node<FileInfo>(this, value, ValueStringFor(value));
    }
}