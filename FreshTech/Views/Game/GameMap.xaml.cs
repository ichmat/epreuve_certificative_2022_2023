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

    public GameMap()
	{
		InitializeComponent();
	}

	public void CenterMap()
	{
		if(_is_size_changed)
		{
            double total_h = SIZE_CASE * NB_CASE_VERTICAL;
            double total_w = SIZE_CASE * NB_CASE_HORIZONTAL;

            double x = total_w / 2 - SV_Map.Width / 2;
            double y = total_h / 2 - SV_Map.Height / 2;

            _ = SV_Map.ScrollToAsync(x,y,true); 
        }
        else
        {
            // on reporte la demande de centrage une fois que la carte sera initialisé
            _need_center = true;
        }
	}

    private void ContentView_Loaded(object sender, EventArgs e)
    {
        if (!_is_init)
        {
            LoadMap();

            _is_init = true;
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
                aCase.Stroke = ColorsTools.Gray500Brush;
                MainGrid.Children.Add(aCase);
                Grid.SetColumn(aCase, i);
                Grid.SetRow(aCase, j);
            }
        }
    }

    private void SwipeLeft()
    {
        SV_Map.ScrollToAsync(SV_Map.ScrollX + SIZE_CASE * 1.5, SV_Map.ScrollY, false);
    }

    private void SwipeRight()
    {
        SV_Map.ScrollToAsync(SV_Map.ScrollX - SIZE_CASE*1.5, SV_Map.ScrollY, false);
    }

    private void SwipeUp()
    {
        SV_Map.ScrollToAsync(SV_Map.ScrollX, SV_Map.ScrollY - SIZE_CASE * 1.5, false);
    }

    private void SwipeDown()
    {
        SV_Map.ScrollToAsync(SV_Map.ScrollX, SV_Map.ScrollY + SIZE_CASE * 1.5, false);
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

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        Point? point = e.GetPosition(MainGrid);
        if(point.HasValue)
        {
            int x = Convert.ToInt32(Math.Floor(point.Value.X / SIZE_CASE));
            int y = Convert.ToInt32(Math.Floor(point.Value.Y / SIZE_CASE));
        }
    }

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

    public delegate void OnFinishingLoading();
    public event OnFinishingLoading FinishingLoaded;
}