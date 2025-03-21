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
            List<Product> products = DatabaseHelper.FetchAllProducts();
            LoadProducts(products);
        }

        private void LoadProducts(List<Product> products)
        {
            ProductList.Children.Clear();

            foreach (var product in products)
            {
                // Create a StackPanel for each product
                StackPanel productPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Width = 700,
                    Height = 60,
                    Margin = new Thickness(5)
                };

                // Create a TextBlock for product name
                TextBlock nameTextBlock = new TextBlock
                {
                    Text = product.Name,
                    VerticalAlignment = VerticalAlignment.Center,
                    Width = 200

                };
                nameTextBlock.Margin = new Thickness(20);

                // Create a TextBlock for product price
                TextBlock priceTextBlock = new TextBlock
                {
                    Text = product.Price.ToString(), // Format as currency
                    VerticalAlignment = VerticalAlignment.Center,
                    Width = 100
                };

                // Create a TextBlock for product tax percentage
                TextBlock taxTextBlock = new TextBlock
                {
                    Text = product.TaxPercentage.ToString() + "%",
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
                    Tag = product.Id
                };

                //editButton.Click += EditButton_Click;

                // Create Delete button
                Button deleteButton = new Button
                {
                    Content = "Delete",
                    Margin = new Thickness(5),
                    Width = 100,
                    Height = 30,
                    Tag = product.Id
                };
                
                //deleteButton.Click += DeleteButton_Click;

                // Add elements to the StackPanel
                productPanel.Children.Add(nameTextBlock);
                productPanel.Children.Add(priceTextBlock);
                productPanel.Children.Add(taxTextBlock);
                productPanel.Children.Add(editButton);
                productPanel.Children.Add(deleteButton);

                // Add the product panel to the ProductList StackPanel
                ProductList.Children.Add(productPanel);
            }
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

            List<Product> products = DatabaseHelper.FetchAllProducts();
            LoadProducts(products);
            LabelStack.Visibility = Visibility.Collapsed;

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
