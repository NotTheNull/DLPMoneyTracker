﻿using DLPMoneyTracker.Core.Models;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using DLPMoneyTracker2.Core;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker2.Config.AddEditMoneyAccounts
{
    public class CSVMappingVM : BaseViewModel
    {
        private ICSVMapping _mapOriginal;
        private MoneyAccountVM _money;

        public CSVMappingVM()
        {
            
        }
        public CSVMappingVM(MoneyAccountVM money)
        {
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


        private int _colDesc = 2;

        public int DescriptionColumn
        {
            get { return _colDesc; }
            set
            {
                _colDesc = value;
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



        #endregion


        #region Commands

        private RelayCommand _cmdDiscard;
        public RelayCommand CommandDiscard
        {
            get
            {
                return _cmdDiscard ??= new RelayCommand((o) =>
                {
                    this.Reset();
                });
            }
        }


        private RelayCommand _cmdSave;
        public RelayCommand CommandSave
        {
            get
            {
                return _cmdSave ??= new RelayCommand((o) =>
                {
                    if (_money.Mapping is null) _money.Mapping = new CSVMapping();
                    _money.Mapping.Copy(this.BuildMap());
                });
            }
        }


        #endregion


        private void Reset()
        {
            if (_mapOriginal is null) _mapOriginal = new CSVMapping();

            this.StartingRow = _mapOriginal.StartingRow;
            this.IsAmountInverted = _mapOriginal.IsAmountInverted;

            // Remember to add 1 since arrays are 0-index
            this.TransDateColumn = _mapOriginal.GetMapping(ICSVMapping.TRANS_DATE)+1;
            this.DescriptionColumn = _mapOriginal.GetMapping(ICSVMapping.DESCRIPTION)+1;
            this.AmountColumn = _mapOriginal.GetMapping(ICSVMapping.AMOUNT)+1;
        }


        public ICSVMapping BuildMap()
        {
            CSVMapping newMapping = new CSVMapping()
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
            _mapOriginal = money.Mapping;
            _money = money;
            this.Reset();
        }
    }
}
