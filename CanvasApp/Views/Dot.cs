using System;
using Avalonia.Controls.Shapes;
using Avalonia.Media;

namespace Sweeper.Views;

public class Dot : Ellipse
{
    Dot()
    {
        Width = 10;
        Height = 10;
        Fill = Brushes.OrangeRed;
        Stroke = Brushes.Black;
        StrokeThickness = 1;
        Tag = key; // store key instead of model
    }
    //ToolTip.SetTip(dot, $"({p.X:0.##}, {p.Y:0.##})");
}
