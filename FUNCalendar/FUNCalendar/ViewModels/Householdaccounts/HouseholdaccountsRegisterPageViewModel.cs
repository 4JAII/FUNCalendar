using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using OxyPlot;
using OxyPlot.Xamarin.Forms;
using OxyPlot.Series;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using FUNCalendar.Models;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Xamarin.Forms;
using Prism.Navigation;
using Prism.Services;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;

namespace FUNCalendar.ViewModels
{
    public class HouseholdaccountsRegisterPageViewModel : BindableBase,INavigationAware,IDisposable
    {
        private IHouseHoldAccounts _householdaccount;
        private INavigationService _navigationservice;
        private IPageDialogService _pageDialogService;

        private CompositeDisposable disposable { get; } = new CompositeDisposable();

        /* 正しい遷移か確認するためのkey */
        public static readonly string InputKey = "InputKey";

        /* 遷移されたときの格納用変数 */
        public HouseholdaccountNavigationItem NavigatedItem { get; set; }

        /* 現在選択されている各データを保持 */
        public DateTime CurrentDate { get; private set; }
        public Range CurrentRange { get; private set; }
        
        /* 収支変更用コマンド */
        public ReactiveCommand IncomeCommand { get; private set; }
        public ReactiveCommand OutgoingCommand { get; private set; }

        /* ピッカー */
        public List<HouseholdaccountsSCategoryItem> IncomeScategoryNames { get; private set; }
        public List<HouseholdaccountsSCategoryItem> OutgoingScategoryNames { get; private set; }
        public ReactiveCollection<HouseholdaccountsSCategoryItem> ScategoryNames { get; private set; }
        public ReactiveCollection<HouseholdaccountsDcategoryItem> DcategoryNames { get; private set; }
        public List<HouseholdaccountsStorageTypeItem> StorageNames { get; private set; }

        /* householdaccount登録用 */
        public int ID { get; private set; } = -1;
        [Required(ErrorMessage = "タイトルを入力してください")]
        public ReactiveProperty<string> Name { get; private set; } = new ReactiveProperty<string>();

        [Required(ErrorMessage ="金額を入力してください")]
        [RegularExpression("[0-9]+")]
        public ReactiveProperty<string> Price { get; private set; } = new ReactiveProperty<string>();

        public ReactiveProperty<DateTime> Date { get; private set; } = new ReactiveProperty<DateTime>();
        public ReactiveProperty<HouseholdaccountsSCategoryItem> CurrentScategory { get; private set; }
        public ReactiveProperty<HouseholdaccountsDcategoryItem> CurrentDcategory { get; private set; }
        public ReactiveProperty<HouseholdaccountsStorageTypeItem> CurrentStorageType { get; private set; }
        public ReactiveProperty<bool> IsOutgoing { get; private set; }

        /* 登録・キャンセルするときの処理用 */
        public ReactiveProperty<bool> CanRegister { get; private set; }
        public AsyncReactiveCommand RegisterHouseholdaccountsCommand { get; private set; }
        public AsyncReactiveCommand CancelCommand { get; private set; }

        /* エラー時の色 */
        public ReactiveProperty<Color> ErrorColor { get; private set; } = new ReactiveProperty<Color>();

        /* データベース用 */
        //private LocalStorage localStorage; 

        
        
