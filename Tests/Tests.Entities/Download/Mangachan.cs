﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MangaReader.Core.Manga;
using MangaReader.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Entities.Download
{
  [TestClass]
  public class Mangachan
  {
    [TestMethod]
    public async Task DownloadMangachan()
    {
      var rm = Mangas.Create(new Uri(@"http://mangachan.me/manga/35617--rain-.html"));
      var sw = new Stopwatch();
      sw.Start();
      await rm.Download();
      sw.Stop();
      Log.Add($"manga loaded {sw.Elapsed.TotalSeconds}");
      Assert.IsTrue(Directory.Exists(rm.Folder));
      var files = Directory.GetFiles(rm.Folder, "*", SearchOption.AllDirectories);
      Assert.AreEqual(16, files.Length);
      var fileInfos = files.Select(f => new FileInfo(f)).ToList();
      Assert.AreEqual(4369315, fileInfos.Sum(f => f.Length));
      Assert.AreEqual(1, fileInfos.GroupBy(f => f.Length).Max(g => g.Count()));
      Assert.IsTrue(rm.IsDownloaded);
    }
  }
}
