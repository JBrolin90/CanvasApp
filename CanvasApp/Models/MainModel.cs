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

	public void Add(Point p) => Points.Add(p);
	public void Remove(Point p) => Points.Remove(p);

	public event PropertyChangedEventHandler? PropertyChanged;
	protected void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
