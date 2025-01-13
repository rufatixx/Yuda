using Android.Runtime;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;

namespace yuda
{
    public partial class MainPage : ContentView
    {
        private WeatherViewModel _viewModel;

        public MainPage()
        {
            try
            {
                InitializeComponent();
                SetFormattedDate();
                LoadDataAsync();
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void SetFormattedDate()
        {
            var ci = new CultureInfo("az-AZ");
            var today = DateTime.Now;
            var monthName = ci.DateTimeFormat.GetMonthName(today.Month);
            monthName = ci.TextInfo.ToTitleCase(monthName);
            var formattedDate = $"{monthName} {today.Day}, {today.Year}";

            MainThread.BeginInvokeOnMainThread(() =>
            {
                todayDate.Text = formattedDate;
            });
        }

        private void ShowError(string message)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                var errorLabel = new Label
                {
                    Text = $"Error: {message}",
                    TextColor = Colors.Red,
                    FontSize = 16,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                };

                Content = new StackLayout
                {
                    Children = { errorLabel }
                };
            });
        }

        private void LoadDataAsync()
        {
            _viewModel = new WeatherViewModel(mainImage);
            BindingContext = _viewModel;

            _viewModel.FirstRun();
        }
    }

    [Preserve(AllMembers = true)]
    public class WeatherViewModel : BindableObject
    {
        private ObservableCollection<RainyDay> _weatherItems;
        public ObservableCollection<RainyDay> WeatherItems
        {
            get => _weatherItems;
            set
            {
                _weatherItems = value;
                OnPropertyChanged();
            }
        }

        private bool _showRainyDays;
        public bool ShowRainyDays
        {
            get => _showRainyDays;
            set
            {
                _showRainyDays = value;
                OnPropertyChanged();
            }
        }

        private string _locationName;
        public string LocationName
        {
            get => _locationName;
            set
            {
                _locationName = value;
                OnPropertyChanged();
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        private ImageSource _emoji;
        public ImageSource Emoji
        {
            get => _emoji;
            set
            {
                _emoji = value;
                OnPropertyChanged();
            }
        }

        private FormattedString _carWashRecommendation;
        public FormattedString CarWashRecommendation
        {
            get => _carWashRecommendation;
            set
            {
                _carWashRecommendation = value;
                OnPropertyChanged();
            }
        }

        private bool _showRequestPermissionButton;
        public bool ShowRequestPermissionButton
        {
            get => _showRequestPermissionButton;
            set
            {
                _showRequestPermissionButton = value;
                OnPropertyChanged();
            }
        }

        public ICommand RefreshDataCommand { get; }
        public ICommand OpenSettingsCommand { get; }

        private readonly Image _mainImage;
        private bool _isRefreshing;

        private bool _isFirtstLoading;
        public bool IsFirtstLoading
        {
            get => _isFirtstLoading;
            set
            {
                _isFirtstLoading = value;
                OnPropertyChanged();
            }
        }

        public WeatherViewModel(Image mainImage)
        {
            _mainImage = mainImage;
            RefreshDataCommand = new Command(async () => await RefreshDataAsync(), () => !_isRefreshing);
            OpenSettingsCommand = new Command(OpenAppSettings);
            IsFirtstLoading = true;
        }

        public async void FirstRun()
        {
            IsFirtstLoading = true; // Show the loading indicator
            await InitializeAsync();
            IsFirtstLoading = false; // Hide the loading indicator
        }

        private async Task RefreshDataAsync()
        {
            if (_isRefreshing) return;

            _isRefreshing = true;
            IsLoading = true;
            await InitializeAsync();
            IsLoading = false;
            _isRefreshing = false;
        }

        private async Task InitializeAsync()
        {
            try
            {
                var status = await RequestLocationPermissionAsync();

                if (status == PermissionStatus.Granted)
                {
                    ShowRequestPermissionButton = false;

                    var location = await Geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Best));
                    if (location != null)
                    {
                        var weatherData = await WeatherService.GetWeatherDataAsync(location.Latitude, location.Longitude);
                        if (weatherData != null)
                        {
                            UpdateWeatherData(weatherData);
                        }
                    }
                }
                else
                {
                    SetPermissionDeniedUI();
                }
            }
            catch (Exception ex)
            {
                SetErrorUI(ex.Message);
            }
            finally
            {
                ToggleImageAnimation();
            }
        }

        private async Task<PermissionStatus> RequestLocationPermissionAsync()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }
            return status;
        }

        private void UpdateWeatherData(WeatherData weatherData)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                SetWeatherEmoji(weatherData);
                SetCarWashRecommendation(weatherData);

                WeatherItems = new ObservableCollection<RainyDay>();
                foreach (var day in weatherData.Forecast.ForecastDay)
                {
                    if (day.Day.AvgHumidity > 70 || day.Day.Condition.Text.Contains("Rain"))
                    {
                        var culture = new CultureInfo("az-Latn-AZ");
                        var dateOfTheDay = Convert.ToDateTime(day.Date);
                        WeatherItems.Add(new RainyDay
                        {
                            WeatherIcon = $"https:{day.Day.Condition.Icon}",
                            Day = dateOfTheDay.ToString("dddd, dd.MM", culture),
                            Temperature = $"{day.Day.AvgHumidity}%"
                        });
                        ShowRainyDays = true;
                    }
                }

                LocationName = weatherData.Location.Name;
            });
        }

        private void SetPermissionDeniedUI()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Emoji = ImageSource.FromFile("error.gif");
                LocationName = "Məkan təyin edilməyib";
                CarWashRecommendation = new FormattedString
                {
                    Spans = { new Span { Text = "Məkan icazəsi verilməyib. Təkrar icazə istə düyməsini sıxaraq yenidən cəhd edin.", FontAttributes = FontAttributes.Italic } }
                };
                ShowRequestPermissionButton = true;
            });
        }

        private void SetErrorUI(string errorMessage)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Emoji = ImageSource.FromFile("error.gif");
                LocationName = "Məkan təyin edilməyib";
                CarWashRecommendation = new FormattedString
                {
                    Spans = { new Span { Text = $"Xəta baş verdi: {errorMessage}", FontAttributes = FontAttributes.Italic } }
                };
            });
        }

        private void ToggleImageAnimation()
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                _mainImage.IsAnimationPlaying = false;
                await Task.Delay(100);
                _mainImage.IsAnimationPlaying = true;
            });
        }

        private void SetCarWashRecommendation(WeatherData weatherData)
        {
            var formattedText = new FormattedString();
            if (weatherData.IsCarWashRecommended)
            {
                formattedText.Spans.Add(new Span { Text = "Avtomobilinizi yuya bilərsiniz", FontAttributes = FontAttributes.Bold });
                formattedText.Spans.Add(new Span { Text = ", yaxın 7 gün ərzində hava şəraiti əlverişlidir" });
            }
            else
            {
                formattedText.Spans.Add(new Span { Text = "Avtomobilinizi yumayın", FontAttributes = FontAttributes.Bold });
                formattedText.Spans.Add(new Span { Text = ", yaxın 7 gün ərzində hava şəraiti əlverişsizdir" });
            }
            CarWashRecommendation = formattedText;
        }

        private void SetWeatherEmoji(WeatherData weatherData)
        {
            Emoji = weatherData.IsCarWashRecommended
                ? ImageSource.FromFile("sun.png")
                : ImageSource.FromFile("rain.png");
        }

        public void OpenAppSettings()
        {
            try
            {
                Launcher.OpenAsync(new Uri("app-settings:az.rufatixx.yuda"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Cannot open app settings: {ex.Message}");
            }
        }
    }

    public static class WeatherService
    {
        private const string APIKey = "750f2b83daae4fbb812120207250101";
        private const string ForecastUrl = "https://api.weatherapi.com/v1/forecast.json";

        public static async Task<WeatherData> GetWeatherDataAsync(double latitude, double longitude)
        {
            string latStr = latitude.ToString(CultureInfo.InvariantCulture);
            string lonStr = longitude.ToString(CultureInfo.InvariantCulture);

            var url = $"{ForecastUrl}?q={latStr},{lonStr}&days=7&key={APIKey}";

            var httpClient = new HttpClient();
            using var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var weatherData = JsonSerializer.Deserialize<WeatherData>(content, options);

            if (weatherData != null && weatherData.Forecast?.ForecastDay != null)
            {
                weatherData.IsCarWashRecommended = CheckCarWashRecommendation(weatherData);
            }

            return weatherData;
        }

        private static bool CheckCarWashRecommendation(WeatherData weatherData)
        {
            bool goodConditions = true;

            foreach (var day in weatherData.Forecast.ForecastDay)
            {
                if (day.Day.AvgHumidity > 70 || day.Day.Condition.Text.Contains("Rain"))
                {
                    goodConditions = false;
                    break;
                }
            }

            return goodConditions;
        }
    }

    public class WeatherData
    {
        public Location Location { get; set; }
        public Current Current { get; set; }
        public Forecast Forecast { get; set; }
        public bool IsCarWashRecommended { get; set; }
    }

    public class Location
    {
        public string Name { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
    }

    public class Current
    {
        public string LastUpdated { get; set; }
        public double TempC { get; set; }
        public double TempF { get; set; }
        public Condition Condition { get; set; }
        public int Humidity { get; set; }
        public double WindKph { get; set; }
    }

    public class Condition
    {
        public string Text { get; set; }
        public string Icon { get; set; }
    }

    public class Forecast
    {
        public List<ForecastDay> ForecastDay { get; set; }
    }

    public class ForecastDay
    {
        public string Date { get; set; }
        public Day Day { get; set; }
    }

    public class Day
    {
        public double MaxtempC { get; set; }
        public double MintempC { get; set; }
        public double AvgtempC { get; set; }
        public double AvgHumidity { get; set; }
        public Condition Condition { get; set; }
    }

    public class RainyDay
    {
        public string WeatherIcon { get; set; }
        public string Day { get; set; }
        public string Temperature { get; set; }
    }
}
