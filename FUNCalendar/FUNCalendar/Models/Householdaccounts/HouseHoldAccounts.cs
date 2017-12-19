using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Prism.Mvvm;
using Xamarin.Forms;
using System.Reactive.Linq;
using OxyPlot;
using OxyPlot.Xamarin.Forms;
using OxyPlot.Series;


namespace FUNCalendar.Models
{
    public class HouseHoldAccounts : BindableBase, IHouseHoldAccounts
    {
        private List<HouseHoldAccountsItem> allHouseHoldAccounts;
        private static int idCount;
        public int IDCount { get { return idCount; } private set { idCount = value; } }


        private int StartPoint = 0;
        private int EndPoint = 0;

        /* 全体の統計(メイン画面) */
        private string _settotalincome;
        private string _settotaloutgoing;
        private string _setdifference;
        public ObservableCollection<HouseholdaccountScStatisticsItem> SetSIncomes { get; private set; }       //各概略カテゴリーの収入の合計値を格納
        public ObservableCollection<HouseholdaccountScStatisticsItem> SetSOutgoings { get; private set; }         //各概略カテゴリーの支出の合計値を格納

        /* グラフ */
        public ObservableCollection<HouseholdaccountPieSliceItem> SetPieSlice { get; private set; }
        /* 概略カテゴリーの統計画面用 
        public ObservableCollection<int> SBalance { get; private set; }         //概略カテゴリーの収入or支出の合計値を格納
        public ObservableCollection<int> DTotal { get; private set; }         //各詳細カテゴリーの合計値を格納
        public ObservableCollection<int> DTotalRatio { get; private set; }    //各詳細カテゴリーの割合を格納
        */
        /* 詳細画面用 
        public ObservableCollection<HouseHoldAccountsItem> DisplayHouseHoldAccountsList { get; private set; }
        */

        /* 残高画面 */
        public ObservableCollection<HouseholdaccountBalanceITem> SetBalances { get; private set; } = new ObservableCollection<HouseholdaccountBalanceITem>()
        {
                new HouseholdaccountBalanceITem
                {
                    St = StorageTypes.財布,
                    Price = 0,
                    //Image = Imagesource.FromFile(".png")
                },
                new HouseholdaccountBalanceITem
                {
                    St = StorageTypes.クレジットカード,
                    Price = 0,
                    //Image = Imagesource.FromFile(".png")
                },
                new HouseholdaccountBalanceITem
                {
                    St = StorageTypes.貯金,
                    Price = 0,
                    //Image = Imagesource.FromFile(".png")
                },
                new HouseholdaccountBalanceITem
                {
                    St = StorageTypes.銀行,
                    Price = 0,
                    //Image = Imagesource.FromFile(".png")
                },
                new HouseholdaccountBalanceITem
                {
                    St = StorageTypes.その他,
                    Price = 0,
                    //Image = Imagesource.FromFile(".png")
                }
        };
        public int TotalBalance { get; private set; }


        public string SetTotalIncome
        {
            get { return this._settotalincome; }
            set { this.SetProperty(ref this._settotalincome, value); }
        }
        public string SetTotalOutgoing
        {
            get { return this._settotaloutgoing; }
            set { this.SetProperty(ref this._settotaloutgoing, value); }
        }
        public string SetDifference
        {
            get { return this._setdifference; }
            set { this.SetProperty(ref this._setdifference, value); }
        }

        public HouseHoldAccounts()
        {
            allHouseHoldAccounts = new List<HouseHoldAccountsItem>();
            SetSIncomes = new ObservableCollection<HouseholdaccountScStatisticsItem>();
            SetSOutgoings = new ObservableCollection<HouseholdaccountScStatisticsItem>();
            SetPieSlice = new ObservableCollection<HouseholdaccountPieSliceItem>();

        }

        public void AddHouseHoldAccountsItem(string name, int count, int price, DateTime date, DCategorys detailcategory, SCategorys summarycategory, StorageTypes storagetype, bool isoutgoings)
        {
            HouseHoldAccountsItem item = new HouseHoldAccountsItem(IDCount, name, count, price, date, detailcategory, summarycategory, storagetype, isoutgoings);
            IDCount++;
            allHouseHoldAccounts.Add(item);
        }

