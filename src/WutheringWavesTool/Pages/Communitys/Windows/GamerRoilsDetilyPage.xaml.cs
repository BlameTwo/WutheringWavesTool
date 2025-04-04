﻿using System.Threading.Tasks;
using Windows.System;
using WutheringWavesTool.Models.Wrapper.WindowRoils;

namespace WutheringWavesTool.Pages.Communitys.Windows;

public sealed partial class GamerRoilsDetilyPage : Page, IWindowPage
{
    public GamerRoilsDetilyPage(GamerRoilsDetilyViewModel viewModel)
    {
        this.InitializeComponent();
        ViewModel = viewModel;
        this.title_bth.Click += Button_Click;
        this.Loaded += this.Page_Loaded;
    }

    public GamerRoilsDetilyViewModel ViewModel { get; }

    public void Dispose()
    {
        this.ViewModel.Dispose();
    }

    public void SetData(object value)
    {
        if (value is ShowRoleData data)
        {
            this.ViewModel.Data = data;
            this.ViewModel.SelectCache = data.Id;
        }
    }

    public void SetWindow(Window window)
    {
        this.titlebar.Window = window;
        window.AppWindow.Closing += AppWindow_Closing;
        titlebar.UpDate();
    }

    private void AppWindow_Closing(AppWindow sender, AppWindowClosingEventArgs args)
    {
        this.title_bth.Click -= Button_Click;
        this.Loaded -= this.Page_Loaded;
        this.ViewModel.Dispose();
        GC.Collect();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        this.view.IsPaneOpen = !this.view.IsPaneOpen;
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        titlebar.UpDate();
    }

    private async void view_SelectionChanged(
        NavigationView sender,
        NavigationViewSelectionChangedEventArgs args
    )
    {
        if (args.SelectedItem != null && args.SelectedItem is NavigationRoilsDetilyItem item)
        {
            await this.ViewModel.SwitchPage(item);
        }
    }

    private void SelectorBarSegmented_SelectionChanged(
        SelectorBar sender,
        SelectorBarSelectionChangedEventArgs args
    )
    {
        if (sender.SelectedItem == null)
            return;
        this.ViewModel.GamerRoilViewModel.SetPage(sender.SelectedItem.Tag.ToString());
    }
}
