using System.Linq;
using kwd.RdfSeed.Query;
using kwd.RdfSeed.Serialize.NTriple;
using kwd.RdfSeed.Tests.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace kwd.RdfSeed.Tests.Samples.General
{
	/// <summary>
	/// Sample using the ordering and isolation in graphs
	/// to provide Settings with overrides.
	/// </summary>
	[TestClass]
	public class SettingsDataOverride
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

			//create full graph with baseSettings as main.
			var g = rdf.GetBlankGraph("baseSettings");

			//log level for Microsoft; using base / default.
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

			//create full graph with debugSettings as main.
			var g = rdf.GetBlankGraph("debugSettings");

			//log level for Microsoft; using base / default.
			g.Query
				.For(rdf, "app:Logger#Microsoft")
				.ToArray()
				.Value<string>(level, out var logLevel);

			Assert.AreEqual("Information", logLevel,
				"Using debug setting");
		}
	}
}