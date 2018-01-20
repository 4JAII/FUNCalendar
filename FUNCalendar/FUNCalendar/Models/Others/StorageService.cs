using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FUNCalendar.Services;
using FUNCalendar.Models;
using Prism.Mvvm;

namespace FUNCalendar.Models
{
    public class StorageService : BindableBase, IStorageService
    {
        private bool isInitialized = false;
        private bool hasError;
        public bool HasError
        {
            get { return this.hasError; }
            private set { this.SetProperty(ref this.hasError, value); }
        }
        private IStorage storage;

        public Configuration Config { get; private set; }
        public IWishList WishList { get; private set; }
        public IToDoList ToDoList { get; private set; }
        public IHouseholdAccounts HouseholdAccounts { get; private set; }

        public StorageService()
        {

        }

        public async Task InitializeAsync(IWishList wishList, IToDoList todoList, IHouseholdAccounts householdAccounts)
        {
            if (isInitialized) return;
            WishList = wishList;
            ToDoList = todoList;
            HouseholdAccounts = householdAccounts;
            Config = await Configuration.InitializeAsync();
            if (Config.IsEnableRemoteStorage)
                storage = new RemoteStorage(Config.Username, Config.Password);
            else
                storage = new LocalStorage();
            isInitialized = true;
        }

        /* 問題を解決したあとのコールバック */
        public void AfterResolveError()
        {
            HasError = false;
        }

        public async Task SetConfig(bool isEnableRemoteStorage, string username, string password)
        {
            Config.IsEnableRemoteStorage = isEnableRemoteStorage;
            Config.Username = username;
            Config.Password = password;
            if (Config.IsEnableRemoteStorage)
                storage = new RemoteStorage(Config.Username, Config.Password);
            else
                storage = new LocalStorage();
            await ReadFile();
            await Config.WriteFile();
        }

        public async Task AddItem(WishItem item, bool needsRegister, int priority)
        {
            if (!isInitialized)
            {
                HasError = true;
                return;
            }
            HasError = !await storage.AddItem(item);
            if (HasError) return;
            item.ID = storage.LastAddedWishItemID;

            if (needsRegister)
            {
                string description = $"{item.Name}を買う({item.Price}円)";
                var todoItem = new ToDoItem { Description = description, Date = item.Date, Priority = priority, IsCompleted = false, WishID = item.ID };
                await AddItem(todoItem);
                if (HasError) return;
                item.ToDoID = storage.LastAddedToDoItemID;
            }
            WishList.AddWishItem(item);
        }

        public async Task AddItem(ToDoItem item)
        {
            if (!isInitialized)
            {
                HasError = true;
                return;
            }
            HasError = !await storage.AddItem(item);
            if (HasError) return;
            item.ID = storage.LastAddedToDoItemID;
            ToDoList.AddToDoItem(item);
        }

        public async Task AddItem(HouseholdAccountsItem item)
        {
            if (!isInitialized)
            {
                HasError = true;
                return;
            }
            HasError = !await storage.AddItem(item);
            if (HasError) return;
            item.ID = storage.LastAddedHouseholdAccountsItemID;
            HouseholdAccounts.AddHouseholdAccountsItem(item);
        }



        public async Task DeleteItem(WishItem item, bool needsDelete = false)
        {
            if (!isInitialized)
            {
                HasError = true;
                return;
            }
            HasError = !await storage.DeleteItem(item);
            if (HasError) return;
            WishList.Remove(item);
            if (item.ToDoID != 0)
            {
                if (needsDelete)
                {
                    var todoItem = ToDoList.SortedToDoList.First(x => x.ID == item.ToDoID);
                    todoItem.WishID = 0;
                    await DeleteItem(todoItem, false);
                }
                else
                {
                    var deleteItem = ToDoList.SortedToDoList.First(x => x.ID == item.ToDoID);
                    var addItem = deleteItem;
                    addItem.WishID = 0;
                    await EditItem(deleteItem, addItem);
                }
            }
        }

        public async Task DeleteItem(ToDoItem item, bool needsDelete = false)
        {
            if (!isInitialized)
            {
                HasError = true;
                return;
            }
            HasError = !await storage.DeleteItem(item);
            if (HasError) return;
            ToDoList.Remove(item);
            if (item.WishID != 0)
            {
                if (needsDelete)
                {
                    var wishItem = WishList.SortedWishList.First(x => x.ID == item.WishID);
                    wishItem.ToDoID = 0;
                    await DeleteItem(wishItem);
                }
                else
                {
                    var deleteItem = WishList.SortedWishList.First(x => x.ID == item.WishID);
                    var addItem = deleteItem;
                    addItem.ToDoID = 0;
                    await EditItem(deleteItem, addItem);
                }
            }
        }

        public async Task DeleteItem(HouseholdAccountsItem item)
        {
            if (!isInitialized)
            {
                HasError = true;
                return;
            }
            HasError = !await storage.DeleteItem(item);
            if (HasError) return;
            //HouseholdAccounts.Remove(item);
        }

