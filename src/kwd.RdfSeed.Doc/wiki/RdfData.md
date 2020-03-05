# Overview
The RdfData acts as the single point collection for 
Quads in a process.

Generally; this should be a Singleton.

The RdfData uses a *NodeFactory* for all nodes,
and maintains the set of Quads.

Basic Query and Update is on the IRdfData interface.

## Thread lock versions.
Both *RdfData* and *NodeFactory* have proxy wrappers
that use lock statements to give thread safe access to the 
*IRDfData* and *INodeFactory* interfaces.

## Helper factory

The **RdfDataFactory** utility provides simple 
quick creation for an *RdfData* object.

It also has a simple Singleton, 
though IoC would be the recommended approach.
