using System;
using BenchmarkDotNet.Attributes;

// ReSharper disable UnusedTypeParameter
// ReSharper disable UnusedVariable

namespace kwd.RdfSeed.Benchmark.Micro
{
	/// <summary>
	/// Look at cost to use a generic for Quad.
	/// </summary>
	/// <remarks>
	/// Too costly to resolve generic type at run-time;
	/// not including.
	/// </remarks>
	public class RuntimeGenericCreate
	{
		public class MyThing
		{
			public MyThing(string name)
			{
				Name = name;
			}

			public string Name;
		}

		public class MyThing<T> : MyThing
		{
			public MyThing(string name):base(name)
			{}
		}

		[Benchmark(Baseline = true)]
		public void NewBasicThing()
		{
			var x = new MyThing("basic");
		}

		[Benchmark]
		public void DynamicNewTypedThing()
		{
			var t = typeof(MyThing<>).MakeGenericType(typeof(int));

			var x = (MyThing<int>)Activator.CreateInstance(t, "fancy");
		}

		[Benchmark]
		public void NewTypedThing()
		{
			var x = new MyThing<int>("static");
		}
	}
}