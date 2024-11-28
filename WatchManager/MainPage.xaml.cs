using System.Collections.ObjectModel;
using System.Text.Json;
using WatchManager.Views;

namespace WatchManager
{
    public partial class MainPage : ContentPage
    {
        private WatchList watchList;
        private string filePath;
        private readonly string defaultFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "watches.json");
        public List<Watch> Watches { get; set; }

        public MainPage()
        {
            InitializeComponent();
            filePath = defaultFilePath;
            LoadWatches();
            BindingContext = this;
        }

        private void LoadWatches()
        {
            if (File.Exists(filePath))
            {
                try
                {
                    var watches = JsonFileHandler.LoadWatches(filePath);
                    watchList = new WatchList(watches);
                }
                catch (Exception ex)
                {
                    DisplayAlert("Error", $"Failed to load data: {ex.Message}", "OK");
                    watchList = new WatchList();
                }
            }
            else
            {
                watchList = new WatchList();
            }

            Watches = watchList.GetAllWatches();
        }

        private void SaveWatchesToFile()
        {
            try
            {
                JsonFileHandler.SaveWatches(filePath, watchList.GetAllWatches());
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", $"Failed to save data: {ex.Message}", "OK");
            }
        }

        private async void OnChooseFileClicked(object sender, EventArgs e)
        {
            try
            {
                var jsonFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.iOS, new[] { "public.json" } },
                    { DevicePlatform.Android, new[] { "application/json" } },
                    { DevicePlatform.WinUI, new[] { ".json" } },
                    { DevicePlatform.macOS, new[] { "json" } }
                });

                var result = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = "Select a JSON file",
                    FileTypes = jsonFileType
                });

                if (result != null)
                {
                    string fileContent = await File.ReadAllTextAsync(result.FullPath);
                    var watches = JsonFileHandler.DeserializeWatches(fileContent);

                    if (watches != null && JsonFileHandler.IsValidWatchData(watches))
                    {
                        filePath = result.FullPath;
                        watchList = new WatchList(watches);
                        RefreshWatches();
                        await DisplayAlert("Success", "Data successfully loaded!", "OK");
                    }
                    else
                    {
                        await DisplayAlert("Error", "Invalid JSON file data.", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load file: {ex.Message}", "OK");
            }
        }

        private void RefreshWatches()
        {
            Watches = watchList.GetAllWatches();
            BindingContext = null;
            BindingContext = this;
        }

        private async void OnSaveChangesClicked(object sender, EventArgs e)
        {
            try
            {
                SaveWatchesToFile();
                await DisplayAlert("Success", "Changes saved successfully!", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error saving changes: {ex.Message}", "OK");
            }
        }

        private async void OnRemoveWatchClicked(object sender, EventArgs e)
        {
            try
            {
                string idStr = await DisplayPromptAsync("Remove Watch", "Enter the ID of the watch to remove:");
                if (!int.TryParse(idStr, out int id) || id <= 0)
                {
                    await DisplayAlert("Error", "ID must be a positive number.", "OK");
                    return;
                }

                var watch = Watches.FirstOrDefault(w => w.Id == id);
                if (watch != null)
                {
                    bool confirm = await DisplayAlert("Confirm", $"Do you really want to remove the watch \"{watch.Brand} {watch.Model}\"?", "Yes", "No");
                    if (!confirm) return;

                    watchList.RemoveWatch(watch.Id);
                    SaveWatchesToFile();
                    await DisplayAlert("Success", $"Watch \"{watch.Brand} {watch.Model}\" removed successfully.", "OK");
                    RefreshWatches();
                }
                else
                {
                    await DisplayAlert("Error", "Watch with this ID not found.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error: {ex.Message}", "OK");
            }
        }

        private async void OnAddWatchClicked(object sender, EventArgs e)
        {
            var newWatch = new Watch
            {
                Id = Watches.Count + 1
            };

            await Navigation.PushAsync(new AddEditWatchPage(newWatch, "Add Watch", watch =>
            {
                watchList.AddWatch(watch);
                SaveWatchesToFile();
                RefreshWatches();
            }));
        }

        private async void OnEditWatchClicked(object sender, EventArgs e)
        {
            try
            {
                string idStr = await DisplayPromptAsync("Edit Watch", "Enter the ID of the watch to edit:");
                if (!int.TryParse(idStr, out int id))
                {
                    await DisplayAlert("Error", "ID must be a numeric value.", "OK");
                    return;
                }

                var watch = Watches.FirstOrDefault(w => w.Id == id);
                if (watch == null)
                {
                    await DisplayAlert("Error", "Watch with this ID not found.", "OK");
                    return;
                }

                await Navigation.PushAsync(new AddEditWatchPage(watch, "Edit Watch", updatedWatch =>
                {
                    watchList.EditWatch(updatedWatch.Id, updatedWatch);
                    SaveWatchesToFile();
                    RefreshWatches();
                }));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error: {ex.Message}", "OK");
            }
        }

        private async void OnAdvancedSearchClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Views.AdvancedSearchPage(watchList));
        }

        private async void OnAboutClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AboutPage());
        }
    }
}
