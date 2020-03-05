using System;
using System.Collections.Generic;
using kwd.RdfSeed.Serialize.NTriple;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace kwd.RdfSeed.Tests.Serialize.NTriple
{
	[TestClass]
	public class NTripleTokenizerTests
	{
		[TestMethod]
		public void EscapeEscapeEndings()
		{
			var data = "\"" + "\\\\" + "\"";

			var token = NTripleTokenizer.NextToken(data);

			Assert.IsTrue(token.Value.SequenceEqual("\\\\"));
		}

		[TestMethod]
		public void ReadBlankDotAttached()
		{
			var data = "_:g.";

			var token = NTripleTokenizer.NextToken(data);

			Assert.AreEqual(NTripleTokenType.blank, token.Type);
			Assert.IsTrue(token.Value.SequenceEqual("g"));
			Assert.IsTrue(token.Rest.SequenceEqual("."));
		}

		[TestMethod]
		public void ReadEscapedLiteral()
		{
			var data = "\\\"";//== \"
			var expected = "\"";

			var literal = "\"" + data + "\"";

			var token = NTripleTokenizer.NextToken(literal).Value;

			Assert.AreEqual('\\', token[0]);
			Assert.AreEqual('"', token[1]);
			Assert.AreEqual(2, token.Length);

			var result = ValueEncoder.LiteralUnEscape(token);

			Assert.AreEqual(expected, result,
					"Preserve and skip escaped");
		}

		[TestMethod]
		public void ReadBlank()
		{
			var token = NTripleTokenizer.NextToken("_:blank ");
			Assert.IsTrue(token.Value.SequenceEqual("blank"));
			Assert.AreEqual(NTripleTokenType.blank, token.Type);
			Assert.IsTrue(token.Rest.SequenceEqual(" "));

			token = NTripleTokenizer.NextToken("_:blank");
			Assert.IsTrue(token.Value.SequenceEqual("blank"));
			Assert.IsTrue(token.Rest.SequenceEqual(""));
		}

		[TestMethod]
		public void ReadLineOfTokens()
		{
			var data = " <s1> <p1> \"Literal\"@en .".AsSpan();

			var tokens = new List<(string value, NTripleTokenType type)>();

			var expected = new List<(string value, NTripleTokenType type)>
						{ ("s1", NTripleTokenType.uri),
								("p1", NTripleTokenType.uri),
								("Literal", NTripleTokenType.literal),
								("en", NTripleTokenType.lang),
								(".", NTripleTokenType.dot)};

			var t = NTripleTokenizer.NextToken(data);
			while (t.Type != NTripleTokenType.Invalid)
			{
				if (t.Type != NTripleTokenType.ws)
					tokens.Add((new string(t.Value), t.Type));

				t = NTripleTokenizer.NextToken(t.Rest);
			}

			Assert.IsTrue(t.IsEnd);

			Assert.AreEqual(expected.Count, tokens.Count);
			CollectionAssert.AreEqual(expected, tokens);
		}

		[TestMethod]
		public void ReadIncomplete()
		{
			var data = "<uri:/notClosed";
			var token = NTripleTokenizer.NextToken(data);
			Assert.AreEqual(NTripleTokenType.Invalid, token.Type);
			Assert.IsTrue(token.Value.IsEmpty);
			Assert.AreEqual(data, new string(token.Rest));

			token = NTripleTokenizer.NextToken("\"non terminated \n line");
			Assert.AreEqual(NTripleTokenType.Invalid, token.Type);
		}
	}
}