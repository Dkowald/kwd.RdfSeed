namespace kwd.RdfSeed.Core
{
	/// <summary>Some basic stats about a node factory</summary>
	public class NodeFactoryStats
	{
		/// <summary>Create new <see cref="NodeFactoryStats"/></summary>
		public NodeFactoryStats(int total, int blanks, int blankAutos)
		{
			TotalNodes = total;
			BlankNodes = blanks;
			AutoNodes = blankAutos;
		}

		/// <summary>Count Total nodes</summary>
		public readonly int TotalNodes;

		/// <summary>Count Blank nodes.</summary>
		public readonly int BlankNodes;

		/// <summary>Count Auto blank nodes</summary>
		public readonly int AutoNodes;
	}
}