using System;

namespace kwd.RdfSeed.Errors
{
	/// <summary>
	/// Raised if call the <see cref="RdfDataFactory.Init"/>,
	/// more than once in a process.
	/// </summary>
	public class FactoryAlreadyInitialized : Exception
	{
		/// <summary>
		/// Create a new <see cref="FactoryAlreadyInitialized"/>
		/// </summary>
		public FactoryAlreadyInitialized()
			:base($"The {nameof(RdfDataFactory.AppData)} " +
			      "has already been initialized"){}
	}
}