        /* 全体の支出・収入の合計を計算 */
        public int CalucAllBalance(Range r, DateTime date, bool isOutgoings)
        {
            int sum = 0;
            switch (r)
            {
                case Range.Day:
                    foreach (HouseHoldAccountsItem n in allHouseHoldAccounts)
                    {
                        if (n.Date == date && n.IsOutGoings == isOutgoings)
                            sum += n.Price;
                    }
                    break;

                case Range.Month:
                    foreach (HouseHoldAccountsItem n in allHouseHoldAccounts)
                    {
                        if (n.Date.Year == date.Year && n.Date.Month == date.Month && n.IsOutGoings == isOutgoings)
                            sum += n.Price;
                    }
                    break;

                case Range.Year:
                    foreach (HouseHoldAccountsItem n in allHouseHoldAccounts)
                    {
                        if (n.Date.Year == date.Year && n.IsOutGoings == isOutgoings)
                            sum += n.Price;
                    }
                    break;
            }

            return sum;
        }


        /* 全体の差分を計算 */
        public int CalucAllDifference(Range r, DateTime date)
        {
            return CalucAllBalance(r, date, false) - CalucAllBalance(r, date, true);
        }

        /* 概略カテゴリーごとの合計を計算 */
        public int CalucSCategory(Range r, SCategorys sc, DateTime date)
        {
            int sum = 0;

            ScToDcRange(sc);

            switch (r)
            {
                case Range.Day:
                    foreach (HouseHoldAccountsItem n in allHouseHoldAccounts)
                    {
                        if (n.Date == date && (int)n.DetailCategory > StartPoint && (int)n.DetailCategory < EndPoint)
                            sum += n.Price;
                    }
                    break;

                case Range.Month:
                    foreach (HouseHoldAccountsItem n in allHouseHoldAccounts)
                    {
                        if (n.Date.Year == date.Year && n.Date.Month == date.Month && (int)n.DetailCategory > StartPoint && (int)n.DetailCategory < EndPoint)
                            sum += n.Price;
                    }
                    break;

                case Range.Year:
                    foreach (HouseHoldAccountsItem n in allHouseHoldAccounts)
                    {
                        if (n.Date.Year == date.Year && (int)n.DetailCategory > StartPoint && (int)n.DetailCategory < EndPoint)
                            sum += n.Price;
                    }
                    break;

            }
            return sum;
        }

        /* 概要カテゴリーごとの支出の割合を計算 */
        public int CalucSCategoryRatio(Range r, SCategorys sc, DateTime date, bool isOutgoings)
        {
            int ratio;
            double remain;
            int Total, Stotal;
            Stotal = CalucSCategory(r, sc, date);
            Total = CalucAllBalance(r, date, isOutgoings);
            if (Total > 0)
            {
                ratio = 100 * Stotal / Total;
                remain = 100 * Stotal % Total;
                if (remain >= 0.5 * Total)
                    ratio++;
            }
            else
            {
                ratio = 0;
                remain = 0;
            }
            return ratio;
        }

        /* 詳細カテゴリーごとの合計を計算 */
        public int CalucDCategory(Range r, DCategorys dc, DateTime date)
        {
            int sum = 0;
            switch (r)
            {
                case Range.Day:
                    foreach (HouseHoldAccountsItem n in allHouseHoldAccounts)
                    {
                        if (n.Date == date && n.DetailCategory == dc)
                            sum += n.Price;
                    }
                    break;

                case Range.Month:
                    foreach (HouseHoldAccountsItem n in allHouseHoldAccounts)
                    {
                        if (n.Date.Year == date.Year && n.Date.Month == date.Month && n.DetailCategory == dc)
                            sum += n.Price;
                    }
                    break;

                case Range.Year:
                    foreach (HouseHoldAccountsItem n in allHouseHoldAccounts)
                    {
                        if (n.Date.Year == date.Year && n.DetailCategory == dc)
                            sum += n.Price;
                    }
                    break;
            }

            return sum;

        }

        /* 詳細カテゴリーごとの割合を計算 */
        public int CalucDCategoryRatio(Range r, DCategorys dc, DateTime date)
        {
            int ratio, scnum;
            double remain;
            int Dtotal, Stotal;

            scnum = DcToIntSc(dc);
            Dtotal = CalucDCategory(r, dc, date);
            Stotal = CalucSCategory(r, (SCategorys)scnum, date);

            if (Stotal > 0)
            {
                ratio = Dtotal / Stotal;
                remain = Dtotal % Stotal;
                if (remain >= 0.5 * Stotal)
                    ratio++;
            }
            else
            {
                ratio = 0;
                remain = 0;
            }
            return ratio;
        }

