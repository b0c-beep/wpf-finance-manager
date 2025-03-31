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
using finance_manager.Models;
using finance_manager.Data;
using System.Runtime.Serialization;
using SkiaSharp;
using ScottPlot;
using ScottPlot.Plottables;
using DocumentFormat.OpenXml.Math;
using DocumentFormat.OpenXml.Drawing;

namespace finance_manager.Views
{
    /// <summary>
    /// Interaction logic for StatisticsPage.xaml
    /// </summary>
    public partial class StatisticsPage : Page
    {

        public StatisticsPage()
        {
            InitializeComponent();
            CreateGraph1();
            
        }

        private void CreateGraph1()
        {
            List<Profit> profits = DatabaseHelper.FetchAllProfits();
            List<Cost> costs = DatabaseHelper.FetchAllCosts();
            double totalProfits = 0;
            double totalCosts = 0;
            double totalTax = 0;

            foreach (Profit profit in profits)
            {
                totalProfits += (double)profit.Price;
                totalTax += (double)profit.TaxAmount;
            }

            foreach (Cost cost in costs)
            {
                totalCosts += (double)cost.Price;
                totalTax += (double)cost.TaxAmount;
            }

            // Data for the chart
            double[] values = { totalProfits, totalCosts, totalTax }; 
            string[] labels = { "Total Profit", "Total Costs", "Total Tax" };


            plot1.Plot.Clear();

           

            var bars = plot1.Plot.Add.Bars(values);

            plot1.Plot.Axes.Margins(bottom: 0);

            foreach(var bar in bars.Bars)
            {
                bar.Label = bar.Value.ToString();
            }

            bars.Bars[0].FillColor = ScottPlot.Colors.DarkGreen;
            bars.Bars[1].FillColor = ScottPlot.Colors.Crimson;
            bars.Bars[2].FillColor = ScottPlot.Colors.DodgerBlue;

            bars.ValueLabelStyle.Bold = true;
            bars.ValueLabelStyle.FontSize = 16;

            Tick[] ticks =
            {
                new(0, "Profits"),
                new(1, "Costs"),
                new(2, "Taxes"),
            };

            plot1.Plot.FigureBackground.Color = ScottPlot.Color.FromHex("#E3ECF8");
            plot1.Plot.DataBackground.Color = ScottPlot.Color.FromHex("#E3ECF8");
            plot1.Plot.Grid.MajorLineColor = ScottPlot.Color.FromHex("#E3ECF8");

            plot1.Plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(ticks);
            plot1.Plot.Axes.AutoScale();

            plot1.Refresh();

        }
    }
}
