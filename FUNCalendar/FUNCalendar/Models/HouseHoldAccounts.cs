using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Prism.Mvvm;
using Xamarin.Forms;
namespace FUNCalendar.Models
{
    public enum Range
    {
        Day = 1,
        Month,
        Year,
    }
    public class HouseHoldAccounts : BindableBase
    {
        private List<HouseHoldAccountsItem> allHouseHoldAccounts;
        private Func<HouseHoldAccountsItem, HouseHoldAccountsItem, int> selectedSortMethod;
        private int IDCount;

        private int StartPoint = 0;
        private int EndPoint = 0;

        /* 全体の統計(メイン画面) */
        public ObservableCollection<int> Balance{ get; private set; }           //全収入、全支出、差分を格納。0:収入、1:支出、2:差分
        public ObservableCollection<int> SIncomes { get; private set; }          //各概略カテゴリーの収入の合計値を格納
        public ObservableCollection<int> SOutgoings { get; private set;}         //各概略カテゴリーの支出の合計値を格納
        public ObservableCollection<int> SIncomesRatio { get; private set; }     //各概略カテゴリーの収入の割合を格納
        public ObservableCollection<int> SOutgoingsRatio { get; private set; }   //各概略カテゴリーの支出の割合を格納

        /* 概略カテゴリーの統計画面用 */
        public ObservableCollection<int> SBalance { get; private set; }         //概略カテゴリーの収入or支出の合計値を格納
        public ObservableCollection<int> DTotal { get; private set; }         //各詳細カテゴリーの合計値を格納
        public ObservableCollection<int> DTotalRatio { get; private set; }    //各詳細カテゴリーの割合を格納

        /* 詳細画面用 */
        public ObservableCollection<HouseHoldAccountsItem> DisplayHouseHoldAccountsList { get; private set; }
        

        /* コンストラクタ(デバッグ用) */
        public HouseHoldAccounts(List<HouseHoldAccountsItem> list,Range r,DateTime date)
        {
            allHouseHoldAccounts = list;
            Balance = new ObservableCollection<int>();
            SetAllStatics(r,date);
        }

