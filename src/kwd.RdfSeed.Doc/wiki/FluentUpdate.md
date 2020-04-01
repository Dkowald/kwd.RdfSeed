
[[Query|Query]] and Update assistance.

## How to Update

Both IRdfData and Graph implement interfaces for 
directly adding and removing Quads. But it can be cumbersome to use this interface.

So, both also provide a Update property for a fluent-update approach.

```cs
//Assert vs Fluent
var rdf = RdfDataFactory.CreateDefault();

var myGraph = rdf.Uri("g:other");
var linkTo = rdf.Uri("a:linkTo");

//link to other graph: using assert
var g = rdf.BlankGraph();
rdf.Assert(g, g, linkTo, myGraph);

//link to other graph: using fluent
rdf.Update
    .From(rdf.BlankGraph())//a graph
    .Let(out var demo)//inline graph.
    .For(demo)//subject is demo
    .With(linkTo)//predicate is linkTo
    .Add(myGraph);//Assert 
```

The Fluent-update interface is built to provide a similar feel as the 
[[Query extensions|Query]]. So Update and Query look similar.


### node converter calls
The Builder objects all take *Nodes*, but there are 
a number of extensions which can create the node(s) for simpler objects.
These inline-extensions create nodes and export then via out parameter.

Note: conversion comes at a cost; Its prefeable to use 
a node; to encourage this, converters retrun the created
node as a out parameter.

```cs
//Fluent builder with node creation
var rdf = RdfDataFactory.CreateNoLock();

rdf.Update
    .From("g:aGraph", out var aGraph)
    .For("x:me", out var me)
    .With("a:name", out var name)
    .Add("Fred", out var myName)
    .Then()
    //ignore created node
    .With("a:age", out _)
    .Add(123, out _);
```