using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Interactivity;
using Sweeper.ViewModels;
using Avalonia;

namespace avalonia.app;

public partial class MainWindow : Window
{
    private readonly MainViewModel _vm;
    private readonly Dictionary<Guid, Ellipse> _visualsByKey = new(); // replaces Point->Ellipse

    private Sweeper.Math.Point? _dragPoint;
    private Ellipse? _dragVisual;
    private Avalonia.Point _dragOffset; // offset inside the ellipse when grabbing
    private readonly Canvas _canvas;
    private IPointer? _capturedPointer;

    public MainWindow()
    {
        Title = "Avalonia C# App";
        _vm = new MainViewModel();
        DataContext = _vm;

        _canvas = new Canvas
        {
            Background = Brushes.Transparent // Needed so empty areas receive pointer events
        };

        _vm.UiEventRaised += (_, evt) =>
        {
            switch (evt.Kind)
            {
                case UiEventKind.PointAdded:
                    var p = evt.Payload as Sweeper.Math.Point;
                    if(p!=null)
                        AddPointVisual(p, _canvas);
                    break;
                case UiEventKind.FlashPoint:
                    // custom flash logic
                    break;
            }
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
        Opened += (_, _) => { CenterText(_canvas, title); PositionInfo(_canvas, info, title); };
        SizeChanged += (_, _) => { CenterText(_canvas, title); PositionInfo(_canvas, info, title); };

        _canvas.Children.Add(title);
        _canvas.Children.Add(info);

        // Pointer: click to add point if not on existing; start drag if on existing.
        _canvas.PointerPressed += (s, e) =>
        {
            if (e.Source is Ellipse)
            {
                return; // Ellipse handles its own press
            }
            if (!e.GetCurrentPoint(_canvas).Properties.IsLeftButtonPressed) return;
            Avalonia.Point pt = e.GetPosition(_canvas);
            _vm.AddNewPointFromUI(pt.X, pt.Y);
        };

        // Canvas-level move & release to ensure we keep receiving events even if pointer leaves ellipse.
        // Fallback canvas-level handlers (handledEventsToo) in case pointer leaves ellipse without capture.
        _canvas.AddHandler(PointerMovedEvent, (object? _, PointerEventArgs e) =>
        {
            if (_dragPoint is null || _dragVisual is null) return;
            if (_capturedPointer != null && e.Pointer != _capturedPointer) return;
            var props = e.GetCurrentPoint(_canvas).Properties;
            if (!props.IsLeftButtonPressed) { EndDrag(); return; }
            var pt = e.GetPosition(_canvas);
            UpdateDragPosition(pt,_dragVisual);
        }, RoutingStrategies.Tunnel | RoutingStrategies.Bubble, true);

        _canvas.AddHandler(PointerReleasedEvent, (object? _, PointerReleasedEventArgs e) =>
        {
            if (_capturedPointer != null && e.Pointer == _capturedPointer)
                EndDrag();
        },  RoutingStrategies.Tunnel | RoutingStrategies.Bubble, true);

        // Incremental visuals: hook collection changed.
        _vm.Points.CollectionChanged += OnPointsCollectionChanged;
        // Seed existing points.
        foreach (var p in _vm.Points) AddPointVisual(p, _canvas);
            Content = _canvas;
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

    private void OnPointsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (Content is not Canvas canvas) return;
        if (e.NewItems != null)
            foreach (Sweeper.Math.Point p in e.NewItems)
                AddPointVisual(p, canvas);
        if (e.OldItems != null)
            foreach (Sweeper.Math.Point p in e.OldItems)
                RemovePointVisual(p, canvas);
    }

    private void AddPointVisual(Sweeper.Math.Point p, Canvas canvas)
    {
        // Get stable key from VM
        var key = p.Id;
        if (_visualsByKey.ContainsKey(key)) return;

        var dot = new Ellipse
        {
            Width = 10,
            Height = 10,
            Fill = Brushes.OrangeRed,
            Stroke = Brushes.Black,
            StrokeThickness = 1,
            Tag = key // store key instead of model
        };
        PositionDot(dot, p);
        ToolTip.SetTip(dot, $"({p.X:0.##}, {p.Y:0.##})");
        canvas.Children.Add(dot);
        _visualsByKey[key] = dot;

        p.PropertyChanged += Point_PropertyChanged;
        dot.PointerPressed += (s, e) => OnPointPressed(p, dot, e);
        dot.PointerMoved += (s, e) => OnPointMoved(p, dot, e);
        dot.PointerReleased += (s, e) => OnPointReleased(p, dot, e);
    }

    private void RemovePointVisual(Sweeper.Math.Point p, Canvas canvas)
    {
        // if (!_vm.TryGetVisualKey(p, out var key)) return;
        // if (_visualsByKey.TryGetValue(key, out var dot))
        // {
        //     canvas.Children.Remove(dot);
        //     _visualsByKey.Remove(key);
        //     p.PropertyChanged -= Point_PropertyChanged;
        //     if (_dragPoint == p)
        //         EndDrag();
        // }
    }

    private void Point_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (sender is not Sweeper.Math.Point p) return;
        // if (!_vm.TryGetVisualKey(p, out var key)) return;
        // if (!_visualsByKey.TryGetValue(key, out var dot)) return;
        if (e.PropertyName is nameof(Sweeper.Math.Point.X) or nameof(Sweeper.Math.Point.Y))
        {
            // PositionDot(dot, p);
            // ToolTip.SetTip(dot, $"({p.X:0.##}, {p.Y:0.##})");
        }
    }

