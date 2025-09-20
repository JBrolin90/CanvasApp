using System;

namespace Sweeper.Math;

public class Segment
{
    private readonly Point startPoint;
    private readonly Point endPoint;
    private readonly Guid id;
    public Guid Id { get { return id; } }

    public Point StartPoint
    {
        get => startPoint;
    }

    public Point EndPoint
    {
        get => endPoint;
    }


    Segment(Point startPoint, Point endPoint)
    {
        this.startPoint = startPoint;
        this.endPoint = endPoint;
        id = Guid.NewGuid();
    }

}
