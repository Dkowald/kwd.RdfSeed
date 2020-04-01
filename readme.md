# Overview
An RDF library for .NET

This is early code. 
I am still experimenting with the API to 
balance readability with performance.
Feel free to use as you like, but expect breaking changes in the 
future.

### Features

* __Efficient memory__ Nodes from the same NodeFactory are re-used to reduce memory allocations.

* __Efficient query__ Because nodes are re-used; get fast node equality via ReferenceEquals.

* __Typed nodes__ RDF Nodes modeled using .NET types.

* __N-Tripple Files__ Inbuilt Read / Write data using [n-triples](https://www.w3.org/TR/n-triples/) format.

* __LINQ Friendly__ Leverage LINQ for Query support.

* __Fluent Update__ Includes a fluent interface for building quads via code.

### Quick start.
```cs
private readonly FileInfo _sampleData = Files.TestData.Brazil;
private const string SubjectId = "http://www.heml.org/docs/samples/heml/2002-05-29/brazil.xmlFRAGbrazil";

//Create a store
var rdf = RdfDataFactory.CreateDefault();

//new graph node.
var sample = rdf.GetBlankGraph("SampleDataGraph");

//load some data.
new NTripleFile(_sampleData).Read(sample).Wait();

//find some data.
var brazil = rdf.Uri(SubjectId);
var name = sample.Query
    .For(brazil)
    .With(rdf, RDFS.Label)
    .SelectValues<string>()
    .FirstOrNull();

//Add some data.
var haveVisited = rdf.Uri("app:haveVisited");
sample.Update
    .For(brazil)
	.With(haveVisited)
	.Assert(false);
```

See github wiki for more detail.
