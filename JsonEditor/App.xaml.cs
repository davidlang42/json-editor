using JsonEditor.Models;
using JsonEditor.Views;

namespace JsonEditor;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

        var files = GetFilePathsFromCLI();

        MainPage = new NavigationPage(new OpenFiles(files));
    }

	static FilePaths? GetFilePathsFromCLI()
	{
        string[] args;
        try
        {
            args = Environment.GetCommandLineArgs();
        }
        catch (NotSupportedException)
        {
            return null;
        }
        if (args.Length > 2)
            return new FilePaths { SchemaFile = args[1], JsonFile = args[2] };
        if (args.Length > 1)
            return new FilePaths { SchemaFile = args[1] };
        return null;
    }
}
