using AppCore.Property;

namespace FreshTech.Views.Activity;

public partial class ViewDifficulty : ContentView
{
    private DifficulteCourse _level = DifficulteCourse.Normal;

    private ViewDifficultyVisual _visual = ViewDifficultyVisual.ALL;

    private bool _is_init = false;
    private bool _is_editable = true;

    public bool IsEditable
    {
        get => _is_editable;
        set
        {
            if (_is_editable != value)
            {
                _is_editable = value;
                triggerChangeIsEditable();
            }
        }
    }

    public DifficulteCourse Level
    {
        get => _level;
        set
        {
            if(value != _level)
            {
                _level = value;
                ChangeViewState();
                DifficultyChanged?.Invoke(_level);
            }
        }
    }

    public ViewDifficultyVisual ViewDifficultyVisual 
    { 
        get => _visual; 
        set
        {
            if(value != _visual)
            {
                _visual = value;
                ChangeVisibililyButtons();
            }
        }
    }

    public delegate void OnDifficultyChanged(DifficulteCourse newLevel);

    public event OnDifficultyChanged DifficultyChanged;

    public ViewDifficulty()
	{
		InitializeComponent();
	}

    private void ChangeViewState()
    {
        if (_is_init)
        {
            Dispatcher.Dispatch(() =>
            {
                BEasy.StrokeThickness = Level == DifficulteCourse.Facile ? 2 : 0;
                BMedium.StrokeThickness = Level == DifficulteCourse.Normal ? 2 : 0;
                BHard.StrokeThickness = Level == DifficulteCourse.Difficile ? 2 : 0;
                BCustom.StrokeThickness = Level == DifficulteCourse.Personnaliser ? 2 : 0;
                BExhaustion.StrokeThickness = Level == DifficulteCourse.Epuisement ? 2 : 0;
            });

            if (!IsEditable)
            {
                triggerChangeIsEditable();
            }
        }
    }

    private void ChangeVisibililyButtons()
    {
        if (_is_init)
        {
            Dispatcher.Dispatch(() =>
            {
                BCustom.IsVisible =
                _visual == ViewDifficultyVisual.WITH_CUSTOM ||
                _visual == ViewDifficultyVisual.ALL
                ? true : false;
                BExhaustion.IsVisible =
                    _visual == ViewDifficultyVisual.WITH_EXHAUSTION ||
                    _visual == ViewDifficultyVisual.ALL
                    ? true : false;
            });
        }
    }

    private void triggerChangeIsEditable()
    {
        if (_is_init)
        {
            if (IsEditable)
            {
                Dispatcher.Dispatch(() =>
                {
                    BEasy.IsVisible = true;
                    BMedium.IsVisible = true;
                    BHard.IsVisible = true;
                    ChangeVisibililyButtons();
                });
            }
            else
            {
                Dispatcher.Dispatch(() =>
                {
                    BEasy.IsVisible = Level == DifficulteCourse.Facile ? true : false;
                    BMedium.IsVisible = Level == DifficulteCourse.Normal ? true : false;
                    BHard.IsVisible = Level == DifficulteCourse.Difficile ? true : false;
                    BCustom.IsVisible = Level == DifficulteCourse.Personnaliser ? true : false;
                    BExhaustion.IsVisible = Level == DifficulteCourse.Epuisement ? true : false;
                });
            }
        }
    }

    private void Easy_Tapped(object sender, TappedEventArgs e)
    {
        Level = DifficulteCourse.Facile;
    }

    private void Medium_Tapped(object sender, TappedEventArgs e)
    {
        Level = DifficulteCourse.Normal;
    }

    private void Hard_Tapped(object sender, TappedEventArgs e)
    {
        Level = DifficulteCourse.Difficile;
    }

    private void Custom_Tapped(object sender, TappedEventArgs e)
    {
        Level = DifficulteCourse.Personnaliser;
    }

    private void Exhaustion_Tapped(object sender, TappedEventArgs e)
    {
        Level = DifficulteCourse.Epuisement;
    }

    private void ContentView_SizeChanged(object sender, EventArgs e)
    {
        if (!_is_init)
        {
            _is_init = true;
            ChangeViewState();
            triggerChangeIsEditable();
        }
    }
}

public enum ViewDifficultyVisual
{
    /// <summary>
    /// Afficher les boutons easy, medium et hard
    /// </summary>
    NORMAL = 0,
    /// <summary>
    /// Afficher les boutons easy, medium, hard et custom
    /// </summary>
    WITH_CUSTOM = 1,
    /// <summary>
    /// Afficher les boutons easy, medium, hard et exhaustion
    /// </summary>
    WITH_EXHAUSTION = 2,
    /// <summary>
    /// Afficher les boutons easy, medium, hard, custom et exhaustion
    /// </summary>
    ALL = 3
}