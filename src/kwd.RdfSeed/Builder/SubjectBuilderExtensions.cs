using System;
using System.Linq;

using kwd.RdfSeed.Core.Nodes;
using kwd.RdfSeed.Core.Nodes.Builtin;
using kwd.RdfSeed.Errors;

namespace kwd.RdfSeed.Builder
{
	/// <summary>
	/// Extensions for <see cref="SubjectBuilder"/>.
	/// </summary>
	public static class SubjectBuilderExtensions
    {
	    /// <summary>Select predicate with Uri.</summary>
	    public static PredicateBuilder With(this SubjectBuilder self, ReadOnlySpan<char> predicate, out UriNode node)
	    {
            node = self.Rdf().Uri(predicate);
		    return self.With(node);
	    }

	    /// <summary>Select predicate with Uri.</summary>
	    public static PredicateBuilder With(this SubjectBuilder self, string predicate, out UriNode node)
	    {
			node = self.Rdf().Uri(predicate);
		    return self.With(node);
	    }

        /// <summary>Adds literal items as a RDFS list set (return last item)</summary>
        /// <returns><see cref="SubjectBuilder"/> for last list item</returns>
        public static SubjectBuilder List(this SubjectBuilder self, params string[] literals)
	        => self.List(literals.Select(x => self.Rdf().New(x))
		        .Cast<Node>().ToArray()
	        );

        /// <summary>Adds object items as a RDFS list set (return last item)</summary>
        /// <returns><see cref="SubjectBuilder"/> for last list item</returns>
        /// <exception cref="TypeMustNotBeANode"></exception>
        public static SubjectBuilder List<T>(this SubjectBuilder self, params T[] literals)where T:notnull
	        => self.List(literals.Select(x => self.Rdf().New(x))
		        .Cast<Node>().ToArray()
	        );

        private static IRdfData Rdf(this SubjectBuilder self) =>
	        ((IQuadBuilderContext) self).Rdf;
    }
}