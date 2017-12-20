using Prism.Commands;
using Prism.Mvvm;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using FUNCalendar.Models;
using FUNCalendar.Views;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Binding;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Xamarin.Forms;
using Microsoft.Practices.Unity;
using Prism.Navigation;
using System.Text.RegularExpressions;

namespace FUNCalendar.ViewModels
{
    public class ToDoListRegisterPageViewModel : BindableBase, INavigationAware
    {
        private IToDoList _todoList;
        private INavigationService _navigationService;
        private IPageDialogService _pageDialogService;

        /* ToDoItem登録用 */
        public int ID { get; private set; } = -1;
        [Required(ErrorMessage = "項目名がありません")]
        public ReactiveProperty<string> Name { get; private set; } = new ReactiveProperty<string>();
        [Required(ErrorMessage = "日付がありません")]
        public ReactiveProperty<DateTime> Date { get; private set; } = new ReactiveProperty<DateTime>();
        [Required(ErrorMessage = "優先度がありません")]
        public ReactiveProperty<int> Priority { get; private set; } = new ReactiveProperty<int>();
        private string isCompleted;

        /* 登録・キャンセルするときの処理用 */
        public ReactiveProperty<bool> CanRegister { get; private set; }
        public AsyncReactiveCommand RegisterToDoItemCommand { get; private set; }
        public AsyncReactiveCommand CancelCommand { get; private set; }

        /* エラー時の色 */
        public ReactiveProperty<Color> ErrorColor { get; private set; } = new ReactiveProperty<Color>();


        public ToDoListRegisterPageViewModel(IToDoList todoList, INavigationService navigationService, IPageDialogService pageDialogService)
        {
            /* コンストラクタインジェクションされたインスタンスを保持 */
            this._todoList = todoList;
            this._navigationService = navigationService;
            this._pageDialogService = pageDialogService;
            /* 属性を有効化 */
            Name.SetValidateAttribute(() => this.Name);
            Date.SetValidateAttribute(() => this.Date);
            Priority.SetValidateAttribute(() => this.Priority);

            Date.Value = DateTime.Now;/* とりあえず現在の日付に合わせておく */
            Priority.Value = 3;/* とりあえず3に合わせておく */

            /* 全てにエラーなしなら登録できるようにする */
            CanRegister = new[]
            {
                this.Name.ObserveHasErrors,
                this.Date.ObserveHasErrors,
                this.Priority.ObserveHasErrors,
            }
            .CombineLatestValuesAreAllFalse()
            .ToReactiveProperty<bool>();

            /* 登録して遷移 */
            RegisterToDoItemCommand = CanRegister.ToAsyncReactiveCommand();
            RegisterToDoItemCommand.Subscribe(async () =>
            {
                if (ID != -1)
                {
                    var vmToDoItem = new VMToDoItem(ID, Name.Value, Date.Value, Priority.Value.ToString(), isCompleted);
                    _todoList.EditToDoItem(_todoList.DisplayToDoItem, VMToDoItem.ToToDoItem(vmToDoItem));
                }
                else
                {
                    var todoItem = new ToDoItem { Name = this.Name.Value, Date = Date.Value, Priority = this.Priority.Value, IsCompleted = false };
                    _todoList.AddToDoItem(todoItem);
                }
                await _navigationService.NavigateAsync($"/RootPage/NavigationPage/ToDoListPage");
            });

            /* 登録をキャンセルして遷移(確認もあるよ)  */
            CancelCommand = new AsyncReactiveCommand();
            CancelCommand.Subscribe(async () =>
            {
                var result = await _pageDialogService.DisplayAlertAsync("確認", "入力をキャンセルし画面を変更します。よろしいですか？", "はい", "いいえ");
                if (result) await _navigationService.NavigateAsync($"/RootPage/NavigationPage/ToDoListPage");
            });

            /* 登録できるなら水色,エラーなら灰色 */
            CanRegister.Subscribe(x =>
            {
                ErrorColor.Value = x ? Color.SkyBlue : Color.Gray;
            });
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {

        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            /* 編集目的で遷移してきたならセット */
            if (parameters["CanEdit"] as string != "T") return;
            VMToDoItem vmToDoItem = new VMToDoItem(_todoList.DisplayToDoItem);
            ID = vmToDoItem.ID;
            Name.Value = vmToDoItem.Name;
            Date.Value = _todoList.DisplayToDoItem.Date;
            Priority.Value = int.Parse(vmToDoItem.Priority);
            isCompleted = vmToDoItem.IsCompleted;
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {

        }
    }
}
