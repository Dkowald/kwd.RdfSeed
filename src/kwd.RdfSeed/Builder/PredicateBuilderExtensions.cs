
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

        /// <summary>Assert quad with value.</summary>
        /// <exception cref="TypeMustNotBeANode"></exception>
        public static PredicateBuilder Assert<T>(this PredicateBuilder self, T value)
	        => self.Assert(value, out _);

        /// <summary>Assert quad with value.</summary>
        /// <exception cref="TypeMustNotBeANode"></exception>
        public static PredicateBuilder Assert<T>(this PredicateBuilder self, T value, out Node<T> node)
        where T:notnull
        {
	        node = self.Rdf().New(value);
            return self.Add(node);
        }

        /// <summary>
        /// Assert a value, with a handy Let to preserve the object.
        /// </summary>
        public static PredicateBuilder AssertLink(this PredicateBuilder self, 
	        Node<UriOrBlank> value, out Node<UriOrBlank> obj)
        {
            obj = value;
	        return self.Add(value);
        }

        /// <summary>Assert a text value language.</summary>
        public static PredicateBuilder Assert(this PredicateBuilder self, string text, string lang, out Node<Text> node)
        {
            node = self.Rdf().Text(text, lang);
	        return self.Add(node);
        }

        /// <summary>Assert a text value language.</summary>
        public static PredicateBuilder Assert(this PredicateBuilder self, string text, string lang)
	        => self.Assert(text, lang, out _);

        /// <summary>
        /// Retract any value of type <typeparamref name="T"/>.
        /// </summary>
        /// <exception cref="TypeMustNotBeANode"></exception>
        public static PredicateBuilder Retract<T>(this PredicateBuilder self, T _)
	        where T : notnull => self.Remove<T>();
        
        /// <summary>
        /// Retract value type and then assert new value.
        /// </summary>
        /// <exception cref="TypeMustNotBeANode"></exception>
        public static PredicateBuilder Set<T>(this PredicateBuilder self, T value, out Node<T> node)
	        where T : notnull
	        => self.Remove<T>().Assert(value, out node);

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

        private static IRdfData Rdf(this PredicateBuilder self) =>
	        ((IQuadBuilderContext) self).Rdf;

        private static Node<UriOrBlank> GraphId(this PredicateBuilder self) =>
	        ((IQuadBuilderContext) self).GraphNode ??
	        throw new Exception($"Expected Graph id for {nameof(GraphBuilder)}.");
    }
}