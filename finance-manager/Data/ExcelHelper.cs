using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;
using finance_manager.Models;
using Microsoft.Win32;
using finance_manager.Services;
using System.IO;
using System.Windows;

namespace finance_manager.Data
{
    class ExcelHelper
    {
        private static readonly string AppFolder = Path.GetFullPath(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\"));
        private static readonly string ResourcesFolder = Path.Combine(AppFolder, "Resources");

        public static void CreateProfitSheet(IXLWorksheet sheet, string sheetTitle, List<Profit> profits)
        {
            // Headers
            sheet.Cell(1, 1).Value = "ID";
            sheet.Cell(1, 2).Value = "Name";
            sheet.Cell(1, 3).Value = "Price (RON)";
            sheet.Cell(1, 4).Value = "Tax % (RON)";
            sheet.Cell(1, 5).Value = "Calc Tax (RON)";
            sheet.Cell(1, 6).Value = "Date";

            sheet.Row(1).Style.Font.Bold = true;
            sheet.Row(1).Style.Fill.BackgroundColor = XLColor.LightGray;

            decimal totalPrice = 0;
            decimal totalTax = 0;

            int row = 2; // Start at row 2 (row 1 is headers)
            foreach (Profit profit in profits)
            {
                sheet.Cell(row, 1).Value = profit.Id;
                sheet.Cell(row, 2).Value = profit.Name;
                sheet.Cell(row, 3).Value = profit.Price;
                sheet.Cell(row, 4).Value = profit.TaxPercentage;
                sheet.Cell(row, 5).Value = profit.TaxAmount;
                sheet.Cell(row, 6).Value = profit.Date;

                totalPrice += profit.Price;
                totalTax += profit.TaxAmount;
                row++;
            }

            // Add Total Row
            sheet.Cell(row, 2).Value = "Total:";
            sheet.Cell(row, 3).Value = totalPrice;
            sheet.Cell(row, 5).Value = totalTax;
            sheet.Row(row).Style.Font.Bold = true;

            // Adjust column widths
            sheet.Columns().AdjustToContents();
        }

        public static void CreateCostSheet(IXLWorksheet sheet, string sheetTitle, List<Cost> costs)
        {
            // Headers
            sheet.Cell(1, 1).Value = "ID";
            sheet.Cell(1, 2).Value = "Name";
            sheet.Cell(1, 3).Value = "Price (RON)";
            sheet.Cell(1, 4).Value = "Tax % (RON)";
            sheet.Cell(1, 5).Value = "Calc Tax (RON)";
            sheet.Cell(1, 6).Value = "Date";

            sheet.Row(1).Style.Font.Bold = true;
            sheet.Row(1).Style.Fill.BackgroundColor = XLColor.LightGray;

            decimal totalPrice = 0;
            decimal totalTax = 0;

            int row = 2; // Start at row 2 (row 1 is headers)
            foreach (Cost cost in costs)
            {
                sheet.Cell(row, 1).Value = cost.Id;
                sheet.Cell(row, 2).Value = cost.Name;
                sheet.Cell(row, 3).Value = cost.Price;
                sheet.Cell(row, 4).Value = cost.TaxPercentage;
                sheet.Cell(row, 5).Value = cost.TaxAmount;
                sheet.Cell(row, 6).Value = cost.Date;

                totalPrice += cost.Price;
                totalTax += cost.TaxAmount;
                row++;
            }

            // Add Total Row
            sheet.Cell(row, 2).Value = "Total:";
            sheet.Cell(row, 3).Value = totalPrice;
            sheet.Cell(row, 5).Value = totalTax;
            sheet.Row(row).Style.Font.Bold = true;

            // Adjust column widths
            sheet.Columns().AdjustToContents();
        }

        public static void ProfitAndCostToExcel()
        {
            List<Profit> profits = DatabaseHelper.FetchAllProfits();
            List<Cost> costs = DatabaseHelper.FetchAllCosts();

            // Ask user where to save the file
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel Files|*.xlsx",
                Title = "Save the Financial Report",
                FileName = DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Year.ToString() + "_FinancialReport"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;

                try
                {
                    using (var workbook = new XLWorkbook())
                    {
                        // Create "Profits" Sheet
                        var profitSheet = workbook.Worksheets.Add("Profits");
                        CreateProfitSheet(profitSheet, "Profits", profits);

                        // Create "Costs" Sheet
                        var costSheet = workbook.Worksheets.Add("Costs");
                        CreateCostSheet(costSheet, "Costs", costs);

                        // Save the Excel file
                        workbook.SaveAs(filePath);
                    }

                    //MessageBox.Show("Export successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error exporting data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public static void ExportCurrentMonth()
        {
            List<Profit> profits = DatabaseHelper.FetchAllProfits();
            List<Cost> costs = DatabaseHelper.FetchAllCosts();


            string FileName = DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Year.ToString();
            string filePath = Path.Combine(ResourcesFolder, FileName + "_FinancialReport.xlsx");

            try
            {
                using (var workbook = new XLWorkbook())
                {
                        
                        // Create "Profits" Sheet
                        var profitSheet = workbook.Worksheets.Add("Profits");
                        CreateProfitSheet(profitSheet, "Profits", profits);

                        // Create "Costs" Sheet
                        var costSheet = workbook.Worksheets.Add("Costs");
                        CreateCostSheet(costSheet, "Costs", costs);

                        // Save the Excel file
                        workbook.SaveAs(filePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }
    }
}
