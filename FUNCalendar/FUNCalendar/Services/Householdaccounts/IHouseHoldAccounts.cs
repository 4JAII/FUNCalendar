using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Prism.Mvvm;
using Xamarin.Forms;
using System.Reactive.Linq;
using System.ComponentModel;

namespace FUNCalendar.Models
{
    public interface IHouseholdAccounts : INotifyPropertyChanged
    {
        /* 全体の統計画面用 */
        string TotalIncome { get; }     //収入の合計
        string TotalOutgoing { get; }   //支出の合計
        string Difference { get; }      //差分値
        ObservableCollection<HouseholdAccountsScStatisticsItem> SIncomes { get; }       //収入の統計データ
        ObservableCollection<HouseholdAccountsScStatisticsItem> SOutgoings { get; }     //支出の統計データ
        void SetAllStatics(Range r, DateTime date);                         //全体の統計を表示するためのメソッド
        void SetAllStaticsPie(Range r, BalanceTypes b, DateTime date);      //全体の統計のグラフを表示するためのメソッド

        /* 概要カテゴリーの統計画面用 */
        string SCategoryTotal { get; }  //指定された概要カテゴリーの合計
        ObservableCollection<HouseholdAccountsDcStatisticsItem> ScategoryItems { get; } //指定された概要カテゴリーの統計データ
        void SetSCategoryStatics(Range r, BalanceTypes b, DateTime date, SCategorys sc);    //指定された概要カテゴリーの統計を表示するためのメソッド
        void SetSCategoryStatisticsPie(Range r, DateTime date, SCategorys sc);              //指定された概要カテゴリーの統計のグラフを表示するためのメソッド

        /* 共通・統計画面 */
        ObservableCollection<HouseholdAccountsPieSliceItem> PieSlice { get; }       //グラフのデータ
        ObservableCollection<HouseholdAccountsLegendItem> Legend { get; }           //グラフの凡例


        /* 残高画面用 */
        string TotalBalance { get; }    //残高の合計
        ObservableCollection<HouseholdAccountsBalanceItem> Balances { get; }    //ストレージごとの残高のデータ
        void SetBalance();          //残高を表示するためのメソッド

        /* 履歴画面用 */
        ObservableCollection<HouseholdAccountsItem> DisplayHouseholdaccountList { get; }        //表示する履歴のリスト
        void SetAllHistory(Range r, DateTime date);                                             //全履歴を表示するためのメソッド
        void SetDCategoryHistory(Range r, DateTime date, DCategorys dc);                        //指定された詳細カテゴリーの履歴を表示するためのメソッド
        


        /* アイテム追加・編集・削除用 */
        HouseholdAccountsItem SelectedItem { get; }                                                             //削除するアイテム
        HouseholdAccountsBalanceItem SelectedBalanceItem { get; }                                               //削除する残高アイテム
        void AddHouseholdAccountsItem(HouseholdAccountsItem item);                                              //アイテムの追加を行うメソッド
        void SetHouseholdAccountsItem(HouseholdAccountsItem item);                                              //削除するアイテムを設定するメソッド
        void SetHouseholdAccountsBalanceItem(HouseholdAccountsBalanceItem item);                                //削除する残高アイテムを設定するメソッド
        void EditHouseholdAccountsItem(HouseholdAccountsItem deleteItem, HouseholdAccountsItem additem);        //アイテムの編集を行うメソッド
        void RemoveHouseholdAccountsItem(HouseholdAccountsItem deleteitem);                                     //アイテムの削除を行うメソッド

        /* その他 */
        int ScToDcStart(SCategorys sc);     //ScategoryからDcategoryの範囲を求めるメソッド
        int ScToDcEnd(SCategorys sc);       //同上

        /* カレンダー用 */
        ObservableCollection<HouseholdAccountsItem> HouseholdAccountsListForCalendar { get; }   //指定された日にちの履歴を格納
        bool DateWithHouseholdAccounts { get; }                  //カレンダー用　指定された日にちのアイテムの有無を格納
        string IncomeForCalendar { get; }                        //指定された月の収入
        string OutgoingForCalendar { get; }                      //指定された月の支出
        void SetDateWithHouseholdAccounts(DateTime date);        //カレンダー用　指定された日にちのアイテムの有無を判定するメソッド
        void SetHouseholdAccountsListForCalendar(DateTime date);        //指定された日にちの履歴を表示するためのメソッド
        void ClearHouseholdAccountsListForCalendar();                   //HouseholdAccountsListを初期化するメソッド
        void SetMonthBalance(DateTime date);            //カレンダー用 指定された月の収支を求めるメソッド


        void UpdateList(List<HouseholdAccountsItem> list);          //リストのアップデート
        
    }
}

