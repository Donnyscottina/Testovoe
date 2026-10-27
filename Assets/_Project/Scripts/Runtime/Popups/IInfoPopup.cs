namespace ClickerTest.Popups
{
    public interface IInfoPopup
    {
        bool IsOpen { get; }
        void Show(string title, string body);
        void Hide();
    }
}
