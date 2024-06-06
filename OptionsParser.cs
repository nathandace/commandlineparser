using System.Collections;
using System.Reflection;
using static System.Type;


namespace PocketDews.CommandLineParser;

public static class OptionsParser
{

    #region Methods

    /// <summary>
    /// Maps key value pairs to a property of the given type.
    /// </summary>
    public static T MapProperties<T>(Dictionary<string, string> values)
    {
        var instance = Activator.CreateInstance(typeof(T));

        foreach (var kvp in values)
        {
            var property = typeof(T).GetProperty(kvp.Key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (property == null)
            {
                continue;
            }

            var value = Convert.ChangeType(kvp.Value, property.PropertyType);
            property.SetValue(instance, value);
        }

        return (T)instance!;
    }

    /// <summary>
    /// Parses the arguments into the values of the specified object.
    /// </summary>
    public static T Parse<T>(string[] args)
    {
        var parameters = args.ToList();
        var optionsObject = (T)typeof(T).GetConstructor(EmptyTypes)!.Invoke(Array.Empty<object>());

        // Get all properties on the object that have the options attribute.
        var properties = typeof(T).GetProperties().Where(x => x.GetCustomAttributes(typeof(OptionsAttribute), true).Any()).ToList();

        foreach (var property in properties)
        {
            // Get the value from the options attribute, and then retrieve the indexes to find the associated value.
            // Set this value back to the property so that the command line argument is parsed back into the object.

            var token = ((OptionsAttribute)property.GetCustomAttribute(typeof(OptionsAttribute))!).Name;
            var tokenIndex = parameters.FindIndex(p => p.Equals(token, StringComparison.OrdinalIgnoreCase));

            // If no match, then the token did not exist within the arguments for this property, so continue
            // on to the next property.

            if (tokenIndex > -1)
            {
                if (property.PropertyType == typeof(string))
                {
                    // String properties will have an associated value with them, so retrieve that value
                    // to set as the value of the option.

                    if (IndexInBounds(tokenIndex + 1, parameters))
                    {
                        var value = parameters[tokenIndex + 1];
                        property.SetValue(optionsObject, Convert.ChangeType(value, property.PropertyType), null);
                    }
                }
                else if (property.PropertyType == typeof(bool))
                {
                    // The presence of a bool properties will indicate that the value should always be set as true.
                    property.SetValue(optionsObject, Convert.ChangeType(true, property.PropertyType), null);
                }
            }
        }

        return optionsObject;
    }
    /// <summary>
    /// Returns true if the specified index is within bounds of the array.
    /// </summary>
    private static bool IndexInBounds(int index, ICollection array)
    {
        return index >= 0 && index < array.Count;
    }

    #endregion Methods

}