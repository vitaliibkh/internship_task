using System.Globalization;
using System.Reflection;
using project.Models;

namespace project.Utils;

public class Parser(List<FieldConfig> configs) {
    private readonly List<FieldConfig> _configs = configs;

    public T Parse<T>(string line) where T : new() {
        T entity = new();
        Type type = typeof(T);

        foreach (var field in _configs) {
            if (line.Length < field.StartIndex + field.Length) {
                throw new Exception("invalid row length");
            }

            string rawValue = line.Substring(field.StartIndex, field.Length).Trim();
            object parsedValue = ConvertType(rawValue, field.Type);

            PropertyInfo? prop = type.GetProperty(field.PropertyName) ?? throw new Exception($"property '{field.PropertyName}' not found in model '{type.Name}'");
            prop.SetValue(entity, parsedValue);
        }

        return entity;
    }

    private static object ConvertType(string value, string targetType) {
        return (object)targetType switch {
            "Integer" => int.Parse(value),
            "Long" => long.Parse(value),
            "String" => value,
            "Date" => DateTime.ParseExact(value, "yyyyMMdd", CultureInfo.InvariantCulture),
            "DateTime" => DateTime.ParseExact(value, "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
            "Money" => ParseMoney(value),
            _ => value
        };
    }

    private static decimal ParseMoney(string value) {
        if (long.TryParse(value, out long cents)) {
            return (decimal)cents / 100;
        }

        throw new FormatException("incorrect money format");
    }
}
