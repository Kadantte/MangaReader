using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MangaReader.Core.Manga;

namespace MangaReader.Core.Services
{
  public class DownloadManager
  {
    public static bool IsPaused { get; set; }

    public static async Task CheckPause()
    {
      if (!IsPaused)
        return;

      while (IsPaused)
      {
        await Task.Delay(1000);
      }
    }

    public static async Task TestDownloadSpeed(IManga manga)
    {
      manga.UpdateContent();
      var pages = manga.Pages.ToList();
      var chapters = manga.Chapters.ToList();
      chapters.AddRange(manga.Volumes.SelectMany(c => c.Container));
      MethodInfo methodInfo = null;
      if (chapters.Any())
        foreach (var chapter in chapters.Where(c => c.Container == null || !c.Container.Any()))
        {
          methodInfo = methodInfo ?? chapter.GetType().GetMethod("UpdatePages", BindingFlags.NonPublic | BindingFlags.Instance);
          methodInfo.Invoke(chapter, null);
        }
      pages.AddRange(chapters.SelectMany(c => c.Container));

      var sw = new Stopwatch();
      sw.Start();
      var tasks = pages.Select(c => DownloadImage(c.ImageLink));
      await Task.WhenAll(tasks);
      sw.Stop();
      var size = tasks.Sum(t => t.Result.Body.LongLength);
      var log = $"Загружено {pages.Count} картинок, размером {size.HumanizeByteSize()} за {sw.Elapsed.TotalSeconds} секунд, итого {(size / sw.Elapsed.TotalSeconds).HumanizeByteSize()} в секунду";
      Log.Info(log);
    }

    /// <summary>
    /// Скачать файл.
    /// </summary>
    /// <param name="uri">Ссылка на файл.</param>
    /// <returns>Содержимое файла.</returns>
    public static async Task<ImageFile> DownloadImage(Uri uri)
    {
      byte[] result;
      WebResponse response;
      var file = new ImageFile();
      var request = (HttpWebRequest)WebRequest.Create(uri);
      request.Referer = uri.Host;

      try
      {
        response = await request.GetResponseAsync();
        result = await CopyTo(response.GetResponseStream());
      }
      catch (System.Exception ex)
      {
        Log.Exception(ex, string.Format("Загрузка {0} не завершена.", uri));
        return file;
      }
      if (response.ContentLength <= result.LongLength)
        file.Body = result;
      return file;
    }

    private static async Task<byte[]> CopyTo(Stream from)
    {
      using (var memory = new MemoryStream())
      {
        byte[] buffer = new byte[81920];
        while (true)
        {
          int num = await from.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
          if (num != 0)
          {
            memory.Write(buffer, 0, num);
            NetworkSpeed.AddInfo(num);
          }
          else
          {
            break;
          }
        }
        return memory.ToArray();
      }
    }
  }
}
