namespace FreshTech.Pages;

using AppCore.Services;
using AppCore.Services.GeneralMessage.Args;
using FreshTech.Tools;
using System;
public partial class RegisterPage : ContentPage
{
    
    private string message = "Click me";

    public RegisterPage()
	{
		InitializeComponent();

    }

    private async void OnRegisterClicked(object sender, System.EventArgs e)
    {
        string pseudoText = Pseudo.Text;
        string mailText = Mail.Text;
        string motDePasseText = MotDePasse.Text;
        string confMotDePasseText = ConfirmMotDePasse.Text;
        bool isOk = true;
        Pseudo.PlaceholderColor = Colors.LightGray;
        Mail.PlaceholderColor = Colors.LightGray;
        MotDePasse.PlaceholderColor = Colors.LightGray;
        ConfirmMotDePasse.PlaceholderColor = Colors.LightGray;
        MotDePasse.TextColor = Colors.Black;
        ConfirmMotDePasse.TextColor = Colors.Black;


        if (string.IsNullOrWhiteSpace(pseudoText))
        {
            Pseudo.PlaceholderColor = ColorsTools.Danger;
            isOk = false;
        }

        if(string.IsNullOrWhiteSpace(mailText))
        {
            Mail.PlaceholderColor = ColorsTools.Danger;
            isOk = false;
        }

        if (string.IsNullOrWhiteSpace(motDePasseText))
        {
            MotDePasse.PlaceholderColor = ColorsTools.Danger;
            isOk = false;
        }

        if (string.IsNullOrWhiteSpace(confMotDePasseText))
        {
            ConfirmMotDePasse.PlaceholderColor = ColorsTools.Danger;
            isOk = false;
        }

        if (isOk && motDePasseText != confMotDePasseText)
        {
            MotDePasse.TextColor = ColorsTools.Danger;
            ConfirmMotDePasse.TextColor = ColorsTools.Danger;
            isOk = false;
            await DisplayAlert("Mot de passe différent", "Les mots de passes inscrit sont différents", "Ok");
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

            string motDePasseHash = Password.HashPasword(motDePasseText, out string str_salt);
            bool resp = await App.client.Register(new EPCreateUser(mailText, pseudoText, motDePasseHash, str_salt, null, null));

            if (resp)
            {
                if(await App.client.Login(pseudoText, null, motDePasseText))
                {
                    await Shell.Current.GoToAsync("//GamePage");
                }
                else
                {
                    await DisplayAlert("Une erreur est survenu", "Le compte à été créer mais la connexion à echoué, veillez tenter de vous connecter.", "Ok");
                    await Shell.Current.GoToAsync("//LoginPage");
                }
            }
            else
            {
                _ = DisplayAlert("Enregistrement echoué", "Le mail utilisé existe déjà", "Ok");
            }

            AI_Loading.IsRunning = false;
        }
        

    }

    private async void Connect_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//LoginPage", true);
    }
}