using kwd.RdfSeed.Core.Nodes.Builtin;
using kwd.RdfSeed.Errors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace kwd.RdfSeed.Tests.Core.Nodes.Builtin
{
	[TestClass]
	public class UriOrBlankTests
	{
		[TestMethod]
		public void Normalize()
		{
			var m = new UriOrBlankMap();

			var n1 = m.Create("x:n1");
			var n2 = m.Create(" x:n1 ");
			Assert.AreEqual(n1.ValueString, n2.ValueString, "trimmed");

			var b1 = m.NewSelfScoped(" a-label");
			var b2 = m.NewSelfScoped("a-label ");
			Assert.AreEqual(b1.ValueString, b2.ValueString, "trimmed");

		}

		[TestMethod]
		public void AbsoluteRestrictsSchema()
		{
			Assert.ThrowsException<InvalidUri>(() => new UriOrBlank("12:/x"));
		}

		[TestMethod]
		public void BlankLabelIsCleaned()
		{
			var b1 = new UriOrBlank(null, "label");
			var b2 = new UriOrBlank(null, "_:label");
			var b3 = new UriOrBlank(null, " _:label ");

			Assert.AreEqual("label", b1.Label);
			Assert.AreEqual("label", b2.Label);
			Assert.AreEqual("label", b3.Label);
		}
	}
}
