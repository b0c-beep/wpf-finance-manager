using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace finance_manager.Data
{
    internal class DatabaseHelper
    {
        private static readonly string AppFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FinanceManager");

        private static readonly string ProductsDbPath = Path.Combine(AppFolder, "products.db");
        private static readonly string ExpensesDbPath = Path.Combine(AppFolder, "expenses.db");
        private static readonly string DashboardDbPath = Path.Combine(AppFolder, "dashboard.db");

        public static void InitializeDatabases()
        {
            if (!Directory.Exists(AppFolder))
            {
                Directory.CreateDirectory(AppFolder);
            }

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
    }
}