        /* 全体の統計を表示するためのメソッド(main page) */
        public void SetAllStatics(Range r, DateTime date)
        {
            HouseholdaccountScStatisticsItem item;
            int price, ratio;
            SetSIncomes.Clear();
            SetSOutgoings.Clear();

            SetTotalIncome = String.Format("{0}円", CalucAllBalance(r, date, false));
            SetTotalOutgoing = String.Format("{0}円", CalucAllBalance(r, date, true));
            SetDifference = String.Format("{0}円", CalucAllDifference(r, date));

            for (int i = (int)SCategorys.start_of_収入 + 1; i < (int)SCategorys.end_of_収入; i++)
            {
                price = CalucSCategory(r, (SCategorys)Enum.ToObject(typeof(SCategorys), i), date);
                ratio = CalucSCategoryRatio(r, (SCategorys)Enum.ToObject(typeof(SCategorys), i), date, false);

                item = new HouseholdaccountScStatisticsItem(Balancetype.incomes, (SCategorys)Enum.ToObject(typeof(SCategorys), i), price, ratio);
                SetSIncomes.Add(item);
            }

            for (int i = (int)SCategorys.start_of_支出 + 1; i < (int)SCategorys.end_of_支出; i++)
            {
                price = CalucSCategory(r, (SCategorys)Enum.ToObject(typeof(SCategorys), i), date);
                ratio = CalucSCategoryRatio(r, (SCategorys)Enum.ToObject(typeof(SCategorys), i), date, true);
                item = new HouseholdaccountScStatisticsItem(Balancetype.outgoings, (SCategorys)Enum.ToObject(typeof(SCategorys), i), price, ratio);
                SetSOutgoings.Add(item);
            }
        }
        /* グラフを表示するためのメソッド */
        public void SetAllStaticsPie(Range r, Balancetype b, DateTime date)
        {
            int i, j;
            HouseholdaccountPieSliceItem item;
            string label, color;
            int value;
            SetPieSlice.Clear();

            switch (b)
            {
                case Balancetype.incomes:
                    for (j = 1, i = (int)SCategorys.start_of_収入 + 1; i < (int)SCategorys.end_of_収入; i++, j++)
                    {
                        label = Enum.GetName(typeof(SCategorys), i);
                        value = CalucSCategory(r, (SCategorys)Enum.ToObject(typeof(SCategorys), i), date);
                        color = IntToColorPath(j);
                        item = new HouseholdaccountPieSliceItem(label, value, color);
                        SetPieSlice.Add(item);
                    }
                    break;
                case Balancetype.outgoings:
                    for (j = 1, i = (int)SCategorys.start_of_支出 + 1; i < (int)SCategorys.end_of_支出; i++, j++)
                    {
                        label = Enum.GetName(typeof(SCategorys), i);
                        value = CalucSCategory(r, (SCategorys)Enum.ToObject(typeof(SCategorys), i), date);
                        color = IntToColorPath(j);
                        item = new HouseholdaccountPieSliceItem(label, value, color);
                        SetPieSlice.Add(item);
                    }
                    break;
                case Balancetype.difference:
                    label = "収入";
                    value = CalucAllBalance(r, date, false);
                    color = IntToColorPath(1);
                    item = new HouseholdaccountPieSliceItem(label, value, color);
                    SetPieSlice.Add(item);

                    label = "支出";
                    value = CalucAllBalance(r, date, true);
                    color = IntToColorPath(2);
                    item = new HouseholdaccountPieSliceItem(label, value, color);
                    SetPieSlice.Add(item);
                    break;
            }

        }


        /* 概要カテゴリーごとの統計を表示するためのメソッド 
        public void SetSCategoryStatics(Range r,DateTime date,SCategorys sc)
        {
            int i, j;
            SBalance.Clear();
            DTotal.Clear();
            DTotalRatio.Clear();

            ScToDcRange(sc);

            SBalance.Insert(0, CalucSCategory(r, sc, date));

            for (i = 0, j = StartPoint + 1; j < EndPoint; i++, j++)
                DTotal.Insert(i, CalucDCategory(r, (DCategorys)j, date));
            for (i = 0, j = StartPoint + 1; j < EndPoint; i++, j++)
                DTotalRatio.Insert(i, CalucDCategoryRatio(r, (DCategorys)j, date));

        }*/

