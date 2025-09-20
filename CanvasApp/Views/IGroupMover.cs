using System;
using Avalonia;

namespace Sweeper.Views;

public interface IGroupMover
{
    internal void PrepareMove(Guid guid);
    internal void MoveGroup(Guid guid, Point point);
    internal void CompleteMoveGroup(Guid guid, Point point);

}
