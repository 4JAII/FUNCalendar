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
using FUNCalendar.Services;

namespace FUNCalendar.ViewModels
{
    public class HouseholdAccountsRegisterPageViewModel : BindableBase,INavigationAware,IDisposable
    {
        private IHouseholdAccounts _householdaccount;
        private IStorageService _storageService;
        private INavigationService _navigationservice;
        private IPageDialogService _pageDialogService;

        private CompositeDisposable disposable { get; } = new CompositeDisposable();

        /* 正しい遷移か確認するためのkey */
        public static readonly string InputKey = "InputKey";
        public static readonly string EditKey = "EditKey";
        public static readonly string CalendarKey = "CalendarKey";

        /* 遷移されたときのデータ格納用変数 */
        public HouseholdAccountsNavigationItem NavigatedItem { get; set; }

        /* 現在選択されている各データを保持 */
        public DateTime CurrentDate { get; private set; }
        public Range CurrentRange { get; private set; }
        public BalanceTypes CurrentBalancetype { get; private set; }
        public SCategorys CurrentScategory { get; private set; }
        public DCategorys CurrentDcategory { get; private set; }

        /* 残高のデータ */
        public ReadOnlyReactiveCollection<VMHouseholdAccountsBalanceItem> Balances { get; private set; }
        
        /* 収支変更用コマンド */
        public ReactiveCommand IncomeCommand { get; private set; }
        public ReactiveCommand OutgoingCommand { get; private set; }

        /* ピッカー */
        public List<HouseholdAccountsSCategoryItem> IncomeScategoryNames { get; private set; }
        public List<HouseholdAccountsSCategoryItem> OutgoingScategoryNames { get; private set; }
        public ReactiveCollection<HouseholdAccountsSCategoryItem> ScategoryNames { get; private set; }
        public ReactiveCollection<HouseholdAccountsDcategoryItem> DcategoryNames { get; private set; }
        public List<HouseholdAccountsStorageTypeItem> StorageNames { get; private set; }

        /* householdaccount登録用 */
        public int ID { get; private set; } = -1;
        [Required(ErrorMessage = "タイトルを入力してください"), StringLength(32)]
        public ReactiveProperty<string> Name { get; private set; } = new ReactiveProperty<string>();

        [Required(ErrorMessage ="金額を入力してください")]
        [RegularExpression("[0-9]+")]
        public ReactiveProperty<string> Price { get; private set; } = new ReactiveProperty<string>();

        public ReactiveProperty<DateTime> Date { get; private set; } = new ReactiveProperty<DateTime>();
        public ReactiveProperty<HouseholdAccountsSCategoryItem> SelectedScategory { get; private set; }
        public ReactiveProperty<HouseholdAccountsDcategoryItem> SelectedDcategory { get; private set; }
        public ReactiveProperty<HouseholdAccountsStorageTypeItem> SelectedStorageType { get; private set; }
        public ReactiveProperty<bool> IsOutgoing { get; private set; }

        /* 登録・キャンセルするときの処理用 */
        public ReactiveProperty<bool> CanRegister { get; private set; }
        public AsyncReactiveCommand RegisterHouseholdaccountsCommand { get; private set; }
        public AsyncReactiveCommand CancelCommand { get; private set; }

        /* 支出・収入の色 */
        public ReactiveProperty<Color> IncomeColor { get; private set; } = new ReactiveProperty<Color>();
        public ReactiveProperty<Color> OutgoingColor { get; private set; } = new ReactiveProperty<Color>();

        /* エラー時の色 */
        public ReactiveProperty<Color> ErrorColor { get; private set; } = new ReactiveProperty<Color>();

        /* ページ遷移用 */
        private PageName backPage;

