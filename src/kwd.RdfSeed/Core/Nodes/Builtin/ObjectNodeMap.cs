using System;

namespace kwd.RdfSeed.Core.Nodes.Builtin
{
    /// <summary>
    /// Describe the ValueType string to be used for
    /// Object based nodes.
    /// </summary>
	public static class ObjectNodeMap
	{
		/// <summary>
		/// Namespace prefix for object type nodes,
		/// <see cref="ObjectNodeMap{T}"/>
		/// </summary>
		public const string ObjectNamespace = "object:/";

        /// <summary>
        /// The TypeValue string unique to this <typeparamref name="T"/>.
        /// </summary>
		public static string GetTypeString<T>()
			=> ObjectNamespace + typeof(T).FullName;

        /// <summary>
        /// True if provided <paramref name="valueType"/> is for Object
        /// </summary>
		public static bool IsObjectType(string valueType)
			=> valueType.StartsWith(ObjectNamespace);

        /// <summary>
        /// True if provided <paramref name="valueType"/> is for Object
        /// </summary>
        public static bool IsObjectType(ReadOnlySpan<char> valueType)
			=> valueType.StartsWith(ObjectNamespace);
	}

    /// <summary>
    /// Generic catch-all mapping from object type <typeparamref name="T"/>
    /// to <see cref="Node{T}"/>.
    /// Type string generated from
    /// <see cref="ObjectNodeMap.ObjectNamespace"/> +
    /// typeof(<typeparamref name="T"/>) .
    /// </summary>
    public class ObjectNodeMap<T> : NodeMap<T> where T:notnull
    {
	    /// <summary>Create new <see cref="ObjectNodeMap{T}"/></summary>
        public ObjectNodeMap()
            :base(ObjectNodeMap.GetTypeString<T>()){}

        /// <inheritdoc />
        public override Node Create(ReadOnlySpan<char> valueString)
            => throw new Exception("Cannot create from string");

        /// <inheritdoc />
        public override Node<T> Create(T value)
            => new Node<T>(this, value);
    }
}