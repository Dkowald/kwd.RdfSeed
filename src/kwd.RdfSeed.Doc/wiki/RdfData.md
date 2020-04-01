# Overview
The RdfData acts as the single point collection for 
Quads in a process.

In generally, there should be one instance for a process.

The RdfData uses a [[NodeFactory|NodeFactory]] for all nodes,
and maintains the set of Quads.

## Helper factory

The **RdfDataFactory** utility provides simple 
quick creation for an *IRdfData* object.

```cs
//create default thread-locked version.
IRdfData rdf = RdfDataFactory.CreateDefault();
Assert.IsTrue(rdf is RdfDataThreadLock);

//create basic version.
rdf = RdfDataFactory.CreateNoLock();
Assert.IsTrue(rdf is RdfData);

//setup app singleton
RdfDataFactory.Init(RdfDataFactory.CreateNoLock());

//use singleton
rdf = RdfDataFactory.AppData;

Assert.IsNotNull(rdf);
```

Though it has a Singleton; Using IRdfData via IoC is preferred.

These helper include all the pre-built NodeMappings, see [[NodeFactory|NodeFactory]].

## GeneralUsage

**Create nodes**
*IRdfData* provides a node creation interface, internally forwarding
calls to the contained [[NodeFactory|NodeFactory]].
```cs
//Self scoped blank node.
var graph = rdf.BlankSelf("self-blank");

//Blank node scoped to graph
var subject = rdf.Blank(graph);

//uri predicate, from Dublin core
var predicate = rdf.Uri(Terms.Title);

var @object = rdf.New("a title from my stuff");
```

**Assert / Query / Retract / Replace**
*IRdfData* provide quad-level add / remove / find
type capability

IRdfData.Query returns a snap-shot of the current quads. 
This is a IReadOnlyList<Quad>, so LINQ can be used to query the data.
A number of [[query extensions|Query]] also help when working with quad collections.

```cs
//add quad to rdf data
rdf.Assert(graph, subject, predicate, @object);

//query
var quad = rdf.Query.Single();

//remove
rdf.Retract(quad);
```

## Other usage

**[[Graphs|Graphs]]**

*IRdfData* defines 2 inbuilt graphs:

1. System - Used to hold data about the RdfData itself.
2. Default - Used as the default graph.

It also has a list of the current defined graphs.

The Update property acts an an entry point for [[Fulent Updates|FluentUpdate]].


## Thread lock versions.

Both *RdfData* and [[NodeFactory|NodeFactory]] have proxy wrappers
that use lock statements to give thread safe access to the 
*IRDfData* and *INodeFactory* interfaces respectivly.

These proxy wrappers are used by the RdfDataFactory to create 
a thread-safe *IRdfData*.