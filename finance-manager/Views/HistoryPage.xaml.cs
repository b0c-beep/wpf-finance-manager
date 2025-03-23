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
using finance_manager.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Automation;
using Microsoft.Win32;
using System.IO.Compression;

namespace finance_manager.Views
{
    /// <summary>
    /// Interaction logic for HistoryPage.xaml
    /// </summary>
    public partial class HistoryPage : Page
    {
        public HistoryPage()
        {
            InitializeComponent();
            showFiles(ExcelHelper.loadFiles());

        }

        private void showFiles(List<string> files)
        {
            historyList.Children.Clear();

            foreach (string file in files)
            {
                string fileName = System.IO.Path.GetFileName(file);
                Match match = ExcelHelper.datePattern.Match(fileName);

                string displayText = match.Success
                    ? $"Financial Report - {match.Groups[1].Value}/{match.Groups[2].Value}/{match.Groups[3].Value}"
                    : fileName;

                // Create a horizontal StackPanel
                StackPanel filePanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Margin = new Thickness(5),
                    Width = 690,
                    Height = 50,
                };

                // File name TextBlock
                TextBlock fileNameText = new TextBlock
                {
                    Text = displayText,
                    VerticalAlignment = VerticalAlignment.Center,
                    Width = 550
                };

                // Download Button
                Button downloadButton = new Button
                {
                    Content = "Download",
                    Width = 60,
                    Height = 30,
                    Margin = new Thickness(10, 0, 0, 0),
                    Tag = file
                };
                downloadButton.Click += downloadFileButton_Click;

                // Add elements to the panel
                filePanel.Children.Add(fileNameText);
                filePanel.Children.Add(downloadButton);

                // Add panel to historyList
                historyList.Children.Add(filePanel);
            }

        }

        private void exportZipButton_Click(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(ExcelHelper.ResourcesFolder))
            {
                MessageBox.Show("No reports found!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                FileName = "FinancialReports.zip",
                Filter = "ZIP Files (*.zip)|*.zip",
                Title = "Save Financial Reports ZIP"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    string tempZipPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "FinancialReports.zip");

                    if (File.Exists(tempZipPath))
                        File.Delete(tempZipPath);

                    ZipFile.CreateFromDirectory(ExcelHelper.ResourcesFolder, tempZipPath);
                    File.Copy(tempZipPath, saveFileDialog.FileName, true);

                    //MessageBox.Show("All files downloaded as ZIP successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error while creating ZIP: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void downloadFileButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;
            if (clickedButton != null)
            {
                string sourceFilePath = clickedButton.Tag as string;
                if (File.Exists(sourceFilePath))
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog
                    {
                        FileName = System.IO.Path.GetFileName(sourceFilePath),
                        Filter = "Excel Files (*.xlsx)|*.xlsx",
                        Title = "Save Financial Report"
                    };

                    if (saveFileDialog.ShowDialog() == true)
                    {
                        try
                        {
                            File.Copy(sourceFilePath, saveFileDialog.FileName, true);
                            //MessageBox.Show("File downloaded successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        catch (Exception ex)
                        {
                            //MessageBox.Show("Error while saving file: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                else
                {
                    //MessageBox.Show("File not found!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
