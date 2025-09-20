using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;

namespace Sweeper.Views;

/// <summary>
/// Visual representation of a line segment that can be moved as part of a group.
/// Inherits from Avalonia's Line shape and implements IGroupMovable for group movement functionality.
/// Each endpoint is visualized with a Dot object.
/// </summary>
public class Segment : Line, IGroupMovable
{
    Guid IGroupMovable.Id => Id;
    private const double StrokeThicknessNormal = 2;
    private const double StrokeThicknessDragging = 3;
    private Canvas? Canvas => Parent as Canvas;

    internal Guid Id => (Guid)Tag!;
    private readonly Dot _startDot;
    private readonly Dot _endDot;
    private Action<Point> pointMover;


    internal Segment(Sweeper.Math.Segment segment, IGroupMover mover)
    {
        // Set up line properties
        StartPoint = segment.StartPoint;
        EndPoint = segment.EndPoint;
        Stroke = Brushes.DarkBlue;
        StrokeThickness = StrokeThicknessNormal;
        StrokeLineCap = PenLineCap.Round;
        Tag = segment.Id;

        // Create dots for the endpoints
        _startDot = new Dot(segment.StartPoint, mover);
        _endDot = new Dot(segment.EndPoint, mover);
        UpdateTooltip();
        pointMover = StartMover;
    }


    internal void AddToCanvas(Canvas canvas)
    {
        canvas.Children.Add(this);
        
        canvas.Children.Add(_startDot);
        canvas.Children.Add(_endDot);
    }


    private void UpdateTooltip()
    {
        var tooltipText = $"Segment: ({StartPoint.X:0.##}, {StartPoint.Y:0.##}) -> ({EndPoint.X:0.##}, {EndPoint.Y:0.##})";
        ToolTip.SetTip(this, tooltipText);
    }



    private void StartMover(Point point)
    {
        StartPoint = point;
    }
    private void EndMover(Point point)
    {
        EndPoint = point;
    }

    private IGroupMovable GetMover(Guid guid)
    {
        if (guid == _startDot.Id)
            pointMover = StartMover;
        else if (guid == _endDot.Id)
            pointMover = EndMover;
        return this;
    }
    void MoveMe(Point newCenter)
    {
        pointMover(newCenter);
        UpdateTooltip();
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

