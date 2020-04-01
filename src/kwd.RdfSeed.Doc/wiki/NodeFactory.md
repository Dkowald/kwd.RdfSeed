## Overview

One of the core features is the use of a NodeFactory
to maintian the in-memory set of nodes.

When requesting a [[Node]] the factory does a logical equivilance
to check if it already exists. If so, the current instance is returned.

This re-use of instances 
1. Greatly increases query speed, since object reference equality can be used.
2. Reduces memory use by reusing equivilent nodes.
3. Improves performance by reducing the number of node allocations.

## Types of nodes

All nodes have a non-null Native value.
All nodes have a **NodeMap** that describes their type.
All NodeMap's have a ValueType.

Nodes _may_ have a **ValueString** to represent their native value as string.

The factory has a small set of built-in nodes needed to make basic nodes.

### ObjectNodeMap
This builtin node is used as the fall-back to create a node from 
a Native object. 

These nodes are object-only; they have no ValueString.

The factory lets me create a node from any old object:
```cs
var f = new NodeFactory();

//A node from a native object.
var n1 = f.New(new FileInfo("c:/temp"));
Assert.IsNull(n1.ValueString, "ObjectNode has no value string");

var n2 = f.New(new FileInfo("c:/temp"));
Assert.IsFalse(ReferenceEquals(n1, n2), "Different instances are different nodes");

```

Super handy for creating Quads with interesting Object node(s). 
But not persistable.

### TypedLiteralMap
This builtin node is used as the fallback to create a node with 
both a value and type represented as strings.
Useful where there is no more natural mapping to a native type.
They fall back to use a simple string as the native type.

```cs
var f = new NodeFactory();
			
//Value is a string 
var n1 = (Node<string>) f.New("a:value", "x:type");
Assert.AreEqual("a:value", n1.Value);

//Same input, same node
var n2 = f.New("a:value", "x:type");
Assert.IsTrue(ReferenceEquals(n1, n2));

//Maps to inbuilt for Uri
var uri = (UriNode)f.New("a:uri", XMLSchema.AnyUri);
Assert.AreEqual("a:uri", uri.Uri);
```

### UriOrBlank Nodes
This builtin node is used to present a uri or blank node.

Blank nodes are special; since they scope to another node
(usually a graph).

Blanks and Uri nodes are grouped for the conveniance 
of having a BlankOrUri node
This makes it cleaner when working with Quad subject nodes, which can be either.

```cs
var f = new NodeFactory();

//a uri
var uri = f.Uri("a:uri");

//a blank
var blank = f.Blank(uri, "label");

//blank is scoped to uri
Assert.AreEqual(uri, blank.Scope);
```

### Pre built
There are also a set of pre-built Node types.
These can be used, or not, as desired.

|                       ValueType               |   Native type  |
| ----------------------------------------------|----------------|
| http://www.w3.org/2001/XMLSchema#boolean      | bool |
| http://www.w3.org/2001/XMLSchema#base64Binary | byte[] |
| http://www.w3.org/2001/XMLSchema#dateTime     | DateTime |
| http://www.w3.org/2001/XMLSchema#double       | double |
| http://www.w3.org/2001/XMLSchema#integer      | int |
| http://www.w3.org/2001/XMLSchema#string       | string |
| http://www.w3.org/1999/02/22-rdf-syntax-ns#langString | Text |

(see code for complete list)

When included, these pre-built node maps are used to map 
a ValueType to a Native type, and vice-versa.

Finally, you can extend by providing a custom **NodeMap**
to the **NodeFactory** on construction.

```cs
var f = new NodeFactory(TypedNodesHelpers.AllNodeMappings());

//create bool from .net type
var boolNode = f.New(true);
Assert.AreEqual(XMLSchema.Boolean, boolNode.ValueType.DataType);

//create from string data
var intNode = (Node<int>)f.New("001", XMLSchema.Integer);
Assert.AreEqual(1, intNode.Value);

//same int from .net type
var intNode2 = f.New(1);
Assert.IsTrue(ReferenceEquals(intNode, intNode2));
```

## Creating nodes.

Creating nodes uses a series of approaches to 
focus on re-use of existing nodes.

