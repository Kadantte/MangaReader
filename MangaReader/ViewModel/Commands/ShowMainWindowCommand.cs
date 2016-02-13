﻿using System.Windows;
using WindowState = System.Windows.WindowState;

namespace MangaReader.ViewModel.Commands
{
  public class ShowMainWindowCommand : BaseCommand
  {
    public override void Execute(object parameter)
    {
      var mainWindow = Application.Current.MainWindow;
      if (mainWindow != null)
      {
        mainWindow.Show();
        if (mainWindow.WindowState == WindowState.Minimized)
          mainWindow.WindowState = WindowState.Normal;
      }
    }
  }
}