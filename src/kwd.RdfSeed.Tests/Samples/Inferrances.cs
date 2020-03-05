using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace kwd.RdfSeed.Tests.Samples
{
    /// <summary>
    /// A look at possible interface
    /// to add inference quads.
    /// </summary>
    [TestClass]
    public class Inference
    {
        [TestMethod]
	    public void IsAInfer()
	    {
		    Assert.Inconclusive("under consideration");
            
		    //Model infer as a quad factory inside a graph
            // object which serves up quads from 
            // some the graphs own data.

            //Add a standard RDF IsA relationship.
            // use an inference to locate all items of a 
            // base type.
	    }

        [TestMethod]
        public void ConvertInfer()
        {
            Assert.Inconclusive("under consideration");
            //a triple with type FileInfo.
            //Add an inference to convert to string.
            //use the inference to read quad with 
            // string version
            
            // the infer is only valid while original triple exists.
            //it should be in its own graph.

            //should be able to promote it to original graph.
            
            //e.g infer 
        }

        [TestMethod]
        public void InferPromote()
        {
	        Assert.Inconclusive("under consideration");

            //Setup a is-a inference.

            //locate the inferred quad.
            //add (promote) it to the graph.

            //re-run inference.
            //should no-longer have the inferred quad,
            // since it is now an actual.
        }

        [TestMethod]
        public void LinkInfer()
        {
	        Assert.Inconclusive("under consideration");

            //Create a more complex inference.

            //Say have a subject s1 that has 
            // some text note t1.

            //Scan the t1 note, looking for words
            // where the subject includes the
            // word as a key word.

            //Then infer that s1 seeAlso s2.
        }

        [TestMethod]
        public void InferWithInfer()
        {
	        Assert.Inconclusive("under consideration");

            //If I have 2 inferences inf1, inf2.

            //If inf1 uses a quad from inf2.
            // And inf2 uses a quad from inf1.

            //This should be ok.
            //Don't want adding inferences to be order dependent.
            
            //But this then has a potential oscillator loop.
            // need some kind of damping.
        }
    }
}