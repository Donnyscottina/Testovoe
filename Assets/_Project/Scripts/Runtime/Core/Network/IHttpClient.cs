using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ClickerTest.Core.Network
{
    public interface IHttpClient
    {
        UniTask<string> GetTextAsync(string url, CancellationToken cancellationToken);
        UniTask<Texture2D> GetTextureAsync(string url, CancellationToken cancellationToken);
    }
}
