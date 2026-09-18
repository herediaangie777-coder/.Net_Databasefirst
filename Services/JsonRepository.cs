using System.Text.Json;

namespace ProyectoConsolaObjetos1.Services;

public sealed class JsonRepository<T>
{
    private readonly string filePath;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public JsonRepository(string fileName, string? directory = null)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("El nombre del archivo es obligatorio.", nameof(fileName));
        }

        string safeFileName = Path.GetFileName(fileName);
        if (!safeFileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
        {
            safeFileName += ".json";
        }

        string dataDirectory = directory ?? Path.Combine(AppContext.BaseDirectory, "output");
        filePath = Path.Combine(dataDirectory, safeFileName);
    }

    public List<T> Load()
    {
        try
        {
            if (!File.Exists(filePath))
            {
                return new List<T>();
            }

            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<T>>(json, JsonOptions) ?? new List<T>();
        }
        catch (IOException)
        {
            return new List<T>();
        }
        catch (JsonException)
        {
            return new List<T>();
        }
    }

    public void Save(IEnumerable<T>? items)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
        string json = JsonSerializer.Serialize(items ?? Enumerable.Empty<T>(), JsonOptions);
        File.WriteAllText(filePath, json);
    }

    public int GetNextId(IEnumerable<T> items, Func<T, int> idSelector)
    {
        return items.Any() ? items.Max(idSelector) + 1 : 1;
    }
}