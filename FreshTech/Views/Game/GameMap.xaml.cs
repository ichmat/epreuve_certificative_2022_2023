using FreshTech.Tools;
using Microsoft.Maui.Controls.Shapes;
using System;

namespace FreshTech.Views.Game;

public partial class GameMap : ContentView
{
	private const int NB_CASE_HORIZONTAL = 16;
	private const int NB_CASE_VERTICAL = 16;

	private const double SIZE_CASE = 50;
	private bool _is_init = false;
    private bool _need_center = false;

    private bool _is_size_changed = false;

    private bool _cancel_scroll = false;
    private double _scroll_x = 0;
    private double _scroll_y = 0;

    public GameMap()
	{
		InitializeComponent();
    }

    #region PUBLIC

    /// <summary>
    /// Centre la carte au milieu
    /// </summary>
    public void CenterMap()
	{
		if(_is_size_changed)
		{
            double total_h = SIZE_CASE * NB_CASE_VERTICAL;
            double total_w = SIZE_CASE * NB_CASE_HORIZONTAL;

            _scroll_x = total_w / 2 - SV_Map.Width / 2;
            _scroll_y = total_h / 2 - SV_Map.Height / 2;

            _ = SV_Map.ScrollToAsync(_scroll_x, _scroll_y, true); 
        }
        else
        {
            // on reporte la demande de centrage une fois que la carte sera initialisé
            _need_center = true;
        }
	}

    public void AddElement(VisualElement view, int x, int y)
    {
        _cancel_scroll = true;
        MainGrid.Children.Add(view);
        Grid.SetColumn(view, x);
        Grid.SetRow(view, y);
    }

    /// <summary>
    /// Demande à la carte de se recharger.
    /// </summary>
    /// <remarks>
    /// 💬 <i>C'est nécessaire de le faire notamment après l'ajout d'un élément. Car la vue du scroll 
    /// fournit par Microsoft est buggé quand on lui demande un scroll vertical et horizontal. <br></br>
    /// C'est donc une solution temporaire</i> ＞︿＜
    /// </remarks>
    public void ReloadViewElement()
    {
        Dispatcher.Dispatch(triggerReloadView);
    }

    private async void triggerReloadView()
    {
        await Task.Delay(1);
        await SV_Map.ScrollToAsync(_scroll_x, _scroll_y, false);
    }

    public void RemoveElement(VisualElement view)
    {
        _cancel_scroll = true;
        MainGrid.Children.Remove(view);
    }

    #endregion

    #region LOADING

    private void ContentView_Loaded(object sender, EventArgs e)
    {
        if (!_is_init)
        {
            LoadMap();
            _is_init = true;

            FinishingLoaded?.Invoke();
        }
    }

	private void LoadMap()
	{
        MainGrid.RowDefinitions.Clear();
        MainGrid.ColumnDefinitions.Clear();

        int center_x = Convert.ToInt32(Math.Floor(((double)NB_CASE_HORIZONTAL) / 2));
        int center_y = Convert.ToInt32(Math.Floor(((double)NB_CASE_VERTICAL) / 2));

        bool vertical_pair = ((double)NB_CASE_VERTICAL) % 2 == 0;
        bool horizontal_pair = ((double)NB_CASE_HORIZONTAL) % 2 == 0;

        for (int i = 0; i < NB_CASE_HORIZONTAL; i++)
        {
            ColumnDefinition columnDefinition = new ColumnDefinition();
            columnDefinition.Width = new GridLength(SIZE_CASE, GridUnitType.Absolute);
            MainGrid.ColumnDefinitions.Add(columnDefinition);
        }

        for (int i = 0; i < NB_CASE_VERTICAL; i++)
        {
            RowDefinition rowDefinition = new RowDefinition();
            rowDefinition.Height = new GridLength(SIZE_CASE, GridUnitType.Absolute);
            MainGrid.RowDefinitions.Add(rowDefinition);
        }

        for (int i = 0; i < NB_CASE_HORIZONTAL; i++)
        {
            for (int j = 0; j < NB_CASE_VERTICAL; j++)
            {
                Rectangle aCase = new Rectangle();
                if((center_x == i || (center_x-1 == i && horizontal_pair)) 
                    && (center_y == j || (center_y-1 == j && vertical_pair)))
                    aCase.BackgroundColor = ColorsTools.Secondary;
                else
                    aCase.BackgroundColor = Colors.Transparent;

                aCase.StrokeThickness = 1;
                aCase.Stroke = ColorsTools.Gray200Brush;
                MainGrid.Children.Add(aCase);
                Grid.SetColumn(aCase, i);
                Grid.SetRow(aCase, j);
            }
        }
    }

