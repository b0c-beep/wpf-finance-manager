using finance_manager.Data;
using finance_manager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace finance_manager.Views
{
    /// <summary>
    /// Interaction logic for ExpensesPage.xaml
    /// </summary>
    public partial class ExpensesPage : Page
    {
        public ExpensesPage()
        {
            InitializeComponent();
            List<Expense> expenses = DatabaseHelper.FetchAllExpenses();
            LoadExpenses(expenses);
        }

        private void LoadExpenses(List<Expense> expenses)
        {
            ExpenseList.Children.Clear();

            foreach (var expense in expenses)
            {
                // Create a StackPanel for each expense
                StackPanel expensePanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Width = 700,
                    Height = 60,
                    Margin = new Thickness(5)
                };

                // Create a TextBlock for expense name
                TextBlock nameTextBlock = new TextBlock
                {
                    Text = expense.Name,
                    VerticalAlignment = VerticalAlignment.Center,
                    Width = 200

                };
                nameTextBlock.Margin = new Thickness(20);

                // Create a TextBlock for expense price
                TextBlock priceTextBlock = new TextBlock
                {
                    Text = expense.Price.ToString(), // Format as currency
                    VerticalAlignment = VerticalAlignment.Center,
                    Width = 100
                };

                // Create a TextBlock for expense tax percentage
                TextBlock taxTextBlock = new TextBlock
                {
                    Text = expense.TaxPercentage.ToString() + "%",
                    VerticalAlignment = VerticalAlignment.Center,
                    Width = 100
                };

                // Create Edit button
                Button editButton = new Button
                {
                    Content = "Edit",
                    Margin = new Thickness(5),
                    Width = 100,
                    Height = 30,
                    Tag = expense.Id
                };

                editButton.Click += EditButton_Click;

                // Create Delete button
                Button deleteButton = new Button
                {
                    Content = "Delete",
                    Margin = new Thickness(5),
                    Width = 100,
                    Height = 30,
                    Tag = expense.Id
                };

                deleteButton.Click += DeleteButton_Click;

                // Add elements to the StackPanel
                expensePanel.Children.Add(nameTextBlock);
                expensePanel.Children.Add(priceTextBlock);
                expensePanel.Children.Add(taxTextBlock);
                expensePanel.Children.Add(editButton);
                expensePanel.Children.Add(deleteButton);

                // Add the expense panel to the ExpenseList StackPanel
                ExpenseList.Children.Add(expensePanel);
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Button editButton = sender as Button;
            if (editButton == null) return;

            // Get the parent expensePanel
            StackPanel expensePanel = editButton.Parent as StackPanel;
            if (expensePanel == null) return;

            // Find all TextBlocks inside this specific expense panel
            TextBlock nameTextBlock = expensePanel.Children.OfType<TextBlock>().FirstOrDefault(tb => tb.Width == 200);
            TextBlock priceTextBlock = expensePanel.Children.OfType<TextBlock>().FirstOrDefault(tb => tb.Width == 100);
            TextBlock taxTextBlock = expensePanel.Children.OfType<TextBlock>().LastOrDefault(tb => tb.Width == 100);
            Button deleteButton = expensePanel.Children.OfType<Button>().FirstOrDefault(btn => btn.Content.ToString() == "Delete");

            if (nameTextBlock == null || priceTextBlock == null || taxTextBlock == null || deleteButton == null) return;

            // Convert to TextBoxes
            TextBox nameTextBox = new TextBox { Text = nameTextBlock.Text, Width = 200, Height = 20, Margin = nameTextBlock.Margin };
            TextBox priceTextBox = new TextBox { Text = priceTextBlock.Text, Width = 100, Height = 20, Margin = priceTextBlock.Margin };
            TextBox taxTextBox = new TextBox { Text = taxTextBlock.Text.Replace("%", ""), Width = 100, Height = 20, Margin = taxTextBlock.Margin };

            // Get indexes
            int nameIndex = expensePanel.Children.IndexOf(nameTextBlock);
            int priceIndex = expensePanel.Children.IndexOf(priceTextBlock);
            int taxIndex = expensePanel.Children.IndexOf(taxTextBlock);

            // Remove old TextBlocks
            expensePanel.Children.RemoveAt(taxIndex);
            expensePanel.Children.RemoveAt(priceIndex);
            expensePanel.Children.RemoveAt(nameIndex);

            // Insert new TextBoxes
            expensePanel.Children.Insert(nameIndex, nameTextBox);
            expensePanel.Children.Insert(priceIndex, priceTextBox);
            expensePanel.Children.Insert(taxIndex, taxTextBox);

            // Hide the Delete button
            deleteButton.Visibility = Visibility.Collapsed;

            // Change Edit button to "Save"
            editButton.Content = "Save";
            editButton.Click -= EditButton_Click;
            editButton.Click += SaveEditedProduct;
        }

        private void SaveEditedProduct(object sender, RoutedEventArgs e)
        {
            // Get the parent StackPanel for the expense being edited
            StackPanel expensePanel = (StackPanel)((Button)sender).Parent;

            // Find the TextBoxes inside the StackPanel
            TextBox nameTextBox = expensePanel.Children.OfType<TextBox>().FirstOrDefault(tb => tb.Width == 200);
            TextBox priceTextBox = expensePanel.Children.OfType<TextBox>().FirstOrDefault(tb => tb.Width == 100);
            TextBox taxTextBox = expensePanel.Children.OfType<TextBox>().LastOrDefault(tb => tb.Width == 100);
            Button deleteButton = expensePanel.Children.OfType<Button>().FirstOrDefault(btn => btn.Content.ToString() == "Delete");

            Button saveButton = sender as Button;
            int expenseId = (int)saveButton.Tag;

            Expense originalExpense = DatabaseHelper.GetExpenseById(expenseId);

            string newName = nameTextBox.Text;
            if (saveButton == null) return;

            // Validate the input
            if (string.IsNullOrEmpty(newName) ||
                !decimal.TryParse(priceTextBox.Text, out decimal newPrice) ||
                !decimal.TryParse(taxTextBox.Text, out decimal newTax))
            {
                MessageBox.Show("Please ensure all fields are valid.");
                return;
            }

            // Check if the new values are not the same as the original ones
            if (!(originalExpense.Name == newName &&
                originalExpense.Price == newPrice &&
                originalExpense.TaxPercentage == newTax))
            {

                // Update in database
                DatabaseHelper.UpdateExpense(expenseId, newName, newPrice, newTax);

            }

            // Convert tax back to a percentage string
            string formattedTax = $"{newTax}%";



            // Convert back to TextBlocks
            TextBlock nameTextBlock = new TextBlock { Text = newName, Width = 200, VerticalAlignment = VerticalAlignment.Center, Margin = nameTextBox.Margin };
            TextBlock priceTextBlock = new TextBlock { Text = newPrice.ToString(), Width = 100, VerticalAlignment = VerticalAlignment.Center, Margin = priceTextBox.Margin };
            TextBlock taxTextBlock = new TextBlock { Text = formattedTax, Width = 100, VerticalAlignment = VerticalAlignment.Center, Margin = taxTextBox.Margin };

            // Get indexes
            int nameIndex = expensePanel.Children.IndexOf(nameTextBox);
            int priceIndex = expensePanel.Children.IndexOf(priceTextBox);
            int taxIndex = expensePanel.Children.IndexOf(taxTextBox);

            // Remove old TextBlocks
            expensePanel.Children.RemoveAt(taxIndex);
            expensePanel.Children.RemoveAt(priceIndex);
            expensePanel.Children.RemoveAt(nameIndex);

            // Insert new TextBoxes
            expensePanel.Children.Insert(nameIndex, nameTextBlock);
            expensePanel.Children.Insert(priceIndex, priceTextBlock);
            expensePanel.Children.Insert(taxIndex, taxTextBlock);

            // Show the Delete button again
            deleteButton.Visibility = Visibility.Visible;

            // Change Save button back to "Edit"
            saveButton.Content = "Edit";
            saveButton.Click -= SaveEditedProduct;
            saveButton.Click += EditButton_Click;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            StackPanel expensePanel = button.Parent as StackPanel;

            int expenseId = (int)button.Tag;
            DatabaseHelper.DeleteExpense(expenseId);
            ExpenseList.Children.Remove(expensePanel);
            LoadExpenses(DatabaseHelper.FetchAllExpenses());
        }

        private void AddExpense_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            button.IsEnabled = false;
            addExpenseDockPanel.Visibility = Visibility.Visible;
            ExpenseList.Height -= (addExpenseDockPanel.Height + 2);
        }

        private void ModalAddExpense_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ExpenseNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(ExpensePriceTextBox.Text) ||
                string.IsNullOrWhiteSpace(ExpenseTaxTextBox.Text))
            {
                MessageBox.Show("Please fill all the fields");
                return;
            }
            else
            {
                string name = ExpenseNameTextBox.Text;
                decimal price, tax;

                if (!decimal.TryParse(ExpensePriceTextBox.Text, out price))
                {
                    MessageBox.Show("Invalid price. Please enter a valid number.");
                    return;
                }

                if (!decimal.TryParse(ExpenseTaxTextBox.Text, out tax))
                {
                    MessageBox.Show("Invalid tax percentage. Please enter a valid number.");
                    return;
                }

                var newExpense = new Expense(name, price, tax);

                // Insert the expense into the database
                DatabaseHelper.InsertExpense(newExpense);
                //MessageBox.Show("Expense added successfully!");


                ExpenseNameTextBox.Text = "";
                ExpensePriceTextBox.Text = "";
                ExpenseTaxTextBox.Text = "";
                addExpenseButton.IsEnabled = true;
            }

            List<Expense> expenses = DatabaseHelper.FetchAllExpenses();
            LoadExpenses(expenses);
            //LabelStack.Visibility = Visibility.Collapsed;

            addExpenseDockPanel.Visibility = Visibility.Collapsed;
            ExpenseList.Height += (addExpenseDockPanel.Height + 2);
        }

        private void ModalCancelExpense_Click(object sender, RoutedEventArgs e)
        {
            addExpenseDockPanel.Visibility = Visibility.Collapsed;
            ExpenseList.Height += (addExpenseDockPanel.Height + 2);

            ExpenseNameTextBox.Text = "";
            ExpensePriceTextBox.Text = "";
            ExpenseTaxTextBox.Text = "";

            addExpenseButton.IsEnabled = true;
        }
    }
}
