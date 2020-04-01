# Overivew
A(nother) RDF data managment library for .NET

I've been interested in using RDF data on .NET Core. 
There are a couple of [[other libraries|Alternates]] 
but they didn't suite. So, heres another one! 

My focus is to have a core RDF library for in-proc data,
that is efficient, and readable.

# Main features

__Native typed nodes__
Nodes are presented using Native .NET types, rather than 
strings. This also allows for nodes of any .Net type.

__Efficient NodeFactory__
The *NodeFactory* minimising allocations by re-using nodes where possible.
This re-use means node equality reduced down to ReferenceEquality.

Flexable node creation via customizable NodeMap's allowing 
consumers to model nodes to suite.

__Simple entry point__

Top-level RdfData to coordinate all (in-memory) data via single
point. Starts with pre-defined System and Default graphs to bootstrap.

__Union style Graph__

Graphs presents Queries over multiple graph quads; whilst limiting Updates to a single graph.

__Fluent interface for Updates__

Improve code readability with a set of fluent
builders to (try) keep code readable. 

__Linq style Query__

Extension based queries to navigate the quads.
Flows similar to Linq code;
keeping a familure Linq query style.

__NTripple persistance__
Simple line-based single graph persistance for triples.

With builtin tracking data presented in System graph.
See [[Serialzation|Serialization]]
