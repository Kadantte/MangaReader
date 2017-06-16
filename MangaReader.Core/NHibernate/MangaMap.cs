﻿using FluentNHibernate.Mapping;
using MangaReader.Core.Manga;

namespace MangaReader.Core.NHibernate
{

  public class MangaMap : ClassMap<Mangas>
  {
    public MangaMap()
    {
      Not.LazyLoad();
      Id(x => x.Id).GeneratedBy.Native();
      Map(x => x.LocalName);
      Map(x => x.ServerName);
      Map(x => x.IsNameChanged);
      Map(x => x.HasVolumes);
      Map(x => x.HasChapters);
      Map(x => x.Uri);
      Map(x => x.Status);
      Map(x => x.NeedUpdate);
      Map(x => x.IsCompleted);
      Map(x => x.Folder);
      Map(x => x.NeedCompress);
      Map(x => x.CompressionMode);
      Map(x => x.Cover);
      HasMany(x => x.Histories).AsBag().Cascade.AllDeleteOrphan().Not.LazyLoad();
      DiscriminateSubClassesOnColumn("Type");
    }
  }
}
