using System;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;

namespace Sweeper.Views;

public class Dot : Ellipse
{
    private IPointer? _capturedPointer;


    private const double DotSize = 10;
    private const double StrokeThicknessNormal = 1;
    private const double StrokeThicknessDragging = 2;


    private readonly Action<Guid, double, double>? _onMoved;

    internal Dot(Sweeper.Math.Point p, Action<Guid, double, double>? onMoved = null)
    {
        Height = Width = DotSize;
        Fill = Brushes.OrangeRed;
        Stroke = Brushes.Black;
        StrokeThickness = StrokeThicknessNormal;
        Tag = p.Id;
        Canvas.SetLeft(this, p.X - Width / 2);
        Canvas.SetTop(this, p.Y - Height / 2);
        ToolTip.SetTip(this, $"({p.X:0.##}, {p.Y:0.##})");

        _onMoved = onMoved;

        PointerPressed += OnDotPressed;
    }

    private Canvas? Canvas => Parent as Canvas;


    private void OnDotPressed(object? sender, PointerPressedEventArgs e)
    {
        if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed) return;

        Stroke = Brushes.Yellow;
        StrokeThickness = StrokeThicknessDragging;
        e.Pointer.Capture(this);
        _capturedPointer = e.Pointer;

        PointerMoved += OnDotMoved;
        PointerReleased += OnDotReleased;
    }

    private void OnDotMoved(object? sender, PointerEventArgs e)
    {
        if (Canvas == null) return;
        var pt = e.GetPosition(Canvas);
        Canvas.SetLeft(this, pt.X - Width / 2);
        Canvas.SetTop(this, pt.Y - Height / 2);
    }

    private void OnDotReleased(object? sender, PointerEventArgs e)
    {
        Stroke = Brushes.Black;
        StrokeThickness = StrokeThicknessNormal;

        if (Canvas != null && Tag is Guid id)
        {
            var centerX = Canvas.GetLeft(this) + Width / 2;
            var centerY = Canvas.GetTop(this) + Height / 2;
            _onMoved?.Invoke(id, centerX, centerY);
        }


        _capturedPointer?.Capture(null);
        _capturedPointer = null;
        PointerMoved -= OnDotMoved;
        PointerReleased -= OnDotReleased;

        
    }


}
