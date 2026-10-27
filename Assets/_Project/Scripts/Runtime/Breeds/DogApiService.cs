using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ClickerTest.Config;
using ClickerTest.Core.Network;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ClickerTest.Breeds
{
    public sealed class DogApiService
    {
        private readonly IRequestQueue _queue;
        private readonly IHttpClient _http;
        private readonly NetworkConfig _config;

        public DogApiService(IRequestQueue queue, IHttpClient http, NetworkConfig config)
        {
            _queue = queue;
            _http = http;
            _config = config;
        }

        public RequestHandle<IReadOnlyList<BreedSummary>> RequestBreeds() =>
            _queue.Enqueue<IReadOnlyList<BreedSummary>>(RequestTags.Breeds, LoadBreedsAsync);

        public RequestHandle<BreedDetails> RequestBreedDetails(string breedId) =>
            _queue.Enqueue(RequestTags.BreedDetails, token => LoadBreedDetailsAsync(breedId, token));

        public void CancelBreedDetails() => _queue.Cancel(RequestTags.BreedDetails);

        public void CancelAll()
        {
            _queue.Cancel(RequestTags.Breeds);
            _queue.Cancel(RequestTags.BreedDetails);
        }

        private async UniTask<IReadOnlyList<BreedSummary>> LoadBreedsAsync(CancellationToken token)
        {
            var url = $"{_config.DogApiBaseUrl}/breeds?page%5Bsize%5D={_config.BreedsCount}";
            var json = await _http.GetTextAsync(url, token);
            var response = JsonUtility.FromJson<BreedListResponseDto>(json);
            if (response?.data == null)
                throw new FormatException("Breeds response contains no data.");

            return response.data
                .Where(breed => breed?.attributes != null)
                .Take(_config.BreedsCount)
                .Select(breed => new BreedSummary(breed.id, breed.attributes.name))
                .ToList();
        }

        private async UniTask<BreedDetails> LoadBreedDetailsAsync(string breedId, CancellationToken token)
        {
            var json = await _http.GetTextAsync($"{_config.DogApiBaseUrl}/breeds/{Uri.EscapeDataString(breedId)}", token);
            var breed = JsonUtility.FromJson<BreedResponseDto>(json)?.data;
            if (breed?.attributes == null)
                throw new FormatException($"Breed {breedId} response contains no data.");

            return new BreedDetails(breed.id, breed.attributes.name, breed.attributes.description);
        }
    }
}
