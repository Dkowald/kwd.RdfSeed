# Overview
A Graph is a subset of all the quads in
an *[[RdfData|RdfData]]* collection.

The graph is described by a _main_ graphId,
and an optional set of _other_ graphId's.

All quads that belong to any of these graphId's 
make up the set inside the Graph

## Self-only Graph
In general a Graph represents the union 
of quads with different graphId's

Use Graph.SelfOnly() to get a graph with only its own Quads.

## Full graph.
A full graph is one with all the other graph data included.

Unless otherwise specified [RdfData] returns full graphs.

## Simplified Updates
Updates to a graph are converted to Quads 
with the _main_ Id. A Graph only updates data 
for one graphId.

This makes it simpler to Assert Quads with a graph; 
one less Node to specify.

## Current graph data for Queries.

When created the Graph takes a snap-shot of 
the current data for all the _other_ graphId's.

But, the return from *Graph.Query* has an 
up-to-date snapshot of the _main_ graph.

So the graph is isolated from changes to _other_ graphs.
But you can query the latest from the _main_ graph.
