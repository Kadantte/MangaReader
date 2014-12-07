﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using NHibernate.Linq;
using MangaReader.Manga;

namespace MangaReader.Services
{
  static class History
  {
    /// <summary>
    /// Ссылка на файл лога.
    /// </summary>
    private static readonly string HistoryPath = Settings.WorkFolder + @".\history";

    /// <summary>
    /// Добавление записи в историю.
    /// </summary>
    /// <param name="manga">Манга, к которой относится сообщение.</param>
    /// <param name="message">Сообщение.</param>
    public static void AddHistory(this Mangas manga, string message)
    {
      using (var session = Mapping.Environment.SessionFactory.OpenSession())
      using (var tranc = session.BeginTransaction())
      {
        if (manga.Histories.Any(h => h.Url == message))
          return;

        var history = new MangaHistory(message);
        manga.Histories.Add(history);
        tranc.Commit();
      }
    }

    /// <summary>
    /// Сконвертировать в новый формат.
    /// </summary>
    public static void Convert(ConverterProcess process)
    {
      if (!File.Exists(HistoryPath))
        return;

      // ReSharper disable CSharpWarnings::CS0612
      var histories = new List<MangaHistory>();

      var serializedStrings = Serializer<List<string>>.Load(HistoryPath);
      var isStringList = serializedStrings != null;

      var serializedMangaHistoris = Serializer<List<MangaHistory>>.Load(HistoryPath);
      var isMangaHistory = serializedMangaHistoris != null;

      var strings = File.Exists(HistoryPath) ? new List<string>(File.ReadAllLines(HistoryPath)) : new List<string>();

      if (!isMangaHistory && !isStringList)
        histories = MangaHistory.CreateHistories(strings);
      if (!isMangaHistory && isStringList)
        histories = MangaHistory.CreateHistories(serializedStrings);
      if (isMangaHistory && !isStringList)
        histories = serializedMangaHistoris;

      var session = Mapping.Environment.Session;
      var mangas = session.Query<Mangas>().ToList();
      var historyInDb = session.Query<MangaHistory>().Select(h => h.Url).ToList();
      histories = histories.Where(h => !historyInDb.Contains(h.Url)).Distinct().ToList();
      if (process != null && histories.Any())
        process.IsIndeterminate = false;

      foreach (var manga in mangas)
      {
        if (process != null)
          process.Percent += 100.0 / mangas.Count;
        using (var tranc = session.BeginTransaction())
        {
          foreach (var history in histories.Where(h => h.MangaUrl == manga.Url))
          {
            manga.Histories.Add(history);
          }
          tranc.Commit();
        }
      }
      // ReSharper restore CSharpWarnings::CS0612

      File.Move(HistoryPath, HistoryPath + ".dbak");
    }
  }
}
