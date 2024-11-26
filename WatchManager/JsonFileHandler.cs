using System.Text.Json;

public class JsonFileHandler
{
    public static List<Watch> LoadWatches(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                return new List<Watch>(); // Якщо файл не існує, повертаємо порожній список.
            }

            string jsonContent = File.ReadAllText(filePath); // Читаємо вміст файлу.
            return JsonSerializer.Deserialize<List<Watch>>(jsonContent) ?? new List<Watch>(); // Десеріалізуємо JSON у список годинників.
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка під час читання файлу: {ex.Message}");
            return new List<Watch>();
        }
    }

    public static void SaveWatches(string filePath, List<Watch> watches)
    {
        try
        {
            var jsonString = JsonSerializer.Serialize(watches, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, jsonString);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка під час збереження файлу: {ex.Message}");
        }
    }

    public static List<Watch> DeserializeWatches(string jsonContent)
    {
        try
        {
            return 
                JsonSerializer.Deserialize<List<Watch>>(jsonContent);
        }
        catch
        {
            return null;
        }
    }

    public static bool IsValidWatchData(List<Watch> watches)
    {
        return watches != null && watches.All(w => w.Id > 0 && !string.IsNullOrWhiteSpace(w.Brand));
    }

    public static void CopyFile(string sourceFilePath, string destinationFilePath)
    {
        try
        {
            File.Copy(sourceFilePath, destinationFilePath, true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка під час копіювання файлу: {ex.Message}");
        }
    }
}
