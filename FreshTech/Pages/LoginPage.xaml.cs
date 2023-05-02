namespace FreshTech.Pages;

using AppCore.Services;
using AppCore.Services.GeneralMessage.Args;
using FreshTech.Tools;
using Microsoft.Maui.Controls;

public partial class LoginPage : ContentPage
{
    int count = 0;
    public LoginPage()
	{
        InitializeComponent();
    }

    private async void OnLoginClicked(object sender, System.EventArgs e)
    {
        string mailText = Mail.Text;
        string motDePasseText = MotDePasse.Text;
        bool isOk = true;
        Mail.PlaceholderColor = Colors.LightGray;
        MotDePasse.PlaceholderColor = Colors.LightGray;

        if (string.IsNullOrWhiteSpace(mailText))
        {
            isOk = false;
            Mail.PlaceholderColor = ColorsTools.Danger;
        }

        if (string.IsNullOrWhiteSpace(motDePasseText))
        {
            isOk = false;
            MotDePasse.PlaceholderColor = ColorsTools.Danger;
        }

        if (isOk)
        {
            AI_Loading.IsRunning = true;
            if (!await App.client.IsConnected())
            {
                if (!await App.client.ConnexionStart())
                {
                    _ = DisplayAlert("Problème de connexion", "Vérifier votre connexion internet", "Ok");
                    AI_Loading.IsRunning = false;
                    return;
                }
            }

            if(await App.client.Login(null, mailText, motDePasseText))
            {
                await Shell.Current.GoToAsync("//GamePage");
            }
            else
            {
                _ = DisplayAlert("Connexion echoué", "Le mail ou le mot de passe ne correspond pas", "Ok");
            }

            AI_Loading.IsRunning = false;
        }
    }

    private async void ContentPage_Loaded(object sender, EventArgs e)
    {
        AI_Loading.IsRunning = true;
        if (!await App.client.IsConnected())
        {
            if(!await App.client.ConnexionStart())
            {
                _ = DisplayAlert("Problème de connexion", "Vérifier votre connexion internet", "Ok");
            }
        }
        AI_Loading.IsRunning = false;
    }

    private async void Register_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//RegisterPage", true);
    }
}