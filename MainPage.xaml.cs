using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Compatibility;
using System;
using System.Collections.Generic;
using CommunityToolkit.Maui;
using Grid = Microsoft.Maui.Controls.Grid;
using Microsoft.Maui.Storage;
using CommunityToolkit.Maui.Storage;
using System.Text;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;
using System.Text.RegularExpressions;

namespace Lab1MAUI
{
    public partial class MainPage : ContentPage
    {
        int countColumn = 3; // кількість стовпчиків (A to Z)
        int countRow = 3; // кількість рядків
        List<List<EntryStructure>> cells = new List<List<EntryStructure>>();

        public MainPage()
        {
            InitializeComponent();
            CreateGrid();

            for (int i = 0; i < countColumn; i++)
            {
                cells.Add(new List<EntryStructure>());
            }
        }

        //створення таблиці
        private void CreateGrid()
        {
            AddColumnsAndColumnLabels();
            AddRowsAndCellEntries();
        }

        private void AddColumnsAndColumnLabels()
        {
            grid.RowDefinitions.Add(new RowDefinition());
            // Додати стовпці та підписи для стовпців
            for (int col = 0; col < countColumn + 1; col++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                if (col > 0)
                {
                    var label = new Label
                    {
                        Text = GetColumnName(col),
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center
                    };
                    Grid.SetRow(label, 0);
                    Grid.SetColumn(label, col);
                    grid.Children.Add(label);
                }
            }
        }

        private void AddRowsAndCellEntries()
        {
            for (int i = 0; i < countColumn; i++)
            {
                cells.Add(new List<EntryStructure>());
            }

            // Додати рядки, підписи для рядків та комірки
            for (int row = 0; row < countRow; row++)
            {
                grid.RowDefinitions.Add(new RowDefinition());
                // Додати підпис для номера рядка
                var label = new Label
                {
                    Text = (row + 1).ToString(),
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center
                };
                Grid.SetRow(label, row + 1);
                Grid.SetColumn(label, 0);
                grid.Children.Add(label);
                // Додати комірки (Entry) для вмісту
                for (int col = 0; col < countColumn; col++)
                {
                    var entry = new Entry
                    {
                        Text = "",
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center
                    };
                    entry.Unfocused += Entry_Unfocused; // обробник події Unfocused
                    entry.Focused += Entry_Focused;
                    Grid.SetRow(entry, row + 1);
                    Grid.SetColumn(entry, col + 1);
                    grid.Children.Add(entry);

                    cells[col].Add(new EntryStructure() { Formula = String.Empty, Entry = entry });
                }
            }
        }

        private string GetColumnName(int colIndex)
        {
            int dividend = colIndex;
            string columnName = string.Empty;
            while (dividend > 0)
            {
                int modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo) + columnName;
                dividend = (dividend - modulo) / 26;
            }
            return columnName;
        }

        // викликається, коли користувач вийде зі зміненої клітинки (втратить фокус)
        private void Entry_Unfocused(object sender, FocusEventArgs e)
        {
            var entry = (Entry)sender;
            int row = Grid.GetRow(entry) - 1;
            int col = Grid.GetColumn(entry) - 1;
            string content = entry.Text;

            cells[col][row].Formula = content;
        }

        private void Entry_Focused(object sender, FocusEventArgs e)
        {
            var entry = (Entry)sender;
            var row = Grid.GetRow(entry) - 1;
            var col = Grid.GetColumn(entry) - 1;
            var content = entry.Text;

            cells[col][row].Entry.Text = cells[col][row].Formula;
        }

        private void CalculateButton_Clicked(object sender, EventArgs e)
        {
            for (int i = 0; i < countColumn; i++)
            {
                for (int j = 0; j < countRow; j++)
                {
                    CalculateCell(i, j);
                }
            }
        }

