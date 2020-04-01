# Overview
A Graph is a subset of all the quads in
an *[[IRdfData|RdfData]]* collection.

The graph is described by a _main_ graphId,
and an optional set of _other_ graphId's.

All quads that belong to any of these graphId's make up the set inside the Graph.

## Full graph.
A full graph is one with all the other graph data included.

Note: this is a snap-shot of the current graphs. If another 
graph is added to the [[RdfData|RdfData]] it will not be reflected in 
and earlier created graph.

```cs
//Create new Full graph
var rdf = RdfDataFactory.CreateNoLock();

//Full graph with new labeled blank node
var myGraph = rdf.GetBlankGraph("test");

//A new graph; after I got my first graph.
var subject = rdf.Uri("test:sub");
rdf.Assert(rdf.Uri("test:g1"), 
    subject, rdf.Uri("test:int"), rdf.New(123));

var quadFromOther = myGraph.Query
    .FirstOrNull(x => x.Subject == subject);

Assert.IsNull(quadFromOther);
```

## Self-only Graph
In general a Graph represents the union of quads with different graphId's.

Use Graph.SelfOnly() to get a graph with only its own Quads.

```cs
//Convert from mixed to self only graph.
var rdf = RdfDataFactory.CreateNoLock();

//Mixed graph has main and some other graphs.
var myGraph = rdf.GetGraph(
    rdf.BlankGraph("MainGraph"),
    rdf.BlankGraph("Other1"),
    rdf.BlankGraph("Other2"));

//Self only graph.
myGraph = myGraph.SelfOnly();

Assert.IsFalse(myGraph.Other.Any());

//Or directly get self-only from rdf.
myGraph = rdf.GetSelfGraph(rdf.BlankGraph("MainGraph"));
Assert.IsFalse(myGraph.Other.Any());
```

## Simplified Updates
Updates to a graph are converted to Quads 
with the _main_ Id. A Graph only updates data 
for one graphId.

This makes it simpler to Assert Quads with a graph; 
one less Node to specify.

```cs
//Assert some data for a graph.
var rdf = RdfDataFactory.CreateNoLock();

//My fancy graph.
var g = rdf.GetFullGraph("app:test");

//The inbuilt system graph
var sys = rdf.GetSystem();

//Assert some data.
sys.Assert(g.Id, sys.Uri(RDF.A), sys.Uri("app:graphId"));
```

## Graph data is ordered.
When createing a Graph, the order of graphId's is important.
The provided graphId order is preserved. 
Queries can use this to thier advantage.

The graph represents a snap-shot of other graph data, and 
a live-query on its main graphId. This gives graph an isolated
data-set to query in.

```cs
//Graph data ordered.
var rdf = RdfDataFactory.CreateNoLock();

var g1 = rdf.GetBlankGraph("g1");
var g2 = rdf.GetBlankGraph("g2");

var s = rdf.Uri("app:test");
var p = rdf.Uri("app:count");

g1.Assert(s, p, g1.New(1));
g2.Assert(s, p, g2.New(2));

//Get Full data
g2 = rdf.GetFullGraph(g2.Id);

var theCount = g2.Query
    .For(s).With(p)
    .Get<int>().First();

Assert.AreEqual(2, theCount);
```

## Current graph data for Queries.

When created the Graph takes a snap-shot of 
the current data for all the _other_ graphId's.

But, the return from *Graph.Query* has an 
up-to-date snapshot of the _main_ graph.

So the graph is isolated from changes to _other_ graphs.
But you can query the latest from the _main_ graph.

```cs
var rdf = RdfDataFactory.CreateDefault();

//Get 2 graphs.
var g1 = rdf.GetFullGraph(rdf.BlankGraph("graph1"));
var g2 = rdf.GetFullGraph(rdf.BlankGraph("graph2"));

//update g2.
g2.Assert(g2.Uri("app:me"), g2.Uri("app:update"), g2.New(true));

//graph1 isolated from change in graph2
Assert.AreEqual(0, g1.Query.Count,
    "Update in graph2 not reflected in graph1 snapshot");

//graph2 has the update ready to use.
Assert.AreEqual(1, g2.Query.Count, 
    "Update in graph 2 is available");
```