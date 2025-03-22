using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using finance_manager.Models;
using System.Threading;
using System.Xml.Linq;

namespace finance_manager.Data
{
    internal class DatabaseHelper
    {
        private static readonly string AppFolder = Path.GetFullPath(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\"));

        private static readonly string ProductsDbPath = Path.Combine(AppFolder, "products.db");
        private static readonly string ExpensesDbPath = Path.Combine(AppFolder, "expenses.db");
        private static readonly string ProfitsDbPath = Path.Combine(AppFolder, "profits.db");
        private static readonly string CostsDbPath = Path.Combine(AppFolder, "costs.db");

        public static void InitializeDatabases()
        {
            //System.Diagnostics.Debug.WriteLine(AppFolder);

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

            CreateDatabaseIfNotExists(ProfitsDbPath, @"
                CREATE TABLE IF NOT EXISTS Profits (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Price REAL NOT NULL,
                    TaxPercentage REAL NOT NULL,
                    Date TEXT NOT NULL
                );");

            CreateDatabaseIfNotExists(CostsDbPath, @"
                CREATE TABLE IF NOT EXISTS Costs (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Price REAL NOT NULL,
                    TaxPercentage REAL NOT NULL,
                    Date TEXT NOT NULL
                );");


        }

        private static void CreateDatabaseIfNotExists(string dbPath, string createTableQuery)
        {
            // Extract the database name from the dbPath
            string dbName = Path.GetFileNameWithoutExtension(dbPath).ToLower();

            // Determine the expected table name based on the database
            string expectedTableName = dbName switch
            {
                "products" => "Products",
                "expenses" => "Expenses",
                "profits" => "Profits",
                "costs" => "Costs",
                _ => throw new ArgumentException("Unknown database type.")
            };

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
                    string checkTableQuery = "SELECT name FROM sqlite_master WHERE type='table' AND name='{expectedTableName}';"; 
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
                "profits" => ProfitsDbPath,
                "costs" => CostsDbPath,
                _ => throw new ArgumentException("Invalid database name")
            };

            return new SQLiteConnection($"Data Source={dbPath};Version=3;");
        }

        public static List<Product> FetchAllProducts()
        {
            List<Product> products = new List<Product>();

            using (var connection = new SQLiteConnection($"Data Source={ProductsDbPath};Version=3;"))
            {
                connection.Open();
                string query = "SELECT Id, Name, Price, TaxPercentage FROM Products;";
                using (var command = new SQLiteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var product = new Product
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Price = reader.GetDecimal(2),
                                TaxPercentage = reader.GetDecimal(3)
                            };
                            products.Add(product);
                        }
                    }
                }
            }

            return products;
        }

