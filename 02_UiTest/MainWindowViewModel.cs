using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace UiTest;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private int _counter = 0;

    [ObservableProperty]
    private string _status = "Środowisko Avalonia + .NET 10 uruchomione poprawnie.";

    [RelayCommand]
    private void Increment()
    {
        Counter++;
        Status = $"Kliknięto {Counter} raz(y) - bindowanie MVVM działa.";
    }
}
