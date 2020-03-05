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
        ws,

        /// <summary>Uri token</summary>
        uri,

        /// <summary>String literal</summary>
        literal,

        /// <summary>a . the triple end character.</summary>
        dot,

        /// <summary>A comment line.</summary>
        comment,

        /// <summary>A blank label</summary>
        blank,

        /// <summary>data type uri</summary>
        dataType,

        /// <summary>text language specifier</summary>
        lang
    }
}