        /* 詳細カテゴリーごとの履歴を表示するためのメソッド 
        public void SetDCategoryHistory(Range r,DateTime date,DCategorys dc)
        {
            allHouseHoldAccounts.Sort(HouseHoldAccountsItem.CompareByDate);

            DisplayHouseHoldAccountsList.Clear();
            switch (r)
            {
                case Range.Day:
                    foreach (HouseHoldAccountsItem n in allHouseHoldAccounts)
                    {
                        if (n.Date == date && n.DetailCategory == dc)
                            DisplayHouseHoldAccountsList.Add(n);
                    }
                    break;

                case Range.Month:
                    foreach (HouseHoldAccountsItem n in allHouseHoldAccounts)
                    {
                        if (n.Date.Year == date.Year && n.Date.Month == date.Month && n.DetailCategory == dc)
                            DisplayHouseHoldAccountsList.Add(n);

                    }
                    break;

                case Range.Year:
                    foreach (HouseHoldAccountsItem n in allHouseHoldAccounts)
                    {
                        if (n.Date.Year == date.Year && n.DetailCategory == dc)
                            DisplayHouseHoldAccountsList.Add(n);
                    }
                    break;
            }

        }
        */
        /* 全履歴を表示するためのメソッド 
        public void SetAllHistory(Range r, DateTime date)
        {
            allHouseHoldAccounts.Sort(HouseHoldAccountsItem.CompareByDate);

            DisplayHouseHoldAccountsList.Clear();
            switch (r)
            {
                case Range.Day:
                    foreach (HouseHoldAccountsItem n in allHouseHoldAccounts)
                    {
                        if (n.Date == date)
                            DisplayHouseHoldAccountsList.Add(n);
                    }
                    break;

                case Range.Month:
                    foreach (HouseHoldAccountsItem n in allHouseHoldAccounts)
                    {
                        if (n.Date.Year == date.Year && n.Date.Month == date.Month )
                            DisplayHouseHoldAccountsList.Add(n);

                    }
                    break;

                case Range.Year:
                    foreach (HouseHoldAccountsItem n in allHouseHoldAccounts)
                    {
                        if (n.Date.Year == date.Year)
                            DisplayHouseHoldAccountsList.Add(n);
                    }
                    break;
            }
        }
        */

        /* 残高を編集するメソッド */
        public void EditHouseholdaccountBalance(StorageTypes st, int price)
        {
            foreach (HouseholdaccountBalanceITem n in SetBalances)
            {
                if (n.St == st)
                    n.Price = price;
            }
        }

        /* 残高の増減を行うメソッド */
        public void IncrementBalancePrice(StorageTypes st, int price)
        {
            foreach (HouseholdaccountBalanceITem n in SetBalances)
            {
                if (n.St == st)
                    n.Price += price;
            }
        }

        /* 残高を表示するためのメソッド(実質合計残高を求めるだけ) */
        public void SetBalamce()
        {
            int sum = 0;
            foreach (HouseholdaccountBalanceITem n in SetBalances)
                sum += n.Price;
            TotalBalance = sum;
        }

        public void ScToDcRange(SCategorys sc)
        {
            switch (sc)
            {
                case SCategorys.食費:
                    StartPoint = (int)DCategorys.start_of_食費;
                    EndPoint = (int)DCategorys.end_of_食費;
                    break;
                case SCategorys.日用雑貨:
                    StartPoint = (int)DCategorys.start_of_日用雑貨;
                    EndPoint = (int)DCategorys.end_of_日用雑貨;
                    break;
                case SCategorys.交通費:
                    StartPoint = (int)DCategorys.start_of_交通費;
                    EndPoint = (int)DCategorys.end_of_交通費;
                    break;
                case SCategorys.娯楽費:
                    StartPoint = (int)DCategorys.start_of_娯楽費;
                    EndPoint = (int)DCategorys.end_of_娯楽費;
                    break;
                case SCategorys.医療費:
                    StartPoint = (int)DCategorys.start_of_医療費;
                    EndPoint = (int)DCategorys.end_of_医療費;
                    break;
                case SCategorys.通信費:
                    StartPoint = (int)DCategorys.start_of_通信費;
                    EndPoint = (int)DCategorys.end_of_通信費;
                    break;
                case SCategorys.水道_光熱費:
                    StartPoint = (int)DCategorys.start_of_水道_光熱費;
                    EndPoint = (int)DCategorys.end_of_水道_光熱費;
                    break;
                case SCategorys.その他_支出:
                    StartPoint = (int)DCategorys.start_of_その他_支出;
                    EndPoint = (int)DCategorys.end_of_その他_支出;
                    break;
                case SCategorys.給料:
                    StartPoint = (int)DCategorys.start_of_給料;
                    EndPoint = (int)DCategorys.end_of_給料;
                    break;
                case SCategorys.投資収入:
                    StartPoint = (int)DCategorys.start_of_投資収入;
                    EndPoint = (int)DCategorys.end_of_投資収入;
                    break;
                case SCategorys.その他_収入:
                    StartPoint = (int)DCategorys.start_of_その他_収入;
                    EndPoint = (int)DCategorys.end_of_その他_収入;
                    break;
            }

        }

