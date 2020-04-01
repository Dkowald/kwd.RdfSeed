
Query and [[Update|FluentUpdate]] assistance.

## How to Query
Both [[RdfData|RdfData]] and [[Graph|Graph]] provide a **Query** property to get
a list of the current Quads.

The Query returns a snap-shot of the current data as a 
**IReadOnlyCollection<Quad>**

A set of extensions then help build a query in a LINQ like way.

**QuadQueryExtensions**
This set of extensions provide an interface that mirrors the [[fluent update|FluentUpdate]] interface.

>Use **From** to select quads via their Graph.
>Use **For** to select quads via their Subject.
>Use **With** to select quads via their Predicate.

Then to extract the object value(s).
>Use Get to reduce to a set of values.

Get has overloads to use an out parameter to report results; and 
to retrieve the values from the query.


```cs
var rdf = RdfDataFactory.CreateDefault();

var g = rdf.BlankGraph();
var subject = rdf.Blank(g, "test");
var predicate = rdf.Uri(RDF.A);
var value = rdf.Uri("test:query#basic");

//add some data
rdf.Update
  .From(g)
  .For(subject)
  .With(predicate)
  .Add(value);

//query some data.
var qry = rdf.Query;
var uriValue = qry
  .From(g)
  .For(subject)
  .With(predicate)
  .GetUri(out var result)//inline get with result
  .GetUri();//get with select.

Assert.AreEqual(value.Uri, result.Single(), "from inline select");
Assert.AreEqual(value.Uri, uriValue.Single(), "from query select");
```

In more complex case, perhapse I want the quads for
links or names. The *With* can take a func to allow this.
```cs
var rdf = RdfDataFactory.CreateDefault();
var myGraph = rdf.Uri("x:myGraph");

LoadSampleData(rdf, myGraph);

var linkedTo = rdf.Uri("a:linkTo");
var name = rdf.Uri("a:name");

//Select all links and names within graph g.
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
//Read all linkTo URI's.
var linkedUris = rdf.Query
  .From(g)
  .For(rdf, "x:me")
  .With(rdf, "a:linkTo")
  .GetUri()
  .ToList();
```

Note: These require conversion, so the data is 
acuratly presented as a Node. 
By going through the NodeFactory; the native value is normalized to
match existing node where applicable.

