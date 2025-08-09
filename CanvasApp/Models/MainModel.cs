using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Sweeper.Math;

namespace Sweeper.Models;

/// <summary>
/// Main view model holding a collection of points.
/// </summary>
public class MainModel : INotifyPropertyChanged
{
	public ObservableCollection<Point> Points { get; } = [];

	private Point? _selected;
	public Point? Selected
	{
		get => _selected;
		set
		{
			if (_selected == value) return;
			_selected = value;
			OnPropertyChanged();
		}
	}
	
	public Point Add(Point p)
	{
		Points.Add(p);
		return p;
	}
	public Point Add(double x, double y, Guid key)
	{
		var p = new Point(x, y, key);
		Points.Add(p);
		return p;
	}
	public Point Add(double x, double y)
	{
		var p = new Point(x, y);
		Points.Add(p);
		return p;
	}

	public void Remove(Point p) => Points.Remove(p);

	public event PropertyChangedEventHandler? PropertyChanged;
	protected void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
