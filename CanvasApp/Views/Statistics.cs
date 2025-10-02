using System;
using Avalonia.Controls;
using Avalonia.Media;

namespace Sweeper.Views;

public class Statistics
{
    Canvas canvas;
    internal Statistics(Canvas canvas)
    {
        this.canvas = canvas;
        AddStatusElements();
    }
    readonly TextBlock title = new()
    {
        Text = "Welcome to Avalonia c# only markup!",
        TextAlignment = TextAlignment.Center,
        FontSize = 24
    };

    readonly TextBlock info = new()
    {
        FontSize = 14,
        Foreground = Brushes.Gray
    };
    private void AddStatusElements()
    {
        // void updateInfo() => info.Text = $"Points: {_vm.Points.Count}";
        // updateInfo();
        // _vm.Points.CollectionChanged += (_, _) => updateInfo();

        // Center and keep centered both blocks.
        canvas.Children.Add(title);
        canvas.Children.Add(info);
    }

    private static void CenterText(Canvas canvas, TextBlock text)
    {
        if (canvas.Bounds.Width <= 0 || canvas.Bounds.Height <= 0) return;
        text.Measure(new Avalonia.Size(double.PositiveInfinity, double.PositiveInfinity));
        var size = text.DesiredSize;
        Canvas.SetLeft(text, (canvas.Bounds.Width - size.Width) / 2);
        Canvas.SetTop(text, (canvas.Bounds.Height - size.Height) / 2);
    }
    private static void PositionInfo(Canvas canvas, TextBlock info, TextBlock title)
    {
        info.Measure(new Avalonia.Size(double.PositiveInfinity, double.PositiveInfinity));
        title.Measure(new Avalonia.Size(double.PositiveInfinity, double.PositiveInfinity));
        var infoSize = info.DesiredSize;
        var titleSize = title.DesiredSize;
        Canvas.SetLeft(info, (canvas.Bounds.Width - infoSize.Width) / 2);
        Canvas.SetTop(info, (canvas.Bounds.Height - titleSize.Height) / 2 + titleSize.Height + 8);
    }
    public void UpdateLayout(Window window)
    {
        window.Opened += (_, _) => { CenterText(canvas, title); PositionInfo(canvas, info, title); };
        window.SizeChanged += (_, _) => { CenterText(canvas, title); PositionInfo(canvas, info, title); };
    }

}
