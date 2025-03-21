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
using System.Xml.Linq;

namespace finance_manager.Views
{
    /// <summary>
    /// Interaction logic for ProductsPage.xaml
    /// </summary>
    public partial class ProductsPage : Page
    {
        public ProductsPage()
        {
            InitializeComponent();
        }
            
        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            addProductDockPanel.Visibility = Visibility.Visible;
            ProductList.Height -= (addProductDockPanel.Height + 2);
        }

        private void ModalAddProduct_Click(object sender, RoutedEventArgs e)
        {
            if(ProductNameTextBox.Text == "" || ProductPriceTextBox.Text == "" || ProductTaxTextBox.Text == "")
            {
                MessageBox.Show("Please fill all the fields");
                return;
            }
            else
            {
                string name = ProductNameTextBox.Text;
                decimal price, tax; 

                if (decimal.TryParse(ProductPriceTextBox.Text, out price) && decimal.TryParse(ProductTaxTextBox.Text, out tax))
                {
                    
                    var newProduct = new Product(name, price, tax);

                    // Insert the product into the database
                    DatabaseHelper.InsertProduct(newProduct);
                    MessageBox.Show("Product added successfully!");
                }

                ProductNameTextBox.Text = "";
                ProductPriceTextBox.Text = "";
                ProductTaxTextBox.Text = "";
            }

            addProductDockPanel.Visibility = Visibility.Collapsed;
            ProductList.Height += (addProductDockPanel.Height + 2);
        }

        private void ModalCancelProduct_Click(object sender, RoutedEventArgs e)
        {
            addProductDockPanel.Visibility = Visibility.Collapsed;
            ProductList.Height += (addProductDockPanel.Height + 2);

            ProductNameTextBox.Text = "";
            ProductPriceTextBox.Text = "";
            ProductTaxTextBox.Text = "";
        }


    }
}
