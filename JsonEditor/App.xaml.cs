using JsonEditor.Models;
using JsonEditor.Views;

namespace JsonEditor;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

        var files = GetDefaultFilePaths();

        //TODO remove test defaults
        var path = $"{AppDomain.CurrentDomain.BaseDirectory}\\..\\..\\..\\..\\..\\..\\examples";
        files.JsonFile = $"{path}\\rd300nx.json";
        files.SchemaFile = $"{path}\\rd300nx-schema.json";

        MainPage = new NavigationPage(new OpenFiles(files));
    }

	static FilePaths GetDefaultFilePaths()
	{
        string[] args;
        try
        {
            args = Environment.GetCommandLineArgs();
        }
        catch (NotSupportedException)
        {
            args = Array.Empty<string>();
        }
        if (args.Length > 2)
            return new FilePaths { SchemaFile = args[1], JsonFile = args[2] };
        if (args.Length > 1)
            return new FilePaths { SchemaFile = args[1] };
        return new FilePaths();
    }
}
