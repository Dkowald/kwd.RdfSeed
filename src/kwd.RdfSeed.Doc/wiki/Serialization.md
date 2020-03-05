
# NTripple files
RdfSeed includes Read / Write for 
NTriple(https://www.w3.org/TR/n-triples/) files.

At this stage the focus is on an efficient in-memory
Rdf data store, other serializations may come later.

The NTripple serializer can be used directly to 
load (and save) a Graph in .nt format.
```cs
var rdf = RdfDataFactory.CreateNoLock();

var file = SampleFile();

var ntFile = new NTripleFile(file);
var g = rdf.GetGraph(rdf.Uri(file.AsUri()));

//Load some data
ntFile.Read(g).Wait();

//Save some data
ntFile.Write(g).Wait();
```