﻿using System;
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



        /* グラフ */
        public ObservableCollection<HouseholdaccountPieSliceItem> PieSlice { get; private set; }
        public ObservableCollection<HouseholdaccountLegendItem> Legend { get; private set; }

        /* 全体の統計(メイン画面) */
        private string _totalIncome;
        public string TotalIncome
        {
            get { return this._totalIncome; }
            set { this.SetProperty(ref this._totalIncome, value); }
        }
        private string _totalOutgoing;
        public string TotalOutgoing
        {
            get { return this._totalOutgoing; }
            set { this.SetProperty(ref this._totalOutgoing, value); }
        }
        private string _difference;
        public string Difference
        {
            get { return this._difference; }
            set { this.SetProperty(ref this._difference, value); }
        }
        public ObservableCollection<HouseholdaccountScStatisticsItem> SIncomes { get; private set; }       //各概略カテゴリーの収入の合計値を格納
        public ObservableCollection<HouseholdaccountScStatisticsItem> SOutgoings { get; private set; }         //各概略カテゴリーの支出の合計値を格納

        /* 概略カテゴリーの統計画面用 */
        private string _sCategoryTotal;
        public string SCategoryTotal
        {
            get { return this._sCategoryTotal; }
            set { this.SetProperty(ref this._sCategoryTotal, value); }
        }
        public ObservableCollection<HouseholdaccountDcStatisticsItem> ScategoryItems { get; private set; }

        /* 履歴画面用*/
        public ObservableCollection<HouseHoldAccountsItem> DisplayHouseholdaccountList { get; private set; }

        /* 残高画面 */
        public ObservableCollection<HouseholdaccountBalanceItem> Balances { get; private set; }
        private List<HouseholdaccountBalanceItem> TempBalances { get; set; } = new List<HouseholdaccountBalanceItem>();
        private string _totalbalance;
        public string TotalBalance
        {
            get { return this._totalbalance; }
            set { this.SetProperty(ref this._totalbalance, value); }
        }

        /* コンストラクタ */
        public HouseHoldAccounts()
        {
            allHouseHoldAccounts = new List<HouseHoldAccountsItem>();
            SIncomes = new ObservableCollection<HouseholdaccountScStatisticsItem>();
            SOutgoings = new ObservableCollection<HouseholdaccountScStatisticsItem>();
            PieSlice = new ObservableCollection<HouseholdaccountPieSliceItem>();
            Balances = new ObservableCollection<HouseholdaccountBalanceItem>()
            {
                new HouseholdaccountBalanceItem
                {
                    St = StorageTypes.財布,
                    Price = 0,
                    //Image = Imagesource.FromFile(".png")
                },
                new HouseholdaccountBalanceItem
                {
                    St = StorageTypes.クレジットカード,
                    Price = 0,
                    //Image = Imagesource.FromFile(".png")
                },
                new HouseholdaccountBalanceItem
                {
                    St = StorageTypes.貯金,
                    Price = 0,
                    //Image = Imagesource.FromFile(".png")
                },
                new HouseholdaccountBalanceItem
                {
                    St = StorageTypes.銀行,
                    Price = 0,
                    //Image = Imagesource.FromFile(".png")
                },
                new HouseholdaccountBalanceItem
                {
                    St = StorageTypes.その他,
                    Price = 0,
                    //Image = Imagesource.FromFile(".png")
                }
            };
            Legend = new ObservableCollection<HouseholdaccountLegendItem>();
            ScategoryItems = new ObservableCollection<HouseholdaccountDcStatisticsItem>();
            DisplayHouseholdaccountList = new ObservableCollection<HouseHoldAccountsItem>();
        }

        /* アイテム追加 */
        public void AddHouseHoldAccountsItem(string name, int price, DateTime date, DCategorys detailcategory, SCategorys summarycategory, StorageTypes storagetype, bool isoutgoings)
        {
            HouseHoldAccountsItem item = new HouseHoldAccountsItem(IDCount, name, price, date, detailcategory, summarycategory, storagetype, isoutgoings);
            IDCount++;
            allHouseHoldAccounts.Add(item);
            if (isoutgoings)
            {
                price = -price;
            }
            IncrementBalancePrice(storagetype, price);
            SetBalance();
        }

        /* 全体の統計を表示するためのメソッド(main page) */
        public void SetAllStatics(Range r, DateTime date)
        {
            HouseholdaccountScStatisticsItem item;
            int price, ratio;
            SIncomes.Clear();
            SOutgoings.Clear();

            TotalIncome = String.Format("{0}円", CalucAllBalance(r, date, false));
            TotalOutgoing = String.Format("{0}円", CalucAllBalance(r, date, true));
            Difference = String.Format("{0}円", CalucAllDifference(r, date));

            for (int i = (int)SCategorys.start_of_収入 + 1; i < (int)SCategorys.end_of_収入; i++)
            {
                price = CalucSCategory(r, (SCategorys)Enum.ToObject(typeof(SCategorys), i), date);
                ratio = CalucSCategoryRatio(r, (SCategorys)Enum.ToObject(typeof(SCategorys), i), date, false);

                item = new HouseholdaccountScStatisticsItem(BalanceTypes.incomes, (SCategorys)Enum.ToObject(typeof(SCategorys), i), price, ratio);
                SIncomes.Add(item);
            }

            for (int i = (int)SCategorys.start_of_支出 + 1; i < (int)SCategorys.end_of_支出; i++)
            {
                price = CalucSCategory(r, (SCategorys)Enum.ToObject(typeof(SCategorys), i), date);
                ratio = CalucSCategoryRatio(r, (SCategorys)Enum.ToObject(typeof(SCategorys), i), date, true);
                item = new HouseholdaccountScStatisticsItem(BalanceTypes.outgoings, (SCategorys)Enum.ToObject(typeof(SCategorys), i), price, ratio);
                SOutgoings.Add(item);
            }
        }

        /* 全体の統計のグラフを表示するためのメソッド */
        public void SetAllStaticsPie(Range r, BalanceTypes b, DateTime date)
        {
            int i, j;

            /* グラフ用 */
            HouseholdaccountPieSliceItem pieitem;
            string pielabel, piecolor;
            int pievalue;

            /* グラフ凡例用 */
            HouseholdaccountLegendItem legenditem;
            string legendcolor, legendlabel;

            Legend.Clear();
            PieSlice.Clear();

            switch (b)
            {
                case BalanceTypes.incomes:
                    for (j = 1, i = (int)SCategorys.start_of_収入 + 1; i < (int)SCategorys.end_of_収入; i++, j++)
                    {
                        pielabel = Enum.GetName(typeof(SCategorys), i);
                        pievalue = CalucSCategory(r, (SCategorys)Enum.ToObject(typeof(SCategorys), i), date);
                        piecolor = IntToColorPath(j);
                        pieitem = new HouseholdaccountPieSliceItem(pielabel, pievalue, piecolor);
                        PieSlice.Add(pieitem);

                        legendcolor = IntToColorPath(j);
                        legendlabel = Enum.GetName(typeof(SCategorys), i);
                        legenditem = new HouseholdaccountLegendItem(legendcolor, legendlabel);
                        Legend.Add(legenditem);
                    }

                    break;
                case BalanceTypes.outgoings:
                    for (j = 1, i = (int)SCategorys.start_of_支出 + 1; i < (int)SCategorys.end_of_支出; i++, j++)
                    {
                        pielabel = Enum.GetName(typeof(SCategorys), i);
                        pievalue = CalucSCategory(r, (SCategorys)Enum.ToObject(typeof(SCategorys), i), date);
                        piecolor = IntToColorPath(j);
                        pieitem = new HouseholdaccountPieSliceItem(pielabel, pievalue, piecolor);
                        PieSlice.Add(pieitem);

                        legendcolor = IntToColorPath(j);
                        legendlabel = Enum.GetName(typeof(SCategorys), i);
                        legenditem = new HouseholdaccountLegendItem(legendcolor, legendlabel);
                        Legend.Add(legenditem);
                    }
                    break;
                case BalanceTypes.difference:
                    pielabel = "収入";
                    pievalue = CalucAllBalance(r, date, false);
                    piecolor = IntToColorPath(1);
                    pieitem = new HouseholdaccountPieSliceItem(pielabel, pievalue, piecolor);
                    PieSlice.Add(pieitem);

                    pielabel = "支出";
                    pievalue = CalucAllBalance(r, date, true);
                    piecolor = IntToColorPath(2);
                    pieitem = new HouseholdaccountPieSliceItem(pielabel, pievalue, piecolor);
                    PieSlice.Add(pieitem);

                    legendcolor = IntToColorPath(1);
                    legendlabel = "収入";
                    legenditem = new HouseholdaccountLegendItem(legendcolor, legendlabel);
                    Legend.Add(legenditem);

                    legendcolor = IntToColorPath(2);
                    legendlabel = "支出";
                    legenditem = new HouseholdaccountLegendItem(legendcolor, legendlabel);
                    Legend.Add(legenditem);
                    break;
            }

        }

        /* 概要カテゴリーごとの統計を表示するためのメソッド */
        public void SetSCategoryStatics(Range r, BalanceTypes b, DateTime date, SCategorys sc)
        {
            HouseholdaccountDcStatisticsItem item;
            int price, ratio;

            ScategoryItems.Clear();
            ScToDcRange(sc);

            SCategoryTotal = String.Format("{0}円", CalucSCategory(r, sc, date));

            for (int i = StartPoint + 1; i < EndPoint; i++)
            {
                price = CalucDCategory(r, (DCategorys)Enum.ToObject(typeof(DCategorys), i), date);
                ratio = CalucDCategoryRatio(r, (DCategorys)Enum.ToObject(typeof(DCategorys), i), date);
                item = new HouseholdaccountDcStatisticsItem(b, sc, (DCategorys)Enum.ToObject(typeof(DCategorys), i), price, ratio);
                ScategoryItems.Add(item);
            }

        }

        /* 概要カテゴリーの統計のグラフを表示するためのメソッド */
        public void SetSCategoryStatisticsPie(Range r, DateTime date, SCategorys sc)
        {
            int i, j;

            /* グラフ用 */
            HouseholdaccountPieSliceItem pieitem;
            string pielabel, piecolor;
            int pievalue;

            /* グラフ凡例用 */
            HouseholdaccountLegendItem legenditem;
            string legendcolor, legendlabel;

            Legend.Clear();
            PieSlice.Clear();

            ScToDcRange(sc);

            for (j = 1, i = StartPoint + 1; i < EndPoint; i++, j++)
            {
                pielabel = Enum.GetName(typeof(DCategorys), i);
                pievalue = CalucDCategory(r, (DCategorys)Enum.ToObject(typeof(DCategorys), i), date);
                piecolor = IntToColorPath(j);
                pieitem = new HouseholdaccountPieSliceItem(pielabel, pievalue, piecolor);
                PieSlice.Add(pieitem);

                legendcolor = IntToColorPath(j);
                legendlabel = Enum.GetName(typeof(DCategorys), i);
                legenditem = new HouseholdaccountLegendItem(legendcolor, legendlabel);
                Legend.Add(legenditem);
            }
        }

        /* 全履歴を表示するためのメソッド */
        public void SetAllHistory(Range r, DateTime date)
        {
            allHouseHoldAccounts.Sort(HouseHoldAccountsItem.CompareByDate);

            DisplayHouseholdaccountList.Clear();
            switch (r)
            {
                case Range.Day:
                    foreach (HouseHoldAccountsItem n in allHouseHoldAccounts)
                    {
                        if (n.Date == date)
                            DisplayHouseholdaccountList.Add(n);
                    }
                    break;

                case Range.Month:
                    foreach (HouseHoldAccountsItem n in allHouseHoldAccounts)
                    {
                        if (n.Date.Year == date.Year && n.Date.Month == date.Month)
                            DisplayHouseholdaccountList.Add(n);

                    }
                    break;

                case Range.Year:
                    foreach (HouseHoldAccountsItem n in allHouseHoldAccounts)
                    {
                        if (n.Date.Year == date.Year)
                            DisplayHouseholdaccountList.Add(n);
                    }
                    break;
            }
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
                        if (n.Date == date && (int)n.DCategory > StartPoint && (int)n.DCategory < EndPoint)
                            sum += n.Price;
                    }
                    break;

                case Range.Month:
                    foreach (HouseHoldAccountsItem n in allHouseHoldAccounts)
                    {
                        if (n.Date.Year == date.Year && n.Date.Month == date.Month && (int)n.DCategory > StartPoint && (int)n.DCategory < EndPoint)
                            sum += n.Price;
                    }
                    break;

                case Range.Year:
                    foreach (HouseHoldAccountsItem n in allHouseHoldAccounts)
                    {
                        if (n.Date.Year == date.Year && (int)n.DCategory > StartPoint && (int)n.DCategory < EndPoint)
                            sum += n.Price;
                    }
                    break;

            }
            return sum;
        }

        /* 概要カテゴリーごとの割合を計算 */
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
                        if (n.Date == date && n.DCategory == dc)
                            sum += n.Price;
                    }
                    break;

                case Range.Month:
                    foreach (HouseHoldAccountsItem n in allHouseHoldAccounts)
                    {
                        if (n.Date.Year == date.Year && n.Date.Month == date.Month && n.DCategory == dc)
                            sum += n.Price;
                    }
                    break;

                case Range.Year:
                    foreach (HouseHoldAccountsItem n in allHouseHoldAccounts)
                    {
                        if (n.Date.Year == date.Year && n.DCategory == dc)
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
            Stotal = CalucSCategory(r, (SCategorys)Enum.ToObject(typeof(SCategorys), scnum), date);

            if (Stotal > 0)
            {
                ratio = 100 * Dtotal / Stotal;
                remain = 100 * Dtotal % Stotal;
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

        /* 詳細カテゴリーごとの履歴を表示するためのメソッド */
        public void SetDCategoryHistory(Range r, DateTime date, DCategorys dc)
        {
            allHouseHoldAccounts.Sort(HouseHoldAccountsItem.CompareByDate);

            DisplayHouseholdaccountList.Clear();
            switch (r)
            {
                case Range.Day:
                    foreach (HouseHoldAccountsItem n in allHouseHoldAccounts)
                    {
                        if (n.Date == date && n.DCategory == dc)
                            DisplayHouseholdaccountList.Add(n);
                    }
                    break;

                case Range.Month:
                    foreach (HouseHoldAccountsItem n in allHouseHoldAccounts)
                    {
                        if (n.Date.Year == date.Year && n.Date.Month == date.Month && n.DCategory == dc)
                            DisplayHouseholdaccountList.Add(n);

                    }
                    break;

                case Range.Year:
                    foreach (HouseHoldAccountsItem n in allHouseHoldAccounts)
                    {
                        if (n.Date.Year == date.Year && n.DCategory == dc)
                            DisplayHouseholdaccountList.Add(n);
                    }
                    break;
            }

        }

        /* 残高を編集するメソッド */
        public void EditHouseholdaccountBalance(StorageTypes st, int price)
        {
            foreach (HouseholdaccountBalanceItem n in Balances)
            {
                if (n.St == st)
                    n.Price = price;
            }
        }

        /* 残高の増減を行うメソッド */
        public void IncrementBalancePrice(StorageTypes st, int price)
        {
            TempBalances.Clear();
            foreach (HouseholdaccountBalanceItem n in Balances)
            {
                if(n.St == st)
                {
                    n.Price += price;
                }
                TempBalances.Add(n);
            }

            Balances.Clear();
            foreach(HouseholdaccountBalanceItem n in TempBalances)
            {
                Balances.Add(n);
            }
        }

        /* 残高を表示するためのメソッド(実質合計残高を求めるだけ) */
        public void SetBalance()
        {
            int sum = 0;
            foreach (HouseholdaccountBalanceItem n in Balances)
                sum += n.Price;
            TotalBalance = String.Format("{0}円",sum);
        }

        /* ScategoryからDCategoryの範囲を求めるメソッド */
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

        /* DcategoryからScategoryを求めるメソッド */
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

        /* カラーパスを指定するメソッド */
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
                    colorpath = "#FFFFD700"; //Gold
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