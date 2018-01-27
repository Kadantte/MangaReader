﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MangaReader.Core.Services;

namespace MangaReader.Core.Manga
{
  public abstract class DownloadableContainerImpl<T> : Entity.Entity, IDownloadableContainer<T> where T : IDownloadable
  {
    private string name;

    /// <summary>
    /// Статус загрузки.
    /// </summary>
    public virtual bool IsDownloaded
    {
      get { return InDownloading != null && InDownloading.Any() && InDownloading.All(v => v.IsDownloaded); }
    }

    /// <summary>
    /// Процент загрузки манги.
    /// </summary>
    public virtual double Downloaded
    {
      get { return (InDownloading != null && InDownloading.Any()) ? InDownloading.Average(ch => ch.Downloaded) : 0; }
      set { }
    }

    public string Name
    {
      get => name;
      set => name = GetNormalizedName(value);
    }

    protected virtual string GetNormalizedName(string newName)
    {
      if (newName == null)
        return null;

      newName = newName.Trim();
      newName = Helper.SpaceRegex.Replace(newName, " ");
      return newName;
    }

    public Uri Uri { get; set; }

    public virtual string Folder { get; protected set; }

    public DateTime? DownloadedAt { get; set; }

    public abstract Task Download(string folder, Throttler throttler);

    public virtual void ClearHistory()
    {
      foreach (var element in this.Container)
        element.ClearHistory();
      this.DownloadedAt = null;
    }

    public ICollection<T> Container { get; set; }

    IEnumerable<T> IDownloadableContainer<T>.Container => Container;

    public virtual IEnumerable<T> InDownloading { get; protected set; }

    public IManga Manga { get; set; }

    protected DownloadableContainerImpl()
    {
      Container = new List<T>();
    }
  }
}