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
                
            }
        }

        private void addCostButton_Click(object sender, RoutedEventArgs e)
        {
            if (costDropDown.SelectedIndex == 0) {
                MessageBox.Show("Please select an expense to add to the cost list.");
                return;
            }
            else
            {
                
            }
        }
    }
}
