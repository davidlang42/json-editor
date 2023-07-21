using JsonEditor.Models;
using JsonEditor.Views;

namespace JsonEditor;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

        //TODO implement file picker window: https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/storage/file-picker?tabs=windows
        var path = $"{AppDomain.CurrentDomain.BaseDirectory}\\..\\..\\..\\..\\..\\..\\examples";
        var file = JsonFile.Load($"{path}\\song_rhythm-schema.json", $"{path}\\song_rhythm.json");
        var model = new JsonModel(file, file.Root, file.Schema);

        MainPage = new NavigationPage(new EditJson(model));
    }

	private FilePaths GetDefaultFilePaths()
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
