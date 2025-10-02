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
        private readonly Canvas _canvas;
        private readonly Dictionary<Guid, Shape> _visualsByKey = new();
        private readonly Dictionary<Guid, Segment> _segmentsByKey = new();
        private readonly MainViewModel _vm;

        public Canvas Canvas => _canvas;

        public GraphCanvas(MainViewModel viewModel)
        {
            _vm = viewModel;
            _canvas = new Canvas
            {
                Width = 8000,
                Height = 400,
                Background = Brushes.Transparent
            };

            SetupEventHandlers();
            AddStatusElements();
        }


        private void SetupEventHandlers()
        {
            _vm.UiEventRaised += HandleUiEvent;
            _canvas.PointerPressed += HandleCanvasPointerPressed;
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
            if (!e.GetCurrentPoint(_canvas).Properties.IsLeftButtonPressed) return;

            var pt = e.GetPosition(_canvas);
            _vm.AddNewPointFromUI(pt.X, pt.Y);
        }

        private void AddNewDot(Math.Point p)
        {
            var dot = new Dot(p, this);
            _canvas.Children.Add(dot);
        }

        private void OnDotMoved(Guid id, double x, double y)
        {
            _vm.MovePoint(id, x, y);
        }

        readonly TextBlock title = new()
        {
            Text = "Welcome to Avalonia c# only markup!",
            TextAlignment = TextAlignment.Center,
            FontSize = 24
        };

        readonly TextBlock info = new()
        {
            FontSize = 14,
            Foreground = Brushes.Gray
        };
        private void AddStatusElements()
        {
            // void updateInfo() => info.Text = $"Points: {_vm.Points.Count}";
            // updateInfo();
            // _vm.Points.CollectionChanged += (_, _) => updateInfo();

            // Center and keep centered both blocks.
            _canvas.Children.Add(title);
            _canvas.Children.Add(info);
        }

        private static void CenterText(Canvas canvas, TextBlock text)
        {
            if (canvas.Bounds.Width <= 0 || canvas.Bounds.Height <= 0) return;
            text.Measure(new Avalonia.Size(double.PositiveInfinity, double.PositiveInfinity));
            var size = text.DesiredSize;
            Canvas.SetLeft(text, (canvas.Bounds.Width - size.Width) / 2);
            Canvas.SetTop(text, (canvas.Bounds.Height - size.Height) / 2);
        }
        private static void PositionInfo(Canvas canvas, TextBlock info, TextBlock title)
        {
            info.Measure(new Avalonia.Size(double.PositiveInfinity, double.PositiveInfinity));
            title.Measure(new Avalonia.Size(double.PositiveInfinity, double.PositiveInfinity));
            var infoSize = info.DesiredSize;
            var titleSize = title.DesiredSize;
            Canvas.SetLeft(info, (canvas.Bounds.Width - infoSize.Width) / 2);
            Canvas.SetTop(info, (canvas.Bounds.Height - titleSize.Height) / 2 + titleSize.Height + 8);
        }
        public void UpdateLayout(Window window)
        {
            window.Opened += (_, _) => { CenterText(_canvas, title); PositionInfo(_canvas, info, title); };
            window.SizeChanged += (_, _) => { CenterText(_canvas, title); PositionInfo(_canvas, info, title); };
        }
        List<IGroupMovable>? groupMovables = [];

        void IGroupMover.PrepareMove(Guid guid)
        {
            // Create a list of Canvas children where the Tag equals guid
            groupMovables = new List<IGroupMovable>();
            foreach (var child in _canvas.Children)
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