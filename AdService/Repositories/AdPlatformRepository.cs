/// <summary>
/// Хранит данные в памяти. 
/// LoadFromText вызывается редко и полностью перезаписывает хранилище.
/// FindPlatforms вызывается часто: он собирает площадки, зарегистрированные
/// для запрошенной локации и её предков (например, /ru -> /ru/svrd).
/// </summary>
public class AdPlatformRepository
{
    // map: точная локация => список площадок, которые явно указаны для этой локации
    private Dictionary<string, string[]> _map = new(StringComparer.OrdinalIgnoreCase);
    private readonly object _lock = new();

    /// <summary>
    /// Загружает данные из текстового содержимого файла (полная перезапись).
    /// Формат строки: "PlatformName:/ru/svrd/revda,/ru/svrd/pervik"
    /// Неправильные строки игнорируются.
    /// </summary>
    public void LoadFromText(string text)
    {
        var tmp = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

        if (!string.IsNullOrWhiteSpace(text))
        {
            var lines = text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var raw in lines)
            {
                var line = raw.Trim();
                if (line.Length == 0) continue;

                var idx = line.IndexOf(':');
                if (idx <= 0) continue;

                var platform = line.Substring(0, idx).Trim();
                if (platform.Length == 0) continue;

                var locPart = line.Substring(idx + 1).Trim();
                if (locPart.Length == 0) continue;

                var locations = locPart.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                foreach (var lr in locations)
                {
                    var loc = NormalizeLocation(lr);
                    if (loc == null) continue;

                    if (!tmp.TryGetValue(loc, out var set))
                    {
                        set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                        tmp[loc] = set;
                    }
                    set.Add(platform);
                }
            }
        }

        // финализируем — массивы и сортировка для детерминированности
        var final = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
        foreach (var kv in tmp)
        {
            var arr = kv.Value.ToArray();
            Array.Sort(arr, StringComparer.OrdinalIgnoreCase);
            final[kv.Key] = arr;
        }

        lock (_lock)
        {
            _map = final; // атомарная замена ссылки
        }
    }

    private static string? NormalizeLocation(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return null;
        var s = input.Trim();
        if (!s.StartsWith("/")) s = "/" + s;
        if (s.Length > 1 && s.EndsWith("/")) s = s.TrimEnd('/');
        return s;
    }
}
