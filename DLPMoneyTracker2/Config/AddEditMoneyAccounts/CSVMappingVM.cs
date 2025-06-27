using DLPMoneyTracker.Core.Models;
using DLPMoneyTracker2.Core;

namespace DLPMoneyTracker2.Config.AddEditMoneyAccounts
{
    public class CSVMappingVM : BaseViewModel
    {
        private MoneyAccountVM? _money;
        private ICSVMapping? _mapOriginal;

        public CSVMappingVM() { }

        public CSVMappingVM(MoneyAccountVM money)
        {
            _money = money;
            this.LoadMoneyAccount(money);
        }

        #region Properties

        private int _rowStart = 1;

        public int StartingRow
        {
            get { return _rowStart; }
            set
            {
                _rowStart = value;
                NotifyPropertyChanged(nameof(StartingRow));
            }
        }

        private int _colTransDate = 1;

        public int TransDateColumn
        {
            get { return _colTransDate; }
            set
            {
                _colTransDate = value;
                NotifyPropertyChanged(nameof(TransDateColumn));
            }
        }

        private int _colDescription = 2;

        public int DescriptionColumn
        {
            get { return _colDescription; }
            set
            {
                _colDescription = value;
                NotifyPropertyChanged(nameof(DescriptionColumn));
            }
        }

        private int _colAmount = 3;

        public int AmountColumn
        {
            get { return _colAmount; }
            set
            {
                _colAmount = value;
                NotifyPropertyChanged(nameof(AmountColumn));
            }
        }

        private bool _isInverted;

        public bool IsAmountInverted
        {
            get { return _isInverted; }
            set
            {
                _isInverted = value;
                NotifyPropertyChanged(nameof(this.IsAmountInverted));
            }
        }

        #endregion Properties

        #region Commands

        public RelayCommand CommandDiscard =>
            new((o) =>
            {
                this.Reset();
            });

        public RelayCommand CommandSave =>
            new((o) =>
            {
                if (_money is null) return;
                _money.Mapping ??= new CSVMapping();
                _money.Mapping.Copy(this.BuildMap());
            });

        #endregion Commands

        private void Reset()
        {
            _mapOriginal ??= new CSVMapping();

            this.StartingRow = _mapOriginal.StartingRow;
            this.IsAmountInverted = _mapOriginal.IsAmountInverted;

            // Remember to add 1 since arrays are 0-index
            this.TransDateColumn = _mapOriginal.GetMapping(ICSVMapping.TRANS_DATE) + 1;
            this.DescriptionColumn = _mapOriginal.GetMapping(ICSVMapping.DESCRIPTION) + 1;
            this.AmountColumn = _mapOriginal.GetMapping(ICSVMapping.AMOUNT) + 1;
        }

        public ICSVMapping BuildMap()
        {
            CSVMapping newMapping = new()
            {
                StartingRow = this.StartingRow,
                IsAmountInverted = this.IsAmountInverted
            };

            newMapping.SetMapping(ICSVMapping.TRANS_DATE, this.TransDateColumn);
            newMapping.SetMapping(ICSVMapping.DESCRIPTION, this.DescriptionColumn);
            newMapping.SetMapping(ICSVMapping.AMOUNT, this.AmountColumn);

            return newMapping;
        }

        public void LoadMoneyAccount(MoneyAccountVM money)
        {
            _money = money;
            _mapOriginal = money.Mapping;
            this.Reset();
        }
    }
}