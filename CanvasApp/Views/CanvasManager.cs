using System;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;
using Sweeper.ViewModels;

namespace Sweeper.Views;

public class CanvasManager
{
    private Canvas canvas;
    private CanvasViewModel vm;
    public Canvas Canvas
    {
        get { return canvas; }
        private set { canvas = value; }
    }
    CanvasManager(CanvasViewModel vm)
    {
        canvas = new Canvas
        {
            Background = Brushes.Transparent // Needed so empty areas receive pointer events
        };
        this.vm = vm;
        RegisterEventHandlers();
    }

    private void RegisterEventHandlers()
    {
        canvas.PointerPressed += PointerPressed;
    }
    private void PointerPressed(object? source, PointerPressedEventArgs e)
    {
        if (e.Source is Ellipse)
        {
            return; // Ellipse handles its own press
        }
        if (!e.GetCurrentPoint(canvas).Properties.IsLeftButtonPressed) return;
        Avalonia.Point pt = e.GetPosition(canvas);
        vm.AddNewPointFromUI(pt.X, pt.Y);

    }

}
