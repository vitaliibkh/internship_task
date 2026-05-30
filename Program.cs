using Microsoft.Extensions.Configuration;
using project.Models;
using project.Utils;

namespace project;

class Program {
    public static void Main() {
        DotNetEnv.Env.Load();
        string connectionString = Environment.GetEnvironmentVariable("ConnectionString")
            ?? throw new Exception("ConnectionString is missing in .env");

        var config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

        string inputFolder = config["FileProcessing:InputFolder"] ?? "NewData";
        string outputFolder = config["FileProcessing:OutputFolder"] ?? "ProcessedData";
        string errorLog = config["FileProcessing:ErrorLogFile"] ?? "Errors.txt";
        int batchSize = int.Parse(config["FileProcessing:BatchSize"] ?? "100");

        string parserConfigPath = config["FileProcessing:ParserConfigPath"]!;

        string clientConfigName = config["Entities:Client:ConfigName"]!;
        string clientPattern = config["Entities:Client:FilePattern"]!;

        string accountConfigName = config["Entities:Account:ConfigName"]!;
        string accountPattern = config["Entities:Account:FilePattern"]!;

        DatabaseManager databaseManager = new(connectionString);
        Utils.ConfigurationManager configurationManager = new(parserConfigPath);
        FileHandler fileHandler = new(inputFolder, outputFolder, errorLog, batchSize);

        try {
            Parser clientParser = new(configurationManager.GetConfigs(clientConfigName)!);
            var files = fileHandler.RetrieveFiles(clientPattern);
            foreach (var file in files) {
                fileHandler.ProcessFile(file, clientParser.Parse<Client>, databaseManager.SaveClients);
            }

            Parser accountParser = new(configurationManager.GetConfigs(accountConfigName)!);
            files = fileHandler.RetrieveFiles(accountPattern);
            foreach (var file in files) {
                fileHandler.ProcessFile(file, accountParser.Parse<Account>, databaseManager.SaveAccounts);
            }
        } catch (Exception ex) {
            Console.WriteLine($"{ex.Message}");
        }
    }
}
