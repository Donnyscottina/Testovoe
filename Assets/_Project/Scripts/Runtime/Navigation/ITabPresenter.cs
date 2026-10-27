namespace ClickerTest.Navigation
{
    public enum TabId
    {
        Clicker = 0,
        Weather = 1,
        Breeds = 2
    }

    public interface ITabPresenter
    {
        TabId Id { get; }

        void Show();

        void Hide();
    }
}
