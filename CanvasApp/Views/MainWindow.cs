using Avalonia.Controls;
using Sweeper.ViewModels;
using Sweeper.Views;

namespace avalonia.app;

public partial class MainWindow : Window
{
    private readonly MainViewModel _vm;
    private readonly GraphCanvas _graphCanvas;

    public MainWindow()
    {
        Title = "NavIntel Graph Editor";
        Width = 400;  // Force small window
        Height = 300;

        _vm = new MainViewModel();
        DataContext = _vm;

        _graphCanvas = new GraphCanvas(_vm);
        Content = _graphCanvas.Canvas;

        _graphCanvas.UpdateLayout(this);

    }


}