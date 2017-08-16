using GalaSoft.MvvmLight;

namespace ECS.ViewModel
{
    public class ResultErrorViewModel : ViewModelBase
    {
        public ResultErrorViewModel(string err, string name)
        {
            ErrorMessage = err;
            Name = name;
        }

        public string ErrorMessage { get; }
        public string Name { get; }
    }
}
