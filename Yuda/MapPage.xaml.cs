using System.Collections.ObjectModel;
using System.IO;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace yuda
{
    public partial class MapPage : ContentView
    {

        private ObservableCollection<FilterOption> _filterOptions;
        public ObservableCollection<FilterOption> FilterOptions
        {
            get => _filterOptions;
            set
            {
                _filterOptions = value;
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
        public MapPage()
        {

            InitializeComponent();

            InitializeAsync();


        }
        private async void InitializeAsync()
        {
            await Task.Delay(100);
            LoadingIndicator.IsRunning = false;
            await Task.Delay(100);
            LoadingIndicator.IsRunning = true;
            IsLoading = true;
            await LoadUserLocationAsync();
             LoadFilterOptionsAsync();
            IsLoading = false ;
        }
        private void OnCloseJoinUsClicked(object sender, EventArgs e)
        {
            joinUsBanner.IsVisible = false;
        }
        private async void OnJoinUsButtonTappedAsync(object sender, EventArgs e)
        {

            if (Email.Default.IsComposeSupported)
            {

                string subject = "Əməkdaşlıq üçün sorğu";
                string body = "Salam,\n\nMən sizin şəbəkənizə qoşulmaqda maraqlıyam və hansı xidmətləri təqdim etdiyinizi və qoşulmaq üçün hansı tələblərin olduğunu bilmək istərdim.";
                string[] recipients = new[] { "asadzade99@gmail.com", "asadzade99@icloud.com" };

                var message = new EmailMessage
                {
                    Subject = subject,
                    Body = body,
                    BodyFormat = EmailBodyFormat.PlainText,
                    To = new List<string>(recipients)
                };

                await Email.Default.ComposeAsync(message);
            }
        }
        //protected override void OnAppearing()
        //{
        //    base.OnAppearing();
        //    AppShell.SetNavBarIsVisible(this, false);
        //}
        //private async void OnLocationButtonClicked(object sender, EventArgs e)
        //{
        //    // Center the map on the device's current location
        //    var location = await Geolocation.GetLocationAsync();
        //    if (location != null)
        //    {
        //        var span = new MapSpan(new Microsoft.Maui.Devices.Sensors.Location(location.Latitude, location.Longitude), 0.01, 0.01);
        //        map.Dispatcher.Dispatch(() => map.MoveToRegion(span));
        //    }
        //}
        private async void OnLocationButtonClicked(object sender, EventArgs e)
        {
            // Center the map on the device's current location
            try
            {
                var location = await Geolocation.GetLocationAsync();
                if (location != null)
                {
                    var span = new MapSpan(new Microsoft.Maui.Devices.Sensors.Location(location.Latitude, location.Longitude), 0.01, 0.01);
                    map.Dispatcher.Dispatch(() => map.MoveToRegion(span));
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                await Application.Current.MainPage.DisplayAlert("Xəta", "Cihazınız bu xüsusiyyəti dəstəkləmir.", "Bağla");
            }
            catch (FeatureNotEnabledException fneEx)
            {
                await Application.Current.MainPage.DisplayAlert("Xəta", "Cihazınızda bu xüsusiyyət aktiv deyil.", "Bağla");
            }
            catch (PermissionException pEx)
            {
                await Application.Current.MainPage.DisplayAlert("Xəta", "Uygulamaya cihazınızın məkan xidmətlərinə giriş etməyə icazə verilməyib.", "Bağla");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Xəta", "Məkan təyin edilərkən bir xəta baş verdi.", "Bağla");
            }
        }
        public  void LoadFilterOptionsAsync()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                // Code to run on the main thread
         
            FilterOptions = new ObservableCollection<FilterOption>
            {
                new FilterOption {

                    Name = "Avtoyuma və qayğı",
                    Pins = new List<Pin>
                    {
                      ///////Poland
                        new Pin
                        {
                            Label = "MonoWash Myjnia Samochodowa",
                            Location = new Microsoft.Maui.Devices.Sensors.Location(51.76313569275165, 19.486340099749437),
                            Type = PinType.Place
                        }, new Pin
                        {
                            Label = "AutoSpa",
                            Location = new Microsoft.Maui.Devices.Sensors.Location(51.75807703747513, 19.44297465264492),
                            Type = PinType.Place
                        }, new Pin
                        {
                            Label = "EHRLE CarWash",
                            Address="Wydawnicza 10, 92-333 Łódź",
             Location = new Microsoft.Maui.Devices.Sensors.Location(51.76633887316457, 19.48758094930212),
                            Type = PinType.Place
                        }, new Pin
                        {
                            Label = "Myjnia samochodowa",
                            Address="Admiralska 1, 91-857 Łódź",
             Location = new Microsoft.Maui.Devices.Sensors.Location(51.80240117105883, 19.46436317352298),
                            Type = PinType.Place
                        }
                          ///////Azerbaijan
                        ,
                        new Pin
                        {
                            Label = "Car WOW Baku",
                            Location = new Microsoft.Maui.Devices.Sensors.Location(40.392581, 49.857317),
                            Type = PinType.Place
                        },
                         new Pin
                        {
                            Label = "Avtoyuma",
                            Location = new Microsoft.Maui.Devices.Sensors.Location(40.396291, 49.877736),
                            Type = PinType.Place
                        },
                        new Pin
                        {
                            Label = "Avtoyuma",
                            Location = new Microsoft.Maui.Devices.Sensors.Location(40.5503103607949, 49.71195331445173),
                            Type = PinType.Place
                        },
                         new Pin
                        {
                            Label = "Ceramic pro Sumgayit",
                            Location = new Microsoft.Maui.Devices.Sensors.Location(40.564992, 49.697753),
                            Type = PinType.Place
                        }, new Pin
                        {
                            Label = "Moykar.az",
                            Location = new Microsoft.Maui.Devices.Sensors.Location(40.37657359718434, 49.83450052270812),
                            Type = PinType.Place
                        },new Pin
                        {
                            Label = "A.D.Malacanec",
                            Location = new Microsoft.Maui.Devices.Sensors.Location(40.37125978140019, 49.841579538805114),
                            Type = PinType.Place
                        },new Pin
                        {
                            Label = "50 qəpik avtoyuma",
                            Location = new Microsoft.Maui.Devices.Sensors.Location(40.402102162245534, 49.816436463897126),
                            Type = PinType.Place
                        },
                    }
                },
                new FilterOption {

                    Name = "Servis və xidmət",
                    Pins = new List<Pin>
                    {
                        new Pin
                        {
                            Label = "Hyundai Service Center",
                            Location = new Microsoft.Maui.Devices.Sensors.Location(40.428046, 49.897694),
                            Type = PinType.Place
                        },
                        new Pin
                        {
                            Label = "tekertap.az Teker & Akkumulyator Satis ve Servisi",
                            Location = new Microsoft.Maui.Devices.Sensors.Location(40.395852, 49.877518),
                            Type = PinType.Place
                        },
                        new Pin
                        {
                            Label = "СТО",
                            Location = new Microsoft.Maui.Devices.Sensors.Location(40.40300771623023, 49.81391440844383),
                            Type = PinType.Place
                        }, new Pin
                        {
                            Label = "INTER AVTOSERVIS",
                            Location = new Microsoft.Maui.Devices.Sensors.Location(40.4101658535209, 49.86503292060266),
                            Type = PinType.Place
                        }, new Pin
                        {
                            Label = "AVTOSERVIS",

                        Location = new Microsoft.Maui.Devices.Sensors.Location(40.3913540,49.9042930),
                            Type = PinType.Place
                        },


                    }
                },
                  new FilterOption {

                    Name = "Elektrik",
                    Pins = new List<Pin>
                    {

                        new Pin
                        {
                            Label = "Porsche Destination Charging Station",
                            Location = new Microsoft.Maui.Devices.Sensors.Location(40.58705949222607, 50.06205054302259),
                            Type = PinType.Place
                        },
                        new Pin
                        {
                            Label = "Porsche Destination Charging Station",
                            Location = new Microsoft.Maui.Devices.Sensors.Location(40.41864744308984, 49.9145863903889),
                            Type = PinType.Place
                        },
                        new Pin
                        {
                            Label = "Electric Vehicle Charging Station",
                            Location = new Microsoft.Maui.Devices.Sensors.Location(40.40578037236909, 50.299545035372994),
                            Type = PinType.Place
                        }, new Pin
                        {
                            Label = "Electric Vehicle Charging Station",
                            Location = new Microsoft.Maui.Devices.Sensors.Location(40.37896310060024, 49.8838646728485),
                            Type = PinType.Place
                        }, new Pin
                        {
                            Label = "Electric Vehicle Charging Station",
                            Location = new Microsoft.Maui.Devices.Sensors.Location(40.41120215608236, 49.83451257072043),
                            Type = PinType.Place
                        }


                    }
                },
                // Add more filter options as needed
            };
           

            //Show All pins
            foreach (var filterOption in FilterOptions)
            {
                foreach (var pin in filterOption.Pins)
                {
                    map.Pins.Add(pin);
                }
            }

            // Bind the FilterOptions to the CollectionView in XAML


            this.BindingContext = this;
            });

        }
        private void OnFilterTapped(object sender, EventArgs e)
        {

            var tappedFilterGrid = sender as Frame;
            var tappedFilterOption = tappedFilterGrid.BindingContext as FilterOption;

            if (tappedFilterOption == null)
            {
                return;
            }

            // Remove all pins from the map
            map.Pins.Clear();

            // Add pins related to the tapped filter
            foreach (var pin in tappedFilterOption.Pins)
            {
                map.Pins.Add(pin);
            }

            // Uncheck all other filters
            foreach (var filterOption in FilterOptions.Where(f => f != tappedFilterOption))
            {
                filterOption.IsChecked = false;
                filterOption.IsSelected = false;
            }
            // Check the tapped filter
            tappedFilterOption.IsChecked = true;
            tappedFilterOption.IsSelected = true;
        }




        private async Task LoadUserLocationAsync()
        {
            //LoadingIndicator.IsVisible = true;
            //shadowBox.IsVisible = true;
            try
            {


                var location = await GetCurrentUserLocationAsync();
                if (location != null && map != null)
                {
                    var mapSpan = new MapSpan(new Microsoft.Maui.Devices.Sensors.Location(location.Latitude, location.Longitude), 0.01, 0.01);
                    map.Dispatcher.Dispatch(() => map.MoveToRegion(mapSpan));
                    //map.MoveToRegion(mapSpan);
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                await Application.Current.MainPage.DisplayAlert("Xəta", "Cihazınız bu xüsusiyyəti dəstəkləmir.", "Bağla");
            }
            catch (FeatureNotEnabledException fneEx)
            {
                await Application.Current.MainPage.DisplayAlert("Xəta", "Cihazınızda bu xüsusiyyət aktiv deyil.", "Bağla");
            }
            catch (PermissionException pEx)
            {
                await Application.Current.MainPage.DisplayAlert("Xəta", "Uygulamaya cihazınızın məkan xidmətlərinə giriş etməyə icazə verilməyib.", "Bağla");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Xəta", "Məkan təyin edilərkən bir xəta baş verdi.", "Bağla");
            }
            // Show a frame with an image to indicate that the user's location could not be retrieved
            //LoadingIndicator.IsVisible = false;
            //shadowBox.IsVisible = false;

        }

        private async Task<Microsoft.Maui.Devices.Sensors.Location> GetCurrentUserLocationAsync()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                var location = await Geolocation.GetLocationAsync(request);

                if (location != null)
                {
                    return location;
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                await Application.Current.MainPage.DisplayAlert("Xəta", "Cihazınız bu xüsusiyyəti dəstəkləmir.", "Bağla");
            }
            catch (FeatureNotEnabledException fneEx)
            {
                await Application.Current.MainPage.DisplayAlert("Xəta", "Cihazınızda bu xüsusiyyət aktiv deyil.", "Bağla");
            }
            catch (PermissionException pEx)
            {
                await Application.Current.MainPage.DisplayAlert("Xəta", "Uygulamaya cihazınızın məkan xidmətlərinə giriş etməyə icazə verilməyib.", "Bağla");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Xəta", "Məkan təyin edilərkən bir xəta baş verdi.", "Bağla");
            }

            return null;
        }
    }

    public class FilterOption : INotifyPropertyChanged
    {
        private bool isChecked;
        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                isChecked = value;
                OnPropertyChanged();
            }
        }
        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                OnPropertyChanged();
            }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }

        private List<Pin> pins;
        public List<Pin> Pins
        {
            get { return pins; }
            set
            {
                pins = value;
                OnPropertyChanged();
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
