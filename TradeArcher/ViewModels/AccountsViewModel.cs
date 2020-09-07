using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using Microsoft.EntityFrameworkCore;
using TradeArcher.Core.Models;
using TradeArcher.Views;

namespace TradeArcher.ViewModels
{
    public class AccountsViewModel : Screen
    {
        private readonly WinRTContainer _container;

        public AccountsViewModel(WinRTContainer container)
        {
            _container = container;
        }

        private ObservableCollection<Account> _accounts;

        public ObservableCollection<Account> Accounts
        {
            get => _accounts;
            set => Set(ref _accounts, value);
        }

        private ObservableCollection<Broker> _brokers;

        public ObservableCollection<Broker> Brokers
        {
            get => _brokers;
            set => Set(ref _brokers, value);
        }

        private Account _selectedAccount;

        public Account SelectedAccount
        {
            get => _selectedAccount;
            set => Set(ref _selectedAccount, value);
        }

        private Broker _selectedBroker;

        public Broker SelectedBroker
        {
            get => _selectedBroker;
            set => Set(ref _selectedBroker, value);
        }

        private AccountsPageVisualState _state;
        public AccountsPageVisualState State
        {
            get => _state;
            set => Set(ref _state, value);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            var view = GetView() as IShellView;

            UpdateAccounts();
            UpdateBrokers();
        }

        public void AddAccount()
        {
            State = AccountsPageVisualState.AddOrUpdate;
            SelectedAccount = new Account();
        }

        public void EditAccount(Account account)
        {
            State = AccountsPageVisualState.AddOrUpdate;
            SelectedAccount = account;
        }

        public void SaveAccount()
        {
            using (var context = new TradeArcherDataContext())
            {
                //context.Entry(SelectedAccount).State = SelectedAccount.AccountId == 0 ? EntityState.Added : EntityState.Modified;

                var broker = context.Brokers.FirstOrDefault(b => b.BrokerId == SelectedBroker.BrokerId);

                Account account = null;

                if (SelectedAccount != null && SelectedAccount.AccountId == 0)
                {
                    account = SelectedAccount;
                    account.Broker = broker;
                    context.Accounts.Add(account);
                }
                else
                {
                    account = context.Accounts.FirstOrDefault(a => a.AccountId == SelectedAccount.AccountId);

                    if (account != null)
                    {

                        account.Broker = broker;
                        account.DisplayName = SelectedAccount.DisplayName;
                        account.Name = SelectedAccount.Name;

                        context.Accounts.Update(account);
                    }
                    else
                    {
                        account = new Account();
                        account.Broker = broker;
                        context.Accounts.Add(account);
                    }
                }

                context.SaveChanges();

                UpdateAccounts();

                var accountToSelect = Accounts.FirstOrDefault(a => a.AccountId == account.AccountId);

                SelectedAccount = accountToSelect;

                State = AccountsPageVisualState.List;
            }
        }

        public void DeleteAccount(Account selectedAccount)
        {
            using (var context = new TradeArcherDataContext())
            {
                var account = context.Accounts.Find(selectedAccount.AccountId);
                context.Accounts.Remove(account);
                context.SaveChanges();

                UpdateAccounts();

                State = AccountsPageVisualState.List;
            }
        }

        public void CancelAddOrUpdateAccount()
        {
            State = AccountsPageVisualState.List;
        }

        private void UpdateAccounts()
        {
            using (var context = new TradeArcherDataContext())
            {
                Accounts = new ObservableCollection<Account>(context.Accounts.Include(a => a.Broker));
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
    }

    public enum AccountsPageVisualState
    {
        List,
        AddOrUpdate
    }

    //public class AccountModel : PropertyChangedBase
    //{
    //    public AccountModel(Account account)
    //    {
    //        if (account == null)
    //        {
    //            throw new ArgumentException("account is required.");
    //        }
    //    }
    //    private int _accountId;
    //    public int AccountId
    //    {
    //        get => _accountId;
    //        set => Set(ref _accountId, value);
    //    }

    //    private string _name;
    //    public string Name
    //    {
    //        get => _name;
    //        set => Set(ref _name, value);
    //    }

    //    private string _displayName;
    //    public string DisplayName
    //    {
    //        get => _displayName;
    //        set => Set(ref _displayName, value);
    //    }

    //    private Broker _broker;
    //    public virtual Broker Broker
    //    {
    //        get => _broker;
    //        set => Set(ref _broker, value);
    //    }
    //}
}
