using Plugin.Firebase.Auth;

namespace MauiFirebaseDemo.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
        CheckRememberMe();
    }

    private void CheckRememberMe()
    {
        bool isRememberMe = Preferences.Get("is_remember_me", false);
        if (isRememberMe)
        {
            EmailEntry.Text = Preferences.Get("saved_email", string.Empty);
            RememberMeCheckbox.IsChecked = true;
        }
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(EmailEntry.Text) || string.IsNullOrWhiteSpace(PasswordEntry.Text))
        {
            await DisplayAlert("Uyarı", "Tüm alanları doldurun", "Tamam");
            return;
        }

        
        SetLoading(true);

        try
        {
            string email = EmailEntry.Text.Trim();
            string password = PasswordEntry.Text.Trim();

            var result = await CrossFirebaseAuth.Current.SignInWithEmailAndPasswordAsync(email, password);

            if (result != null)
            {
                if (RememberMeCheckbox.IsChecked)
                {
                    Preferences.Set("is_remember_me", true);
                    Preferences.Set("saved_email", email);
                }
                else
                {
                    Preferences.Remove("is_remember_me");
                    Preferences.Remove("saved_email");
                }

                Preferences.Set("is_logged_in", true);
                await Shell.Current.GoToAsync("//MainPage");
            }
        }
        catch (Exception)
        {
            await DisplayAlert("Login Error", "Invalid Email or Password. Please try again.", "Ok");
        }
        finally
        {
            
            SetLoading(false);
        }
    }

    
    private void SetLoading(bool isLoading)
    {
        LoginBtn.IsEnabled = !isLoading;
        LoginIndicator.IsVisible = isLoading;
        LoginIndicator.IsRunning = isLoading;

        
        LoginBtn.Text = isLoading ? string.Empty : "Login";
    }

    private void OnRememberMeLabelTapped(object sender, EventArgs e)
    {
        RememberMeCheckbox.IsChecked = !RememberMeCheckbox.IsChecked;
    }

    private async void OnRegisterTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("RegisterPage");
    }

    private async void OnForgotPasswordTapped(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(EmailEntry.Text))
        {
            await DisplayAlert("Warning", "Please enter your email address", "Ok");
            return;
        }

        try
        {
            await CrossFirebaseAuth.Current.SendPasswordResetEmailAsync(EmailEntry.Text.Trim());
            await DisplayAlert("Success", "Reset Password url sent to your email address.", "Ok");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "Ok");
        }
    }

    private async void OnGoogleLoginClicked(object sender, EventArgs e) => await DisplayAlert("Info", "Google Login Clicked", "Ok");
    private async void OnFacebookLoginClicked(object sender, EventArgs e) => await DisplayAlert("Info", "Facebook Login Clicked", "Ok");
    private async void OnAppleLoginClicked(object sender, EventArgs e) => await DisplayAlert("Info", "Apple Login Clicked", "Ok");
}





