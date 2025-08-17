using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Sweeper.Math;

/// <summary>
/// Simple 2D point (mutable). Mutating instances used as dictionary keys or set members can lead to lookup issues.
/// </summary>
public class Point : IEquatable<Point>, INotifyPropertyChanged
{
	private double _x;
	private double _y;
	private Guid id; // Unique key for this point, used in ViewModel to track visuals.

	public Guid Id { set { id = value; } get { return id; }} 
	public double X
	{
		get => _x;
		set => SetField(ref _x, value);
	}

	public double Y
	{
		get => _y;
		set => SetField(ref _y, value);
	}

	public static readonly Point Zero = new(0, 0);

    public event PropertyChangedEventHandler? PropertyChanged;

    public Point() : this(0, 0, Guid.NewGuid()) { }

	public Point(double x, double y, Guid id)
	{
		_x = x;
		_y = y;
		this.id = id;
	}
	public Point(double x, double y)
	{
		_x = x;
		_y = y;
		id = Guid.NewGuid();
	}

	public double DistanceTo(Point other)
	{
		var dx = other.X - X;
		var dy = other.Y - Y;
		return global::System.Math.Sqrt(dx * dx + dy * dy);
	}

	/// <summary>Create a new point offset by (dx, dy).</summary>
	public Point Offset(double dx, double dy) => new(X + dx, Y + dy);

	/// <summary>Translate this point in-place by (dx, dy).</summary>
	public Point Translate(double dx, double dy)
	{
		// Use properties to ensure notifications fire.
		X = _x + dx;
		Y = _y + dy;
		return this;
	}

	public void Deconstruct(out double x, out double y)
	{
		x = X; y = Y;
	}

	public override string ToString() => $"({X:0.###}, {Y:0.###})";

	public bool Equals(Point? other)
	{
		if (ReferenceEquals(null, other)) return false;
		if (ReferenceEquals(this, other)) return true;
		return X.Equals(other.X) && Y.Equals(other.Y);
	}

	public override bool Equals(object? obj) => obj is Point p && Equals(p);

	public override int GetHashCode() => HashCode.Combine(X, Y);

	protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

	private bool SetField(ref double field, double value, [CallerMemberName] string? propertyName = null)
	{
		if (field.Equals(value)) return false;
		field = value;
		OnPropertyChanged(propertyName);
		return true;
	}

	public static Point operator +(Point a, Point b) => new(a.X + b.X, a.Y + b.Y);
	public static Point operator -(Point a, Point b) => new(a.X - b.X, a.Y - b.Y);
	public static bool operator ==(Point? a, Point? b) => ReferenceEquals(a, b) || (a is not null && b is not null && a.Equals(b));
	public static bool operator !=(Point? a, Point? b) => !(a == b);
}
