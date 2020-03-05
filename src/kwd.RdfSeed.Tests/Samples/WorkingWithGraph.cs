using System.Linq;
using kwd.RdfSeed.Query;
using kwd.RdfSeed.Serialize.NTriple;
using kwd.RdfSeed.Tests.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace kwd.RdfSeed.Tests.Samples
{
	/// <summary>
	/// Sample working with the union graph
	/// </summary>
	[TestClass]
	public class WorkingWithGraph
	{
		private void LoadSettings(IRdfData rdf)
		{
			var baseSettings = rdf.GetBlankGraph("baseSettings");
			new NTripleFile(Files.TestData.Settings)
				.Read(baseSettings).Wait();

			var debugSettings = rdf.GetBlankGraph("debugSettings");
			new NTripleFile(Files.TestData.SettingsDebug)
				.Read(debugSettings).Wait();
		}

		[TestMethod]
		public void ReadDefaultValue()
		{
			var rdf = RdfDataFactory.CreateNoLock();
			LoadSettings(rdf);
			
			//schema
			var level = rdf.Uri("app:Log#Level");

			//create graph with baseSettings as main and other data.
			var g = rdf.GetBlankGraph("baseSettings");
			g.Query
				.For(rdf, "app:Logger#Microsoft").ToArray()
				.Value<string>(level, out var logLevel)
				.For();
			
			Assert.AreEqual("Warning", logLevel, "Default is warning level");
		}

		[TestMethod]
		public void ReadActualValue()
		{
			var rdf = RdfDataFactory.CreateNoLock();
			LoadSettings(rdf);

			//schema
			var level = rdf.Uri("app:Log#Level");

			//create graph with debugSettings and all.
			var g = rdf.GetBlankGraph("debugSettings");

			g.Query
				.For(rdf, "app:Logger#Microsoft")
				.ToArray()
				.Value<string>(level, out var logLevel);

			Assert.AreEqual("Information", logLevel,
				"Using debug setting");
		}

		[TestMethod]
		public void IsolatedFromOtherGraphs()
		{
			var rdf = RdfDataFactory.CreateDefault();
			
			//Add 2 graphs.
			rdf.Update.From(rdf.BlankGraph("g1"))
				.Let(out var g1)
				.Then()
				.From(rdf.BlankGraph("g2")).Let(out var g2);
		}
	}
}