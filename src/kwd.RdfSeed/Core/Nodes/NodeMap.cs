using System;

namespace kwd.RdfSeed.Core.Nodes
{
    /// <summary>
    /// Mapping from data type string and object type to
    /// a <see cref="Node"/>.
    /// </summary>
    public abstract class NodeMap
    {
	    /// <summary>
        /// Create new with native type and dataType
        /// </summary>
        protected NodeMap(Type native, string dataType)
        {
            UriHelper.VerifyIsUri(dataType);
            Native = native;
            DataType = dataType;
            DataTypeHash = dataType.GetHashCode();
        }

        /// <summary>
        /// Native in-memory type for value.
        /// </summary>
        public readonly Type Native;

        /// <summary>
        /// Value type; like xsd:string 
        /// </summary>
        public readonly string DataType;

        /// <summary>Hash for <see cref="DataType"/>.</summary>
        public readonly int DataTypeHash;

        /// <summary>
        /// Create node from valueString
        /// </summary>
        public abstract Node Create(ReadOnlySpan<char> valueString);
    }

    /// <summary>
    /// Mapping from data type string and object of type <typeparamref name="T"/>
    /// to corresponding <see cref="Node{T}"/>.
    /// </summary>
    public abstract class NodeMap<T> : NodeMap where T:notnull
    {
        /// <summary>Create new <see cref="NodeMap{T}"/>.</summary>
        protected NodeMap(string dataType)
            :base(typeof(T), dataType){}

        /// <summary>
        /// Create a <see cref="Node{T}"/> from its value
        /// </summary>
        public abstract Node<T> Create(T value);
    }
}