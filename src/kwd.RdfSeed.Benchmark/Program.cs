using System;
using System.IO;

using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Running;

namespace kwd.RdfSeed.Benchmark
{
	public class Program
    {
	    public static string SourceFolderRoot = "Project";

        static void Main()
        {
	        var cfg = ManualConfig.Create(new DebugBuildConfig())
			        //ManualConfig.Create(new DebugInProcessConfig())
			        .With(MarkdownExporter.Console)
			        .With(MemoryDiagnoser.Default)
			        .WithArtifactsPath(Directory.GetCurrentDirectory())
					.With(ConfigOptions.DisableLogFile | ConfigOptions.StopOnFirstError);

            Environment.SetEnvironmentVariable
				(SourceFolderRoot, Directory.GetCurrentDirectory());

            BenchmarkRunner
	            .Run(typeof(Program).Assembly, cfg);
	            //.Run<NodeFactoryPerformance>(cfg);
	            //.Run<StringVsHashThenString>(cfg);
	            //.Run<NTripleReadWrite>(cfg);
	            //.Run<RuntimeGenericCreate>(cfg);
	            //.Run<CostOfLockOnSingleThread>(cfg);

			Console.WriteLine(@"All done.");
            Console.ReadKey();
        }
    }
}
