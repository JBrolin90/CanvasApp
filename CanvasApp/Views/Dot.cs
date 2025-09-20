using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;

namespace Sweeper.Views;

public class Dot : Ellipse, IGroupMovable
{
    private IPointer? _capturedPointer;
    private IGroupMover mover;


    private const double DotSize = 10;
    private const double StrokeThicknessNormal = 1;
    private const double StrokeThicknessDragging = 2;

    internal Guid Id
    {
        get
        {
            return (Guid)Tag;
        }
    }


    private readonly Action<Guid, double, double>? _onMoved;

    internal Dot(Sweeper.Math.Point p, IGroupMover mover, Action<Guid, double, double>? onMoved = null)
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
        this.mover = mover;
        

        PointerPressed += OnDotPressed;
    }

    private Canvas? Canvas => Parent as Canvas;

    Guid IGroupMovable.Id => Id;

    private void OnDotPressed(object? sender, PointerPressedEventArgs e)
    {
        if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed) return;

        Stroke = Brushes.Yellow;
        StrokeThickness = StrokeThicknessDragging;
        e.Pointer.Capture(this);
        _capturedPointer = e.Pointer;

        PointerMoved += OnDotMoved;
        PointerReleased += OnDotReleased;
        mover.PrepareMove(Id);
    }

    private void OnDotMoved(object? sender, PointerEventArgs e)
    {
        if (Canvas == null) return;
        var pt = e.GetPosition(Canvas);
        mover.MoveGroup(Id, pt);
    }

    private void OnDotReleased(object? sender, PointerEventArgs e)
    {
        Stroke = Brushes.Black;
        StrokeThickness = StrokeThicknessNormal;
        mover.CompleteMoveGroup(Id, e.GetPosition(Canvas));

        _capturedPointer?.Capture(null);
        _capturedPointer = null;
        PointerMoved -= OnDotMoved;
        PointerReleased -= OnDotReleased;
    }

    private IGroupMovable GetMover(Guid _)
    {
        return this;
    }

    void MoveMe(Point point)
    {
        Canvas.SetLeft(this, point.X - Width / 2);
        Canvas.SetTop(this, point.Y - Height / 2);
    }

    IGroupMovable IGroupMovable.GetMover(Guid guid)
    {
        return GetMover(guid);
    }

    void IGroupMovable.MoveMe(Point point)
    {
        MoveMe(point);
    }
}