        public static List<Expense> FetchAllExpenses()
        {
            List<Expense> expenses = new List<Expense>();

            using (var connection = new SQLiteConnection($"Data Source={ExpensesDbPath};Version=3;"))
            {
                connection.Open();
                string query = "SELECT Id, Name, Price, TaxPercentage FROM Expenses;";
                using (var command = new SQLiteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var expense = new Expense
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Price = reader.GetDecimal(2),
                                TaxPercentage = reader.GetDecimal(3)
                            };
                            expenses.Add(expense);
                        }
                    }
                }
            }

            return expenses;
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

        public static void InsertExpense(Expense expense)
        {
            using (var connection = GetConnection("expenses"))
            {
                connection.Open();
                string query = "INSERT INTO Expenses (Name, Price, TaxPercentage) VALUES (@Name, @Price, @TaxPercentage)";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", expense.Name);
                    command.Parameters.AddWithValue("@Price", expense.Price);
                    command.Parameters.AddWithValue("@TaxPercentage", expense.TaxPercentage);
                    command.ExecuteNonQuery();
                }
            }
        }

        // Delete a product by ID
        public static bool DeleteProduct(int productId)
        {
            using (var connection = GetConnection("products"))
            {
                connection.Open();
                string query = "DELETE FROM Products WHERE Id = @Id";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", productId);
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0; // Returns true if deletion was successful
                }
            }
        }

        //Delete an expense by ID
        public static bool DeleteExpense(int expenseId)
        {
            using (var connection = GetConnection("expenses"))
            {
                connection.Open();
                string query = "DELETE FROM Expenses WHERE Id = @Id";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", expenseId);
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0; // Returns true if deletion was successful
                }
            }
        }

        // Update a product by ID
        public static bool UpdateProduct(int productId, string newName, decimal newPrice, decimal newTax)
        {
            using (var connection = GetConnection("products"))
            {
                connection.Open();
                string query = "UPDATE Products SET Name = @Name, Price = @Price, TaxPercentage = @TaxPercentage WHERE Id = @Id";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", productId);
                    command.Parameters.AddWithValue("@Name", newName);
                    command.Parameters.AddWithValue("@Price", newPrice);
                    command.Parameters.AddWithValue("@TaxPercentage", newTax);

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0; // Returns true if update was successful
                }
            }
        }

        // Update an expense by ID
        public static bool UpdateExpense(int expenseId, string newName, decimal newPrice, decimal newTax)
        {
            using (var connection = GetConnection("expenses"))
            {
                connection.Open();
                string query = "UPDATE Expenses SET Name = @Name, Price = @Price, TaxPercentage = @TaxPercentage WHERE Id = @Id";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", expenseId);
                    command.Parameters.AddWithValue("@Name", newName);
                    command.Parameters.AddWithValue("@Price", newPrice);
                    command.Parameters.AddWithValue("@TaxPercentage", newTax);

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0; // Returns true if update was successful
                }
            }
        }

        // Get a product by ID
        public static Product GetProductById(int productId)
        {
            using (var connection = GetConnection("products"))
            {
                connection.Open();
                string query = "SELECT Id, Name, Price, TaxPercentage FROM Products WHERE Id = @Id;";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", productId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())  // Check if the product is found
                        {
                            return new Product
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                                TaxPercentage = reader.GetDecimal(reader.GetOrdinal("TaxPercentage"))
                            };
                        }
                        else
                        {
                            return null;  // Return null if no product is found with the given ID
                        }
                    }
                }
            }
        }

        // Get an expense by ID
        public static Expense GetExpenseById(int expenseId)
        {
            using (var connection = GetConnection("expenses"))
            {
                connection.Open();
                string query = "SELECT Id, Name, Price, TaxPercentage FROM Expenses WHERE Id = @Id;";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", expenseId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())  // Check if the expense is found
                        {
                            return new Expense
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                                TaxPercentage = reader.GetDecimal(reader.GetOrdinal("TaxPercentage"))
                            };
                        }
                        else
                        {
                            return null;  // Return null if no expense is found with the given ID
                        }
                    }
                }
            }
        }

        public static List<Profit> FetchAllProfits()
        {
            List<Profit> profits = new List<Profit>();

            using (var connection = new SQLiteConnection($"Data Source={ProfitsDbPath};Version=3;"))
            {
                connection.Open();
                string query = "SELECT Id, Name, Price, TaxPercentage, Date FROM Profits;";
                using (var command = new SQLiteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var profit = new Profit
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Price = reader.GetDecimal(2),
                                TaxPercentage = reader.GetDecimal(3),
                                Date = reader.GetString(4)
                            };
                            profits.Add(profit);
                        }
                    }
                }
            }

            return profits;
        }

        public static List<Cost> FetchAllCosts()
        {
            List<Cost> costs = new List<Cost>();

            using (var connection = new SQLiteConnection($"Data Source={CostsDbPath};Version=3;"))
            {
                connection.Open();
                string query = "SELECT Id, Name, Price, TaxPercentage, Date FROM Costs;";
                using (var command = new SQLiteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var cost = new Cost
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Price = reader.GetDecimal(2),
                                TaxPercentage = reader.GetDecimal(3),
                                Date = reader.GetString(4)
                            };
                            costs.Add(cost);
                        }
                    }
                }
            }

            return costs;
        }

        public static void InsertProfit(Profit profit)
        {
            using (var connection = GetConnection("profits"))
            {
                connection.Open();
                string query = "INSERT INTO Profits (Name, Price, TaxPercentage, Date) VALUES (@Name, @Price, @TaxPercentage, @Date)";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", profit.Name);
                    command.Parameters.AddWithValue("@Price", profit.Price);
                    command.Parameters.AddWithValue("@TaxPercentage", profit.TaxPercentage);
                    command.Parameters.AddWithValue("@Date", profit.Date);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void InsertCost(Cost cost)
        {
            using (var connection = GetConnection("costs"))
            {
                connection.Open();
                string query = "INSERT INTO Costs (Name, Price, TaxPercentage, Date) VALUES (@Name, @Price, @TaxPercentage, @Date)";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", cost.Name);
                    command.Parameters.AddWithValue("@Price", cost.Price);
                    command.Parameters.AddWithValue("@TaxPercentage", cost.TaxPercentage);
                    command.Parameters.AddWithValue("@Date", cost.Date);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static bool DeleteProfit(int profitId)
        {
            using (var connection = GetConnection("profits"))
            {
                connection.Open();
                string query = "DELETE FROM Profits WHERE Id = @Id";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", profitId);
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0; // Returns true if deletion was successful
                }
            }
        }

        public static bool DeleteCost(int costId)
        {
            using (var connection = GetConnection("costs"))
            {
                connection.Open();
                string query = "DELETE FROM Costs WHERE Id = @Id";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", costId);
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0; // Returns true if deletion was successful
                }
            }
        }

    }
}
