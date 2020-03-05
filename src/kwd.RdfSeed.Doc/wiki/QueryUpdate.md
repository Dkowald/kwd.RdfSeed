
Query and Update assistance.

## Some terms
For both query and update the following terms are used.
These are to help code readabiltiy.

|Term | Description|
| --- | --- |
| From | Related to the Graph node in a quad |
| For | Related to the Subject node in a quad |
| With | Related to the Predicate node in a quad |
| IsType | Related to the Object; node value type |
| IsValue | Related to the Object; value |

## How to Query
Both RdfData and Graph provide a Query to get
a list of the current Quads.

The Query returns a snap-shot of the curernt data as a 
**IReadOnlyCollection<Quad>**

A set of extensions then help build a query in a LINQ like way.

e.g
```
//Select all linkedTo urls within graph g.
var myGraph = rdf.Uri("x:myGraph");
var linkedTo = rdf.Uri("a:linkedTo");

var linkedUris = rdf.Query
  .From(myGraph).With(linkedTo)
    .SelectUris()
    .ToList();
```

In more complex case, perhapse I want the quads for
links or names. The *With* can take a func to allow this.
```
//Select all links and names within graph g.
var myGraph = rdf.Uri("x:myGraph");
var linkedTo = rdf.Uri("a:linkedTo");
var name = rdf.Uri("a:name");

var linksAndNames = rdf.Query
  .From(myGraph)
  .With(p => p == linkedTo || p == name)
    .Select(x => x.Object)
    .ToList();

```

### node-converter calls
There are a number of extensions which allow use of 
native types for Queries.

These require a **INodeFactory** to first convert items to 
corresponding **Nodes**.

Conversions are expensive; suggestion is to prefer use of existing 
nodes where possible. 

```cs
//Select all linkedTo urls within graph g.
var linkedUris = rdf.Query
  .From(rdf, "x:myGraph").With(rdf, "a:linkedTo")
    .SelectUris()
    .ToList();
```

Note: These require conversion, so the data is 
acuratly presented as a Node. 
By going through the NodeFactory; the native value is normalized to
match existing node where applicable.

## How to Update

Both IRdfData and Graph implement interfaces for 
directly adding and removing Quads. 
But it can be cumbersome to use this interface.

The fluent api is useful for easier-to-read updates.
Each fluent object provides an interface that matches terms from the query helpers

| e.g The RdfBuilder has **From** to create a GraphBuidler.
| The GraphBuilder has With to select a PredicateBuilder.
| The PredicateBuilder has Assert and Retract to app / remove the quad.

Each Builder also has **Then** to go back to the previous.
| PredicateBuilder.Then returns to SubjectBuilder

Each Builder has a **Let** to extract the target node.
| SubjectBuilder.Let, stores the subject node in an out parameter.
This is mostly useful when building with blank nodes. 

```c#
var mainG = rdf.Uri("x:other");
var p = rdf.Uri("x:linkTo");

//link g to other graph.
rdf.Update.From(rdf.BlankGraph())
	.Let(out var demo)
	.For(demo).With(linkTo)
	.Assert(myGraph);
```

### node converter calls
The Builder objects all take *Nodes*, but there are 
a number of extensions which can create the node(s)
for simpler objects.

Extensions that convert from some data to a node,
also include an overload that allows you to capture the 
node via an out parameter.

Note: conversion comes at a cost; Its prefeable to use 
a node; to encourage this, converters retrun the created
node as a out parameter.

```cs
rdf.Update
  .From("g:aGraph", out var aGraph)
    .For("x:me", out var me)
    .With("a:name").Let(out var name)
  .Assert("Fred", out var myName);
```

**From**; **For** and **With** focefully require an out parameter
to store the result (making it more obvious this is prefered). 
If not required, just use C# ignore out syntax:
```cs
rdf.Update
   .From("a:graph, out _);
```

## A note on node converters.
Where a query or update converts from a native type to a node,
There is an explicit node created.

This ensures normalization provided by the NodeMap will be applied.
So