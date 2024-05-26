using System.Diagnostics;
using System.Transactions;
using Npgsql;

namespace Capstone.UnitTests.DAO
{
    public abstract class PostgresDaoTestBase
    {
        protected string TestConnectionString = @"Host=localhost;Port=5432;Database=test_dev_portfolio;Username=test_dev_portfolio_appuser;Password=test_password";
        private TransactionScope transaction;

        //FIXME getting a transaction issue after adding additional tests to PortfolioPostgresDaoTests

        //NOTE possible issue with BlogPosts, Portfolio and User tests all running concurrently instead of one at a time?
        [TestInitialize]
        public void Initialize()
        {
            RunDatabaseSetupScript();

            try
            {
                var transactionOptions = new TransactionOptions
                {
                    IsolationLevel = IsolationLevel.ReadCommitted,
                    Timeout = TimeSpan.FromMinutes(2) // Used to adjust timing for transaction timeout
                };

                transaction = new TransactionScope(TransactionScopeOption.RequiresNew, transactionOptions, TransactionScopeAsyncFlowOption.Enabled);


                using (NpgsqlConnection conn = new NpgsqlConnection(TestConnectionString))
                {
                    conn.Open();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during Initialize: {ex}");
                throw;
            }
        }

        private void RunDatabaseSetupScript()
        {
            try
            {
                // Get the path to the directory containing the currently executing assembly
                string assemblyPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
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
                if (transaction != null)
                {
                    transaction.Complete();
                    transaction.Dispose();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during Cleanup: {ex}");
            }
        }
    }
}