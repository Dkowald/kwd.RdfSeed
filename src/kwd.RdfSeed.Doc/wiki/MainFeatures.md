# Overivew
A(nother) RDF data managment library for .NET

# Main features

__Native typed nodes__

Nodes are presented using Native .NET types, rather than 
strings. Also allows for nodes of any .Net type.

__Efficient NodeFactory__
An efficient node factory minimising allocations by 
re-using nodes where possible.
Strong re-use of node instances, means node equality reduced down to ReferenceEquality.

Flexable node creation via customizable NodeMap's allowing 
consumers to model nodes to suite.

__Simple entry point__

Top-level RdfData to coordinate all (in-memory) data via single
point. Starts with pre-defined System and Default graphs to bootstrap.

__Union style Graph__

Graphs presents Queries over multiple graph quads; whils limiting Updates to a single graph.

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
