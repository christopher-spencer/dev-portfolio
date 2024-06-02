using System.Diagnostics;
using Npgsql;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Capstone.UnitTests.DAO
{
    public abstract class PostgresDaoTestBase
    {
        protected string TestConnectionString = @"Host=localhost;Port=5432;Database=test_dev_portfolio;Username=test_dev_portfolio_appuser;Password=test_password";
        private static bool _databaseInitialized = false;
        private static readonly object _lock = new object();
        
        //NOTE initialized as null! here and for dao in PortfolioPostgresDaoTests
        protected NpgsqlTransaction transaction = null!;
        protected NpgsqlConnection connection = null!;

        //FIXME getting a transaction issue after adding additional tests to PortfolioPostgresDaoTests

        //NOTE possible issue with BlogPosts, Portfolio and User tests all running concurrently instead of one at a time?
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            lock (_lock)
            {
                if (!_databaseInitialized)
                {
                    RunDatabaseSetupScript();
                    _databaseInitialized = true;
                }
            }
        }
        
        [TestInitialize]
        public void Initialize()
        {
            try
            {
                //RunDatabaseSetupScript();

                connection = new NpgsqlConnection(TestConnectionString);
                connection.Open();
                transaction = connection.BeginTransaction();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during Initialize: {ex}");
                throw;
            }
        }

        private static void RunDatabaseSetupScript()
        {
            try
            {
                // Get the path to the directory containing the currently executing assembly
                //string assemblyPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string assemblyPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? string.Empty;
                // Combine it with the relative path to the script using Unix-style paths
                string scriptPath = Path.GetFullPath(Path.Combine(assemblyPath, @"../../../test_database/test_create.sh"));

                // Normalize the path (this handles cases like ".." and ensures the path is correct)
                scriptPath = Path.GetFullPath(scriptPath);

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = scriptPath,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = new Process())
                {
                    process.StartInfo = startInfo;
                    process.Start();

                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    if (process.ExitCode != 0)
                    {
                        throw new Exception($"Script exited with code {process.ExitCode}: {error}");
                    }

                    Console.WriteLine(output);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during RunDatabaseSetupScript: {ex}");
                throw;
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            try
            {
                    transaction?.Rollback();
                    transaction?.Dispose();
                    connection?.Close();
                    connection?.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during Cleanup: {ex}");
            }
        }
    }
}