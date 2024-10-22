using CommunityToolkit.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1MAUI
{
    internal static class FileOperation
    {
        async public static Task<bool> Save(string[][] cellValues)
        {
            string data = string.Join("\n", cellValues.Select(row => string.Join("\t", row)));

            var stream = new MemoryStream(Encoding.Default.GetBytes(data));

            var fileSaveResult = await FileSaver.Default.SaveAsync("test.txt", stream);

            return fileSaveResult.IsSuccessful ? true : false;
        }

        async public static Task<string[,]> Open()
        {
            var file = await FilePicker.PickAsync();

            if (file == null)
            {
                return null; // Користувач скасував вибір файлу
            }

            try
            {
                using (var stream = await file.OpenReadAsync())
                using (var reader = new StreamReader(stream))
                {
                    List<string[]> data = new List<string[]>();
                    while (!reader.EndOfStream)
                    {
                        string line = await reader.ReadLineAsync();
                        data.Add(line.Split('\t'));
                    }

                    string[,] result = new string[data.Count, data[0].Length];
                    for (int i = 0; i < data.Count; i++)
                    {
                        for (int j = 0; j < data[i].Length; j++)
                        {
                            result[i, j] = data[i][j];
                        }
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                // Обробка помилки
                Console.WriteLine($"Помилка під час читання файлу: {ex.Message}");
                return null;
            }
        }
    }
}
