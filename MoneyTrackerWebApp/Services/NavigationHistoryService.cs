using Microsoft.AspNetCore.Components;

namespace MoneyTrackerWebApp.Services
{
    public interface INavigationHistoryService
    {
        void Dispose();
        void NavigateBack(string urlFailsafe);
        void NavigateTo(string url);
    }

    public class NavigationHistoryService : IDisposable, INavigationHistoryService
    {
        private readonly NavigationManager navigation;

        public NavigationHistoryService(NavigationManager navigation)
        {
            this.navigation = navigation;
        }



        private Stack<string> URLHistoryList { get; set; } = new Stack<string>();

        public void NavigateTo(string url)
        {
            string current = navigation.ToBaseRelativePath(navigation.Uri);
            this.URLHistoryList.Push(current);

            navigation.NavigateTo(url);
        }

        public void NavigateBack(string urlFailsafe)
        {
            string url = urlFailsafe;
            if (URLHistoryList.Any())
            {
                url = URLHistoryList.Pop();
            }

            navigation.NavigateTo(url);
        }

        public void Dispose()
        {
            URLHistoryList.Clear();
        }
    }
}
