using System.Collections.Generic;

using kwd.RdfSeed.Core.Nodes;

namespace kwd.RdfSeed.Core
{
	/// <summary>Some basic stats about a node factory</summary>
	public class NodeFactoryStats
	{
		/// <summary>Create new <see cref="NodeFactoryStats"/></summary>
		public NodeFactoryStats(int total, int blanks, int blankAutos, IReadOnlyList<NodeMap> mappings)
		{
			TotalNodes = total;
			BlankNodes = blanks;
			AutoNodes = blankAutos;

			Mappings = mappings;
		}

		/// <summary>Count Total nodes</summary>
		public readonly int TotalNodes;

		/// <summary>Count Blank nodes.</summary>
		public readonly int BlankNodes;

		/// <summary>Count Auto blank nodes</summary>
		public readonly int AutoNodes;

		/// <summary>The (current) set of current node mappings.</summary>
		public readonly IReadOnlyCollection<NodeMap> Mappings;
	}
}