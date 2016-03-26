﻿using System.Threading.Tasks;
using System.Windows;

namespace MangaReader.ViewModel.Primitive
{
  public class BaseViewModel : NotifyPropertyChanged
  {
    protected internal FrameworkElement view;

    public virtual async Task Load()
    {

    }

    public virtual async Task Unload()
    {

    }

    public virtual void Show()
    {
    }

    public BaseViewModel(FrameworkElement view)
    {
      view.DataContext = this;
      view.Loaded += ViewOnLoaded;
      view.Unloaded += ViewOnUnloaded;
      this.view = view;
    }

    private void ViewOnUnloaded(object sender, RoutedEventArgs routedEventArgs)
    {
      Application.Current.Dispatcher.InvokeAsync(() => this.Unload());
    }

    private void ViewOnLoaded(object sender, RoutedEventArgs routedEventArgs)
    {
      Application.Current.Dispatcher.InvokeAsync(() => this.Load());
    }
  }
}