using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Interactivity;
using Sweeper.ViewModels;
using Avalonia;
using Sweeper.Views;

namespace avalonia.app;

public partial class MainWindow : Window
{
    private readonly MainViewModel _vm;
    private readonly Dictionary<Guid, Shape> _visualsByKey = new();

    private readonly Canvas _canvas;


    public MainWindow()
    {
        Title = "NavIntel Graph Editor";
        _vm = new MainViewModel();
        DataContext = _vm;
        _canvas = new Canvas
        {
            Background = Brushes.Transparent // Needed so empty areas receive pointer events
        };
        AddStatusStuff();


        _vm.UiEventRaised += (_, evt) =>
        {
            switch (evt.Kind)
            {
                case UiEventKind.PointAdded:
                    var point = evt.Payload as Sweeper.Math.Point;
                    if (point != null) AddPointVisual(point, _canvas);
                    break;
                case UiEventKind.FlashPoint:
                    // custom flash logic
                    break;
            }
        };
        _canvas.PointerPressed += (s, e) =>
        {
            if (e.Source is Shape)
            {
                return; // Shapes handle their own press
            }
            if (!e.GetCurrentPoint(_canvas).Properties.IsLeftButtonPressed) return;
            Point pt = e.GetPosition(_canvas);
            _vm.AddNewPointFromUI(pt.X, pt.Y);
        };

        // Seed existing points.
        foreach (var p in _vm.Points) AddPointVisual(p, _canvas);

        Content = _canvas;
    }



    private void AddPointVisual(Sweeper.Math.Point p, Canvas canvas)
    {
        if (_visualsByKey.ContainsKey(p.Id)) return;
        var dot = new Dot(p);
        canvas.Children.Add(dot);
        _visualsByKey[p.Id] = dot;
    }

/////////////////////////////////////////////////////////////////////////////////////////////// 
    private void AddStatusStuff()
    {
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
        // Center and keep centered both blocks.
        Opened += (_, _) => { CenterText(_canvas, title); PositionInfo(_canvas, info, title); };
        SizeChanged += (_, _) => { CenterText(_canvas, title); PositionInfo(_canvas, info, title); };

        _canvas.Children.Add(title);
        _canvas.Children.Add(info);

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

}