        public async Task EditItem(WishItem deleteItem, WishItem addItem, bool needsRegister = false, int priority = 0)
        {
            if (!isInitialized)
            {
                HasError = true;
                return;
            }
            HasError = !await storage.EditItem(addItem);
            if (HasError) return;

            string description = $"{addItem.Name}を買う({addItem.Price}円)";
            ToDoItem todoItem;
            if (needsRegister)
            {
                if (deleteItem.ToDoID == 0)/* ToDoに登録してなかったものを登録したいとき */
                {
                    todoItem = new ToDoItem { Description = description, Date = addItem.Date, Priority = priority, IsCompleted = false, WishID = addItem.ID };
                    await AddItem(todoItem);
                    if (HasError) return;
                    addItem.ToDoID = storage.LastAddedToDoItemID;
                }
                else
                {
                    todoItem = new ToDoItem { ID = addItem.ToDoID, Description = description, Date = addItem.Date, Priority = priority, IsCompleted = false, WishID = addItem.ID };
                    await EditItem(ToDoList.SortedToDoList.First(x => x.ID == deleteItem.ToDoID), todoItem);
                }
            }
            else if (!needsRegister && deleteItem.ToDoID != 0)/* ToDoが登録済みかつ登録が消されたとき */
            {
                todoItem = ToDoList.SortedToDoList.First(x => x.ID == deleteItem.ToDoID);
                await DeleteItem(todoItem);
            }
            WishList.EditWishItem(deleteItem, addItem);
        }

        public async Task EditItem(ToDoItem deleteItem, ToDoItem addItem)
        {
            if (!isInitialized)
            {
                HasError = true;
                return;
            }
            HasError = !await storage.EditItem(addItem);
            if (HasError) return;
            ToDoList.EditToDoItem(deleteItem, addItem);
        }

        public async Task EditItem(HouseholdAccountsItem deleteItem, HouseholdAccountsItem addItem)
        {
            if (!isInitialized)
            {
                HasError = true;
                return;
            }
            HasError = !await storage.EditItem(addItem);
            if (HasError) return;
            HouseholdAccounts.EditHouseholdAccountsItem(deleteItem, addItem);
        }

        public async Task EditItem(HouseholdAccountsBalanceItem deleteItem, HouseholdAccountsBalanceItem addItem)
        {
            if (!isInitialized)
            {
                HasError = true;
                return;
            }
            HasError = !await storage.EditItem(addItem);
            if (HasError) return;
            /* バランスアイテムの更新処理 */
        }

        public async Task ReadFile()
        {
            if (!isInitialized)
            {
                HasError = true;
                return;
            }


            WishList.UpdateList(await storage.ReadWishList());
            ToDoList.UpdateList(await storage.ReadToDo());
            /* householdaccountsのlist,balanceの更新処理*/
            // HouseholdAccounts.Update(await storage.ReadHouseholdAccounts,await storage.ReadBalance);
        }

        public async Task CompleteToDo(ToDoItem newTodoItem, bool hasId, bool needsRegister, SCategorys Scategory, DCategorys Dcategory, StorageTypes Storagetype)
        {
            HouseholdAccountsItem NewHouseholdAccountsItem;
            var PreviousTodoItem = newTodoItem;

            /* todoを完了にする */
            newTodoItem.IsCompleted = true;

            if (!isInitialized)
            {
                HasError = true;
                return;
            }
            HasError = !await storage.EditItem(newTodoItem);
            ToDoList.EditToDoItem(PreviousTodoItem, newTodoItem);

            /* 対応するIDのwishitemを購入済みにする hasId == true*/
            if (hasId)
            {
                var PreviousWishItem = WishList.SortedWishList.First(x => x.ID == newTodoItem.WishID);
                var newWishItem = PreviousWishItem;
                newWishItem.IsBought = true;

                if (!isInitialized)
                {
                    HasError = true;
                    return;
                }
                HasError = !await storage.EditItem(newWishItem);
                WishList.EditWishItem(PreviousWishItem, newWishItem);

                /* wishitem -> householdaccountsitemに変換し保存 needsResister == true*/
                if (needsRegister)
                {
                    var name = newWishItem.Name;
                    var price = newWishItem.Price;
                    var date = newWishItem.Date;
                    var scategory = Scategory;
                    var dcategroy = Dcategory;
                    var storagetype = Storagetype;
                    var isoutgoing = true;

                    NewHouseholdAccountsItem = new HouseholdAccountsItem() { Name = name, Price = price, Date = date, SCategory = scategory, DCategory = dcategroy, StorageType = storagetype, IsOutGoings = isoutgoing };
                    if (!isInitialized)
                    {
                        HasError = true;
                        return;
                    }
                    HasError = !await storage.AddItem(NewHouseholdAccountsItem);
                    NewHouseholdAccountsItem.ID = storage.LastAddedHouseholdAccountsItemID;
                    HouseholdAccounts.AddHouseholdAccountsItem(NewHouseholdAccountsItem);
                }

            }
        }



    }
}