        private void CalculateCell(int i, int j)
        {
            try
            {
                if (cells[i][j].Formula == String.Empty)
                    return;

                if (float.TryParse(cells[i][j].Formula, out float val))
                {
                    cells[i][j].Entry.Text = val.ToString();
                    return;
                }

                //string result = CalcLink(cells[i][j].Formula);

                string result = Calculator.Evaluate(CalcLink(cells[i][j].Formula)).ToString();

                if (result == "∞")
                    cells[i][j].Entry.Text = "ERROR";
                else
                    cells[i][j].Entry.Text = result;
            }
            catch
            {
                cells[i][j].Entry.Text = "ERROR";
            }
        }

        private string CalcLink(string input)
        {
            if (string.IsNullOrEmpty(input))
                return null;

            StringBuilder output = new StringBuilder();
            Dictionary<string, string> cellValues = new Dictionary<string, string>();

            for (int i = 0; i < input.Length; i++)
            {
                if (char.IsLetter(input[i]) && i + 1 < input.Length && char.IsDigit(input[i + 1]))
                {
                    int col = char.ToUpper(input[i]) - 'A';
                    int j = i + 1;
                    while (j < input.Length && char.IsDigit(input[j]))
                    {
                        j++;
                    }
                    int row = int.Parse(input.Substring(i + 1, j - i - 1)) - 1;
                    string cellAddress = $"{char.ToUpper(input[i])}{row + 1}";

                    if (!cellValues.ContainsKey(cellAddress))
                    {
                        string cellValue;
                        if (col < cells.Count && row < cells[col].Count && cells[col][row].Formula != string.Empty)
                        {
                            // Рекурсивно обчислюємо значення формули
                            cellValue = CalcLink(cells[col][row].Formula);
                            cellValues[cellAddress] = cellValue;
                        }
                        else if (col < cells.Count && row < cells[col].Count)
                        {
                            // Якщо клітинка не містить формули, використовуємо поточне значення
                            cellValue = cells[col][row].Entry.Text;
                            cellValues[cellAddress] = cellValue;
                        }
                        else
                        {
                            // Якщо клітинка не існує, повертаємо помилку
                            return "ERROR";
                        }
                    }

                    output.Append(cellValues[cellAddress]);
                    i = j - 1; // Пропускаємо оброблені символи
                }
                else
                {
                    output.Append(input[i]);
                }
            }

            return output.ToString().Replace(" ", "");
        }

        private int NextNumber(string input, int index)
        {
            bool isNum = int.TryParse(input.Substring(index), out int num);

            if (isNum)

                return 10 * num + NextNumber(input, ++index);
            else
                return 0;
        }

        public static int CountDigits(int number)
        {
            if (number == 0)
            {
                return 1;
            }

            int count = 0;
            while (number != 0)
            {
                number /= 10;
                count++;
            }
            return count;
        }

        private async void SaveButton_Clicked(object sender, EventArgs e)
        {
            int currentCountRow = grid.RowDefinitions.Count;
            int currentCountColumn = grid.ColumnDefinitions.Count;

            string[][] cellValues = new string[currentCountRow][];

            for (int i = 0; i < currentCountRow; i++)
            {
                cellValues[i] = new string[currentCountColumn];
            }

            List<string> results = new List<string>();

            foreach (var child in grid.Children.OfType<Entry>())
            {
                int row = grid.GetRow(child);
                int col = grid.GetColumn(child);

                if (row == 0 || col == 0)
                    continue;

                cellValues[row - 1][col - 1] = ((Entry)child).Text;
            }

            bool saveResult = await FileOperation.Save(cellValues);
            string strSaveResult = saveResult ? "Файл успішно збережено." : "Файл не збережено";
            await DisplayAlert("Результат збереження", strSaveResult, "Ок");

        }

        private async void ReadButton_Clicked(object sender, EventArgs e)
        {
            string[,] results = await FileOperation.Open();

            if (results == null)
                return;

            foreach (var child in grid.Children.OfType<Entry>())
            {
                int row = grid.GetRow(child);
                int col = grid.GetColumn(child);

                if (row == 0 || col == 0)
                    continue;
                try
                {
                    ((Entry)child).Text = results[row - 1, col - 1];
                }
                catch { }
            }
        }