        /* 全体の支出・収入の合計を計算 */
        public int CalucAllBalance(Range r,DateTime date,bool isOutgoings)
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
        public int CalucAllDifference(Range r,DateTime date)
        {
            return CalucAllBalance(r,date,false) - CalucAllBalance(r,date,true);
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

        /* 概略カテゴリーごとの合計を計算 */
        public int CalucSCategory(Range r,SCategorys sc,DateTime date)
        {
            int sum = 0;

            ScToDcRange(sc);

            switch (r)
            {
                case Range.Day:
                    foreach (HouseHoldAccountsItem n in allHouseHoldAccounts)
                    {
                        if (n.Date == date && n.IsOutGoings == true && (int)n.DetailCategory > StartPoint && (int)n.DetailCategory<EndPoint)
                            sum += n.Price;
                    }
                    break;

                case Range.Month:
                    foreach (HouseHoldAccountsItem n in allHouseHoldAccounts)
                    {
                        if (n.Date.Year == date.Year && n.Date.Month == date.Month && n.IsOutGoings == true && (int)n.DetailCategory > StartPoint && (int)n.DetailCategory < EndPoint)
                            sum += n.Price;
                    }
                    break;

                case Range.Year:
                    foreach (HouseHoldAccountsItem n in allHouseHoldAccounts)
                    {
                        if (n.Date.Year == date.Year && n.IsOutGoings == true && (int)n.DetailCategory > StartPoint && (int)n.DetailCategory < EndPoint)
                            sum += n.Price;
                    }
                    break;

            }
            return sum;
        }

        /* 概要カテゴリーごとの支出の割合を計算 */
        public int CalucSCategoryRatio(Range r, SCategorys sc, DateTime date,bool isOutgoings)
        {
            int ratio, remain;
            ratio = CalucSCategory(r, sc, date) / CalucAllBalance(r, date, isOutgoings);
            remain = CalucSCategory(r, sc, date) % CalucAllBalance(r, date, isOutgoings);
            if (remain >= 0.5)
                ratio++;
            return ratio;
        }
        
        /* 詳細カテゴリーごとの合計を計算 */
        public int CalucDCategory(Range r, DCategorys dc,DateTime date)
        {
            int sum = 0;
            switch (r)
            {
                case Range.Day:
                    foreach (HouseHoldAccountsItem n in allHouseHoldAccounts)
                    {
                        if (n.Date == date && n.IsOutGoings == false && n.DetailCategory == dc)
                            sum += n.Price;
                    }
                    break;

                case Range.Month:
                    foreach (HouseHoldAccountsItem n in allHouseHoldAccounts)
                    {
                        if (n.Date.Year == date.Year && n.Date.Month == date.Month && n.IsOutGoings == false && n.DetailCategory == dc)
                            sum += n.Price;
                    }
                    break;

                case Range.Year:
                    foreach (HouseHoldAccountsItem n in allHouseHoldAccounts)
                    {
                        if (n.Date.Year == date.Year && n.IsOutGoings == false && n.DetailCategory == dc)
                            sum += n.Price;
                    }
                    break;
            }

            return sum;

        }

        private int DcToIntSc(DCategorys dc)
        {
            int dcnum,scnum = 0;

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

         /* 詳細カテゴリーごとの割合を計算 */
         public int CalucDCategoryRatio(Range r, DCategorys dc, DateTime date)
        {
            int ratio, remain,scnum;

            scnum = DcToIntSc(dc);

            ratio = CalucDCategory(r, dc, date) / CalucSCategory(r, (SCategorys)scnum, date);
            remain = CalucDCategory(r, dc, date) % CalucSCategory(r, (SCategorys)scnum, date);
            if (remain >= 0.5)
                ratio++;
            return ratio;
        }

        /* 全体の統計を表示するためのメソッド(main page) */
        public void SetAllStatics(Range r,DateTime date)
        {
            int i, j;
            Balance.Clear();
            SIncomes.Clear();
            SOutgoings.Clear();
            SIncomesRatio.Clear();
            SOutgoingsRatio.Clear();

            Balance.Insert(0, CalucAllBalance(r, date, false));
            Balance.Insert(1, CalucAllBalance(r, date, true));
            Balance.Insert(2, CalucAllDifference(r, date));

            for(i= 0,j = (int)SCategorys.start_of_収入 + 1; j < (int)SCategorys.end_of_収入; i++, j++)
                SIncomes.Insert(i,CalucSCategory(r, (SCategorys)j,date));

            for(i = 0, j = (int)SCategorys.start_of_支出+1; j < (int)SCategorys.end_of_支出; i++, j++)
                SOutgoings.Insert(i, CalucSCategory(r, (SCategorys)j, date));

            for(i = 0, j = (int)SCategorys.start_of_収入+1; j < (int)SCategorys.end_of_収入; i++, j++)
                SIncomesRatio.Insert(i, CalucSCategoryRatio(r, (SCategorys)j, date, false));

            for(i = 0, j = (int)SCategorys.start_of_支出+1; j < (int)SCategorys.end_of_支出; i++,j++)
                SOutgoingsRatio.Insert(i, CalucSCategoryRatio(r, (SCategorys)j, date, true));
        }


        /* 概要カテゴリーごとの統計を表示するためのメソッド */
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

        }

        /* 詳細カテゴリーごとの履歴を表示するためのメソッド */
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
                    foreach (HouseHoldAccountsItem n in allHouseHoldAccounts && n.DetailCategory == dc)
                    {
                        if (n.Date.Year == date.Year)
                            DisplayHouseHoldAccountsList.Add(n);
                    }
                    break;
            }

        }

        /* 全履歴を表示するためのメソッド */
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
    }
}