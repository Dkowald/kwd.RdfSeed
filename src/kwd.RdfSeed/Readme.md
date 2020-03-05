# Overview
An RDF library using .NET core Span to improve performance.

## Api

 AppData : A top-level singleton to provide RDF data to a process.

 RdfData : Coordinates a NodeFactory and Quad collection(s) to 
  	    represent RDF data.

 Rdfs : helper to work with common RDF data forms.

## Core
 The core quads library.

 NodeFactory manages a set of nodes to minimise allocations.

 NQuad The basic relationship describes as subject / predicate / object / graph.

 NQuadCollection maintains a collection of quads.

 Graph maintains a collection of triples.

 ### Node Types
    The foundation set of nodes.

## Builder
 Interface to assist in adding data to a RdfData

## Compare
 Helpers to compare data sets.

## Shapes
 Support to map between .NET complext types and Rdf data.