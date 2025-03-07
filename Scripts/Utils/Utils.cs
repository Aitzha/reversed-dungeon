using Godot;
using System;

public partial class Utils : Node
{
    public static string ToSnakeCase(Type type)
    {
        string result = "";
        foreach (char c in type.ToString())
        {
            if (char.IsUpper(c) && result.Length > 0)
                result += "_";
            result += char.ToLower(c);
        }
        return result;
    }
    
    public static string ToSnakeCase(Enum value)
    {
        string result = "";
        foreach (char c in value.ToString())
        {
            if (char.IsUpper(c) && result.Length > 0)
                result += "_";
            result += char.ToLower(c);
        }
        return result;
    }
}