        private async void ExitButton_Clicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Підтвердження", "Ви дійсно хочете вийти?",
           "Так", "Ні");
            if (answer)
            {
                System.Environment.Exit(0);
            }
        }

        private async void HelpButton_Clicked(object sender, EventArgs e)
        {
            await DisplayAlert("Довідка", "Лабораторна робота 1. Студентки Бивалькевич Софії Григорівни. Варіант 6",
           "OK");
        }

        private void DeleteRowButton_Clicked(object sender, EventArgs e)
        {
            if (grid.RowDefinitions.Count > 2)
            {
                int columnValue = grid.ColumnDefinitions.Count;
                int rowValue = grid.RowDefinitions.Count;
                int elementCount = grid.Children.Count;

                for (int i = 0; i < columnValue; i++)
                {
                    foreach (var child in grid.Children)
                    {
                        if (grid.GetRow(child) == rowValue - 1 && grid.GetColumn(child) == i)
                        {
                            grid.Children.Remove(child);
                            break;
                        }
                    }
                }
                grid.RowDefinitions.RemoveAt(grid.RowDefinitions.Count - 1);
            }

            countRow--;
            for (int i = 0; i < countColumn; i++)
            {
                cells[i].RemoveAt(countRow);
            }
        }


        private void DeleteColumnButton_Clicked(object sender, EventArgs e)
        {
            if (grid.ColumnDefinitions.Count > 2)
            {
                int rowValue = grid.RowDefinitions.Count;
                int columnValue = grid.ColumnDefinitions.Count;
                int elementCount = grid.Children.Count;

                for (int i = 0; i <= rowValue; i++)
                {
                    foreach (var child in grid.Children)
                    {
                        if (grid.GetRow(child) == i && grid.GetColumn(child) == columnValue - 1)
                        {
                            grid.Children.Remove(child);
                            break;
                        }
                    }
                    //grid.Children.RemoveAt(elementCount - 1 - i * columnValue);
                }
                grid.ColumnDefinitions.RemoveAt(grid.ColumnDefinitions.Count - 1);
            }

            countColumn--;
            cells.RemoveAt(countColumn);
        }
        private void AddRowButton_Clicked(object sender, EventArgs e)
        {
            int newRow = grid.RowDefinitions.Count;
            int countColumn = grid.ColumnDefinitions.Count;

            countRow++;
            // Add a new row definition
            grid.RowDefinitions.Add(new RowDefinition());
            // Add label for the row number
            var label = new Label
            {
                Text = newRow.ToString(),
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };
            Grid.SetRow(label, newRow);
            Grid.SetColumn(label, 0);
            grid.Children.Add(label);
            // Add entry cells for the new row
            for (int col = 0; col < countColumn - 1; col++)
            {
                var entry = new Entry
                {
                    Text = "",
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center
                };
                entry.Unfocused += Entry_Unfocused;
                entry.Focused += Entry_Focused;
                Grid.SetRow(entry, newRow);
                Grid.SetColumn(entry, col + 1);
                grid.Children.Add(entry);

                cells[col].Add(new EntryStructure() { Formula = String.Empty, Entry = entry });
            }
        }

        private void AddColumnButton_Clicked(object sender, EventArgs e)
        {
            int newColumn = grid.ColumnDefinitions.Count;
            int countRow = grid.RowDefinitions.Count;

            countColumn++;
            cells.Add(new List<EntryStructure>());

            // Add a new column definition
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            // Add label for the column name
            var label = new Label
            {
                Text = GetColumnName(newColumn),
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };
            Grid.SetRow(label, 0);
            Grid.SetColumn(label, newColumn);
            grid.Children.Add(label);
            // Add entry cells for the new column
            for (int row = 0; row < countRow - 1; row++)
            {
                var entry = new Entry
                {
                    Text = "",
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center
                };
                entry.Unfocused += Entry_Unfocused;
                entry.Focused += Entry_Focused;
                Grid.SetRow(entry, row + 1);
                Grid.SetColumn(entry, newColumn);
                grid.Children.Add(entry);

                cells[countColumn - 1].Add(new EntryStructure() { Formula = String.Empty, Entry = entry });
            }
        }
    }
}
