using Microsoft.Maui.Controls.PlatformConfiguration;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net;
using CommunityToolkit.Maui.Views;


namespace FreshTech.Pages;

public partial class ProfilePage : ContentPage
{
	public ProfilePage()
	{
        InitializeComponent();
        L_UserName.Text = App.client.CurrentUser.Pseudo;
        L_Mail.Text = App.client.CurrentUser.Mail;
    }
    
    private async void Objectif_Clicked()
    {
        await Navigation.PushModalAsync(new ObjectifPage());
    }

    private async void Confidentialite_Clicked()
    {
        await Navigation.PushModalAsync(new ConfidentialitePage());
    }

    private async void Language_Clicked()
    {
        await Navigation.PushModalAsync(new LangagePage());
    }

    private async void Compte_Clicked()
    {
        await Navigation.PushModalAsync(new ComptePage());
    }

    private async void APropos_Clicked()
    {
        await Navigation.PushModalAsync(new ProposPage());
    }
}