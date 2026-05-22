using System.Reflection;

namespace SC.SenseTower.Common.Extensions
{
    public static class ObjectExtensions
    {
        public static Dictionary<string, string?> ToDictionary(this object source, string parentName = "", bool isArrayElement = false, BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
        {
            var properties = source.GetType().GetProperties();
            var result = new Dictionary<string, string?>();
            properties.Where(r => !r.PropertyType.IsArray).ForEach(r =>
            {
                var key = parentName + (isArrayElement ? '[' + r.Name + ']' : r.Name);
                if ((r.PropertyType.IsClass || r.PropertyType.IsInterface) && r.PropertyType.FullName != typeof(string).ToString())
                {
                    var value = r.GetValue(source, null);
                    if (value != null)
                    {
                        result = result.Union(value.ToDictionary(key + ".")).ToDictionary(k => k.Key, v => v.Value);
                    }
                }
                else
                {
                    var value = r.GetValue(source, null);
                    result.Add(key, value?.ToString() ?? string.Empty);
                }
            });
            properties.Where(r => r.PropertyType.IsArray).ForEach(r =>
            {
                var i = 0;
                var arrayValue = r.GetValue(source);
                if (arrayValue != null)
                {
                    foreach (var value in arrayValue as Array)
                    {
                        var type = value.GetType();
                        var key = $"{parentName}{r.Name}[{i++}]";
                        if (type.IsClass)
                        {
                            result = result.Union(value.ToDictionary(key, true)).ToDictionary(x => x.Key, x => x.Value);
                        }
                        else
                            result.Add(key, value?.ToString() ?? string.Empty);
                    }
                }
            });
            return result;
        }
    }
}