    public delegate void OnFinishingLoading();
    public event OnFinishingLoading FinishingLoaded;

    #endregion

    #region GESTURE

    private void SwipeLeft()
    {
        _scroll_x += SIZE_CASE * 1.5;
        SV_Map.ScrollToAsync(_scroll_x, _scroll_y, false).Wait();
        _scroll_x = _scroll_x > SV_Map.ScrollX ? SV_Map.ScrollX : _scroll_x;
    }

    private void SwipeRight()
    {
        _scroll_x -= SIZE_CASE * 1.5;
        SV_Map.ScrollToAsync(_scroll_x, _scroll_y, false).Wait();
        _scroll_x = _scroll_x > SV_Map.ScrollX ? SV_Map.ScrollX : _scroll_x;
    }

    private void SwipeUp()
    {
        _scroll_y -= SIZE_CASE * 1.5;
        SV_Map.ScrollToAsync(_scroll_x, _scroll_y, false).Wait();
        _scroll_y = _scroll_y > SV_Map.ScrollY ? SV_Map.ScrollY : _scroll_y;
    }

    private void SwipeDown()
    {
        _scroll_y += SIZE_CASE * 1.5;
        SV_Map.ScrollToAsync(_scroll_x, _scroll_y, false).Wait();
        _scroll_y = _scroll_y > SV_Map.ScrollY ? SV_Map.ScrollY : _scroll_y;
    }

    private void SwipeGestureRecognizer_SwipedLeft(object sender, SwipedEventArgs e)
    {
        Dispatcher.Dispatch(SwipeLeft);
    }

    private void SwipeGestureRecognizer_SwipedRight(object sender, SwipedEventArgs e)
    {
        Dispatcher.Dispatch(SwipeRight);
    }

    private void SwipeGestureRecognizer_SwipedUp(object sender, SwipedEventArgs e)
    {
        Dispatcher.Dispatch(SwipeUp);
    }

    private void SwipeGestureRecognizer_SwipedDown(object sender, SwipedEventArgs e)
    {
        Dispatcher.Dispatch(SwipeDown);
    }

    #endregion

    #region TAPPED

    public delegate void OnTappedCoord(int x, int y);
    public event OnTappedCoord TappedCoord;

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        if (!_is_init) return;

        Point? point = e.GetPosition(MainGrid);
        if(point.HasValue)
        {
            int x = Convert.ToInt32(Math.Floor(point.Value.X / SIZE_CASE));
            int y = Convert.ToInt32(Math.Floor(point.Value.Y / SIZE_CASE));
            TappedCoord?.Invoke(x, y);
        }
    }

    #endregion

    private void MainGrid_SizeChanged(object sender, EventArgs e)
    {
        if (!_is_size_changed)
        {
            _is_size_changed = true;
            // demande de centrer la carte avant que celui ci ne soit initialisé
            if (_need_center)
            {
                CenterMap();
                _need_center = false;
            }
        }
    }

    private void SV_Map_Scrolled(object sender, ScrolledEventArgs e)
    {
        if (_cancel_scroll)
        {
            _cancel_scroll = false;
            SV_Map.ScrollToAsync(_scroll_x, _scroll_y, false);
        }
        
    }
}