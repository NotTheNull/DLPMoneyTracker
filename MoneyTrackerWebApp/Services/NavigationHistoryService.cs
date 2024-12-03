using Microsoft.AspNetCore.Components;

namespace MoneyTrackerWebApp.Services
{
    public interface INavigationHistoryService
    {
        NavigationManager Navigation { get; set; }

        void Dispose();
        void NavigateBack(string urlFailsafe);
        void NavigateTo(string url);
    }

    public class NavigationHistoryService : IDisposable, INavigationHistoryService
    {

        [Inject]
        public NavigationManager Navigation { get; set; } = default!;

        private Stack<string> URLHistoryList { get; set; } = new Stack<string>();

        public void NavigateTo(string url)
        {
            string current = Navigation.ToBaseRelativePath(Navigation.Uri);
            this.URLHistoryList.Push(current);

            Navigation.NavigateTo(url);
        }

        public void NavigateBack(string urlFailsafe)
        {
            string url = urlFailsafe;
            if (URLHistoryList.Any())
            {
                url = URLHistoryList.Pop();
            }

            Navigation.NavigateTo(url);
        }

        public void Dispose()
        {
            URLHistoryList.Clear();
        }
    }
}
