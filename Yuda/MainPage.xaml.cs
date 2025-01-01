using Microsoft.Maui.Controls;
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
            //InitializeComponent();
            //LoadDataAsync();
            InitializeComponent();
            var ci = new CultureInfo("az-AZ");
            var today = DateTime.Now;
            var monthName = ci.DateTimeFormat.GetMonthName(today.Month);
            monthName = ci.TextInfo.ToTitleCase(monthName);
            var formattedDate = $"{monthName} {today.Day}, {today.Year}";
            todayDate.Text = formattedDate;
            LoadDataAsync();
           
        }
        //protected  override void OnAppearing()
        //{
        //    base.OnAppearing();
           
          
        //    AppShell.SetNavBarIsVisible(this, false);
        //}

        //protected override void OnDisappearing()
        //{
        //    base.OnDisappearing();
        //    AppShell.SetNavBarIsVisible(this, true);
        //}
        private void LoadDataAsync()
        {
          
            _viewModel = new WeatherViewModel(mainImage);
            BindingContext = _viewModel;
             _viewModel.FirstRun();
          
            //BindingContext = await WeatherViewModel.CreateAsync();
        }
    }

    public class WeatherViewModel : BindableObject
    {
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
        Image mImage;
        public WeatherViewModel(Image _mainImage)
        {
            mImage = _mainImage;
            RefreshDataCommand = new Command(async () =>
            {
                IsLoading = true;
                // additional code here
                await InitializeAsync();
                // additional code here
                IsLoading = false;
            });
            OpenSettingsCommand = new Command(OpenAppSettings);
        }
        public async void FirstRun()
        {
            IsFirtstLoading = true;
            await InitializeAsync();
            IsFirtstLoading = false;
        }

            public async Task InitializeAsync()
        {
           
           
            try
            {
                var status = await RequestLocationPermissionAsync();

                if (status == PermissionStatus.Granted)
                {
                    ShowRequestPermissionButton = false;

                    var request = new GeolocationRequest(GeolocationAccuracy.Best);
                    var location = await Geolocation.GetLocationAsync(request);
                    if (location != null)
                    {
                        var weatherData = await WeatherService.GetWeatherDataAsync(location.Latitude, location.Longitude);
                        if (weatherData != null)
                        {
                            SetWeatherEmoji(weatherData);
                            SetCarWashRecommendation(weatherData);
                        }
                    }
                }
                else
                {
                    ShowRequestPermissionButton = true;
                    // Set sad emoji
                    //Emoji = "😔";
                    Emoji = ImageSource.FromFile("error.gif");
                    LocationName = $"Məkan təyin edilməyib";
                    // Set formatted text for not granting location permission

                    CarWashRecommendation = new FormattedString
                    {
                        Spans =
                {
                    new Span { Text = "Məkan icazəsi verilməyib. Təkrar icazə istə düyməsini sıxaraq yenidən cəhd edin.", FontAttributes = FontAttributes.Italic }
                }
                    };
                }
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Feature is not enabled on device
                //Emoji = "😔";
                Emoji = ImageSource.FromFile("error.gif");
                LocationName = $"Məkan təyin edilməyib";
                CarWashRecommendation = new FormattedString
                {
                    Spans =
                {
                    new Span { Text = "Məkan xidməti aktiv deyil. Zəhmət olmasa, cihazınızdakı məkan xidmətini aktiv edin.", FontAttributes = FontAttributes.Italic }
                }
                };
            }
            catch (Exception ex)
            {
                Emoji = ImageSource.FromFile("error.gif");
                LocationName = $"Məkan təyin edilməyib";
                //Emoji = "😔";
                CarWashRecommendation = new FormattedString
                {
                    Spans =
            {
                new Span { Text = "Xəta baş verdi: " + ex.Message, FontAttributes = FontAttributes.Italic }
            }
                };
            }
            finally
            {
                await Task.Delay(100);
                mImage.IsAnimationPlaying = false;
                await Task.Delay(100);
                mImage.IsAnimationPlaying = true;

           
            }
        }
        public void OpenAppSettings()
        {
            var appSettingsUrl = "app-settings:az.codlee.yuda";
            try
            {
                Launcher.OpenAsync(new Uri(appSettingsUrl));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Cannot open app settings: {ex.Message}");
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

        private void SetCarWashRecommendation(WeatherData weatherData)
        {
            var formattedText = new FormattedString();

            if (weatherData.IsCarWashRecommended)
            {
                formattedText.Spans.Add(new Span { Text = "Avtomobilinizi yuya bilərsiniz", FontAttributes = FontAttributes.Bold });
                formattedText.Spans.Add(new Span { Text = $", yaxın 7 gün ərzində " });
                LocationName = $"{weatherData.Location.Name}";
              //  formattedText.Spans.Add(new Span { Text = $"'{weatherData.Location.Name}' ", FontAttributes = FontAttributes.Bold });
                formattedText.Spans.Add(new Span { Text = $"hava şəraiti əlverişlidir" });
            }
            else
            {
                formattedText.Spans.Add(new Span { Text = "Avtomobilinizi yumayın", FontAttributes = FontAttributes.Bold });
                formattedText.Spans.Add(new Span { Text = $", yaxın 7 gün ərzində " });
                LocationName = $"{weatherData.Location.Name}";
               // formattedText.Spans.Add(new Span { Text = $"'{weatherData.Location.Name}' ", FontAttributes = FontAttributes.Bold });
                formattedText.Spans.Add(new Span { Text = $"hava şəraiti əlverişsizdir" });
            }

            CarWashRecommendation = formattedText;
        }
        private void SetWeatherEmoji(WeatherData condition)
        {
            //if (condition.Contains("rain", StringComparison.OrdinalIgnoreCase))
            //{
            //    Emoji = "☔️";
            //}
            //else if (condition.Contains("cloud", StringComparison.OrdinalIgnoreCase))
            //{
            //    Emoji = "☁️";
            //}
            //else if (condition.Contains("sun", StringComparison.OrdinalIgnoreCase) || condition.Contains("clear", StringComparison.OrdinalIgnoreCase))
            //{
            //    Emoji = "☀️";
            //}
            //else
            //{
            //    Emoji = "❓";
            //}
            if (condition.IsCarWashRecommended)
            {
                //Emoji = "🚗🌊";
                 Emoji = ImageSource.FromFile("sun.png");

            }

            else
            {
                Emoji = ImageSource.FromFile("rain.png"); 

                //Emoji = "🚗☔";
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


}