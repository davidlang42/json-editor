using JsonEditor.Models;
using Microsoft.Maui;
using System.Text.RegularExpressions;

namespace JsonEditor.Views;

public class OpenFiles : ContentPage
{
    readonly FilePaths files;

    public OpenFiles(FilePaths? prepopulated_files)
	{
        files = prepopulated_files ?? FilePaths.LoadFromUserPreferences();
        Title = "Choose a JSON schema and file";
        NavigatedTo += Page_NavigatedTo;
        BindingContext = files;
        Content = new VerticalStackLayout
        {
            Spacing = 5,
            Margin = 5,
            Children =
            {
                new Label { Text = "JSON schema definition:" },
                EntryAndBrowse(nameof(FilePaths.SchemaFile), Schema_Clicked),
                new Label { Text = "JSON file to edit:" },
                EntryAndBrowse(nameof(FilePaths.JsonFile), File_Clicked),
                new Label { Text = "Properties to hide while editing:" },
                RegexEntry(nameof(FilePaths.HidePropertiesRegex)),
                OpenButton(Open_Clicked)
            }
        };
	}

    private void Page_NavigatedTo(object? sender, NavigatedToEventArgs e)
    {
        Window.Title = "JSON Editor";
    }

    static View EntryAndBrowse(string path, EventHandler handler)
	{
        var entry = new Entry();
        entry.SetBinding(Entry.TextProperty, path);
        var browse = new Button
        {
            Text = "..."
        };
        browse.Clicked += handler;
		var grid = new Grid
		{
			ColumnSpacing = 5,
			ColumnDefinitions =
			{
				new ColumnDefinition(GridLength.Star),
				new ColumnDefinition(GridLength.Auto)
			}
		};
		grid.Add(entry);
		grid.Add(browse, 1);
		return grid;
    }

    static View OpenButton(EventHandler handler)
    {
        var button = new Button
        {
            Text = "Open JSON file",
            BackgroundColor = Colors.Green
        };
        button.Clicked += handler;
        return button;
    }

    static View RegexEntry(string path)
    {
        var entry = new Entry
        {
            Placeholder = "Enter a Regular Expression (RegEx)"
        };
        entry.SetBinding(Entry.TextProperty, path);
        return entry;
    }

    private async void Open_Clicked(object? sender, EventArgs e)
    {
        if (!File.Exists(files.SchemaFile))
        {
            await DisplayAlert("Error", $"The schema file does not exist: {files.SchemaFile}", "Ok");
            return;
        }
        if (!File.Exists(files.JsonFile))
        {
            await DisplayAlert("Error", $"The JSON file does not exist: {files.JsonFile}", "Ok");
            return;
        }
        Regex? regex = null;
        if (!string.IsNullOrEmpty(files.HidePropertiesRegex))
        {
            try
            {
                regex = new Regex(files.HidePropertiesRegex, RegexOptions.Compiled);
            }
            catch (ArgumentException)
            {
                await DisplayAlert("Error", $"The pattern for hiding properties is not a valid Regular Expression: {files.HidePropertiesRegex}\nLeave this blank to show all properties.", "Ok");
                return;
            }
        }
        files.SaveToUserPreferences();
        var json_file = JsonFile.Load(files.SchemaFile, files.JsonFile);
        json_file.HideProperties = regex;
        var json_model = new JsonModel(json_file, new(json_file.Schema.Title ?? "Root"), json_file.Root, json_file.Schema);
        await Navigation.PushAsync(new EditJson(json_model));
    }

    private async void Schema_Clicked(object? sender, EventArgs e)
    {
        if (await BrowseForJson("Please select a JSON schema") is string path)
            files.SchemaFile = path;
    }

    private async void File_Clicked(object? sender, EventArgs e)
    {
        if (await BrowseForJson("Please select a JSON file") is string path)
            files.JsonFile = path;
    }

    static async Task<string?> BrowseForJson(string title)
    {
        var options = new PickOptions
        {
            PickerTitle = title,
            FileTypes = new FilePickerFileType(
                new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.iOS, new[] { "json", "txt" } }, // UTType values
                    { DevicePlatform.Android, new[] { "application/json", "text/plain" } }, // MIME type
                    { DevicePlatform.WinUI, new[] { ".json", ".txt" } }, // file extension
                    { DevicePlatform.macOS, new[] { "json", "txt" } }, // UTType values
                })
        };
        try
        {
            var result = await FilePicker.Default.PickAsync(options);
            return result?.FullPath;
        }
        catch
        {
            // The user canceled or something went wrong
            return null;
        }
    }
}