        /* コンストラクタ */
        public HouseholdAccountsRegisterPageViewModel(IHouseholdAccounts householdaccount,IStorageService storageService, INavigationService navigationService, IPageDialogService pageDialogService)
        {
            this._householdaccount = householdaccount;
            this._storageService = storageService;
            this._navigationservice = navigationService;
            this._pageDialogService = pageDialogService;

            IncomeColor.Value = Color.White;
            OutgoingColor.Value = Color.SkyBlue;

            /* Balanceitemの保持 */
            this.Balances = _householdaccount.Balances.ToReadOnlyReactiveCollection(x => new VMHouseholdAccountsBalanceItem(x)).AddTo(disposable);

            /* 属性を有効化 */
            Name.SetValidateAttribute(() => this.Name);
            Price.SetValidateAttribute(() => this.Price);

            /* ピッカー用のアイテム作成 */
            IncomeScategoryNames = new List<HouseholdAccountsSCategoryItem>();
            OutgoingScategoryNames = new List<HouseholdAccountsSCategoryItem>();
            SelectedScategory = new ReactiveProperty<HouseholdAccountsSCategoryItem>();

            DcategoryNames = new ReactiveCollection<HouseholdAccountsDcategoryItem>();
            SelectedDcategory = new ReactiveProperty<HouseholdAccountsDcategoryItem>();

            StorageNames = new List<HouseholdAccountsStorageTypeItem>();
            SelectedStorageType = new ReactiveProperty<HouseholdAccountsStorageTypeItem>();

            /* Storagetypeリストを作成 */
            StorageNames.Clear();
            for (int i = (int)StorageTypes.start_of_Stype + 1; i < (int)StorageTypes.end_of_Stype; i++)
            {
                var stname = Enum.GetName(typeof(StorageTypes), i);
                var stdata = (StorageTypes)Enum.ToObject(typeof(StorageTypes), i);
                var item = new HouseholdAccountsStorageTypeItem(stname, stdata);
                StorageNames.Add(item);
            }

            /* 現在のStoragetypeを先頭に設定 */
            SelectedStorageType.Value = StorageNames[0];

            /* 収入・支出のScategoryリストを作成*/
            for (int i = (int)SCategorys.start_of_支出 + 1; i < (int)SCategorys.end_of_支出; i++)
            {
                var scname = Enum.GetName(typeof(SCategorys), i);
                var scdata = (SCategorys)Enum.ToObject(typeof(SCategorys), i);
                var item = new HouseholdAccountsSCategoryItem(scname, scdata);
                OutgoingScategoryNames.Add(item);
            }
            for (int i = (int)SCategorys.start_of_収入 + 1; i < (int)SCategorys.end_of_収入; i++)
            {
                var scname = Enum.GetName(typeof(SCategorys), i);
                var scdata = (SCategorys)Enum.ToObject(typeof(SCategorys), i);
                var item = new HouseholdAccountsSCategoryItem(scname, scdata);
                IncomeScategoryNames.Add(item);
            }

            /* 現在のScategoryリストを支出のScategoryリストに設定 */
            ScategoryNames = new ReactiveCollection<HouseholdAccountsSCategoryItem>();
            foreach(HouseholdAccountsSCategoryItem x in OutgoingScategoryNames)
            {
                ScategoryNames.Add(x);
            }

            /* 現在のScategoryを先頭に設定 */
            SelectedScategory.Value = ScategoryNames[0];

            /* Scategoryが変更されたときの処理 */
            SelectedScategory.Subscribe(_ =>
            {
                if (_ != null)
                {
                    UpdateDcategory(_.ScategoryData);
                }
            }).AddTo(disposable);

            /* 収支を支出に設定 */
            IsOutgoing = new ReactiveProperty<bool>();
            IsOutgoing.Value = true;

            /* 収入ボタンが押されたときの処理 */
            IncomeCommand = new ReactiveCommand();
            IncomeCommand.Subscribe(_ =>
            {
                IsOutgoing.Value = false;
                UpdateScategory(false);
                IncomeColor.Value = Color.SkyBlue;
                OutgoingColor.Value = Color.White;
            }).AddTo(disposable);

            /* 支出ボタンが押されたときの処理 */
            OutgoingCommand = new ReactiveCommand();
            OutgoingCommand.Subscribe(_ =>
            {
                IsOutgoing.Value = true;
                UpdateScategory(true);
                IncomeColor.Value = Color.White;
                OutgoingColor.Value = Color.SkyBlue;
            }).AddTo(disposable);

            /* 登録できるかどうか */
            CanRegister = new[]
            {
                this.Name.ObserveHasErrors,
                this.Price.ObserveHasErrors,
            }.CombineLatestValuesAreAllFalse().ToReactiveProperty<bool>();
            CanRegister.Subscribe(x =>
            {
                ErrorColor.Value = x ? Color.SkyBlue : Color.DarkGray;
            });

            /* 登録ボタンが押された時の処理 */
            RegisterHouseholdaccountsCommand = CanRegister.ToAsyncReactiveCommand();
            RegisterHouseholdaccountsCommand.Subscribe(async () =>
            {
                HouseholdAccountsNavigationItem navigationitem;
                if (backPage == PageName.HouseholdAccountsSCStatisticsPage)
                {
                     navigationitem = new HouseholdAccountsNavigationItem(CurrentBalancetype, CurrentScategory, CurrentDate, CurrentRange);
                }
                else if(backPage == PageName.HouseholdAccountsDCHistoryPage)
                {
                     navigationitem = new HouseholdAccountsNavigationItem(CurrentBalancetype, CurrentScategory, CurrentDcategory, CurrentDate, CurrentRange);
                }
                else
                {
                    navigationitem = new HouseholdAccountsNavigationItem(CurrentDate, CurrentRange);
                }

                var navigationparameter = HouseholdAccountsNavigationItem.CreateNavigationParameter(backPage, navigationitem);
                /* アイテム編集 */
                if (ID != -1)
                {
                    var scategory = Enum.GetName(typeof(SCategorys), SelectedScategory.Value.ScategoryData);
                    var dcategory = Enum.GetName(typeof(DCategorys), SelectedDcategory.Value.DcategoryData);
                    var storagetype = Enum.GetName(typeof(StorageTypes), SelectedStorageType.Value.StorageTypeData);
                    var isoutgoing = IsOutgoing.Value ? "支出": "収入";
                    var vmitem = new VMHouseholdAccountsItem(ID, Name.Value, Price.Value, Date.Value, scategory, dcategory, storagetype, isoutgoing);
                    var item = VMHouseholdAccountsItem.ToHouseholdaccountsItem(vmitem);

                    await _storageService.EditItem(_householdaccount.SelectedItem, item);
                }
                /* アイテム追加 */
                else
                {
                    var name = this.Name.Value;
                    var price = int.Parse(this.Price.Value);
                    var date = this.Date.Value;
                    var dcategory = SelectedDcategory.Value.DcategoryData;
                    var scategory = SelectedScategory.Value.ScategoryData;
                    var storagetype = SelectedStorageType.Value.StorageTypeData;
                    var isoutgoing = IsOutgoing.Value;
                    var item = new HouseholdAccountsItem() { Name = name, Price = price, Date = date, DCategory = dcategory, SCategory = scategory, StorageType = storagetype, IsOutGoings = isoutgoing };
                    await _storageService.AddItem(item);
                }
                var pagename = String.Format("/RootPage/NavigationPage/{0}", backPage);
                await _navigationservice.NavigateAsync(pagename, navigationparameter);
            });

            /* キャンセルボタンが押された時の処理 */
            CancelCommand = new AsyncReactiveCommand();
            CancelCommand.Subscribe(async () =>
            {
                HouseholdAccountsNavigationItem navigationitem;
                if (backPage == PageName.HouseholdAccountsSCStatisticsPage)
                {
                    navigationitem = new HouseholdAccountsNavigationItem(CurrentBalancetype, CurrentScategory, CurrentDate, CurrentRange);
                }
                else if (backPage == PageName.HouseholdAccountsDCHistoryPage)
                {
                    navigationitem = new HouseholdAccountsNavigationItem(CurrentBalancetype, CurrentScategory, CurrentDcategory, CurrentDate, CurrentRange);
                }
                else
                {
                    navigationitem = new HouseholdAccountsNavigationItem(CurrentDate, CurrentRange);
                }

                var navigationparameter = HouseholdAccountsNavigationItem.CreateNavigationParameter(backPage, navigationitem);
                var pagename = String.Format("/RootPage/NavigationPage/{0}", backPage);

                var result = await _pageDialogService.DisplayAlertAsync("確認", "入力をキャンセルし画面を変更します。よろしいですか？", "はい", "いいえ");
                if (result) await _navigationservice.NavigateAsync(pagename, navigationparameter);
            });
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
            disposable.Dispose();
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            backPage = (PageName)parameters["BackPage"];

            /* アイテム追加ボタンで遷移してきた時の処理 */
            if (parameters.ContainsKey(InputKey))
            {
                NavigatedItem = (HouseholdAccountsNavigationItem)parameters[InputKey];
                this.CurrentDate = NavigatedItem.CurrentDate;
                this.CurrentRange = NavigatedItem.CurrentRange;
                if(backPage == PageName.HouseholdAccountsSCStatisticsPage)
                {
                    CurrentBalancetype = NavigatedItem.CurrentBalanceType;
                    CurrentScategory = NavigatedItem.CurrentSCategory;
                }
                if(backPage == PageName.HouseholdAccountsDCHistoryPage)
                {
                    CurrentBalancetype = NavigatedItem.CurrentBalanceType;
                    CurrentScategory = NavigatedItem.CurrentSCategory;
                    CurrentDcategory = NavigatedItem.CurrentDCategory;
                }
                
                Date.Value = CurrentDate;
            }

            /* 編集ボタンで遷移してきた時の処理 */
            else if (parameters.ContainsKey(EditKey))
            {
                NavigatedItem = (HouseholdAccountsNavigationItem)parameters[EditKey];
                this.CurrentDate = NavigatedItem.CurrentDate;
                this.CurrentRange = NavigatedItem.CurrentRange;
                Date.Value = CurrentDate;

                VMHouseholdAccountsItem vmitem = new VMHouseholdAccountsItem(_householdaccount.SelectedItem);
                HouseholdAccountsItem item = _householdaccount.SelectedItem;
                Regex re = new Regex(@"[^0-9]");
                ID = vmitem.ID;
                Name.Value = vmitem.Name;
                Price.Value = re.Replace(vmitem.Price, "");
                Date.Value = _householdaccount.SelectedItem.Date;
                IsOutgoing.Value = _householdaccount.SelectedItem.IsOutGoings;
                UpdateScategory(IsOutgoing.Value);
                foreach (HouseholdAccountsSCategoryItem x in ScategoryNames)
                {
                    if (x.ScategoryData == item.SCategory)
                    {
                        SelectedScategory.Value = x;
                        break;
                    }
                }
                foreach (HouseholdAccountsDcategoryItem x in DcategoryNames)
                {
                    if (x.DcategoryData == item.DCategory)
                    {
                        SelectedDcategory.Value = x;
                        break;
                    }
                }
                foreach (HouseholdAccountsStorageTypeItem x in StorageNames)
                {
                    if (x.StorageTypeData == item.StorageType)
                    {
                        SelectedStorageType.Value = x;
                        break;
                    }
                }
            }

            /* カレンダーのアイテム追加ボタンで遷移してきた時の処理 */
            else if (parameters.ContainsKey(CalendarKey))
            {
                NavigatedItem = (HouseholdAccountsNavigationItem)parameters[CalendarKey];
                this.CurrentDate = NavigatedItem.CurrentDate;
                this.CurrentRange = Range.Day;
                Date.Value = CurrentDate;
            }
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
        }

