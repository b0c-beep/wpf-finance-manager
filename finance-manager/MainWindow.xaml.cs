﻿using System;
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
using finance_manager.Views;
using finance_manager.Data;
using finance_manager.Models;

namespace finance_manager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DatabaseHelper.InitializeDatabases();
            MainFrame.NavigationService.Navigate(new Views.DashboardPage());
        }

        private void NavigateToPage(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                switch (button.Tag)
                {
                    case "Products":
                        MainFrame.Navigate(new ProductsPage());
                        break;
                    case "Expenses":
                        MainFrame.Navigate(new ExpensesPage());
                        break;
                    case "Statistics":
                        MainFrame.Navigate(new StatisticsPage());
                        break;
                    case "History":
                        MainFrame.Navigate(new HistoryPage());
                        break;
                    default:
                        MainFrame.Navigate(new DashboardPage());
                        break;
                }
            }
        }
    }
}
