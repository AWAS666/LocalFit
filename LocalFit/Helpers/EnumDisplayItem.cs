using System;

namespace LocalFit.Helpers;

public class EnumDisplayItem<T> where T : Enum
{
    public T Value { get; set; }
    public string DisplayName { get; set; } = string.Empty;

    public EnumDisplayItem(T value, string displayName)
    {
        Value = value;
        DisplayName = displayName;
    }

    public override string ToString() => DisplayName;
}
