namespace WatchManager.Views
{
    public partial class AboutPage : ContentPage
    {
        public AboutPage()
        {
            InitializeComponent();
        }


        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync(); // Перехід назад на попередню сторінку.
        }
    }
}
