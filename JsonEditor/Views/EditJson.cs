using JsonEditor.Converters;
using JsonEditor.Extensions;
using JsonEditor.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonEditor.Views;

public class EditJson : ContentPage
{
    readonly JsonModel model;
    bool loaded = false;

    public EditJson(JsonModel model)
    {
        this.model = model;
        model.NavigateAction = NavigateAction;
        Title = model.Path.ToString();
        Content = new ActivityIndicator { IsRunning = true };
        NavigatedTo += Page_NavigatedTo;
    }

    private async void Page_NavigatedTo(object? sender, NavigatedToEventArgs e)
    {
        if (!loaded)
        {
            loaded = true;
            Thread.Sleep(1); // allows Loading view to be shown
            Window.Title = $"JSON Editor: {model.File.Filename}";
            var view = await Task.Run(GenerateMainView);
            Content = view;
        }
        else
        {
            model.Refresh();
        }
    }

    #region View generation
    private View GenerateMainView()
    {
        var main = new ScrollView
        {
            Content = PropertyGrid(model.Properties),
        };
        var back = MakeButton("Back", Colors.Red, Cancel_Clicked);
        var save = MakeButton("Save", Colors.Green, Ok_Clicked, nameof(JsonModel.AnyChanges));
        var grid = new Grid
        {
            Margin = 5,
            RowSpacing = 5,
            ColumnSpacing = 5,
            RowDefinitions =
            {
                new RowDefinition(GridLength.Star),
                new RowDefinition(GridLength.Auto)
            },
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star)
            }
        };
        grid.Add(main);
        Grid.SetColumnSpan(main, 2);
        grid.Add(back, 0, 1);
        grid.Add(save, 1, 1);
        return grid;
    }

    Button MakeButton(string text, Color color, EventHandler clicked, string? enabled_binding = null)
    {
        var button = new Button
        {
            Text = text
        };
        button.Clicked += clicked;
        if (enabled_binding != null)
        {
            button.BindingContext = model;
            button.SetBinding(Button.IsEnabledProperty, new Binding(enabled_binding));
            button.SetBinding(Button.BackgroundColorProperty, new Binding(enabled_binding, converter: new BoolToColor(color, Colors.Gray)));
        }
        else
        {
            button.BackgroundColor = color;
        }
        return button;
    }

    static Grid PropertyGrid(List<Property> properties)
    {
        var grid = new Grid
        {
            ColumnSpacing = 5,
            RowSpacing = 5,
            Margin = 5,
            ColumnDefinitions =
            {
                new ColumnDefinition(new GridLength(0.2, GridUnitType.Star)),
                new ColumnDefinition(new GridLength(50, GridUnitType.Absolute)),
                new ColumnDefinition(new GridLength(0.8, GridUnitType.Star))
            }
        };
        for (var i = 0; i < properties.Count; i++)
        {
            var property = properties[i];
            grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            var title = new Label
            {
                Text = property.Key,
                FontAttributes = property.Required ? FontAttributes.Bold : FontAttributes.None
            };
            var content = new ContentView {
                Content = property.Value.EditView
            };
            grid.Add(title);
            grid.SetRow(title, i);
            if (property.Required)
            {
                grid.SetColumnSpan(title, 2);
            }
            else
            {
                var null_switch = new Switch
                {
                    BindingContext = property
                };
                null_switch.SetBinding(Switch.IsToggledProperty, nameof(Property.Include));
                grid.Add(null_switch);
                grid.SetRow(null_switch, i);
                grid.SetColumn(null_switch, 1);
                content.BindingContext = property;
                content.SetBinding(ContentView.IsVisibleProperty, nameof(Property.Include));
                var null_label = new Label
                {
                    BindingContext = property,
                    Text = "(key not set)",
                    FontAttributes = FontAttributes.Italic
                };
                null_label.SetBinding(Label.IsVisibleProperty, nameof(Property.Include), converter: new InvertBoolean());
                grid.Add(null_label);
                grid.SetRow(null_label, i);
                grid.SetColumn(null_label, 2);
            }
            grid.Add(content);
            grid.SetRow(content, i);
            grid.SetColumn(content, 2);
        }
        return grid;
    }
    #endregion

    #region Action handlers
    private async void Cancel_Clicked(object? sender, EventArgs e)
    {
        if (model.AnyChanges)
        {
            if (!await DisplayAlert("Discard changes", $"Changes have been made to the value of this object, are you sure you want to discard them?", "Yes", "Cancel"))
            {
                return;
            }
        }
        await Navigation.PopAsync();
    }

    const int MAX_JSON_LENGTH_IN_MESSAGE = 16384;

    private async void Ok_Clicked(object? sender, EventArgs e)
    {
        if (!model.Commit(out var new_json))
        {
            await DisplayAlert("No changes made", $"No changes have been made to the original value of this object:\n{model.OriginalJson.Truncate(MAX_JSON_LENGTH_IN_MESSAGE)}", "Cancel");
            return;
        }
        model.File.Save();
        var matches = model.FindMatchingObjects();
        if (matches.Length > 0)
        {
            var list_of_paths = string.Join("\n", matches.Select(r => r.Path.ToString()));
            var original_json = model.OriginalJson.Truncate(MAX_JSON_LENGTH_IN_MESSAGE);
            new_json = new_json.Truncate(MAX_JSON_LENGTH_IN_MESSAGE);
            var msg = $"The following objects were IDENTICAL to this one before the changes you made. Would you like to update them to this new value as well?\n\n{list_of_paths}\n\nOriginal JSON:\n{original_json}\n\nNew JSON:\n{new_json}";
            if (await DisplayAlert($"Update {matches.Length} matching objects?", msg, "Yes", "No"))
            {
                foreach (var match in matches)
                    match.Set(model.CloneObject());
                model.File.Save();
            }
        }
        await Navigation.PopAsync();
    }

    private async void NavigateAction(JsonModel new_model)
    {
        await Navigation.PushAsync(new EditJson(new_model));
    }
    #endregion
}