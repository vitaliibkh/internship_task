# Bank Data Processor

## Task Description
This is a console application designed for bank employees to process and import
new client and account information into a database. Since employees do not have
direct database access, they upload fixed-length text files containing the data.

The application acts as an ETL (Extract, Transform, Load) pipeline that:
1. Scans the `NewData` folder for specific files (`CI*.txt` for Client Information,
`AI*.txt` for Account Information).
2. Parses the fixed-length data row by row based on a flexible JSON configuration
(`parser-config.json`).
3. Validates the data. If a row contains invalid formats (e.g., malformed dates),
it is skipped, and the error is logged to an `Errors.txt` file along with the
file name and row number.
4. Uses transactional batch processing (via Dapper) to securely insert valid
records into the database.
5. Moves successfully processed files to the `ProcessedData` folder to prevent
duplicate processing.

---

## Prerequisites
* [.NET 8.0 SDK](https://dotnet.microsoft.com/download) (or later)
* A running instance of PostgreSQL

## Environment Variables & Configuration Setup

To run this project locally, you need to set up your environment variables and configuration files.

### 1. Database Connection (.env)
Database credentials and secrets are managed via a `.env` file.
1. Locate the `.env.example` file in the root directory.
2. Create a copy of it and name it `.env`.
3. Fill in your actual PostgreSQL connection string:

```env
ConnectionString="Host=localhost;Port=5432;Database=bankdb;Username=postgres;Password=your_password"
```

### 2. Application Settings (appsettings.json)

Non-secret application configurations (like folder paths and batch limits) are stored
in appsettings.json. By default, it looks like this:

```JSON
{
  "FileProcessing": {
    "InputFolder": "NewData",
    "OutputFolder": "ProcessedData",
    "ErrorLogFile": "Errors.txt",
    "BatchSize": 100,

    "ParserConfigPath": "parser-config.json"
  },

  "Entities": {
    "Client": {
      "ConfigName": "Client",
      "FilePattern": "CI*"
    },

    "Account": {
      "ConfigName": "Account",
      "FilePattern": "AI*"
    }
  }
}
```

You can modify these paths or batch sizes without needing to recompile the C# code.

### 3. Parsing Rules (parser-config.json)

The rules for how to parse the fixed-length rows into database entities are
defined in parser-config.json. If new file formats are introduced in the future,
their parsing rules should be added here.

## How to Run

1. Clone the repository.
2. Ensure your .env file is created and your PostgreSQL database is running.
3. Place your test .txt files into the NewData folder.
4. Open your terminal in the project root and run:
```Bash
dotnet restore
dotnet build
dotnet run
```
5. Check the ProcessedData folder for completed files, the Errors.txt file
for any skipped rows, and your database for the inserted records.
