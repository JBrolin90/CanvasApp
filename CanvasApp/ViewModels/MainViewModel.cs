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
    private readonly Dictionary<Point, Guid> _visualKeys = [];
    private readonly Dictionary<Guid, Guid>  idMapper= [];

    public ObservableCollection<Sweeper.Math.Point> Points => _model.Points;

    public event EventHandler<Point>? PointRemoved; // optional notification

    public event EventHandler<UiEvent>? UiEventRaised;

    private void Raise(UiEventKind kind, object? payload = null)
        => UiEventRaised?.Invoke(this, new UiEvent(kind, payload));

    public Guid EnsureAndGetVisualKey(Point p)
    {
        if (!_visualKeys.TryGetValue(p, out var id))
        {
            id = Guid.NewGuid();
            _visualKeys[p] = id;
        }
        return id;
    }

    public bool TryGetVisualKey(Point p, out Guid key) => _visualKeys.TryGetValue(p, out key);

    public MainViewModel() : this(new MainModel()) { }

    public MainViewModel(MainModel model)
    {
        _model = model;
        // Seed visual keys for existing points (if any)
        foreach (var p in _model.Points)
            EnsureAndGetVisualKey(p);

        _model.Points.CollectionChanged += OnPointsChanged;
    }

    private void OnPointsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
            foreach (Point p in e.NewItems)
            {
                var modelId = EnsureAndGetVisualKey(p);
                var visualId = new Guid();
                idMapper[modelId] = visualId;
                idMapper[visualId] = modelId;
                Raise(UiEventKind.PointAdded, visualId);
            }

        if (e.OldItems != null)
            foreach (Point p in e.OldItems)
            {
                if (_visualKeys.Remove(p))
                    PointRemoved?.Invoke(this, p);
            }
    }
    public void AddNewPointFromUI(double x, double y)
    {
        _model.Add(x, y);
    }

    public void MovePoint(Point point, double x, double y)
    {
        if (point == null) return;
        point.X = x;
        point.Y = y;
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
