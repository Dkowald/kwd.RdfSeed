
using System;
using kwd.RdfSeed.Core.Nodes;
using kwd.RdfSeed.Core.Nodes.Builtin;
using kwd.RdfSeed.Errors;
using kwd.RdfSeed.TypedNodes;

namespace kwd.RdfSeed.Builder
{
    /// <summary>
    /// Extensions for <see cref="PredicateBuilder"/>.
    /// </summary>
    public static class PredicateBuilderExtensions
    {
        #region Add
        /// <summary>Assert quad with value.</summary>
        /// <exception cref="TypeMustNotBeANode"></exception>
        public static PredicateBuilder Add<T>(this PredicateBuilder self, T value)
	        => self.Add(value, out _);

        /// <summary>Assert quad with value.</summary>
        /// <exception cref="TypeMustNotBeANode"></exception>
        public static PredicateBuilder Add<T>(this PredicateBuilder self, T value, out Node<T> node)
        where T:notnull
        {
	        node = self.Rdf().New(value);
            return self.Add(node);
        }

        /// <summary>Assert a text value language.</summary>
        public static PredicateBuilder Add(this PredicateBuilder self, string text, string lang, out Node<Text> node)
        {
            node = self.Rdf().Text(text, lang);
	        return self.Add(node);
        }

        /// <summary>Assert a text value language.</summary>
        public static PredicateBuilder Add(this PredicateBuilder self, string text, string lang)
	        => self.Add(text, lang, out _);
        #endregion

        /// <summary>
        /// Retract any value of type <typeparamref name="T"/>.
        /// </summary>
        /// <exception cref="TypeMustNotBeANode"></exception>
        public static PredicateBuilder Remove<T>(this PredicateBuilder self, T _)
	        where T : notnull => self.Remove<T>();

        #region Set
        /// <summary>
        /// Retract value type and then assert new value.
        /// </summary>
        /// <exception cref="TypeMustNotBeANode"></exception>
        public static PredicateBuilder Set<T>(this PredicateBuilder self, T value, out Node<T> node)
	        where T : notnull
	        => self.Remove<T>().Add(value, out node);

        /// <summary>
        /// Retract value type and then assert new value.
        /// </summary>
        /// <exception cref="TypeMustNotBeANode"></exception>
        public static PredicateBuilder Set<T>(this PredicateBuilder self, T value)
	        => self.Set(value, out _);

        /// <summary>
        /// Retract value type and then assert new value.
        /// </summary>
        public static PredicateBuilder Set<T>(this PredicateBuilder self, Node<T> value)
	        where T:notnull
	        => self.Remove(value).Add(value);
        #endregion
        
        /// <summary>
        /// Assert links to subject2;
        /// returns build for provided <paramref name="subject2"/>.
        /// e.g Assert(<paramref name="subject2"/>).Then(<paramref name="subject2"/>)
        /// </summary>
        public static SubjectBuilder To(this PredicateBuilder self, Node<UriOrBlank> subject2)
	        => self.Add(subject2).ThenFor(subject2);

        /// <summary>
        /// Assert link, and return builder on that link.
        /// </summary>
        public static SubjectBuilder To(this PredicateBuilder self)
	        => self.To(self.Rdf().Blank(self.GraphId()));

        /// <summary>
        /// Assert a value, with a handy Let to preserve the object.
        /// </summary>
        public static PredicateBuilder AddLink(this PredicateBuilder self, 
	        Node<UriOrBlank> value, out Node<UriOrBlank> obj)
        {
	        obj = value;
	        return self.Add(value);
        }

        private static IRdfData Rdf(this PredicateBuilder self) =>
	        ((IQuadBuilderContext) self).Rdf;

        private static Node<UriOrBlank> GraphId(this PredicateBuilder self) =>
	        ((IQuadBuilderContext) self).GraphNode ??
	        throw new Exception($"Expected Graph id for {nameof(GraphBuilder)}.");
    }
}