namespace ClickerTest.Breeds
{
    public sealed class BreedSummary
    {
        public BreedSummary(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public string Id { get; }
        public string Name { get; }
    }

    public sealed class BreedDetails
    {
        public BreedDetails(string id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }

        public string Id { get; }
        public string Name { get; }
        public string Description { get; }
    }
}
