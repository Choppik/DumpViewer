using DumpViewer.Command;
using DumpViewer.ViewModels.Base;
using System.Windows.Input;

namespace DumpViewer.ViewModels
{
    public class DumpViewerViewModel : BaseViewModel
    {
        private string _resultStr = "";
        public string ResultStr
        {
            get => _resultStr;
            set
            {
                Set(ref _resultStr, value);
                OnPropertyChanged(nameof(ResultStr));
            }
        }
        public ICommand OpenFileCommand { get; }
        public DumpViewerViewModel()
        {
            OpenFileCommand = new OpenFileCommand(this);
        }
    }
}