using Avalonia.Automation;
using Avalonia.Automation.Peers;
using Avalonia.Controls;
using System.ComponentModel;

namespace UiTest;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Opened += (_, _) =>
        {
            KlikButton.Focus();

            if (DataContext is INotifyPropertyChanged vm)
            {
                vm.PropertyChanged += (_, e) =>
                {
                    if (e.PropertyName == nameof(MainWindowViewModel.Status))
                    {
                        var peer = ControlAutomationPeer.CreatePeerForElement(StatusText);
                        peer.RaisePropertyChangedEvent(AutomationElementIdentifiers.NameProperty, null, StatusText.Text);
                    }
                };
            }
        };
    }
}
