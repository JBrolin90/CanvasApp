using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Sweeper.Models;
using Sweeper.Math;
using System.Collections.Generic;
using System;
using System.Collections.Specialized;

namespace Sweeper.ViewModels;

public enum UiEventKind
{
    PointAdded,
    PointRemoved,
    PointMoved, 
    SelectionChanged,
    FlashPoint,
    FocusPoint
}

public sealed record UiEvent(UiEventKind Kind, object? Payload = null);



/// <summary>
/// ViewModel layer between view and MainModel.
/// Exposes Points and selected point plus simple commands.
/// </summary>
public class MainViewModel : INotifyPropertyChanged
{
    private readonly MainModel _model;
    public ObservableCollection<Sweeper.Math.Point> Points => _model.Points;

    // public event EventHandler<Point>? PointRemoved; // optional notification

    public event EventHandler<UiEvent>? UiEventRaised;

    private void Raise(UiEventKind kind, object? payload = null)
        => UiEventRaised?.Invoke(this, new UiEvent(kind, payload));


    // public bool TryGetVisualKey(Point p, out Guid key) => _visualKeys.TryGetValue(p, out key);

    public MainViewModel() : this(new MainModel()) { }

    public MainViewModel(MainModel model)
    {
        _model = model;
        _model.Points.CollectionChanged += OnPointsChanged;
    }

    private void OnPointsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
            foreach (Point modelPoint in e.NewItems)
            {
                Raise(UiEventKind.PointAdded, modelPoint);
            }

        if (e.OldItems != null)
            foreach (Point p in e.OldItems)
            {
                Raise(UiEventKind.PointRemoved, p);
            }
    }
    public void AddNewPointFromUI(double x, double y)
    {
        _model.Add(x, y);
    }

    public bool MovePoint(Guid id, double x, double y)
    {
        if (_model.GetPoint(id) is not { } point)
            return false;

        var oldX = point.X;
        var oldY = point.Y;

        point.X = x;
        point.Y = y;

        Raise(UiEventKind.PointMoved, new { Point = point, OldX = oldX, OldY = oldY });

        return true;
    }

    public void RemovePoint(Point point)
    {
        if (point == null) return;
        _model.Points.Remove(point);
    }


    public Sweeper.Math.Point? Selected
    {
        get => _model.Selected;
        set
        {
            if (_model.Selected == value) return;
            _model.Selected = value;
            OnPropertyChanged();
        }
    }



    public void RemoveSelected()
    {
        if (Selected is null) return;
        _model.Remove(Selected);
        Selected = null;
        OnPropertyChanged(nameof(Points));
    }
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
