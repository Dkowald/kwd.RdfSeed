using System.Collections.Generic;

using kwd.RdfSeed.Core;

namespace kwd.RdfSeed.Compare
{
	/// <summary>
	/// Compare 2 <see cref="Quad"/>,
	/// match by their <seealso cref="Quad.Subject"/>.
	/// </summary>
	public class SameSubject : IEqualityComparer<Quad>
	{
		/// <inheritdoc />
		public bool Equals(Quad? x, Quad? y)
			=> x?.Subject == y?.Subject;

		/// <inheritdoc />
		public int GetHashCode(Quad obj)
			=> obj.Subject.GetHashCode();
	}
}