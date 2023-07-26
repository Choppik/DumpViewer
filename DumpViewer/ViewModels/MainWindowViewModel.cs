using DumpViewer.Store;
using DumpViewer.ViewModels.Base;

namespace DumpViewer.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private readonly NavigationStore _navigationStore;
        public BaseViewModel? CurrentViewModel => _navigationStore.CurrentViewModel;

        #region Заголовок окна
        private string _title = "DumpViewer";

        public string Title { get => _title; set => Set(ref _title, value); }
        #endregion

        public MainWindowViewModel(NavigationStore navigationStore)
        {
            _navigationStore = navigationStore;
            _navigationStore.CurrentViewModelChanged += OnCurrentViewModelChange;
        }

        private void OnCurrentViewModelChange()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }
    }
}