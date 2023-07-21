using JsonEditor.Models;

namespace JsonEditor.Views;

public class EditJson : ContentPage
{
    readonly JsonModel model;

    public EditJson(JsonModel model)
    {
        this.model = model;
        Content = new VerticalStackLayout
        {
            PropertyGrid(model.Properties),
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
                new ColumnDefinition(GridLength.Star)
            }
        };
        for (var i = 0; i < properties.Count; i++)
        {
            var property = properties[i];
            grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            var title = property.GenerateHeaderView();
            var content = property.GenerateEditView();
            grid.Children.Add(title);
            grid.SetRow(title, i);
            grid.Children.Add(content);
            grid.SetRow(content, i);
            grid.SetColumn(content, 1);
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
    #endregion
}