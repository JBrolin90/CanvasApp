using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Sweeper.ViewModels;
using Avalonia;

namespace Sweeper.Views
{
    public class GraphCanvas: IGroupMover
    {
        private readonly Canvas canvas;
        private readonly Dictionary<Guid, Shape> _visualsByKey = new();
        private readonly Dictionary<Guid, Segment> _segmentsByKey = new();
        private readonly MainViewModel _vm;
        internal readonly Statistics statistics;

        public Canvas Canvas => canvas;

        public GraphCanvas(Window parent, MainViewModel viewModel)
        { 
            _vm = viewModel;
        
            canvas = new Canvas
            {
                Width = 8000,
                Height = 400,
                Background = Brushes.Transparent
            };
            SetupEventHandlers();
            statistics = new(canvas);
            statistics.UpdateLayout(parent);
        }


        private void SetupEventHandlers()
        {
            _vm.UiEventRaised += HandleUiEvent;
            canvas.PointerPressed += HandleCanvasPointerPressed;
        }

        private void HandleUiEvent(object? sender, UiEvent evt)
        {
            switch (evt.Kind)
            {
                case UiEventKind.PointAdded:
                    if (evt.Payload is Math.Point point)
                        AddNewDot(point);
                    break;
                case UiEventKind.FlashPoint:
                    // Handle flash logic
                    break;
            }
        }

        private void HandleCanvasPointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            if (e.Source is Shape) return;
            if (!e.GetCurrentPoint(canvas).Properties.IsLeftButtonPressed) return;

            var pt = e.GetPosition(canvas);
            _vm.AddNewPointFromUI(pt.X, pt.Y);
        }

        private void AddNewDot(Math.Point p)
        {
            var dot = new Dot(p, this);
            canvas.Children.Add(dot);
        }

        private void OnDotMoved(Guid id, double x, double y)
        {
            _vm.MovePoint(id, x, y);
        }

        List<IGroupMovable>? groupMovables = [];

        void IGroupMover.PrepareMove(Guid guid)
        {
            // Create a list of Canvas children where the Tag equals guid
            groupMovables = new List<IGroupMovable>();
            foreach (var child in canvas.Children)
            {
                var shape = child as IGroupMovable;
                if (shape is not null)
                {
                    if (shape.Id == guid)
                    {
                        groupMovables.Add(shape);
                    }
                }
            }
        }

        void IGroupMover.MoveGroup(Guid guid, Point point)
        {
            foreach (var movable in groupMovables!)
            {
                movable.MoveMe(point);
            }
        }

        void IGroupMover.CompleteMoveGroup(Guid guid, Point point)
        {
            OnDotMoved(guid, point.X, point.Y);
        }
    }
}