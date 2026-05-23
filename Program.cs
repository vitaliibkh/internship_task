using project.Models;
using project.Utils;

namespace project;

class Program {
    public static void Main() {
        DotNetEnv.Env.Load();
        string connectionString = Environment.GetEnvironmentVariable("ConnectionString")!;

        DatabaseManager databaseManager = new(connectionString);
        ConfigurationManager configurationManager = new("parser-config.json");
        FileHandler fileHandler = new("NewData", "ProcessedData", "Errors.txt", 100);

        try {
            Parser parser = new(configurationManager.GetConfigs("Client")!);
            var files = fileHandler.RetrieveFiles("CI*");
            foreach (var file in files) {
                fileHandler.ProcessFile(file, parser.Parse<Client>, databaseManager.SaveClients);
            }

            parser.Configs = configurationManager.GetConfigs("Account")!;
            files = fileHandler.RetrieveFiles("AI*");
            foreach (var file in files) {
                fileHandler.ProcessFile(file, parser.Parse<Account>, databaseManager.SaveAccounts);
            }
        } catch (Exception ex) {
            Console.WriteLine($"{ex.Message}");
        }
    }
}
