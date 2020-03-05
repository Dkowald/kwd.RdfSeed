# Overview
A RDF library for .NET

This is early code. 
I am still experimenting with the API to 
balance readability with performance.
Feel free to use as you like, but expect breaking changes in the 
future.
 
## Quick start.
```cs
//make a store
var rdf = Rdf
//Assert some quads.

```

## Features

__Efficient memory__
Nodes from the same **NodeFactory** are re-used to reduce memory allocations.

__Efficient query__
Because nodes are re-used; 
get fast node equality via ReferenceEquals.

__N-Tripple Files__
Inbuilt Read / Write data using 
[n-triples](https://www.w3.org/TR/n-triples/) format.

__LINQ Friendly__
Leverage LINQ for Query support.

__Fluent Update__
Includes a fluent interface for buildign quads via code.

__Typed nodes__
RDF Nodes modeled using .NET types. 
Including nodes for normal objects.

Provides a standard set of typed nodes; 
and support for adding custom typed nodes.

## Get started

Create a __RdfData__ to manage in-memory quads.

Use Fluent interface to add data.

Use NTripleFile to save / load data.

Use Linq to query data.



