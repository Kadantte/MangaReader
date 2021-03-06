﻿using System;

namespace MangaReader.Core.DataTrasferObject
{
  public class MangaPageDto
  {
    public int Number { get; set; }
    public string Name { get; set; }
    public Uri Uri { get; set; }
    public Uri ImageLink { get; set; }

    public MangaPageDto(Uri uri, Uri image, int number, string name) : this(uri, image, number)
    {
      this.Name = name;
    }

    public MangaPageDto(Uri uri, Uri image, int number)
    {
      this.Uri = uri;
      this.ImageLink = image;
      this.Number = number;
    }

  }
}