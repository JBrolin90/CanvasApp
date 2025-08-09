using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Sweeper.ViewModels;

namespace avalonia.app;

public partial class MainWindow : Window
{
    private readonly MainViewModel _vm;

    public MainWindow()
    {
        Title = "Avalonia C# App";
        _vm = new MainViewModel();
        DataContext = _vm;

        var canvas = new Canvas
        {
            Background = Brushes.Transparent // Needed so empty areas receive pointer events
        };

        var title = new TextBlock
        {
            Text = "Welcome to Avalonia c# only markup!",
            TextAlignment = TextAlignment.Center,
            FontSize = 24
        };

        var info = new TextBlock
        {
            FontSize = 14,
            Foreground = Brushes.Gray
        };

        void updateInfo() => info.Text = $"Points: {_vm.Points.Count}";
        updateInfo();
        _vm.Points.CollectionChanged += (_, _) => updateInfo();

        // Center both blocks.
        Opened += (_, _) => { CenterText(canvas, title); PositionInfo(canvas, info, title); };
        SizeChanged += (_, _) => { CenterText(canvas, title); PositionInfo(canvas, info, title); };

        canvas.Children.Add(title);
        canvas.Children.Add(info);

        // Pointer: click to add point.
        canvas.PointerPressed += (s, e) =>
        {
            if (e.GetCurrentPoint(canvas).Properties.IsLeftButtonPressed)
            {
                var p = e.GetPosition(canvas);
                _vm.AddPoint(p.X, p.Y);
                RenderPoints(canvas);
            }
        };

        // Re-render points on collection change.
        _vm.Points.CollectionChanged += (_, _) => RenderPoints(canvas);
        RenderPoints(canvas);
        Content = canvas;
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

    private void RenderPoints(Canvas canvas)
    {
        // Remove existing point visuals (tagged by Tag == "pt")
        var old = canvas.Children.OfType<Ellipse>().Where(e => Equals(e.Tag, "pt")).ToList();
        foreach (var o in old) canvas.Children.Remove(o);

        foreach (var p in _vm.Points)
        {
            var dot = new Ellipse
            {
                Width = 10,
                Height = 10,
                Fill = Brushes.OrangeRed,
                Stroke = Brushes.Black,
                StrokeThickness = 1,
                Tag = "pt"
            };
            ToolTip.SetTip(dot, $"({p.X:0.##}, {p.Y:0.##})");
            Canvas.SetLeft(dot, p.X - dot.Width / 2);
            Canvas.SetTop(dot, p.Y - dot.Height / 2);
            canvas.Children.Add(dot);
        }
    }
}