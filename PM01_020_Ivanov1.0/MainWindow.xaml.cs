using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace PM01_020_Ivanov1._0
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Solve_Click(object sender, RoutedEventArgs e)
        {
            // Получение данных из текстовых полей
            int[] demand = Array.ConvertAll(txtDemand.Text.Split(','), int.Parse);
            int[] supply = Array.ConvertAll(txtSupply.Text.Split(','), int.Parse);
            int[][] costMatrix = txtCostMatrix.Text.Split(';')
                .Select(row => row.Split(',').Select(int.Parse).ToArray())
                .ToArray();

            // Решение транспортной задачи
            string result = SolveTransportProblemNorthWest(demand, supply, costMatrix);

            // Вывод результата
            txtResult.Text = result;
        }

        private void LoadData_Click(object sender, RoutedEventArgs e)
        {
            string filePath = string.Empty;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                filePath = openFileDialog.FileName;

                try
                {
                    string[] lines = File.ReadAllLines(filePath);

                    if (lines.Length < 3)
                    {
                        MessageBox.Show("Ошибка: Недостаточно строк для загрузки данных из файла.");
                        return;
                    }

                    txtDemand.Text = lines[0];
                    txtSupply.Text = lines[1];
                    txtCostMatrix.Text = string.Join(";", lines.Skip(2));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Произошла ошибка при загрузке данных из файла: " + ex.Message);
                }
            }
        }

        // Обработчик события для кнопки сохранения результата
        private void SaveResult_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
            {
                // Запись результата в файл
                File.WriteAllText(saveFileDialog.FileName, txtResult.Text);
            }
        }

        private string SolveTransportProblem(int[] demand, int[] supply, int[][] costMatrix)
        {
            int totalDemand = demand.Sum();
            int totalSupply = supply.Sum();

            if (totalDemand != totalSupply)
            {
                return "Ошибка: сумма потребностей не равна сумме предложений!";
            }

            int numRows = costMatrix.Length;
            int numCols = costMatrix[0].Length;

            int[][] result = new int[numRows][];
            for (int i = 0; i < numRows; i++)
            {
                result[i] = new int[numCols];
            }

            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    result[i][j] = 0;
                }
            }

            int[] supplyRemaining = new int[supply.Length];
            Array.Copy(supply, supplyRemaining, supply.Length);

            int[] demandRemaining = new int[demand.Length];
            Array.Copy(demand, demandRemaining, demand.Length);

            while (supplyRemaining.Any(x => x > 0) && demandRemaining.Any(x => x > 0))
            {
                int minCost = int.MaxValue;
                int minCostRow = -1;
                int minCostCol = -1;

                for (int i = 0; i < numRows; i++)
                {
                    for (int j = 0; j < numCols; j++)
                    {
                        if (supplyRemaining[i] > 0 && demandRemaining[j] > 0 && costMatrix[i][j] < minCost)
                        {
                            minCost = costMatrix[i][j];
                            minCostRow = i;
                            minCostCol = j;
                        }
                    }
                }

                int amountTransferred = Math.Min(supplyRemaining[minCostRow], demandRemaining[minCostCol]);
                result[minCostRow][minCostCol] = amountTransferred;
                supplyRemaining[minCostRow] -= amountTransferred;
                demandRemaining[minCostCol] -= amountTransferred;
            }

            string resultString = "Опорный план:\n";

            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    resultString += result[i][j] + "\t";
                }
                resultString += "\n";
            }

            return resultString;
        }

        private string SolveTransportProblemNorthWest(int[] demand, int[] supply, int[][] costMatrix)
        {
            int totalDemand = demand.Sum();
            int totalSupply = supply.Sum();

            if (totalDemand != totalSupply)
            {
                return "Ошибка: сумма потребностей не равна сумме предложений!";
            }

            int numRows = costMatrix.Length;
            int numCols = costMatrix[0].Length;

            int[][] result = new int[numRows][];
            for (int i = 0; i < numRows; i++)
            {
                result[i] = new int[numCols];
            }

            int supplyIndex = 0;
            int demandIndex = 0;

            while (supplyIndex < supply.Length && demandIndex < demand.Length)
            {
                int currentSupply = supply[supplyIndex];
                int currentDemand = demand[demandIndex];

                int amountTransferred = Math.Min(currentSupply, currentDemand);

                result[supplyIndex][demandIndex] = amountTransferred;

                supply[supplyIndex] -= amountTransferred;
                demand[demandIndex] -= amountTransferred;

                if (supply[supplyIndex] == 0)
                {
                    supplyIndex++;
                }

                if (demand[demandIndex] == 0)
                {
                    demandIndex++;
                }
            }

            string resultString = "Опорный план (северо-западный метод):\n";

            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    resultString += result[i][j] + "\t";
                }
                resultString += "\n";
            }

            return resultString;
        }

        private void txtDemand_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void txtCostMatrix_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}