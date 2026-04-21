using Avalonia;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using JobRunner.Avalonia.ViewModels;

namespace JobRunner.Avalonia;

public partial class AddTaskWindow : ReactiveWindow<AddTaskWindowVM>
{
    public AddTaskWindow()
    {
        InitializeComponent();
    }
}