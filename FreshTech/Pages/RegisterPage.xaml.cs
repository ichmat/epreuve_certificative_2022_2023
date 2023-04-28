namespace FreshTech.Pages;

using AppCore.Services;
using AppCore.Services.GeneralMessage.Args;
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


        string motDePasseHash = Password.HashPasword(motDePasseText, out string str_salt);
        // Faites quelque chose avec les valeurs r�cup�r�es ici, par exemple les afficher dans une bo�te de dialogue.
        if(await App.client.ConnexionStart())
        {
            //await DisplayAlert("Valeurs des zones de texte", $"Pseudo : {pseudoText}\nMail : {mailText}\nMot de passe : {motDePasseText}", "OK");
            bool resp = await App.client.Register(new EPCreateUser(mailText, pseudoText, motDePasseHash, str_salt, null, null));
            if(resp)
            {
                await DisplayAlert("vous �tes enregistr�", "houra", "Cool Connecte moi !");
                if(await App.client.Login(pseudoText, null, motDePasseText))
                {
                    await DisplayAlert("vous �tes connect�", "houra", "YES !");
                }
            }
        }
    }
}