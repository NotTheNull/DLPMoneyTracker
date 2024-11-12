using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyTrackerBlaze.Models.Shared
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
        StockExchange
    }



    public static class IconHelper
    {
        const string BASE_URL = @"images\icons\";

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
                default:
                    throw new InvalidOperationException($"Option {opt} is not defined");
            }
        }
    }
}
