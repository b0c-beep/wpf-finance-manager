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
using finance_manager.Services;
using finance_manager.Models;
using System.Data.Entity;
using finance_manager.Data; 

namespace finance_manager.Views
{
    /// <summary>
    /// Interaction logic for DashboardPage.xaml
    /// </summary>
    public partial class DashboardPage : Page
    {
        public DashboardPage()
        {
            InitializeComponent();
            TimeHelper.checkFile();
            if (TimeHelper.didMonthPass())
            {
                resetMonth();
            }
            TimeHelper.logDate();
            printDate();
            loadProfitOptions(DatabaseHelper.FetchAllProducts());
            loadCostOptions(DatabaseHelper.FetchAllExpenses());

            List<Profit> profits = DatabaseHelper.FetchAllProfits();
            loadProfits(profits);
            List<Cost> costs = DatabaseHelper.FetchAllCosts();
            loadCosts(costs);

        }

        private void printDate()
        {
            dateLabel.Content = "Current situation for " + TimeHelper.getCurrentDate();
            dateLabel.FontSize = 18;
            dateLabel.FontFamily = new FontFamily("Arial");
        }

        private void resetMonth()
        {

        }

        private void loadProfitOptions(List<Product> products)
        {
            profitDropDown.Items.Clear();

            profitDropDown.Items.Add("-");
            foreach (Product product in products)
            {
                profitDropDown.Items.Add(product.Name + "   " + product.Price);
            }

            profitDropDown.SelectedIndex = 0;
        }

        private void loadCostOptions(List<Expense> expenses)
        {
            costDropDown.Items.Clear();

            costDropDown.Items.Add("-");
            foreach (Expense expense in expenses)
            {
                costDropDown.Items.Add(expense.Name + "   " + expense.Price);
            }

            costDropDown.SelectedIndex = 0;
        }

        private void addProfitButton_Click(object sender, RoutedEventArgs e)
        {
            if (profitDropDown.SelectedIndex == 0)
            {
                MessageBox.Show("Please select a product to add to the profit list.");
                return;
            }
            else
            {
                List<Product> products = DatabaseHelper.FetchAllProducts();
                int selectedIndex = profitDropDown.SelectedIndex - 1;
                Product selectedProduct = products[selectedIndex];
                Profit profit = new Profit(selectedProduct.Name, selectedProduct.Price, selectedProduct.TaxPercentage);
                DatabaseHelper.InsertProfit(profit);
                loadProfits(DatabaseHelper.FetchAllProfits());
                profitDropDown.SelectedIndex = 0;
            }
        }

        private void addCostButton_Click(object sender, RoutedEventArgs e)
        {
            if (costDropDown.SelectedIndex == 0)
            {
                MessageBox.Show("Please select an expense to add to the cost list.");
                return;
            }
            else
            {
                List<Expense> expenses = DatabaseHelper.FetchAllExpenses();
                int selectedIndex = costDropDown.SelectedIndex - 1;
                Expense selectedExpense = expenses[selectedIndex];
                Cost cost = new Cost(selectedExpense.Name, selectedExpense.Price, selectedExpense.TaxPercentage);
                DatabaseHelper.InsertCost(cost);
                loadCosts(DatabaseHelper.FetchAllCosts());
                costDropDown.SelectedIndex = 0;
            }
        }

        private void loadProfits(List<Profit> profits)
        {

            profitList.Children.Clear();
            foreach (Profit profit in profits)
            {
                // Create a StackPanel for each product
                StackPanel profitPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Width = 345,
                    Height = 40,
                    Margin = new Thickness(5)
                };

                // Create a TextBlock for product name
                TextBlock nameTextBlock = new TextBlock
                {
                    Text = profit.Name,
                    VerticalAlignment = VerticalAlignment.Center,
                    Width = 190

                };
                nameTextBlock.Margin = new Thickness(5);

                // Create a TextBlock for product price
                TextBlock priceTextBlock = new TextBlock
                {
                    Text = profit.Price.ToString(),
                    VerticalAlignment = VerticalAlignment.Center,
                    Width = 80
                };

                // Create Delete button
                Button deleteButton = new Button
                {
                    Content = "X",
                    Margin = new Thickness(5),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Width = 30,
                    Height = 30,
                    Tag = profit.Id
                };

                deleteButton.Click += deleteProfit_Click;

                // Add elements to the StackPanel
                profitPanel.Children.Add(nameTextBlock);
                profitPanel.Children.Add(priceTextBlock);
                profitPanel.Children.Add(deleteButton);


                profitList.Children.Add(profitPanel);
            }
        }

        private void loadCosts(List<Cost> costs)
        {

            costList.Children.Clear();
            foreach (Cost cost in costs)
            {
                // Create a StackPanel for each product
                StackPanel costPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Width = 345,
                    Height = 40,
                    Margin = new Thickness(5)
                };

                // Create a TextBlock for product name
                TextBlock nameTextBlock = new TextBlock
                {
                    Text = cost.Name,
                    VerticalAlignment = VerticalAlignment.Center,
                    Width = 190

                };
                nameTextBlock.Margin = new Thickness(5);

                // Create a TextBlock for product price
                TextBlock priceTextBlock = new TextBlock
                {
                    Text = cost.Price.ToString(),
                    VerticalAlignment = VerticalAlignment.Center,
                    Width = 80
                };

                // Create Delete button
                Button deleteButton = new Button
                {
                    Content = "X",
                    Margin = new Thickness(5),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Width = 30,
                    Height = 30,
                    Tag = cost.Id
                };

                deleteButton.Click += deleteCost_Click;

                // Add elements to the StackPanel
                costPanel.Children.Add(nameTextBlock);
                costPanel.Children.Add(priceTextBlock);
                costPanel.Children.Add(deleteButton);


                costList.Children.Add(costPanel);
            }
        }

        private void deleteProfit_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            int profitId = (int)button.Tag;
            DatabaseHelper.DeleteProfit(profitId);
            loadProfits(DatabaseHelper.FetchAllProfits());
        }

        private void deleteCost_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            int costId = (int)button.Tag;
            DatabaseHelper.DeleteCost(costId);
            loadCosts(DatabaseHelper.FetchAllCosts());
        }

        private void exportToExcel_Click(object sender, RoutedEventArgs e)
        {
            ExcelHelper.ProfitAndCostToExcel();
        }
    }
}
