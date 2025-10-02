using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Sweeper.Models;

public class Dot: INotifyPropertyChanged
{
    private Math.Point midpoint;

    public event PropertyChangedEventHandler? PropertyChanged;

    public Guid Id => midpoint.Id;
    public double X
    {
        get => midpoint.X;
        set
        {
            if (midpoint.X != value)
            {
                midpoint.X = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(X)));
            }
        }
    }

    public double Y
    {
        get => midpoint.Y;
        set
        {
            if (midpoint.Y != value)
            {
                midpoint.Y = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Y)));
            }

        }
    }

    public Dot(Math.Point point)
    {
        midpoint = point;
    }
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

}
