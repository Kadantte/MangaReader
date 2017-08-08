﻿using System;
using System.Linq;
using MangaReader.Core.Manga;
using MangaReader.Core.Services.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Entities.Manga
{
  [TestClass]
  public class MangachanStructure
  {
    private MangaReader.Core.ISiteParser parser = new Hentaichan.Mangachan.Parser();

    [TestMethod]
    public void AddMangachanMultiPages()
    {
      var manga = GetManga("http://mangachan.me/manga/3828-12-prince.html");
	  parser.UpdateContent(manga);
      Assert.AreEqual(16, manga.Volumes.Count);
      Assert.AreEqual(78, manga.Volumes.Sum(v => v.Chapters.Count));
    }

    [TestMethod]
    public void AddMangachanSingleChapter()
    {
      var manga = GetManga("http://mangachan.me/manga/20138-16000-honesty.html");
	  parser.UpdateContent(manga);
      Assert.AreEqual(1, manga.Volumes.Count);
      Assert.AreEqual(1, manga.Volumes.Sum(v => v.Chapters.Count));
    }

    private Hentaichan.Mangachan.Mangachan GetManga(string url)
    {
      var manga = Mangas.CreateFromWeb(new Uri(url)) as Hentaichan.Mangachan.Mangachan;
      return manga;
    }

    [TestMethod]
    public void MangachanNameParsing()
    {
      // Спецсимвол "
      TestNameParsing("http://mangachan.me/manga/48069-isekai-de-kuro-no-iyashi-te-tte-yobarete-imasu.html",
        "Isekai de \"Kuro no Iyashi Te\" tte Yobarete Imasu",
        "В другом мире моё имя - Чёрный целитель");

      // Просто проверка.
      TestNameParsing("http://mangachan.me/manga/46475-shin5-kekkonshite-mo-koishiteru.html",
        "#shin5 - Kekkonshite mo Koishiteru",
        "Любовь после свадьбы");

      // Нет русского варианта.
      TestNameParsing("http://mangachan.me/manga/17625--okazaki-mari.html",
        "& (Okazaki Mari)",
        "& (Okazaki Mari)");
    }

    private void TestNameParsing(string uri, string english, string russian)
    {
      ConfigStorage.Instance.AppConfig.Language = Languages.English;
      var manga = GetManga(uri);
      Assert.AreEqual(english, manga.Name);
      ConfigStorage.Instance.AppConfig.Language = Languages.Russian;
      parser.UpdateNameAndStatus(manga);
      Assert.AreEqual(russian, manga.Name);
    }
  }
}