        public int DcToIntSc(DCategorys dc)
        {
            int dcnum, scnum = 0;

            dcnum = (int)dc;

            if (dcnum > (int)DCategorys.start_of_食費 && dcnum < (int)DCategorys.end_of_食費)
                scnum = (int)SCategorys.食費;

            else if (dcnum > (int)DCategorys.start_of_日用雑貨 && dcnum < (int)DCategorys.end_of_日用雑貨)
                scnum = (int)SCategorys.日用雑貨;

            else if (dcnum > (int)DCategorys.start_of_交通費 && dcnum < (int)DCategorys.end_of_交通費)
                scnum = (int)SCategorys.交通費;

            else if (dcnum > (int)DCategorys.start_of_娯楽費 && dcnum < (int)DCategorys.end_of_娯楽費)
                scnum = (int)SCategorys.娯楽費;

            else if (dcnum > (int)DCategorys.start_of_医療費 && dcnum < (int)DCategorys.end_of_医療費)
                scnum = (int)SCategorys.医療費;

            else if (dcnum > (int)DCategorys.start_of_通信費 && dcnum < (int)DCategorys.end_of_通信費)
                scnum = (int)SCategorys.通信費;

            else if (dcnum > (int)DCategorys.start_of_水道_光熱費 && dcnum < (int)DCategorys.end_of_水道_光熱費)
                scnum = (int)SCategorys.水道_光熱費;

            else if (dcnum > (int)DCategorys.start_of_その他_支出 && dcnum < (int)DCategorys.end_of_その他_支出)
                scnum = (int)SCategorys.その他_支出;

            else if (dcnum > (int)DCategorys.start_of_給料 && dcnum < (int)DCategorys.end_of_給料)
                scnum = (int)SCategorys.給料;

            else if (dcnum > (int)DCategorys.start_of_投資収入 && dcnum < (int)DCategorys.end_of_投資収入)
                scnum = (int)SCategorys.投資収入;

            else if (dcnum > (int)DCategorys.start_of_その他_収入 && dcnum < (int)DCategorys.end_of_その他_収入)
                scnum = (int)SCategorys.その他_収入;

            return scnum;
        }

        public string IntToColorPath(int num)
        {
            string colorpath;

            switch (num)
            {
                case 1:
                    colorpath = "#FF32CD32";
                    break;
                case 2:
                    colorpath = "#FF1E90FF";
                    break;
                case 3:
                    colorpath = "#FF9932CC";
                    break;
                case 4:
                    colorpath = "#FFFF6347"; //tomato
                    break;
                case 5:
                    colorpath = "#FFFFA500"; //orange
                    break;
                case 6:
                    colorpath = "#FFA52A2A"; //Brown
                    break;
                case 7:
                    colorpath = "#FFADFF2F"; //GreenYellow
                    break;
                case 8:
                    colorpath = "#FF6B8E23"; //OlieveDrab
                    break;
                case 9:
                    colorpath = "#FF6A5ACD"; //SlateBlue
                    break;
                case 10:
                    colorpath = "#FFFFA07A"; //LightSalmon
                    break;
                case 11:
                    colorpath = "#FFFF00FF"; //Magenta
                    break;
                default:
                    colorpath = "#FF000000";
                    break;
            };

            return colorpath;
        }


    }
}