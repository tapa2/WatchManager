using System.Collections.ObjectModel;

namespace WatchManager.Views
{
    public partial class AdvancedSearchPage : ContentPage
    {
        private readonly WatchList watchList;

        public AdvancedSearchPage(WatchList watchList)
        {
            InitializeComponent();
            this.watchList = watchList;
        }

        private async void OnSearchClicked(object sender, EventArgs e)
        {
            string brand = BrandEntry.Text?.Trim();
            decimal.TryParse(MinPriceEntry.Text, out decimal minPrice);
            decimal.TryParse(MaxPriceEntry.Text, out decimal maxPrice);

            var searchResults = watchList.GetAllWatches().Where(w =>
                (string.IsNullOrEmpty(brand) || w.Brand.Contains(brand, StringComparison.OrdinalIgnoreCase)) &&
                (minPrice == 0 || w.Price >= minPrice) &&
                (maxPrice == 0 || w.Price <= maxPrice)).ToList();

            if (searchResults.Any())
            {
                await Navigation.PushAsync(new SearchWatchesPage(new ObservableCollection<Watch>(searchResults)));
            }
            else
            {
                await DisplayAlert("Результати пошуку", "Годинники за вказаними параметрами не знайдено.", "OK");
            }
        }
    }
}
