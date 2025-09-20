using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;

namespace Sweeper.Views;

/// <summary>
/// Visual representation of a line segment that can be moved as part of a group.
/// Inherits from Avalonia's Line shape and implements IGroupMovable for group movement functionality.
/// Each endpoint is visualized with a Dot object.
/// </summary>
public class Segment : Line, IGroupMovable
{
    private IPointer? _capturedPointer;
    private readonly IGroupMover _mover;
    private Point _dragOffset;

    private const double StrokeThicknessNormal = 2;
    private const double StrokeThicknessDragging = 3;

    /// <summary>
    /// Gets the unique identifier for this segment.
    /// </summary>
    internal Guid Id => (Guid)Tag!;

    /// <summary>
    /// Optional callback invoked when the segment is moved.
    /// </summary>
    private readonly Action<Guid, double, double>? _onMoved;

    /// <summary>
    /// Dot representing the start point of the segment.
    /// </summary>
    private readonly Dot _startDot;

    /// <summary>
    /// Dot representing the end point of the segment.
    /// </summary>
    private readonly Dot _endDot;

    /// <summary>
    /// Creates a new visual segment from a mathematical segment definition.
    /// </summary>
    /// <param name="segment">The mathematical segment defining start/end points</param>
    /// <param name="mover">Group mover for handling coordinated movements</param>
    /// <param name="onMoved">Optional callback for movement notifications</param>
    internal Segment(Sweeper.Math.Segment segment, IGroupMover mover, Action<Guid, double, double>? onMoved = null)
    {
        // Set up line properties
        StartPoint = new Point(segment.StartPoint.X, segment.StartPoint.Y);
        EndPoint = new Point(segment.EndPoint.X, segment.EndPoint.Y);
        Stroke = Brushes.DarkBlue;
        StrokeThickness = StrokeThicknessNormal;
        StrokeLineCap = PenLineCap.Round;
        Tag = segment.Id;

        // Set up tooltip showing segment info
        ToolTip.SetTip(this, $"Segment: ({segment.StartPoint.X:0.##}, {segment.StartPoint.Y:0.##}) -> ({segment.EndPoint.X:0.##}, {segment.EndPoint.Y:0.##})");

        _onMoved = onMoved;
        _mover = mover;

        // Create dots for the endpoints
        _startDot = new Dot(segment.StartPoint, mover, OnEndpointMoved);
        _endDot = new Dot(segment.EndPoint, mover, OnEndpointMoved);

        // Register event handlers
        PointerPressed += OnSegmentPressed;
    }

    /// <summary>
    /// Gets the parent canvas for coordinate calculations.
    /// </summary>
    private Canvas? Canvas => Parent as Canvas;

    /// <summary>
    /// Adds this segment and its endpoint dots to the specified canvas.
    /// </summary>
    /// <param name="canvas">The canvas to add the segment and dots to</param>
    internal void AddToCanvas(Canvas canvas)
    {
        // Add the line segment
        canvas.Children.Add(this);
        
        // Add the endpoint dots
        canvas.Children.Add(_startDot);
        canvas.Children.Add(_endDot);
    }

    /// <summary>
    /// Removes this segment and its endpoint dots from the canvas.
    /// </summary>
    internal void RemoveFromCanvas()
    {
        if (Canvas is null) return;
        
        Canvas.Children.Remove(this);
        Canvas.Children.Remove(_startDot);
        Canvas.Children.Remove(_endDot);
    }

    /// <summary>
    /// Callback invoked when an endpoint dot is moved.
    /// Updates the segment line to match the new endpoint positions.
    /// This is called during dragging for visual updates only - model updates happen on mouse up.
    /// </summary>
    private void OnEndpointMoved(Guid dotId, double x, double y)
    {
        // Update the line endpoints to match the dot positions
        if (_startDot.Id == dotId)
        {
            StartPoint = new Point(x, y);
        }
        else if (_endDot.Id == dotId)
        {
            EndPoint = new Point(x, y);
        }

        // Update the tooltip to reflect the new endpoint positions
        UpdateTooltip();
    }

    /// <summary>
    /// Updates the segment tooltip with current endpoint coordinates.
    /// </summary>
    private void UpdateTooltip()
    {
        var tooltipText = $"Segment: ({StartPoint.X:0.##}, {StartPoint.Y:0.##}) -> ({EndPoint.X:0.##}, {EndPoint.Y:0.##})";
        ToolTip.SetTip(this, tooltipText);
    }

