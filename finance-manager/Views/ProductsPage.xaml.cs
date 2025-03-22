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

                editButton.Click += EditButton_Click;

                // Create Delete button
                Button deleteButton = new Button
                {
                    Content = "Delete",
                    Margin = new Thickness(5),
                    Width = 100,
                    Height = 30,
                    Tag = product.Id
                };
                
                deleteButton.Click += DeleteButton_Click;

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

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Button editButton = sender as Button;
            if (editButton == null) return;

            // Get the parent productPanel
            StackPanel productPanel = editButton.Parent as StackPanel;
            if (productPanel == null) return;

            // Find all TextBlocks inside this specific product panel
            TextBlock nameTextBlock = productPanel.Children.OfType<TextBlock>().FirstOrDefault(tb => tb.Width == 200);
            TextBlock priceTextBlock = productPanel.Children.OfType<TextBlock>().FirstOrDefault(tb => tb.Width == 100);
            TextBlock taxTextBlock = productPanel.Children.OfType<TextBlock>().LastOrDefault(tb => tb.Width == 100);
            Button deleteButton = productPanel.Children.OfType<Button>().FirstOrDefault(btn => btn.Content.ToString() == "Delete");

            if (nameTextBlock == null || priceTextBlock == null || taxTextBlock == null || deleteButton == null) return;

            // Convert to TextBoxes
            TextBox nameTextBox = new TextBox { Text = nameTextBlock.Text, Width = 200, Height = 20, Margin = nameTextBlock.Margin };
            TextBox priceTextBox = new TextBox { Text = priceTextBlock.Text, Width = 100, Height = 20, Margin = priceTextBlock.Margin };
            TextBox taxTextBox = new TextBox { Text = taxTextBlock.Text.Replace("%", ""), Width = 100, Height = 20, Margin = taxTextBlock.Margin };

            // Get indexes
            int nameIndex = productPanel.Children.IndexOf(nameTextBlock);
            int priceIndex = productPanel.Children.IndexOf(priceTextBlock);
            int taxIndex = productPanel.Children.IndexOf(taxTextBlock);

            // Remove old TextBlocks
            productPanel.Children.RemoveAt(taxIndex);
            productPanel.Children.RemoveAt(priceIndex);
            productPanel.Children.RemoveAt(nameIndex);

            // Insert new TextBoxes
            productPanel.Children.Insert(nameIndex, nameTextBox);
            productPanel.Children.Insert(priceIndex, priceTextBox);
            productPanel.Children.Insert(taxIndex, taxTextBox);

            // Hide the Delete button
            deleteButton.Visibility = Visibility.Collapsed;

            // Change Edit button to "Save"
            editButton.Content = "Save";
            editButton.Click -= EditButton_Click;
            editButton.Click += SaveEditedProduct;
        }

        private void SaveEditedProduct(object sender, RoutedEventArgs e)
        {
            // Get the parent StackPanel for the product being edited
            StackPanel productPanel = (StackPanel)((Button)sender).Parent;

            // Find the TextBoxes inside the StackPanel
            TextBox nameTextBox = productPanel.Children.OfType<TextBox>().FirstOrDefault(tb => tb.Width == 200);
            TextBox priceTextBox = productPanel.Children.OfType<TextBox>().FirstOrDefault(tb => tb.Width == 100);
            TextBox taxTextBox = productPanel.Children.OfType<TextBox>().LastOrDefault(tb => tb.Width == 100);
            Button deleteButton = productPanel.Children.OfType<Button>().FirstOrDefault(btn => btn.Content.ToString() == "Delete");

            Button saveButton = sender as Button;
            int productId = (int)saveButton.Tag;

            Product originalProduct = DatabaseHelper.GetProductById(productId);
            
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
            if (!(originalProduct.Name == newName&&
                originalProduct.Price == newPrice &&
                originalProduct.TaxPercentage == newTax))
            {

                // Update in database
                DatabaseHelper.UpdateProduct(productId, newName, newPrice, newTax);

            }

            // Convert tax back to a percentage string
            string formattedTax = $"{newTax}%";

            

            // Convert back to TextBlocks
            TextBlock nameTextBlock = new TextBlock { Text = newName, Width = 200, VerticalAlignment = VerticalAlignment.Center, Margin = nameTextBox.Margin };
            TextBlock priceTextBlock = new TextBlock { Text = newPrice.ToString(), Width = 100, VerticalAlignment = VerticalAlignment.Center, Margin = priceTextBox.Margin };
            TextBlock taxTextBlock = new TextBlock { Text = formattedTax, Width = 100, VerticalAlignment = VerticalAlignment.Center, Margin = taxTextBox.Margin };

            // Get indexes
            int nameIndex = productPanel.Children.IndexOf(nameTextBox);
            int priceIndex = productPanel.Children.IndexOf(priceTextBox);
            int taxIndex = productPanel.Children.IndexOf(taxTextBox);

            // Remove old TextBlocks
            productPanel.Children.RemoveAt(taxIndex);
            productPanel.Children.RemoveAt(priceIndex);
            productPanel.Children.RemoveAt(nameIndex);

            // Insert new TextBoxes
            productPanel.Children.Insert(nameIndex, nameTextBlock);
            productPanel.Children.Insert(priceIndex, priceTextBlock);
            productPanel.Children.Insert(taxIndex, taxTextBlock);

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
            StackPanel productPanel = button.Parent as StackPanel;

            int productId = (int)button.Tag;
            DatabaseHelper.DeleteProduct(productId);
            ProductList.Children.Remove(productPanel);
            LoadProducts(DatabaseHelper.FetchAllProducts());
        }
            
        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            addProductDockPanel.Visibility = Visibility.Visible;
            ProductList.Height -= (addProductDockPanel.Height + 2);
        }

        private void ModalAddProduct_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrWhiteSpace(ProductNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(ProductPriceTextBox.Text) ||
                string.IsNullOrWhiteSpace(ProductTaxTextBox.Text))
            {
                MessageBox.Show("Please fill all the fields");
                return;
            }
            else
            {
                string name = ProductNameTextBox.Text;
                decimal price, tax;

                if (!decimal.TryParse(ProductPriceTextBox.Text, out price))
                {
                    MessageBox.Show("Invalid price. Please enter a valid number.");
                    return;
                }

                if (!decimal.TryParse(ProductTaxTextBox.Text, out tax))
                {
                    MessageBox.Show("Invalid tax percentage. Please enter a valid number.");
                    return;
                }
                    
                var newProduct = new Product(name, price, tax);

                // Insert the product into the database
                DatabaseHelper.InsertProduct(newProduct);
                //MessageBox.Show("Product added successfully!");
                

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
