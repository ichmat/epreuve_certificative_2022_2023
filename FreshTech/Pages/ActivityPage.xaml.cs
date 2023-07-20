using AppCore.Models;
using AppCore.Services.GeneralMessage.Args;
using FreshTech.Views;
using FreshTech.Views.Activity;

namespace FreshTech.Pages;

public partial class ActivityPage : ContentPage
{
    private bool _is_init = false;
    private const int ACTIVITIES_NUMBER_LOAD = 4;
    private int actual_loaded_activities = 0;

	public ActivityPage()
	{
		InitializeComponent();
	}

    #region INIT

    private void ContentPage_SizeChanged(object sender, EventArgs e)
    {
        if (!_is_init)
        {
            _is_init = true;
            ReloadObjective();
            ReloadActivities();
        }
    }

    #endregion

    private async void ReloadObjective()
    {
        // TODO : afficher l'objectif de l'utilisateur
        // https://xd.adobe.com/view/4fc91b5c-3f6a-48d4-b95a-c6c58a7e0c13-1a9a/screen/04d07141-b2ab-47ec-bb39-f9e38ec1cc59
    }

    private async void ReloadActivities()
    {
        AI_Activity_Loading.IsVisible = true;
        AI_Activity_Loading.IsRunning = true;
        VSL_Activities.IsVisible = false;
        ButtonLoadMore.IsVisible = false;
        actual_loaded_activities = ACTIVITIES_NUMBER_LOAD;
        VSL_Activities.Children.Clear();

        Courses[]? courses = await App.client.SendAndGetResponse<Courses[]>(new EPGetCourses(0, ACTIVITIES_NUMBER_LOAD));

        if(courses != null)
        {
            // créer les vues
            Array.ForEach(courses, CreateActivityView);

            if(courses.Length == ACTIVITIES_NUMBER_LOAD)
            {
                ButtonLoadMore.IsVisible = true;
            }
        }
        else
        {
            _ = DisplayAlert("Erreur chargement",
                    "Les activités n'ont pas pu être charger", "OK");
        }

        VSL_Activities.IsVisible = true;
        AI_Activity_Loading.IsVisible = false;
        AI_Activity_Loading.IsRunning = false;
    }

    private async void ContinueLoadActivities()
    {
        AI_Activity_Loading.IsVisible = true;
        AI_Activity_Loading.IsRunning = true;
        ButtonLoadMore.IsVisible = false;

        Courses[]? courses = await App.client.SendAndGetResponse<Courses[]>(new EPGetCourses(actual_loaded_activities, actual_loaded_activities + ACTIVITIES_NUMBER_LOAD));
        actual_loaded_activities += ACTIVITIES_NUMBER_LOAD;

        if (courses != null)
        {
            // créer les vues
            Array.ForEach(courses, CreateActivityView);

            if (courses.Length == ACTIVITIES_NUMBER_LOAD)
            {
                ButtonLoadMore.IsVisible = true;
            }
        }
        else
        {
            _ = DisplayAlert("Erreur chargement",
                    "Les activités n'ont pas pu être charger", "OK");
        }

        AI_Activity_Loading.IsVisible = false;
        AI_Activity_Loading.IsRunning = false;
    }

    private void CreateActivityView(Courses courses)
    {
        ContentActivity view = new ContentActivity(courses);
        view.Margin = new Thickness(0, 0, 0, 25);
        VSL_Activities.Children.Add(view);
    }

    private async void Activity_LabelBottomClicked()
    {
        ContentObjectiveActivity.Opacity = 0.6;
        AI_StartActivity_Loading.IsRunning = true;
        await Task.Delay(50);
        await Shell.Current.Navigation.PushModalAsync(new BeforeActivityPage(), true);
        AI_StartActivity_Loading.IsRunning = false;
        ContentObjectiveActivity.Opacity = 1;
    }

    private void RefreshView_Refreshing(object sender, EventArgs e)
    {
        ReloadObjective();
        ReloadActivities();
        refreshView.IsRefreshing = false;
    }

    private void ButtonLoadMore_Clicked(object sender, EventArgs e)
    {
        ContinueLoadActivities();
    }

    private void ButtonModifyTarget_Clicked(object sender, EventArgs e)
    {
        _ = Navigation.PushModalAsync(new ObjectifPage());
    }
}