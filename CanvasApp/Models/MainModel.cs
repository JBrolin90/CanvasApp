using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Sweeper.Math;

namespace Sweeper.Models;

/// <summary>
/// Main view model holding a collection of points.
/// </summary>
public class MainModel
{
    private GroupsManager groupsMgr = new();
    private ItemManager<Dot> dotsMgr = new();
    private readonly Dictionary<Guid, Point> _pointLookup = new();
    public ObservableCollection<Point> Points { get; }

    public MainModel()
    {
        Points = new ObservableCollection<Point>();
        Points.CollectionChanged += OnPointsCollectionChanged;
        dotsMgr.Subscribe(OnDotsCollectionChanged);
    }
    public void SubscribeDotsChanged(NotifyCollectionChangedEventHandler handler)
    {
        dotsMgr.Subscribe(handler);
    }
    private void OnDotsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    { }

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
    private Point? Find(double x, double y)
    {
        foreach (var point in Points)
        {
            return point.CloseEnough(x, y);
        }
        return null;
    }
    private Point Add(double x, double y)
    {
        var p = new Point(x, y);
        Points.Add(p);
        return p;
    }

    public Point? GetPoint(Guid id)
    {
        return _pointLookup.TryGetValue(id, out var point) ? point : null;
    }


    public Point GetPoint(double x, double y)
    {
        var group = groupsMgr.GetGroup(x, y);
        dotsMgr.Add(new Dot(group));
        return group;
    }
    // public Point GetPoint(double x, double y)
    // {
    //     Point? p = Find(x, y);
    //     p ??= Add(x, y);
    //     return p;
    // }
}
