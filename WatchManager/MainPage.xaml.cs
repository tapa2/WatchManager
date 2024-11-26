using System.Collections.ObjectModel;
using System.Text.Json;
using WatchManager.Views;


namespace WatchManager
{
    public partial class MainPage : ContentPage
    {
        private WatchList watchList;

        private string filePath;

        // Шлях до файлу за замовчуванням
        private readonly string defaultFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Watches.json");

        public ObservableCollection<Watch> Watches { get; set; }


        public MainPage()
        {
            InitializeComponent();
            filePath = defaultFilePath;
            LoadWatches();

            BindingContext = this; // Встановлення контексту прив'язки для сторінки
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
                    DisplayAlert("Помилка", $"Не вдалося завантажити дані: {ex.Message}", "OK");
                    watchList = new WatchList();
                }
            }
            else
            {
                watchList = new WatchList();
            }

            Watches = new ObservableCollection<Watch>(watchList.GetAllWatches());
        }

        private void SaveWatchesToFile()
        {
            try
            {
                JsonFileHandler.SaveWatches(filePath, watchList.GetAllWatches());
            }
            catch (Exception ex)
            {
                DisplayAlert("Помилка", $"Не вдалося зберегти дані: {ex.Message}", "OK");
            }
        }

        // Обробник події для вибору файлу JSON
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
                    PickerTitle = "Виберіть JSON файл",
                    FileTypes = jsonFileType
                });

                if (result != null)
                {
                    // Читання вмісту вибраного файлу
                    string fileContent = await File.ReadAllTextAsync(result.FullPath);

                    // Десеріалізація JSON в об'єкти
                    var watches = JsonFileHandler.DeserializeWatches(fileContent);

                    if (watches != null)
                    {
                        filePath = result.FullPath;
                        watchList = new WatchList(watches);
                        RefreshWatches();
                        await DisplayAlert("Успіх", "Дані успішно завантажені!", "OK");
                    }
                    else
                    {
                        await DisplayAlert("Помилка", "Файл JSON містить некоректні дані.", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Помилка", $"Не вдалося завантажити файл: {ex.Message}", "OK");
            }
        }

        private void RefreshWatches()
        {
            Watches.Clear();
            foreach (var watch in watchList.GetAllWatches())
            {
                Watches.Add(watch);
            }
        }

        private async void OnSaveChangesClicked(object sender, EventArgs e)
        {
            try
            {
                SaveWatchesToFile();
                await DisplayAlert("Успіх", "Зміни успішно збережені!", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Помилка", $"Сталася помилка при збереженні: {ex.Message}", "OK");
            }
        }

        private async void OnRemoveWatchClicked(object sender, EventArgs e)
        {
            try
            {
                string idStr = await DisplayPromptAsync("Видалити годинник", "Введіть ID годинника для видалення:");
                if (!int.TryParse(idStr, out int id) || id <= 0)
                {
                    await DisplayAlert("Помилка", "ID має бути додатним числовим значенням.", "OK");
                    return;
                }

                var Watch = Watches.FirstOrDefault(w => w.Id == id);
                if (Watch != null)
                {
                    bool confirm = await DisplayAlert("Підтвердження", $"Ви дійсно хочете видалити годинник \"{Watch.Brand}\"?", "Так", "Ні");
                    if (!confirm) return;

                    watchList.RemoveWatch(Watch.Id);
                    SaveWatchesToFile();
                    await DisplayAlert("Успіх", $"Годинник \"{Watch.Brand}\" видалено успішно.", "OK");
                    RefreshWatches();
                }
                else
                {
                    await DisplayAlert("Помилка", "Годинник з таким ID не знайдено.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Помилка", $"Сталася помилка: {ex.Message}", "OK");
            }
        }

        private async void OnAddWatchClicked(object sender, EventArgs e)
        {
            var newWatch = new Watch
            {
                Id = Watches.Count + 1
            };

            await Navigation.PushAsync(new AddEditWatchPage(newWatch, "Додати годинник", Watch =>
            {
                watchList.AddWatch(Watch);
                SaveWatchesToFile();
                RefreshWatches();
            }));
        }

        private async void OnEditWatchClicked(object sender, EventArgs e)
        {
            try
            {
                string idStr = await DisplayPromptAsync("Редагувати годинник", "Введіть ID годинника:");
                if (!int.TryParse(idStr, out int id))
                {
                    await DisplayAlert("Помилка", "ID має бути числовим значенням.", "OK");
                    return;
                }

                var Watch = Watches.FirstOrDefault(w => w.Id == id);
                if (Watch == null)
                {
                    await DisplayAlert("Помилка", "Годинник з таким ID не знайдено.", "OK");
                    return;
                }

                await Navigation.PushAsync(new AddEditWatchPage(Watch, "Редагувати годинник", updatedWatch =>
                {
                    watchList.EditWatch(updatedWatch.Id, updatedWatch);
                    SaveWatchesToFile();
                    RefreshWatches();
                }));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Помилка", $"Сталася помилка: {ex.Message}", "OK");
            }
        }

        private async void OnAdvancedSearchClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AdvancedSearchPage(watchList));
        }

        private async void OnAboutClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AboutPage());
        }
    }
}
