namespace PocketDews.CommandLineParser;

public class OptionsAttribute : Attribute
{
    #region Constructors

    /// <summary>
    /// Creates a new instance of the <see cref="OptionsAttribute"/>.
    /// </summary>
    /// <param name="name">The name of the command line option, such as --directory.</param>
    public OptionsAttribute(string name)
    {
        Name = name;
    }

    #endregion Constructors

    #region Properties

    /// <summary>
    /// Gets or sets the name of the command line option, such as --directory.
    /// </summary>
    public string Name { get; set; }

    #endregion Properties
}