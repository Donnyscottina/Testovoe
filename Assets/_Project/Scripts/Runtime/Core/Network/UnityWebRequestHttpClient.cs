using System.Threading;
using ClickerTest.Config;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace ClickerTest.Core.Network
{
    public sealed class UnityWebRequestHttpClient : IHttpClient
    {
        private readonly NetworkConfig _config;

        public UnityWebRequestHttpClient(NetworkConfig config)
        {
            _config = config;
        }

        public async UniTask<string> GetTextAsync(string url, CancellationToken cancellationToken)
        {
            using var request = UnityWebRequest.Get(url);
            Configure(request);
            await request.SendWebRequest().WithCancellation(cancellationToken, cancelImmediately: true);
            return request.downloadHandler.text;
        }

        public async UniTask<Texture2D> GetTextureAsync(string url, CancellationToken cancellationToken)
        {
            using var request = UnityWebRequestTexture.GetTexture(url);
            Configure(request);
            await request.SendWebRequest().WithCancellation(cancellationToken, cancelImmediately: true);
            return DownloadHandlerTexture.GetContent(request);
        }

        private void Configure(UnityWebRequest request)
        {
            request.timeout = _config.TimeoutSeconds;
            request.SetRequestHeader("User-Agent", _config.UserAgent);
        }
    }
}
