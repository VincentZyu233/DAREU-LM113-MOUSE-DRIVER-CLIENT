using Avalonia.Controls;

namespace Dareu.LM113.App.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Opened += (s, e) =>
        {
            Topmost = true;
            Activate();
            Focus();
            Topmost = false; // 闪烁一下置顶后恢复正常窗口层级
        };
    }
}
