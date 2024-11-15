using System.Reflection;
using System.Text.Json;

namespace MoneyTrackerWebApp.Models.Core.NavMenu
{
    public class NavMenuVM
    {
        public NavMenuVM()
        {
            this.Load();
        }

        public List<NavMenuItemVM> NavLinks { get; set; } = new List<NavMenuItemVM>();

        private void Load()
        {
            var assembly = Assembly.GetExecutingAssembly();

            // We could use the full namespace but I'm avoiding it in case I move stuff around again
            this.NavLinks.Clear();
            string resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith("NavMenuLinks.json"));
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string json = reader.ReadToEnd();
                    var data = JsonSerializer.Deserialize<NavMenuItemVM[]>(json);
                    this.NavLinks.AddRange(data);
                }
            }
        }
    }

    public class NavMenuItemVM
    {
        public string Name { get; set; }
        public string Url { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public List<NavMenuItemVM> Links { get; set; }
    }
}
