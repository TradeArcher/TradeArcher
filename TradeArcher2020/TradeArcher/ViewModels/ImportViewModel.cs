using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Windows.Storage;
using Windows.Storage.Pickers;
using Caliburn.Micro;
using Microsoft.EntityFrameworkCore;
using TradeArcher.Core.Models;
using TradeArcher.Core.Services;
using TradeArcher.Helpers;
using TradeArcher.Views;

namespace TradeArcher.ViewModels
{
    public class ImportViewModel : Screen
    {
        private readonly TradesImporterService _tradesImporterService;
        private StorageFile _selectedFile;

        public ImportViewModel(TradesImporterService tradesImporterService)
        {
            _tradesImporterService = tradesImporterService;
        }

        private ObservableCollection<Account> _accounts;

        public ObservableCollection<Account> Accounts
        {
            get => _accounts;
            set => Set(ref _accounts, value);
        }

        private Account _selectedAccount;
        public Account SelectedAccount
        {
            get => _selectedAccount;
            set => Set(ref _selectedAccount, value);
        }

        private ObservableCollection<Broker> _brokers;

        public ObservableCollection<Broker> Brokers
        {
            get => _brokers;
            set => Set(ref _brokers, value);
        }

        private Broker _selectedBroker;

        public Broker SelectedBroker
        {
            get => _selectedBroker;
            set => Set(ref _selectedBroker, value);
        }

        private string _selectedFilePath;

        public string SelectedFile
        {
            get => _selectedFilePath;
            set => Set(ref _selectedFilePath, value);
        }

        private bool _canImportTrades;
        public bool CanImportTades
        {
            get => _canImportTrades;
            set => Set(ref _canImportTrades, value);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            var view = GetView() as IShellView;

            UpdateBrokers();
            SelectedBroker = Brokers.FirstOrDefault();
            UpdateAccounts();
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            PropertyChanged += OnPropertyChanged;
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            PropertyChanged -= OnPropertyChanged;

        }

        public async void SelectFileAsync()
        {
            var picker = new FileOpenPicker();
            picker.ViewMode = PickerViewMode.List;
            picker.SuggestedStartLocation = PickerLocationId.Desktop;
            picker.FileTypeFilter.Add(".csv");

            _selectedFile = await picker.PickSingleFileAsync();

            SelectedFile = _selectedFile?.Path;
        }

        public async void ImportTades()
        {
            try
            {
                var parserType = GetFileParserType();

                if (parserType != FileParserType.NotSupported)
                {

                    if (!_tradesImporterService.Import(parserType, SelectedAccount.AccountId, await _selectedFile.GetBytesAsync()))
                    {
                        // TODO: show import failure message
                    }
                    else
                    {
                        // TODO: show import success message
                    }
                }
                else
                {
                    // TODO: show import failure message due to not supported
                }
            }
            catch (NotSupportedException ex)
            {
                // TODO: show import failure message due to not supported
            }
            catch (Exception ex)
            {
                // TODO: show import failure message
            }
        }

        private FileParserType GetFileParserType()
        {
            switch (SelectedBroker.Name)
            {
                case "TD Ameritrade":
                    return FileParserType.TDAmeritrade;
                case "ThinkOrSwim":
                    return FileParserType.ThinkOrSwim;
                default:
                    return FileParserType.NotSupported;
            }
        }

        private void UpdateAccounts()
        {
            using (var context = new TradeArcherDataContext())
            {
                Accounts = new ObservableCollection<Account>(context.Accounts.Where(a => a.Broker.Name == SelectedBroker.Name).AsQueryable().Include(a => a.Broker));
            }
        }

        private void UpdateBrokers()
        {
            using (var context = new TradeArcherDataContext())
            {
                Brokers = new ObservableCollection<Broker>(context.Brokers);
            }

            if (Brokers.Count > 0)
            {
                SelectedBroker = Brokers.First();
            }
        }

        private bool ValidateForm()
        {
            return SelectedAccount != null && SelectedFile != null;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(SelectedBroker):
                    UpdateAccounts();
                    break;
                case nameof(SelectedAccount):
                case nameof(SelectedFile):
                    CanImportTades = ValidateForm();
                    break;
            }
        }
    }
}
