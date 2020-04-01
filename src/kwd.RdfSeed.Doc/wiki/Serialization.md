
# NTripple files
RdfSeed includes Read / Write for 
NTriple(https://www.w3.org/TR/n-triples/) files.

At this stage the focus is on an efficient in-memory
Rdf data store, other serializations may come later.

An NTripple file loads triple data into a specific Graph
```cs
//Load a sample N-triple file
var rdf = RdfDataFactory.CreateNoLock();

var file = SampleFile();

var g = rdf.GetBlankGraph();

//Read file data into the graph g
new NTripleFile(file).Read(g).Wait();

var tripleCount = g.Query.Count;
Assert.IsTrue(tripleCount > 0, "Loaded some data");
```

Similarly, can save a graph to a NTripple file
```cs
//Write a sample N-triple file
var file = Files.AppDataDir.GetFile(
    nameof(UsingNTripleFile), nameof(WriteAFile) + ".nt");

file.EnsureDelete();

var rdf = RdfDataFactory.CreateNoLock();
var g = rdf.GetBlankGraph();

g.Assert(g.Uri("test:sub"), g.Uri("test:pred"), g.Literal("a name"));

new NTripleFile(file).Write(g).Wait();

Assert.IsTrue(file.Exists() && file.Length > 0, 
    "File created with some data");
```
