using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using finance_manager.Models;
using System.Threading;

namespace finance_manager.Data
{
    internal class DatabaseHelper
    {
        private static readonly string AppFolder = Path.GetFullPath(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\"));

        private static readonly string ProductsDbPath = Path.Combine(AppFolder, "products.db");
        private static readonly string ExpensesDbPath = Path.Combine(AppFolder, "expenses.db");
        private static readonly string DashboardDbPath = Path.Combine(AppFolder, "dashboard.db");

        public static void InitializeDatabases()
        {
            System.Diagnostics.Debug.WriteLine(AppFolder);

            CreateDatabaseIfNotExists(ProductsDbPath, @"
                CREATE TABLE IF NOT EXISTS Products (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Price REAL NOT NULL,
                    TaxPercentage REAL NOT NULL
                );");

            CreateDatabaseIfNotExists(ExpensesDbPath, @"
                CREATE TABLE IF NOT EXISTS Expenses (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Price REAL NOT NULL,
                    TaxPercentage REAL NOT NULL
                );");

            CreateDatabaseIfNotExists(DashboardDbPath, @"
                CREATE TABLE IF NOT EXISTS MonthlyFinances (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Date TEXT NOT NULL,
                    Income REAL NOT NULL,
                    Expenses REAL NOT NULL,
                    Taxes REAL NOT NULL
                );");


        }

        private static void CreateDatabaseIfNotExists(string dbPath, string createTableQuery)
        {
            if (!File.Exists(dbPath))
            {
                using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
                {
                    connection.Open();
                    using (var command = new SQLiteCommand(createTableQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            else
            {
                // Database file exists, check if the table exists
                using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
                {
                    connection.Open();

                    // Query to check if the table exists
                    string checkTableQuery = "SELECT name FROM sqlite_master WHERE type='table' AND name='Products';";  // Modify table name accordingly
                    using (var command = new SQLiteCommand(checkTableQuery, connection))
                    {
                        var result = command.ExecuteScalar();

                        // If result is null, the table doesn't exist, create the table
                        if (result == null)
                        {
                            using (var createCommand = new SQLiteCommand(createTableQuery, connection))
                            {
                                createCommand.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
        }

        public static SQLiteConnection GetConnection(string dbName)
        {
            string dbPath = dbName switch
            {
                "products" => ProductsDbPath,
                "expenses" => ExpensesDbPath,
                "dashboard" => DashboardDbPath,
                _ => throw new ArgumentException("Invalid database name")
            };

            return new SQLiteConnection($"Data Source={dbPath};Version=3;");
        }

        public static void InsertProduct(Product product)
        {
            using (var connection = GetConnection("products"))
            {
                connection.Open();
                string query = "INSERT INTO Products (Name, Price, TaxPercentage) VALUES (@Name, @Price, @TaxPercentage)";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", product.Name);
                    command.Parameters.AddWithValue("@Price", product.Price);
                    command.Parameters.AddWithValue("@TaxPercentage", product.TaxPercentage);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
