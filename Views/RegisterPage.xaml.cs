using Plugin.Firebase.Auth;
using Plugin.Firebase.Firestore;

namespace MauiFirebaseDemo.Views;

public partial class RegisterPage : ContentPage
{
    public RegisterPage()
    {
        InitializeComponent();
    }

    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        
        if (string.IsNullOrWhiteSpace(NameEntry.Text) ||
            string.IsNullOrWhiteSpace(EmailEntry.Text) ||
            string.IsNullOrWhiteSpace(PasswordEntry.Text))
        {
            await DisplayAlert("Warning", "Please fill all fields.", "Ok");
            return;
        }

        if (!TermsCheckBox.IsChecked)
        {
            await DisplayAlert("Warning", "Please accept Terms of Service", "Ok");
            return;
        }

        
        SetLoading(true);

        try
        {
            var auth = CrossFirebaseAuth.Current;
            if (auth == null) throw new Exception("Auth service error");

            
            var user = await auth.CreateUserAsync(
                EmailEntry.Text.Trim(),
                PasswordEntry.Text.Trim());

            if (user != null)
            {
                
                var db = CrossFirebaseFirestore.Current;
                if (db != null)
                {
                    await db.GetCollection("Users")
                        .GetDocument(user.Uid)
                        .SetDataAsync(new Dictionary<string, object>
                        {
                            { "Name", NameEntry.Text.Trim() },
                            { "Email", EmailEntry.Text.Trim() },
                            { "CreatedAt", DateTime.UtcNow }
                        });
                }

                await DisplayAlert("Success", "Account created successfully", "Ok");
                await Shell.Current.GoToAsync("..");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Register Error", ex.Message, "Ok");
        }
        finally
        {
           
            SetLoading(false);
        }
    }

    
    private void SetLoading(bool isLoading)
    {
        RegisterBtn.IsEnabled = !isLoading;
        RegisterIndicator.IsVisible = isLoading;
        RegisterIndicator.IsRunning = isLoading;
        RegisterBtn.Text = isLoading ? string.Empty : "Create account";
    }

    private async void OnGoogleLoginClicked(object sender, EventArgs e) => await DisplayAlert("Info", "Google Login Clicked", "Ok");
    private async void OnFacebookLoginClicked(object sender, EventArgs e) => await DisplayAlert("Info", "Facebook Login Clicked", "Ok");
    private async void OnAppleLoginClicked(object sender, EventArgs e) => await DisplayAlert("Info", "Apple Login Clicked", "Ok");
}


