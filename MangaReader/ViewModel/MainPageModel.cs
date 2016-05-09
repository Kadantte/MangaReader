﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using MangaReader.Core.Manga;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;
using MangaReader.Properties;
using MangaReader.UI.Services;
using MangaReader.ViewModel.Commands;
using MangaReader.ViewModel.Commands.Primitives;
using MangaReader.ViewModel.Manga;
using MangaReader.ViewModel.Primitive;

namespace MangaReader.ViewModel
{
  public class MainPageModel : BaseViewModel
  {
    private ICommand showSettings;
    private ICommand addNewManga;
    private ObservableCollection<ContentMenuItem> menu;
    private ICommand updateWithPause;
    private string libraryStatus;

    public string LibraryStatus
    {
      get { return libraryStatus; }
      set
      {
        libraryStatus = value;
        OnPropertyChanged();
      }
    }

    public ListCollectionView View { get; set; }

    public ObservableCollection<MangaViewModel> MangaViewModels { get; private set; } 

    public LibraryFilter LibraryFilter { get; set; }

    public ICommand AddNewManga
    {
      get { return addNewManga; }
      set
      {
        addNewManga = value;
        OnPropertyChanged();
      }
    }

    public ICommand ShowSettings
    {
      get { return showSettings; }
      set
      {
        showSettings = value;
        OnPropertyChanged();
      }
    }

    public ICommand UpdateWithPause
    {
      get { return updateWithPause; }
      set
      {
        updateWithPause = value;
        OnPropertyChanged();
      }
    }

    public ObservableCollection<ContentMenuItem> Menu
    {
      get { return menu; }
      set
      {
        menu = value;
        OnPropertyChanged();
      }
    }

    internal virtual bool Filter(object o)
    {
      var mangaModel = o as MangaBaseModel;
      if (mangaModel == null)
        return false;

      var manga = mangaModel.Manga;

      if (LibraryFilter.Uncompleted && manga.IsCompleted)
        return false;

      if (LibraryFilter.OnlyUpdate && !manga.NeedUpdate)
        return false;

      return LibraryFilter.AllowedTypes.Any(t => (t.Value as MangaSetting).Manga == manga.GetType().TypeProperty()) &&
        manga.Name.ToLowerInvariant().Contains(LibraryFilter.Name.ToLowerInvariant());
    }

    private void LibraryOnMangaDeleted(object sender, Mangas mangas)
    {
      var model = this.MangaViewModels.SingleOrDefault(m => Equals(m.Manga, mangas));
      if (model != null)
        this.MangaViewModels.Remove(model);
    }

    private void LibraryOnMangaAdded(object sender, Mangas mangas)
    {
      var model = this.MangaViewModels.SingleOrDefault(m => Equals(m.Manga, mangas));
      if (model == null)
        this.MangaViewModels.Add(new MangaViewModel(mangas));
    }

    public override void Show()
    {
      base.Show();

      var skin = UI.Skin.Skins.GetSkinSetting(ConfigStorage.Instance.ViewConfig.SkinGuid);
      skin.Init();
      Log.AddFormat("Selected skin - '{0}'.", skin.Name);
      WindowModel.Instance.Content = ViewService.Instance.TryGet<System.Windows.FrameworkElement>(this);
    }

    public MainPageModel()
    {
      LibraryFilter = ConfigStorage.Instance.ViewConfig.LibraryFilter;
      this.MangaViewModels = new ObservableCollection<MangaViewModel>(Repository.Get<Mangas>().Select(m => new MangaViewModel(m)));
      Library.MangaAdded += LibraryOnMangaAdded;
      Library.MangaDeleted += LibraryOnMangaDeleted;
      Library.StatusChanged += (sender, s) => LibraryStatus = s;

      View = new ListCollectionView(MangaViewModels)
      {
        Filter = Filter,
        CustomSort = new MangaViewModel(null)
      };

      this.AddNewManga = new AddNewMangaCommand();
      this.ShowSettings = new ShowSettingCommand();
      this.UpdateWithPause = new UpdateWithPauseCommand(View);

      #region Менюшка

      this.Menu = new ObservableCollection<ContentMenuItem>();

      var file = new ContentMenuItem("Файл");
      file.SubItems.Add((BaseCommand)this.AddNewManga);
      file.SubItems.Add(new UpdateVisibleMangaCommand(View));
      file.SubItems.Add(new UpdateAllCommand { Name = "Обновить всё" });
      file.SubItems.Add(new ExitCommand());

      var setting = new ContentMenuItem(Strings.Library_Action_Settings);
      setting.SubItems.Add((BaseCommand)ShowSettings);

      var about = new ContentMenuItem("О программе");
      about.SubItems.Add(new AppUpdateCommand());
      about.SubItems.Add(new ShowUpdateHistoryCommand());

      this.Menu.Add(file);
      this.Menu.Add(setting);
      this.Menu.Add(about);

      #endregion
    }
  }
}