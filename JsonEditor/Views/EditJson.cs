using JsonEditor.Converters;
using JsonEditor.Extensions;
using JsonEditor.Models;
using Newtonsoft.Json;

namespace JsonEditor.Views;

public class EditJson : ContentPage
{
    readonly JsonModel model;

    public EditJson(JsonModel model)
    {
        this.model = model;
        model.NavigateAction = NavigateAction;
        //TODO show the full path (RD300NX.user_set[7].common) at the top
        Content = new VerticalStackLayout
        {
            PropertyGrid(model.Properties), //TODO make property grid scrollable
            new HorizontalStackLayout
            {
                MakeButton("Undo", Colors.Red, Cancel_Clicked),
                MakeButton("Save", Colors.Green, Ok_Clicked)
            }
        };
    }

    #region View generation
    static Button MakeButton(string text, Color color, EventHandler clicked)
    {
        var button = new Button
        {
            Text = text,
            BackgroundColor = color
        };
        button.Clicked += clicked;
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
                new ColumnDefinition(new GridLength(200, GridUnitType.Absolute)),
                new ColumnDefinition(new GridLength(50, GridUnitType.Absolute)),
                new ColumnDefinition(GridLength.Star)
            }
        };
        for (var i = 0; i < properties.Count; i++)
        {
            var property = properties[i];
            grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            var title = property.GenerateHeaderView();
            var content = property.Value.EditView;
            grid.Add(title);
            grid.SetRow(title, i);
            if (!property.Required)
            {
                var null_switch = new Switch
                {
                    BindingContext = property
                };
                null_switch.SetBinding(Switch.IsToggledProperty, nameof(Property.Include));
                grid.Add(null_switch);
                grid.SetRow(null_switch, i);
                grid.SetColumn(null_switch, 1);
                content.SetBindingRecursively(VisualElement.IsVisibleProperty, nameof(Property.Include));
                var null_label = new Label
                {
                    BindingContext = property,
                    Text = "(key not set)",
                    FontAttributes = FontAttributes.Italic
                };
                null_label.SetBinding(VisualElement.IsVisibleProperty, nameof(Property.Include), converter: new InvertBoolean());
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
        await Navigation.PopModalAsync();
    }

    private async void Ok_Clicked(object? sender, EventArgs e)
    {
        foreach (var property in model.Properties)
            property.Commit();
        model.File.Save();
        await Navigation.PopModalAsync();
    }

    private void NavigateAction(JsonModel new_model)
    {
        Navigation.PushModalAsync(new EditJson(new_model));
    }
    #endregion
}