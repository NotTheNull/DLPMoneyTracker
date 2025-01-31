namespace MoneyTrackerWebApp.Models.Core
{
    public enum IconOpt
    {
        AddNew,
        Cancel,
        Card,
        Dollar,
        EditPencil,
        Garbage,
        Exit,
        Books,
        LightBulb,
        Transfer,
        OpenFile,
        Payment,
        Refresh,
        Save,
        Settings,
        StockExchange,
        NONE
    }



    public static class IconHelper
    {
        const string BASE_URL = @"images\icons\";

        public static IconOpt ToIconOption(this string val)
        {
            if (string.IsNullOrWhiteSpace(val)) return IconOpt.NONE;
            switch (val.ToLower())
            {
                case "addnew": return IconOpt.AddNew;
                case "cancel": return IconOpt.Cancel;
                case "card": return IconOpt.Card;
                case "dollar": return IconOpt.Dollar;
                case "editpencil": return IconOpt.EditPencil;
                case "garbage": return IconOpt.Garbage;
                case "exit": return IconOpt.Exit;
                case "books": return IconOpt.Books;
                case "lightbulb": return IconOpt.LightBulb;
                case "transfer": return IconOpt.Transfer;
                case "openfile": return IconOpt.OpenFile;
                case "payment": return IconOpt.Payment;
                case "refresh": return IconOpt.Refresh;
                case "save": return IconOpt.Save;
                case "settings": return IconOpt.Settings;
                case "stockexchange": return IconOpt.StockExchange;
                default: throw new InvalidDataException($"[{val}] is not a valid Icon Option");
            }
        }

        public static string GetURL(IconOpt opt)
        {
            string iconName = IconOptToImageName(opt);

            return BASE_URL + iconName;

        }

        private static string IconOptToImageName(IconOpt opt)
        {
            switch (opt)
            {
                case IconOpt.AddNew:
                    return "Add-New-256.png";
                case IconOpt.Books:
                    return "Library-Books-256.png";
                case IconOpt.Cancel:
                    return "Cancel-256.png";
                case IconOpt.Card:
                    return "Credit-Card-256.png";
                case IconOpt.Dollar:
                    return "Dollar-256.png";
                case IconOpt.EditPencil:
                    return "Edit-Pencil-256.png";
                case IconOpt.Exit:
                    return "Leave-256.png";
                case IconOpt.Garbage:
                    return "Garbage-Closed-256.png";
                case IconOpt.LightBulb:
                    return "Light-Bulb-256.png";
                case IconOpt.OpenFile:
                    return "open-file-50.png";
                case IconOpt.Payment:
                    return "Payment-256.png";
                case IconOpt.Refresh:
                    return "Refresh-256.png";
                case IconOpt.Save:
                    return "Save-256.png";
                case IconOpt.Settings:
                    return "Settings-01-256.png";
                case IconOpt.StockExchange:
                    return "Stock-Exchange-256.png";
                case IconOpt.Transfer:
                    return "Money-Transfer-256.png";
                case IconOpt.NONE:
                    return "";
                default:
                    throw new InvalidOperationException($"Option {opt} is not defined");
            }
        }
    }

}
