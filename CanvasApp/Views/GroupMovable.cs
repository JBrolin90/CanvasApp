using System;
using Avalonia;

namespace Sweeper.Views;

internal interface IGroupMovable
{
    internal Guid Id { get; }
    internal IGroupMovable GetMover(Guid guid);
    internal void MoveMe(Point point);
}
