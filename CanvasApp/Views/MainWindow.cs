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
        _vm = new MainViewModel();
        DataContext = _vm;

        _graphCanvas = new GraphCanvas(_vm);

        Content = _graphCanvas.Canvas;
        _graphCanvas.UpdateLayout();

    }


}