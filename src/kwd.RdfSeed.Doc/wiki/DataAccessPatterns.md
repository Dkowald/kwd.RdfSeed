# Connected Object Pattern

A pattern for using RDF data in applications.

Consider using a connected-object pattern when interacting 
with RDF data. 

This is very similar to the Active-Record pattern, 
but using RDF rather than the RDBMS Table concept.

With this approach; build a set of ConnectedObjects that
encapsulate read and write to RDF, while presenting a more 
natural interface for consupmtion.

The objective is to encapsulate the (potentially complex)
mapping to and from Quads into a set of classes.

The ConnectedObject presents typed data over RDF.

ConectedObjects have static members for listing existing.

__Pro:__

Isolates the rest of application code from concerns
about navigating RDF Quads.

Supports implementing various strategies for read / write 
object state to a set of Quads.

__Con:__

ConnectedObejcts are tightly coupled to an RDF implementation.
Selecting a different RDF library could cause significant re-work.


# Serialized Obejct pattern with RDF

This pattern aligns with common approach of 
modeling objects that are ignorant of the persistance 
approach.

Pro:

Application code ignores persistance; that responsibility 
is seperated into other systems.

Code follows a more DDD appraoch, where the business logic is central,
and the persistance is ancillary.

Con:

With RDF; the code to serialize objects needs to 
apply various patterns. 
Reading can be challenging with different RDF linking appraoches.

Writing can be even more interesting with retracting existing and writing new.