// I got help from AI in this part for JSON Exports

using System.Dynamic;
using System.Text.Json;

namespace Ceng382_week5.Helpers
{
    public sealed class Utils
    {
        private static readonly Lazy<Utils> lazy = new Lazy<Utils>(() => new Utils());
        public static Utils Instance => lazy.Value;
        
        private Utils() {}

        public string ExportToJson<T>(IEnumerable<T> data, string[] selectedColumns = null)
        {
            if (selectedColumns != null && selectedColumns.Length > 0)
            {
                var filteredData = new List<ExpandoObject>();
                foreach (var item in data)
                {
                    dynamic expando = new ExpandoObject();
                    var dict = (IDictionary<string, object>)expando;
                    
                    foreach (var prop in selectedColumns)
                    {
                        var value = item.GetType().GetProperty(prop)?.GetValue(item);
                        dict.Add(prop, value);
                    }
                    filteredData.Add(expando);
                }
                return JsonSerializer.Serialize(filteredData, new JsonSerializerOptions { WriteIndented = true });
            }
            
            return JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        }
    }
}