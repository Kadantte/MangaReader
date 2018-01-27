using System;
using System.Threading;
using System.Threading.Tasks;

namespace MangaReader.Core.Services
{
  public class Throttler : IDisposable
  {
    private readonly SemaphoreSlim throttler;

    public async Task<IDisposable> WaitAsync()
    {
      await throttler.WaitAsync();
      return new Releaser(throttler);
    }

    public IDisposable Wait()
    {
      throttler.Wait();
      return new Releaser(throttler);
    }

    public Throttler(int limit)
    {
      throttler = new SemaphoreSlim(limit);
    }

    public void Dispose()
    {
      throttler?.Dispose();
    }

    private struct Releaser : IDisposable
    {
      private readonly SemaphoreSlim slim;
      public Releaser(SemaphoreSlim slim)
      {
        this.slim = slim;
      }

      public void Dispose()
      {
        slim?.Release();
      }
    }
  }
}