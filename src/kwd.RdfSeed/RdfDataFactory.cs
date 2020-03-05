using System;
using System.Collections.Generic;
using System.Linq;

using kwd.RdfSeed.Core;
using kwd.RdfSeed.Core.Nodes;
using kwd.RdfSeed.Errors;
using kwd.RdfSeed.TypedNodes;

namespace kwd.RdfSeed
{
	/// <summary>
	/// Entry point to create a <see cref="IRdfData"/>.
	/// </summary>
	public static class RdfDataFactory
	{
		private static IRdfData? _instance;

		/// <summary>
		/// Previously initialized <see cref="IRdfData"/> for use
		/// within a process; see <see cref="Init"/>.
		/// </summary>
		/// <remarks>
		/// This is only for those who don't happen to have an IoC
		/// container.
		/// </remarks>
		public static IRdfData AppData =>
			_instance ??
			throw new ArgumentNullException(
				$"{nameof(RdfDataFactory)}.{nameof(RdfDataFactory.Init)}");

		/// <summary>
		/// Initialize the <see cref="AppData"/> singleton.
		/// </summary>
		/// <exception cref="FactoryAlreadyInitialized"></exception>
		public static IRdfData Init(IRdfData data)
		{
			if (_instance != null && _instance != data)
				throw new FactoryAlreadyInitialized();

			_instance= data;
			return _instance;
		}

		/// <summary>
		/// Creates a default <see cref="IRdfData"/>
		/// thread safe; and all Typed node mappings.
		/// Optionally provide a set of custom type mappings to be merged.
		/// </summary>
		public static IRdfData CreateDefault(params NodeMap[] customMappings)
		{
			var mappings = MergeMappings(customMappings);

			return new RdfDataThreadLock(
				new RdfData(
					new NodeFactoryThreadSafe(
						new NodeFactory(mappings))));
		}

		/// <summary>
		/// Creates a default <see cref="IRdfData"/>
		/// thread safe; and all Typed node mappings.
		/// Optionally provide a set of custom type mappings to be merged.
		/// </summary>
		public static IRdfData CreateDefault(IEnumerable<NodeMap> customMappings)
			=> CreateDefault(customMappings.ToArray());

		/// <summary>
		/// Create a slightly faster <see cref="IRdfData"/>,
		/// that is not thread safe; includes all typed node mappings.
		/// </summary>
		public static IRdfData CreateNoLock(params NodeMap[] customMappings)
			=> new RdfData(new NodeFactory(
				MergeMappings(customMappings)));

		/// <summary>
		/// Create a slightly faster <see cref="IRdfData"/>,
		/// that is not thread safe; includes all typed node mappings.
		/// </summary>
		public static IRdfData CreateNoLock(IEnumerable<NodeMap> customMappings)
			=> CreateNoLock(customMappings.ToArray());

		private static NodeMap[] MergeMappings(NodeMap[] customMappings)
		=>customMappings.Union(
				TypedNodesHelpers.AllNodeMappings()
					.Where(x => customMappings.All(c => c.DataType != x.DataType))
			).ToArray();

	}
}