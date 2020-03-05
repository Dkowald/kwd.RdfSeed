namespace kwd.Rdf.Std
{
    /// <summary>
    /// Classes and properties described via rdf
    /// </summary>
    /// <remarks>
    /// https://www.obitko.com/tutorials/ontologies-semantic-web/rdf-schema-rdfs.html
    /// </remarks>
    public partial class RDFS
    {
        /// <summary>
        /// Range; restrict to specific subject(s).
        /// </summary>
        public string ForSubjects => Range;

        /// <summary>
        /// Domain; restrict to certain object(s).
        /// </summary>
        public string ForObjects => Domain;

    }
}