        /* 収支が変更されたときにScategoryを変更する */
        private void UpdateScategory(bool isoutgoing)
        {
            if (isoutgoing)
            {
                ScategoryNames.Clear();
                foreach (HouseholdAccountsSCategoryItem x in OutgoingScategoryNames)
                {
                    ScategoryNames.Add(x);
                }
                SelectedScategory.Value = ScategoryNames[0];
            }
            else
            {
                ScategoryNames.Clear();
                foreach(HouseholdAccountsSCategoryItem x in IncomeScategoryNames)
                {
                    ScategoryNames.Add(x);
                }
                SelectedScategory.Value = ScategoryNames[0];
            }
        }

        /* 収支・Scategoryが変更されたときDcategoryを変更する */
        private void UpdateDcategory(SCategorys sc)
        {
            var startpoint = _householdaccount.ScToDcStart(sc);
            var endpoint = _householdaccount.ScToDcEnd(sc);

            DcategoryNames.Clear();
            for (int i = startpoint + 1; i < endpoint; i++)
            {
                var dcname = Enum.GetName(typeof(DCategorys), i);
                var dcdata = (DCategorys)Enum.ToObject(typeof(DCategorys), i);
                var item = new HouseholdAccountsDcategoryItem(dcname, dcdata);

                DcategoryNames.Add(item);
            }
            SelectedDcategory.Value = DcategoryNames[0];
        }

        /* 購読解除 */
        public void Dispose()
        {
            disposable.Dispose();
        }

    }
}
