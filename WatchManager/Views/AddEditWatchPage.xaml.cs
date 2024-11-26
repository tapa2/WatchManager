using System.Windows.Input;

namespace WatchManager.Views
{
    public partial class AddEditWatchPage : ContentPage
    {
        public Watch Watch { get; set; }
        public string PageTitle { get; set; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        private readonly Action<Watch> _onSaveCallback;

        public AddEditWatchPage(Watch watch, string title, Action<Watch> onSaveCallback)
        {
            InitializeComponent();
            Watch = watch;
            PageTitle = title;
            _onSaveCallback = onSaveCallback;

            SaveCommand = new Command(Save);
            CancelCommand = new Command(Cancel);

            BindingContext = this;
        }

        private async void Save()
        {
            var validationResult = ValidateWatchData();
            if (!validationResult.IsValid)
            {
                await DisplayAlert("Помилка", validationResult.ErrorMessage, "OK");
                return;
            }

            _onSaveCallback?.Invoke(Watch);
            await Navigation.PopAsync();
        }

        private ValidationResult ValidateWatchData()
        {
            if (string.IsNullOrWhiteSpace(Watch.Brand))
                return ValidationResult.Failed("Бренд не може бути порожнім.");

            if (string.IsNullOrWhiteSpace(Watch.Model))
                return ValidationResult.Failed("Модель не може бути порожньою.");

            if (Watch.Price <= 0)
                return ValidationResult.Failed("Ціна має бути більше 0.");

            return ValidationResult.Success();
        }

        private async void Cancel()
        {
            await Navigation.PopAsync();
        }
    }

    public class ValidationResult
    {
        public bool IsValid { get; private set; }
        public string ErrorMessage { get; private set; }

        private ValidationResult(bool isValid, string errorMessage)
        {
            IsValid = isValid;
            ErrorMessage = errorMessage;
        }

        public static ValidationResult Success() => new ValidationResult(true, string.Empty);

        public static ValidationResult Failed(string errorMessage) => new ValidationResult(false, errorMessage);
    }
}