    /// <summary>
    /// Explicit implementation of IGroupMovable.Id
    /// </summary>
    Guid IGroupMovable.Id => Id;

    /// <summary>
    /// Handles pointer press events to initiate dragging of the entire segment.
    /// Note: Individual endpoint dragging is handled by the endpoint Dot objects.
    /// This method handles dragging the segment as a whole (moving both endpoints together).
    /// </summary>
    private void OnSegmentPressed(object? sender, PointerPressedEventArgs e)
    {
        if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed) return;

        // Visual feedback during drag - change segment color to indicate it's being moved
        Stroke = Brushes.Orange;
        StrokeThickness = StrokeThicknessDragging;
        
        // Capture pointer for drag operation
        e.Pointer.Capture(this);
        _capturedPointer = e.Pointer;

        // Calculate offset from segment midpoint to click point for consistent dragging feel
        var clickPos = e.GetPosition(Canvas);
        var midpoint = GetMidpoint();
        _dragOffset = new Point(clickPos.X - midpoint.X, clickPos.Y - midpoint.Y);

        // Register movement handlers and prepare group move
        PointerMoved += OnSegmentMoved;
        PointerReleased += OnSegmentReleased;
        _mover.PrepareMove(Id);
    }

    /// <summary>
    /// Handles pointer movement during drag operations.
    /// </summary>
    private void OnSegmentMoved(object? sender, PointerEventArgs e)
    {
        if (Canvas is null) return;
        
        var currentPos = e.GetPosition(Canvas);
        // Adjust for drag offset to maintain consistent feel
        var targetPos = new Point(currentPos.X - _dragOffset.X, currentPos.Y - _dragOffset.Y);
        _mover.MoveGroup(Id, targetPos);
    }

    /// <summary>
    /// Handles pointer release to complete drag operation.
    /// </summary>
    private void OnSegmentReleased(object? sender, PointerEventArgs e)
    {
        // Restore normal appearance
        Stroke = Brushes.DarkBlue;
        StrokeThickness = StrokeThicknessNormal;
        
        // Calculate final position and complete group move
        var currentPos = e.GetPosition(Canvas);
        var targetPos = new Point(currentPos.X - _dragOffset.X, currentPos.Y - _dragOffset.Y);
        _mover.CompleteMoveGroup(Id, targetPos);

        // Clean up drag state
        _capturedPointer?.Capture(null);
        _capturedPointer = null;
        PointerMoved -= OnSegmentMoved;
        PointerReleased -= OnSegmentReleased;
    }

    /// <summary>
    /// Calculates the midpoint of the segment for movement calculations.
    /// </summary>
    private Point GetMidpoint()
    {
        return new Point(
            (StartPoint.X + EndPoint.X) / 2,
            (StartPoint.Y + EndPoint.Y) / 2
        );
    }

    /// <summary>
    /// Returns the movable object for the specified GUID (self in this case).
    /// </summary>
    private IGroupMovable GetMover(Guid _)
    {
        return this;
    }

    /// <summary>
    /// Moves this segment by translating both start and end points.
    /// The point parameter represents the new center position.
    /// Also moves the endpoint dots to maintain visual consistency.
    /// </summary>
    void MoveMe(Point newCenter)
    {
        var currentMidpoint = GetMidpoint();
        var offset = new Point(newCenter.X - currentMidpoint.X, newCenter.Y - currentMidpoint.Y);
        
        // Translate both line endpoints by the offset
        StartPoint = new Point(StartPoint.X + offset.X, StartPoint.Y + offset.Y);
        EndPoint = new Point(EndPoint.X + offset.X, EndPoint.Y + offset.Y);

        // Move the endpoint dots to match the new line positions
        ((IGroupMovable)_startDot).MoveMe(StartPoint);
        ((IGroupMovable)_endDot).MoveMe(EndPoint);
    }

    /// <summary>
    /// Explicit implementation of IGroupMovable.GetMover
    /// </summary>
    IGroupMovable IGroupMovable.GetMover(Guid guid)
    {
        return GetMover(guid);
    }

    /// <summary>
    /// Explicit implementation of IGroupMovable.MoveMe
    /// </summary>
    void IGroupMovable.MoveMe(Point point)
    {
        MoveMe(point);
    }
}

