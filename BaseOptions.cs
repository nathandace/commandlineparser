using System.Reflection;

namespace PocketDews.CommandLineParser;

public class BaseOptions
{
    /// <summary>
    /// Writes the properties as a command line string.
    /// </summary>
    public override string ToString()
    {
        var properties = new List<string>();

        foreach (var prop in GetType().GetProperties())
        {
            if (prop.GetValue(this) == null)
            {
                continue;
            }

            var options = prop.GetCustomAttribute<OptionsAttribute>();
            var optionName = options != null ? options.Name : prop.Name;
            var value = prop.GetValue(this);

            if (value == null)
            {
                continue;
            }

            // bool data types indicate true/false based on if the
            // option names appears to the command line string.

            if (bool.TryParse(value.ToString(), out var result))
            {
                if (result)
                {
                    properties.Add(optionName);
                }
            }
            else
            {
                properties.Add($"{optionName} \"{value}\"");
            }
        }

        return string.Join(" ", properties);
    }
}