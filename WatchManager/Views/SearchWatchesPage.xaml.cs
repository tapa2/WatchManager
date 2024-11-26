using System.Collections.ObjectModel;
using System.Windows.Input;

namespace WatchManager.Views
{
    public partial class SearchWatchesPage : ContentPage
    {
        public ObservableCollection<Watch> Watches { get; set; }
        public ObservableCollection<Watch> FilteredWatches { get; set; }
        public string SearchQuery { get; set; }
        public ICommand SearchCommand { get; }

        public SearchWatchesPage(ObservableCollection<Watch> watches)
        {
            InitializeComponent();

            Watches = watches;
            FilteredWatches = new ObservableCollection<Watch>(Watches);
            SearchCommand = new Command(ExecuteSearch);

            BindingContext = this;
        }

        private void ExecuteSearch()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                FilteredWatches.Clear();
                foreach (var watch in Watches)
                    FilteredWatches.Add(watch);
            }
            else
            {
                var query = SearchQuery.ToLower();
                var results = Watches.Where(w =>
                    w.Brand.ToLower().Contains(query) ||
                    w.Model.ToLower().Contains(query)).ToList();

                FilteredWatches.Clear();
                foreach (var watch in results)
                    FilteredWatches.Add(watch);
            }
        }
    }
}