At its core the NodeFactory has 4 flows used for node creation.
Each tries to find an existing node; before allocating a new node.

1. Object flow; create using native object.
2. String flow; create from string data.
3. Uri flow: create a uri node from string data.
4. Blank flow: create a scoped blank node with label, or auto-label.

If the input is not sufficient for matching;
A matching **NodeMap** is found (or created),
then used to generate a temporary node: TempNode. 

With the TempNode, now try find existing and return it.

Failing that; the TempNode is added and returned.

With this approach; nodes that have enough data will match existing.

The exception is a Object node with no ValueString. 
In this case, only where the native type value is equal will the nodes match.

## How are 'nodes' equivilent

Node creation returns an existing node where possible.
Because of this equality can be reduced to Object.ReferenceEqual

## Custom NodeMap
The **NodeFactory** can be extended to include custom **NodeMap**.
Each Map must be unique for
1. The ValueType it specifies.
2. The NativeType it specifies.

A NodeMap should generate the same ValueString for all Value's that are logically the same.

For example, a FileInfoNodeMap 
```cs
/// <summary>A custom node type for File info.</summary>
public class FileInfoNodeMap : NodeMap<FileInfo>
{
    /// <summary>A unique Value type string for nodes</summary>
    public const string TypeString = "app:FileInfoNode";

    /// <summary>Helper to create ValueString from <see cref="FileInfo"/></summary>
    private static string ValueStringFor(FileInfo file)
        => file.FullName.ToLower();

    /// <summary>Create file info node.</summary>
    public FileInfoNodeMap() : base(TypeString){}

    /// <summary>Node from value string</summary>
    public override Node Create(ReadOnlySpan<char> valueString)
        => Create(new FileInfo(new string(valueString)));

    /// <summary>Node from object</summary>
    public override Node<FileInfo> Create(FileInfo value)
        => new Node<FileInfo>(this, value, ValueStringFor(value));
}
```

```cs
//node factory with custom mapping
var f = new NodeFactory(new FileInfoNodeMap());

var file = new FileInfo("c:/tmp/test.txt");
      
//map makes these logically the same node.
var n1 = f.New(file);
var n2 = f.New(file.FullName, FileInfoNodeMap.TypeString);
Assert.IsTrue(ReferenceEquals(n1, n2));

//map ignores file name case, so same node
var n3 = f.New("c:/TMP/Test.txt", FileInfoNodeMap.TypeString);
Assert.IsTrue(ReferenceEquals(n1, n3));
```

## Limitations

## Limited Object node equality.
Creating an object node using the builtin ObjectNode
will not have a ValueString. 
As a concequence; 2 object nodes will be considered logically different
if they are created with different instances of a Native object.

```cs
var f = new NodeFactory();

var file1 = new FileInfo("c:/tmp/test.txt");
var file2 = new FileInfo("C:/TMP/TEST.TXT");

//builtin object node; no mapping 
var n1 = f.New(file1);
var n2 = f.New(file2);
Assert.IsFalse(ReferenceEquals(n1, n2));
```

### Limited Native type mapping.
To create a node; first a matching **NodeMap** must be identified.
To do this, for native object node the type of the value is used.
So, the list of **NodeMap** is keyed on **NodeMap.Native**.
Concequently you cannot use multiple **NodeMap**'s for the same native type.
(Internally the NodeFactory breaks this rule for Builtin nodes)

A similar constraint exists for locating a **NodeMap** based on a 
type string.

### Limited lexical-space
The Factory converts well-known data types to equivilent values. 
Because of this string data that matches the same value are considered to be the same.

Another way to say this: 
the lexical-space for an rdf term is reduced to the value space.
This contradicts the [rdf spec](https://www.w3.org/TR/rdf11-concepts/#dfn-datatype-iri)

```cs
var f = new NodeFactory(new IntegerNodeMap());
			
//"001"^^xsd:int
var n1 = (Node<int>)f.New("001", XMLSchema.Integer);
//"1"^^xsd:int
var n2 = (Node<int>) f.New("1", XMLSchema.Integer);

//same logical string value, same node
Assert.IsTrue(ReferenceEquals(n1,n2));
```
