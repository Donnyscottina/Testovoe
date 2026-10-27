using System;

namespace ClickerTest.Breeds
{
    [Serializable]
    internal sealed class BreedListResponseDto
    {
        public BreedDataDto[] data;
    }

    [Serializable]
    internal sealed class BreedResponseDto
    {
        public BreedDataDto data;
    }

    [Serializable]
    internal sealed class BreedDataDto
    {
        public string id;
        public BreedAttributesDto attributes;
    }

    [Serializable]
    internal sealed class BreedAttributesDto
    {
        public string name;
        public string description;
    }
}