        /* コンストラクタ */
        public HouseholdaccountsRegisterPageViewModel(IHouseHoldAccounts householdaccount, INavigationService navigationService, IPageDialogService pageDialogService)
        {
            //localStorage = new LocalStorage();

            this._householdaccount = householdaccount;
            this._navigationservice = navigationService;
            this._pageDialogService = pageDialogService;

            /* 属性を有効化 */
            Name.SetValidateAttribute(() => this.Name);
            Price.SetValidateAttribute(() => this.Price);

            /* ピッカー用のアイテム作成 */
            IncomeScategoryNames = new List<HouseholdaccountsSCategoryItem>();
            OutgoingScategoryNames = new List<HouseholdaccountsSCategoryItem>();
            CurrentScategory = new ReactiveProperty<HouseholdaccountsSCategoryItem>();

            DcategoryNames = new ReactiveCollection<HouseholdaccountsDcategoryItem>();
            CurrentDcategory = new ReactiveProperty<HouseholdaccountsDcategoryItem>();

            StorageNames = new List<HouseholdaccountsStorageTypeItem>();
            CurrentStorageType = new ReactiveProperty<HouseholdaccountsStorageTypeItem>();

            StorageNames.Clear();
            for (int i = (int)StorageTypes.start_of_Stype + 1; i < (int)StorageTypes.end_of_Stype; i++)
            {
                var stname = Enum.GetName(typeof(StorageTypes), i);
                var stdata = (StorageTypes)Enum.ToObject(typeof(StorageTypes), i);
                var item = new HouseholdaccountsStorageTypeItem(stname, stdata);
                StorageNames.Add(item);
            }
            CurrentStorageType.Value = StorageNames[0];

            for (int i = (int)SCategorys.start_of_支出 + 1; i < (int)SCategorys.end_of_支出; i++)
            {
                var scname = Enum.GetName(typeof(SCategorys), i);
                var scdata = (SCategorys)Enum.ToObject(typeof(SCategorys), i);
                var item = new HouseholdaccountsSCategoryItem(scname, scdata);
                OutgoingScategoryNames.Add(item);
            }
            for (int i = (int)SCategorys.start_of_収入 + 1; i < (int)SCategorys.end_of_収入; i++)
            {
                var scname = Enum.GetName(typeof(SCategorys), i);
                var scdata = (SCategorys)Enum.ToObject(typeof(SCategorys), i);
                var item = new HouseholdaccountsSCategoryItem(scname, scdata);
                IncomeScategoryNames.Add(item);
            }

            ScategoryNames = new ReactiveCollection<HouseholdaccountsSCategoryItem>();
            foreach(HouseholdaccountsSCategoryItem x in OutgoingScategoryNames)
            {
                ScategoryNames.Add(x);
            }

            CurrentScategory.Value = ScategoryNames[0];

            IsOutgoing = new ReactiveProperty<bool>();
            IsOutgoing.Value = true;

            IncomeCommand = new ReactiveCommand();
            IncomeCommand.Subscribe(_ =>
            {
                IsOutgoing.Value = false;
                UpdateScategory(false);
            }).AddTo(disposable);

            OutgoingCommand = new ReactiveCommand();
            OutgoingCommand.Subscribe(_ =>
            {
                IsOutgoing.Value = true;
                UpdateScategory(true);
            }).AddTo(disposable);

            /* 登録できるかどうか */
            CanRegister = new[]
            {
                this.Name.ObserveHasErrors,
                this.Price.ObserveHasErrors,
            }.CombineLatestValuesAreAllFalse().ToReactiveProperty<bool>();
            CanRegister.Subscribe(x =>
            {
                ErrorColor.Value = x ? Color.SkyBlue : Color.Gray;
            });

            /* 登録ボタンが押された時の処理 */
            RegisterHouseholdaccountsCommand = CanRegister.ToAsyncReactiveCommand();
            RegisterHouseholdaccountsCommand.Subscribe(async () =>
            {
                var navigationitem = new HouseholdaccountNavigationItem(CurrentDate, CurrentRange);
                var navigationparameter = new NavigationParameters()
                {
                    {HouseHoldAccountsStatisticsPageViewModel.InputKey, navigationitem }
                };
                if (ID != -1)
                {
                    var scategory = Enum.GetName(typeof(SCategorys), CurrentScategory.Value.ScategoryData);
                    var dcategory = Enum.GetName(typeof(DCategorys), CurrentDcategory.Value.DcategoryData);
                    var storagetype = Enum.GetName(typeof(StorageTypes), CurrentStorageType.Value.StorageTypeData);
                    var isoutgoing = IsOutgoing.Value ? "支出": "収入";
                    var vmitem = new VMHouseHoldAccountsItem(ID, Name.Value, Price.Value, Date.Value, scategory, dcategory, storagetype, isoutgoing);
                    var item = VMHouseHoldAccountsItem.ToHouseholdaccountsItem(vmitem);
                    // await localStorage.EditItem(item);

                }
                else
                {
                    var name = this.Name.Value;
                    var price = int.Parse(this.Price.Value);
                    var date = this.Date.Value;
                    var dcategory = CurrentDcategory.Value.DcategoryData;
                    var scategory = CurrentScategory.Value.ScategoryData;
                    var storagetype = CurrentStorageType.Value.StorageTypeData;
                    var isoutgoing = IsOutgoing.Value;
                    var item = new HouseHoldAccountsItem() { Name = name, Price = price, Date = date, DCategory = dcategory, SCategory = scategory, StorageType = storagetype, IsOutGoings = isoutgoing };
                    //await localStorage.AddItem(new HouseholdaccontsItem(this.Name.Value, int.Parse(this.Price.Value), this.Date.Value, this.Dcategory.Value, this.Scategory.Value, this.Storagetype.Value, this.IsOutgoing.Value, -1));
                    item.ID = -1;/* localStorage.LastAddedHouseholdaccountsItemID ;*/
                    _householdaccount.AddHouseHoldAccountsItem(item);
                }
                 await _navigationservice.NavigateAsync("/RootPage/NavigationPage/HouseHoldAccountsStatisticsPage",navigationparameter);
            });

            /* キャンセルボタンが押された時の処理 */
            CancelCommand = new AsyncReactiveCommand();
            CancelCommand.Subscribe(async () =>
            {
                var navigationitem = new HouseholdaccountNavigationItem(CurrentDate, CurrentRange);
                var navigationparameter = new NavigationParameters()
                {
                    {HouseHoldAccountsStatisticsPageViewModel.InputKey, navigationitem }
                };
                var result = await _pageDialogService.DisplayAlertAsync("確認", "入力をキャンセルし画面を変更します。よろしいですか？", "はい", "いいえ");
                if (result) await _navigationservice.NavigateAsync("/RootPage/NavigationPage/HouseHoldAccountsStatisticsPage", navigationparameter);
            });
        }
        public void OnNavigatedFrom(NavigationParameters parameters)
        {
            disposable.Dispose();
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            if (parameters.ContainsKey(InputKey))
            {
                NavigatedItem = (HouseholdaccountNavigationItem)parameters[InputKey];
                this.CurrentDate = NavigatedItem.CurrentDate;
                this.CurrentRange = NavigatedItem.CurrentRange;

                /* 指定された日付に設定 */
                Date.Value = CurrentDate;
                CurrentScategory.Subscribe(_ =>
                {
                    if(_ != null)
                    {
                        UpdateDcategory(_.ScategoryData);
                    }
                }).AddTo(disposable);
            }
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
        }
        private void UpdateScategory(bool isoutgoing)
        {
            if (isoutgoing)
            {
                ScategoryNames.Clear();
                foreach (HouseholdaccountsSCategoryItem x in OutgoingScategoryNames)
                {
                    ScategoryNames.Add(x);
                }
                CurrentScategory.Value = ScategoryNames[0];
            }
            else
            {
                ScategoryNames.Clear();
                foreach(HouseholdaccountsSCategoryItem x in IncomeScategoryNames)
                {
                    ScategoryNames.Add(x);
                }
                CurrentScategory.Value = ScategoryNames[0];
            }
        }

        private void UpdateDcategory(SCategorys sc)
        {
            var startpoint = _householdaccount.ScToDcStart(sc);
            var endpoint = _householdaccount.ScToDcEnd(sc);

            DcategoryNames.Clear();
            for (int i = startpoint + 1; i < endpoint; i++)
            {
                var dcname = Enum.GetName(typeof(DCategorys), i);
                var dcdata = (DCategorys)Enum.ToObject(typeof(DCategorys), i);
                var item = new HouseholdaccountsDcategoryItem(dcname, dcdata);

                DcategoryNames.Add(item);
            }
            CurrentDcategory.Value = DcategoryNames[0];
        }
        /* 購読解除 */
        public void Dispose()
        {
            disposable.Dispose();
        }

    }
}
