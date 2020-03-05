using kwd.RdfSeed.Serialize.NTriple;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace kwd.RdfSeed.Tests.Serialize.NTriple
{
    [TestClass]
    public class ValueEncoderTests
    {
        [TestMethod]
        public void RoundTripSpecialChars()
        {
            var dQuote = ValueEncoder.LiteralEscape("\"");

            Assert.AreEqual("\"", 
                ValueEncoder.LiteralUnEscape(dQuote));
        }
        
        [TestMethod]
        public void HexChars()
        {
            var bang = ValueEncoder.LiteralUnEscape("\u0021");
            Assert.AreEqual("!", bang);
        }
    }
}