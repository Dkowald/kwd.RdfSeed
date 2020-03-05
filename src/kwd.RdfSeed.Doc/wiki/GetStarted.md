# Getting started

The Samples folder in the Test project contains various 
examples.

The **QuickStart** sample gives a brief overview of how to use the library.

```cs
//Create a store
var rdf = RdfDataFactory.CreateDefault();

//load some data.
//  new graph node.
var sample = rdf.GetBlankGraph("SampleDataGraph");
//  n-triple file read / write.
new NTripleFile(SampleData).Read(sample).Wait();

//find some data.
var brazil = rdf.Uri("http://www.heml.org/docs/samples/heml/2002-05-29/brazil.xmlFRAGbrazil");
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

//Save some data
new NTripleFile(Files.AppDataDir.GetFile("Updated.nt"))
    .Write(sample).Wait();
```