    private static void PositionDot(Ellipse dot, Sweeper.Math.Point p)
    {
        Canvas.SetLeft(dot, p.X - dot.Width / 2);
        Canvas.SetTop(dot, p.Y - dot.Height / 2);
    }

    private void OnPointPressed(Sweeper.Math.Point model, Ellipse visual, PointerPressedEventArgs e)
    {
        if (!e.GetCurrentPoint(visual).Properties.IsLeftButtonPressed) return;
        _dragVisual = visual;
        var pt = e.GetPosition(_canvas);
        _dragOffset = new Avalonia.Point(0, 0);
        visual.Stroke = Brushes.Yellow;
        visual.StrokeThickness = 2;
    // Capture pointer to the visual so it continues receiving move events.
        e.Pointer.Capture(visual);
        _capturedPointer = e.Pointer;
        // e.Handled = true;
    }

    private void OnPointMoved(Sweeper.Math.Point model, Ellipse visual, PointerEventArgs e)
    {
        if (_dragVisual != visual) return;
        if (!e.GetCurrentPoint(_canvas).Properties.IsLeftButtonPressed)
        {
            EndDrag();
            return;
        }
        var pt = e.GetPosition(_canvas);
        UpdateDragPosition(pt, visual);
    }

    private void OnPointReleased(Sweeper.Math.Point model, Ellipse visual, PointerReleasedEventArgs e)
    {
        if (_dragPoint == model) EndDrag();
    }

    private void EndDrag()
    {
        // No explicit release needed without capture.
        if (_dragVisual is not null)
        {
            _dragVisual.Stroke = Brushes.Black;
            _dragVisual.StrokeThickness = 1;
        }
        _dragPoint = null;
        _dragVisual = null;
        if (_capturedPointer != null)
        {
            _capturedPointer.Capture(null);
            _capturedPointer = null;
        }
    }

    private void UpdateDragPosition(Avalonia.Point pt, Ellipse dot)
    {
        if (_dragVisual is null) return;
        var newX = pt.X - _dragOffset.X;
        var newY = pt.Y - _dragOffset.Y;
        // Direct visual update for immediate feedback
        Canvas.SetLeft(_dragVisual, newX - _dragVisual.Width / 2);
        Canvas.SetTop(_dragVisual, newY - _dragVisual.Height / 2);
        // Delegate model update to the ViewModel
        _vm.MovePoint((System.Guid)dot.Tag, newX, newY);
    }
}