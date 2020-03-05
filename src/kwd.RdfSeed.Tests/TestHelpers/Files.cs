using System.IO;
using kwd.CoreUtil.FileSystem;

namespace kwd.RdfSeed.Tests.TestHelpers
{
    public static class Files
    {
        public static DirectoryInfo TestProject => 
            new DirectoryInfo(
                Path.GetFullPath("../../../", 
                    Path.GetDirectoryName(typeof(Files).Assembly.Location)??""));

        public static DirectoryInfo AppDataDir => TestProject.CreateSubdirectory("App_Data");

        /// <summary>Transient files in the App_Data folder</summary>
        public static class AppData
        {
            public static DirectoryInfo NqFileTests =>
                new DirectoryInfo(
                    Path.Combine(AppDataDir.FullName, nameof(NqFileTests)));
        }

        /// <summary>Files in the Test_Data folder</summary>
        public static class TestData
        {
            public static FileInfo Sample1 =>
                TestProject.GetFile("Test_Data", "Sample1.nt");

            public static FileInfo Brazil =>
	            TestProject.GetFile("Test_Data", "Brazil.nt");

            public static FileInfo Settings =>
	            TestProject.GetFile("Test_Data", "Settings.nt");

            public static FileInfo SettingsDebug =>
	            TestProject.GetFile("Test_Data", "Settings.Debug.nt");
        }

        public static DirectoryInfo TestDataDir => TestProject.CreateSubdirectory("Test_Data");
    }
}
