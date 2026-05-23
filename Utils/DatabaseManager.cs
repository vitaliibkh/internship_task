using Npgsql;
using Dapper;
using project.Models;

namespace project.Utils;

public class DatabaseManager(string connectionString) {
    public void SaveClients(List<Client> clients) {
        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();

        const string sql = @"INSERT INTO ""Client"" (""Id"", ""FirstName"", ""LastName"", ""DOB"")
                           VALUES (@Id, @FirstName, @LastName, @DOB);";

        connection.Execute(sql, clients);
    }

    public void SaveAccounts(List<Account> accounts) {
        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();

        const string sql = @"INSERT INTO ""Account"" (""Id"", ""CurrentBalance"", ""LastTransaction"", ""CustomerId"")
                           VALUES (@Id, @CurrentBalance, @LastTransaction, @CustomerId);";

        connection.Execute(sql, accounts);
    }
}
