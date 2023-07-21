using JsonEditor.Models;

namespace JsonEditor.Views;

public class EditJson : ContentPage
{
    public EditJson(JsonModel model)
    {
        Content = new VerticalStackLayout
        {
            PropertyGrid(model.Properties),
            new HorizontalStackLayout
            {
                MakeButton("Undo", Colors.Red, Cancel_Clicked),
                MakeButton("Accept", Colors.Green, Ok_Clicked)
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
            var title = new Label { Text = property.Key };
            var content = property.GenerateView();
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
    private void Cancel_Clicked(object? sender, EventArgs e)
    {
        throw new NotImplementedException();
    }

    private void Ok_Clicked(object? sender, EventArgs e)
    {
        throw new NotImplementedException();
    }
    #endregion
}