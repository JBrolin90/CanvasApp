using Sweeper.Models;
using Sweeper.Math;
using System;
using System.Collections.Specialized;

namespace Sweeper.ViewModels;

public enum UiEventKind
{
    PointAdded,
    PointRemoved,
    PointMoved,
    SegmentAdded,        // New: when a segment is added to the model
    SegmentRemoved,      // New: when a segment is removed from the model
    SegmentMoved,        // New: when a segment is moved
    SelectionChanged,
    FlashPoint,
    FocusPoint
}
public sealed record UiEvent(UiEventKind Kind, object? Payload = null);

public partial class MainViewModel
{
    #region INTERFACE
    public event EventHandler<UiEvent>? UiEventRaised;
    public partial Math.Point AddNewPointFromUI(double x, double y);
    public partial bool MovePoint(Guid id, double x, double y);
    #endregion

    #region  CONSTRUCTORS

    public MainViewModel() : this(new MainModel()) { }

    public MainViewModel(MainModel model)
    {
        _model = model;
        _model.Points.CollectionChanged += OnPointsChanged;
    }
    #endregion
    #region IMPLEMENTATION
    private readonly MainModel _model;

    private void Raise(UiEventKind kind, object? payload = null)
        => UiEventRaised?.Invoke(this, new UiEvent(kind, payload));

    private void OnPointsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
            foreach (Point modelPoint in e.NewItems) Raise(UiEventKind.PointAdded, modelPoint);
        if (e.OldItems != null)
            foreach (Point p in e.OldItems) Raise(UiEventKind.PointRemoved, p);
    }
    public partial Math.Point AddNewPointFromUI(double x, double y)
    => _model.GetPoint(x, y);
    public partial bool MovePoint(Guid id, double x, double y)
    {
        if (_model.GetPoint(id) is { } point)
        {
            var oldX = point.X; var oldY = point.Y;
            point.X = x; point.Y = y;
            Raise(UiEventKind.PointMoved, new { Point = point, OldX = oldX, OldY = oldY });
            return true;
        }
        else
            return false;
    }
    #endregion
}
