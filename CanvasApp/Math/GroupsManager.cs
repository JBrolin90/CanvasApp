using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Sweeper.Math;

public class GroupsManager
{
    private readonly Dictionary<Guid, Point> _pointLookup = new();
    private ObservableCollection<Point> groups = [];

    public GroupsManager()
    {
        groups.CollectionChanged += OnPointsCollectionChanged;
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


    private Point Add(double x, double y)
    {
        var p = new Point(x, y);
        groups.Add(p);
        return p;
    }

    public Point? GetPoint(Guid id)
    {
        return _pointLookup.TryGetValue(id, out var point) ? point : null;
    }

    private Point? Find(double x, double y)
    {
        foreach (var point in groups)
        {
            return point.CloseEnough(x, y);
        }
        return null;
    }

    public Point GetGroup(double x, double y)
    {
        Point? p = Find(x, y);
        p ??= Add(x, y);
        return p;
    }

    public void Remove(Point p) => groups.Remove(p);

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

}
