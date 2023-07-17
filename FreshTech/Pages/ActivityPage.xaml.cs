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

    private void Activity_LabelBottomClicked()
    {

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