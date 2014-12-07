﻿using System;
using System.Linq;
using MangaReader.Manga;
using MangaReader.Manga.Acomic;
using MangaReader.Manga.Grouple;

namespace MangaReader.Tests
{
  public static class Builder
  {
    private const string Url = "http:\\example.com";

    public static Readmanga CreateReadmanga()
    {
      var manga = new Readmanga
      {
        Url = Url,
        Status = "example status",
        NeedUpdate = false,
        Name = "readmanga from example"
      };
      manga.Save();
      return manga;
    }

    public static void DeleteReadmanga(Readmanga manga)
    {
      if (manga == null)
        return;

      manga.Delete();
    }

    public static Acomics CreateAcomics()
    {
      var manga = new Acomics
      {
        Url = Url,
        Status = "example status",
        NeedUpdate = false,
        Name = "Acomics from example"
      };
      manga.Save();
      return manga;
    }

    public static void DeleteAcomics(Acomics manga)
    {
      if (manga == null)
        return;

      manga.Delete();
    }

    public static void CreateMangaHistory(Mangas manga)
    {
      var history = new MangaHistory()
      {
        Date = DateTime.Today,
        Url = Url
      };
      manga.Histories.Add(history);
      manga.Save();
    }

    public static void DeleteMangaHistory(Mangas manga)
    {
      var history = manga.Histories.ToList();
      foreach (var mangaHistory in history)
      {
        manga.Histories.Remove(mangaHistory);
      }
      manga.Save();
    }
  }
}
