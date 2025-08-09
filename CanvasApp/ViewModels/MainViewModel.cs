using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Sweeper.Models;
using Sweeper.Math;

namespace Sweeper.ViewModels;

/// <summary>
/// ViewModel layer between view and MainModel.
/// Exposes Points and selected point plus simple commands.
/// </summary>
public class MainViewModel : INotifyPropertyChanged
{
    private readonly MainModel _model;

    public ObservableCollection<Point> Points => _model.Points;

    public Point? Selected
    {
        get => _model.Selected;
        set
        {
            if (_model.Selected == value) return;
            _model.Selected = value;
            OnPropertyChanged();
        }
    }

    public MainViewModel() : this(new MainModel()) { }

    public MainViewModel(MainModel model)
    {
        _model = model;
        // seed demo data
        if (Points.Count == 0)
        {
            Points.Add(new Point(10, 10));
            Points.Add(new Point(50, 30));
            Points.Add(new Point(120, 80));
        }
    }

    public void AddPoint(double x, double y)
    {
        var p = new Point(x, y);
        _model.Add(p);
        Selected = p;
        OnPropertyChanged(nameof(Points));
    }

    public void MovePoint(Point point, double x, double y)
    {
        if (point == null) return;
        if (System.Math.Abs(point.X - x) < 0.0001 && System.Math.Abs(point.Y - y) < 0.0001) return;
        point.X = x;
        point.Y = y;
    }

    public void RemovePoint(Point point)
    {
        if (point == null) return;
        _model.Remove(point);
        if (Selected == point) Selected = null;
        OnPropertyChanged(nameof(Points));
    }

    public void RemoveSelected()
    {
        if (Selected is null) return;
        _model.Remove(Selected);
        Selected = null;
        OnPropertyChanged(nameof(Points));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
