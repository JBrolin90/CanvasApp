using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Sweeper.Math;

/// <summary>
/// Represents a line segment between two points. 
/// Mutable class that tracks changes to endpoints.
/// </summary>
public class Segment : IEquatable<Segment>, INotifyPropertyChanged
{
    private Point _startPoint;
    private Point _endPoint;
    private Guid _id;

    /// <summary>
    /// Unique identifier for this segment, used in ViewModel to track visuals.
    /// </summary>
    public Guid Id 
    { 
        get => _id; 
        set => SetField(ref _id, value); 
    }

    /// <summary>
    /// Starting point of the segment.
    /// </summary>
    public Point StartPoint
    {
        get => _startPoint;
        set => SetField(ref _startPoint, value);
    }

    /// <summary>
    /// Ending point of the segment.
    /// </summary>
    public Point EndPoint
    {
        get => _endPoint;
        set => SetField(ref _endPoint, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Creates a new segment with auto-generated ID.
    /// </summary>
    public Segment(Point startPoint, Point endPoint) : this(startPoint, endPoint, Guid.NewGuid())
    {
    }

    /// <summary>
    /// Creates a new segment with specified ID.
    /// </summary>
    public Segment(Point startPoint, Point endPoint, Guid id)
    {
        _startPoint = startPoint ?? throw new ArgumentNullException(nameof(startPoint));
        _endPoint = endPoint ?? throw new ArgumentNullException(nameof(endPoint));
        _id = id;
    }

    /// <summary>
    /// Calculate the length of this segment.
    /// </summary>
    public double Length => StartPoint.DistanceTo(EndPoint);

    /// <summary>
    /// Calculate the midpoint of this segment.
    /// </summary>
    public Point Midpoint => new((StartPoint.X + EndPoint.X) / 2, (StartPoint.Y + EndPoint.Y) / 2);

    /// <summary>
    /// Deconstruct into start and end points.
    /// </summary>
    public void Deconstruct(out Point startPoint, out Point endPoint)
    {
        startPoint = StartPoint;
        endPoint = EndPoint;
    }

    public override string ToString() => $"Segment({StartPoint} -> {EndPoint})";

    public bool Equals(Segment? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return StartPoint.Equals(other.StartPoint) && EndPoint.Equals(other.EndPoint);
    }

    public override bool Equals(object? obj) => obj is Segment s && Equals(s);

    public override int GetHashCode() => HashCode.Combine(StartPoint, EndPoint);

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    public static bool operator ==(Segment? a, Segment? b) => ReferenceEquals(a, b) || (a is not null && b is not null && a.Equals(b));
    public static bool operator !=(Segment? a, Segment? b) => !(a == b);
}