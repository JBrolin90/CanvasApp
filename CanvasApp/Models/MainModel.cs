using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Sweeper.Math;

namespace Sweeper.Models;

/// <summary>
/// Main view model holding a collection of points.
/// </summary>
public class MainModel : INotifyPropertyChanged
{
    private readonly Dictionary<Guid, Point> _pointLookup = new();
    
    public ObservableCollection<Point> Points { get; }

    public MainModel()
    {
        Points = new ObservableCollection<Point>();
        Points.CollectionChanged += OnPointsCollectionChanged;
    }

    private void OnPointsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
        {
            foreach (Point point in e.NewItems)
            {
                _pointLookup[point.Id] = point;
            }
        }

        if (e.OldItems != null)
        {
            foreach (Point point in e.OldItems)
            {
                _pointLookup.Remove(point.Id);
            }
        }
    }

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
    public Point Add(double x, double y)
    {
        var p = new Point(x, y);
        Points.Add(p);
        return p;
    }

    public Point? GetPoint(Guid id)
    {
        return _pointLookup.TryGetValue(id, out var point) ? point : null;
    }

    public void Remove(Point p) => Points.Remove(p);

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
