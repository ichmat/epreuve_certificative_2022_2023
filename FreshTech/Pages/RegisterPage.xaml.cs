namespace FreshTech.Pages;

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

        // Faites quelque chose avec les valeurs récupérées ici, par exemple les afficher dans une boîte de dialogue.
        DisplayAlert("Valeurs des zones de texte", $"Pseudo : {pseudoText}\nMail : {mailText}\nMot de passe : {motDePasseText}", "OK");
        var resp = await App.client.SendAndGetResponseStruct<Guid>(new EPCreateUser(mailText,pseudoText,motDePasseText,"sel",65,170));
    }
}