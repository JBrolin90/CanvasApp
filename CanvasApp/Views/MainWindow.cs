using Avalonia.Controls;
using Sweeper.ViewModels;
using Sweeper.Views;

namespace avalonia.app;

public partial class MainWindow : Window
{
    private readonly MainViewModel vm;
    private readonly GraphCanvas _graphCanvas;

    public MainWindow()
    {
        Title = "NavIntel Graph Editor";
        Width = 400;  // Force small window
        Height = 300;

        vm = new MainViewModel();
        DataContext = vm;

        _graphCanvas = new GraphCanvas(this, vm);
        Content = _graphCanvas.Canvas;
    }


}