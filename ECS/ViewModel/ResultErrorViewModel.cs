using GalaSoft.MvvmLight;

namespace ECS.ViewModel
{
    public class ResultErrorViewModel : ViewModelBase
    {
        public ResultErrorViewModel(string err)
        {
            ErrorMessage = err;
        }

        public string ErrorMessage { get; }
    }
}
