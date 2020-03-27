namespace kwd.RdfSeed.Serialize.NTriple
{
    /// <summary>
    /// The set of <see cref="NTripleTokenizer.Token"/> types.
    /// </summary>
    public enum NTripleTokenType
    {
        /// <summary>A token could not be extracted.</summary>
        Invalid,

        /// <summary>Whitespace</summary>
        Whitespace,

        /// <summary>Uri token</summary>
        Uri,

        /// <summary>String literal</summary>
        Literal,

        /// <summary>a . the triple end character.</summary>
        Dot,

        /// <summary>A comment line.</summary>
        Comment,

        /// <summary>A blank label</summary>
        Blank,

        /// <summary>data type uri</summary>
        DataType,

        /// <summary>text language specifier</summary>
        Language
    }
}