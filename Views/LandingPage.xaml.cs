using System.Collections.ObjectModel;

namespace MauiFirebaseDemo.Views;

public partial class LandingPage : ContentPage
{
    public ObservableCollection<OnboardingModel> OnboardingSteps { get; set; }

    public LandingPage()
    {
        InitializeComponent();

        // Tasarımdaki 3 farklı içeriği tanımlıyoruz
        OnboardingSteps = new ObservableCollection<OnboardingModel>
        {
            new OnboardingModel
            {
                Title = "Bisa deteksi dini kusta dengan sangat mudah",
                Description = "Periksanya cepat ga sampai 5 menit. Metode yang dipakai mudah, dan ga berbelit.",
                Image = "landingicon.jpg" 
            },
            new OnboardingModel
            {
                Title = "Buat janji periksa dan dapetin notifikasinya",
                Description = "Jadwalin periksa ke dokter untuk berobat, dibantu notifikasi biar ga kelewat.",
                Image = "landingicon2.jpg"
            },
            new OnboardingModel
            {
                Title = "Jadwal minum obat sesuai dosis dari dokter",
                Description = "Jadwalin sekali, dan dapet notifikasi. Konsumsi dosis obat, dibantu sistem yang cermat.",
                Image = "landingicon.jpg"
            }
        };

        OnboardingCarousel.ItemsSource = OnboardingSteps;
    }

    private void OnPositionChanged(object sender, PositionChangedEventArgs e)
    {
        
        if (e.CurrentPosition == OnboardingSteps.Count - 1)
        {
            NextBtn.Text = "Jelajahi Kusta Sekarang";
            LoginLink.IsVisible = true;
        }
        else
        {
            NextBtn.Text = "Lanjutkan";
            LoginLink.IsVisible = true; 
        }
    }

    public async void OnNextClicked(object sender, EventArgs e)
    {
        if (OnboardingCarousel.Position < OnboardingSteps.Count - 1)
        {
            OnboardingCarousel.Position++;
        }
        else
        {
            
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }

    public async void OnLoginTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//LoginPage");
    }
}


public class OnboardingModel
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string Image { get; set; }
}

