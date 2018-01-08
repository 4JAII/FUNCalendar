using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNCalendar.Models
{
    public enum Range
    {
        Day = 1,
        Month,
        Year,
    }

    public enum SCategorys    //購入品の概略カテゴリーの表
    {
        start_of_支出 = 1,
        食費,
        日用雑貨,
        交通費,
        娯楽費,
        医療費,
        通信費,
        水道_光熱費,
        その他_支出,
        end_of_支出,

        start_of_収入,
        給料,
        投資収入,
        その他_収入,
        end_of_収入
    }
    public enum DCategorys
    {     //購入品の詳細カテゴリーの表
        start_of_食費 = 1,
        食料品,
        朝食,
        昼食,
        夕食,
        その他_食費,
        end_of_食費,

        start_of_日用雑貨,
        消耗品,
        子供関連,
        ペット関連,
        その他_日用雑貨,
        end_of_日用雑貨,

        start_of_交通費,
        電車,
        タクシー,
        バス,
        飛行機,
        その他_交通費,
        end_of_交通費,

        start_of_娯楽費,
        レジャー,
        イベント,
        映画,
        音楽,
        漫画,
        書籍,
        ゲーム,
        そのた_娯楽費,
        end_of_娯楽費,

        start_of_医療費,
        病院代,
        薬代,
        生命保険,
        医療保険,
        その他_医療費,
        end_of_医療費,

        start_of_通信費,
        携帯電話料金,
        固定電話料金,
        インターネット関連,
        テレビ受信料,
        宅配便,
        切手_はがき,
        その他_通信費,
        end_of_通信費,

        start_of_水道_光熱費,
        水道料金,
        電気料金,
        ガス料金,
        その他_水道_光熱費,
        end_of_水道_光熱費,

        start_of_その他_支出,
        仕送り,
        お小遣い,
        使途不明金,
        立替金,
        その他_支出,
        end_of_その他_支出,

        start_of_給料,
        給料,
        ボーナス,
        残業代,
        出張旅費,
        その他_給料,
        end_of_給料,

        start_of_投資収入,
        受取利息,
        配当金,
        売却損益,
        その他_投資収入,
        end_of_投資収入,

        start_of_その他_収入,
        健康保険給付,
        懸賞金,
        サイドビジネス,
        児童手当,
        受贈品,
        所得税還付金,
        その他_収入,
        end_of_その他_収入
    }
    public enum StorageTypes        //お金の所在地の表
    {
        財布,
        貯金,
        銀行,
        クレジットカード,
        その他
    }

    public enum BalanceTypes
    {
        incomes = 1,
        outgoings,
        difference
    }

}
