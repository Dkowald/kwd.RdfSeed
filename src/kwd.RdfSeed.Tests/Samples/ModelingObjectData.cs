using System;
using System.IO;
using System.Linq;

using kwd.CoreUtil.FileSystem;
using kwd.Rdf.Std;
using kwd.Rdf.Std.DublinCore;
using kwd.RdfSeed.Builder;
using kwd.RdfSeed.Query;
using kwd.RdfSeed.RdfModel;
using kwd.RdfSeed.Serialize.NTriple;
using kwd.RdfSeed.Tests.TestHelpers;
using kwd.RdfSeed.TypedNodes;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace kwd.RdfSeed.Tests.Samples
{
	/// <summary>
	/// A series of samples building a more complex object.
	/// </summary>
	[TestClass]
    public class ModelingObjectData
    {
	    private readonly FileInfo _data =
		    Files.AppDataDir.GetFile(
			    nameof(ModelingObjectData), "Profile.nt");

	    private readonly string _myProfile = "app:/profile#me";

	    [TestMethod]
	    public void Run()
	    {
			NewBlankProfile();

			AddBasicProfile();
			AddFavorites();

			GetBasicProfile();
			GetMyFavorites();
	    }

	    private void NewBlankProfile()
	    {
		    var rdf = RdfDataFactory.CreateNoLock();
		    _data.EnsureDelete();

		    var g = rdf.GetFullGraph(_myProfile);
		    g.Update
			    .For(g.Uri(_myProfile))
			    .With(g.Uri(RDF.A))
			    .Add(g.Uri("app:/profile"));

		    new NTripleFile(_data).Write(g).Wait();
	    }

	    private void AddBasicProfile()
	    {
		    var rdf = RdfDataFactory.CreateNoLock();
            if(!_data.Exists())throw new Exception("Call start first");

            var g = rdf.GetFullGraph("app:/profile#me");
            var file = new NTripleFile(_data);
		    file.Read(g).Wait();

            g.Update.For(g.Uri(_myProfile))
	            .With(RDFS.Label, out _).Assert("My Name")
	            .Then()
	            .With(Terms.DateModified, out _).Assert(DateTime.UtcNow)
	            .Then()
	            .With(Terms.Description, out _).Assert("My basic profile data")
	            .Then()
	            .With(Terms.AlternativeTitle, out _).Assert("ME");

            file.Write(g).Wait();
	    }

        private void AddFavorites()
	    {
		    var rdf = RdfDataFactory.CreateNoLock();

		    var g = rdf.GetFullGraph(_myProfile);
            var file = new NTripleFile(_data);
            file.Read(g).Wait();

            //some favorites.
            g.Update
	            .ForBlank("favs", out var favs)
	            .With(RDF.A, out _).Assert(RDFS.List, out _)
	            .Then()
	            .List(g.Uri("https://duckduckgo.com/"),
		            g.Uri("https://inrupt.com/"),
		            g.Uri("https://github.com/"),
		            g.Uri("https://portal.azure.com/"))
	            .Then()
	            .For(g.Uri(_myProfile))
	            .With(g.Uri("app:favorites")).Add(favs)
	            //and I want to update the last modified.
	            .Then()
	            .With(Terms.DateModified, out _)
	            .Set(DateTime.UtcNow);

            file.Write(g).Wait();
	    }

	    private void GetBasicProfile()
	    {
		    var rdf = RdfDataFactory.CreateNoLock();

		    new NTripleFile(_data).Read(rdf.GetFullGraph(_myProfile)).Wait();

		    var profile = rdf.Query.For(rdf.Uri(_myProfile));

		    profile
				.Value<DateTime>(rdf.Uri(Terms.DateModified), out var lastModified)
			    .Value<string>(rdf.Uri(RDFS.Label), out var name)
			    //The possible null alternate name
			    .ValueOptional<string>(rdf.Uri(Terms.AlternativeTitle), out var altName);

		    var bestName = altName is null ? name : altName.Value;

			var welcome = 
				$"Hello {bestName}, " +
			    $"your profile was last updated {lastModified.ToShortDateString()}";

			Assert.IsTrue(welcome.Contains("ME"), "Has my alt name");
	    }

	    private void GetMyFavorites()
	    {
		    var rdf = RdfDataFactory.CreateNoLock();

		    var g = rdf.GetFullGraph(_myProfile);

			new NTripleFile(_data)
				.Read(g)
				.Wait();

			//the list of favs.
			var favsList = g.Query
				.For(g.Id)
				.With(g.Uri("app:favorites"))
				.SelectBlanks()
				.First();
			
			//get list items.
			var myFavorites = new RdfsList(rdf, g)
				.GetList(favsList).ToArray();
			
			Assert.IsTrue(myFavorites.Length > 0, "Got some favorites");
	    }

        [TestMethod]
        public void WorkingWithComplexList()
        {
	        var rdf = RdfDataFactory.CreateNoLock();

            var g = rdf.GetGraph(rdf.BlankGraph("_:"));

            var rdfs = new RdfsList(rdf, g);

			//Sparse list with different types of data.
            var root = rdfs.AddList(g.Blank(),
	            g.Literal("a"), null, g.New(34.6));

            var data = rdfs.GetList(root);

            var nil = rdf.Uri(RDFS.Nil);
            Assert.AreEqual(nil, data.ElementAt(1), "Null modeled as nil");
        }
    }
}