using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using AiForms.Layouts;
using System.Collections.Specialized;
using System.Collections;
using System.Windows.Input;

namespace FUNCalendar.Controls
{
    public class CalendarLayout : WrapLayout
    {
        public static BindableProperty ItemsSourceProperty =
            BindableProperty.Create(
                nameof(ItemsSource),
                typeof(IEnumerable),
                typeof(CalendarLayout),
                null,
                defaultBindingMode: BindingMode.OneWay,
                propertyChanged: ItemsChanged
            );

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static BindableProperty ItemTemplateProperty =
            BindableProperty.Create(
                nameof(ItemTemplate),
                typeof(DataTemplate),
                typeof(CalendarLayout),
                default(DataTemplate),
                propertyChanged: (bindable, oldValue, newValue) => {
                    var control = (CalendarLayout)bindable;
                    if (newValue != null && control.ItemsSource != null && !control.doneItemSourceChanged)
                    {
                        ItemsChanged(bindable, null, control.ItemsSource);
                    }
                }
            );

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        public static BindableProperty ItemTapCommandProperty =
            BindableProperty.Create(
                nameof(ItemTapCommand),
                typeof(ICommand),
                typeof(CalendarLayout),
                default(ICommand),
                defaultBindingMode: BindingMode.OneWay,
                propertyChanged: ItemTapCommandChanged
            );
        
        public ICommand ItemTapCommand
        {
            get { return (ICommand)GetValue(ItemTapCommandProperty); }
            set { SetValue(ItemTapCommandProperty, value); }
        }

        private bool doneItemSourceChanged = false;

        private static void ItemTapCommandChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (CalendarLayout)bindable;
            if (oldValue != newValue && control.ItemsSource != null)
            {
                UpdateCommand(control);
            }
        }

        private static void ItemsChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (CalendarLayout)bindable;
            if (control.ItemTemplate == null)
            {
                control.doneItemSourceChanged = false;
                return;
            }

            control.doneItemSourceChanged = true;

            IEnumerable newValueAsEnumerable;
            try
            {
                newValueAsEnumerable = newValue as IEnumerable;
            }
            catch (Exception e)
            {
                throw e;
            }

            var oldObservableCollection = oldValue as INotifyCollectionChanged;

            if (oldObservableCollection != null)
            {
                oldObservableCollection.CollectionChanged -= control.OnItemsSourceCollectionChanged;
            }

            var newObservableCollection = newValue as INotifyCollectionChanged;

            if (newObservableCollection != null)
            {
                newObservableCollection.CollectionChanged += control.OnItemsSourceCollectionChanged;
            }

            control.Children.Clear();

            if (newValueAsEnumerable != null)
            {
                foreach (var item in newValueAsEnumerable)
                {
                    var view = CreateChildViewFor(control.ItemTemplate, item, control);
                    control.Children.Add(view);
                }
            }

            if (control.ItemTapCommand != null)
            {
                UpdateCommand(control);
            }

            control.UpdateChildrenLayout();
            control.InvalidateLayout();
        }

        private static void UpdateCommand(CalendarLayout control)
        {
            foreach (var view in control.Children)
            {
                view.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = control.ItemTapCommand,
                    CommandParameter = view.BindingContext,
                });
            }
        }

        private void OnItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var invalidate = false;

            if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                /*
                this.Children.RemoveAt(e.OldStartingIndex);

                var item = e.NewItems[e.NewStartingIndex];
                var view = CreateChildViewFor(this.ItemTemplate, item, this);

                if (ItemTapCommand != null)
                {
                    view.GestureRecognizers.Add(new TapGestureRecognizer
                    {
                        Command = ItemTapCommand,
                        CommandParameter = item,
                    });
                }
                this.Children.Insert(e.NewStartingIndex, view);
                */
                this.Children.Clear();
                if (e.NewItems != null)
                {
                    for (var i = 0; i < e.NewItems.Count; ++i)
                    {
                        var item = e.NewItems[i];
                        var view = CreateChildViewFor(this.ItemTemplate, item, this);
                        

                        if (ItemTapCommand != null)
                        {
                            view.GestureRecognizers.Add(new TapGestureRecognizer
                            {
                                Command = ItemTapCommand,
                                CommandParameter = item,
                            });
                        }

                        this.Children.Insert(i, view);
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (e.NewItems != null)
                {
                    for (var i = 0; i < e.NewItems.Count; ++i)
                    {
                        var item = e.NewItems[i];
                        var view = CreateChildViewFor(this.ItemTemplate, item, this);

                        if (ItemTapCommand != null)
                        {
                            view.GestureRecognizers.Add(new TapGestureRecognizer
                            {
                                Command = ItemTapCommand,
                                CommandParameter = item,
                            });
                        }

                        this.Children.Insert(i + e.NewStartingIndex, view);
                    }
                }
            }

            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                if (e.OldItems != null)
                {
                    this.Children.RemoveAt(e.OldStartingIndex);
                }
            }

            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {

                    this.Children.Clear();

            }

            else
            {
                return;
            }

            if (invalidate)
            {
                this.UpdateChildrenLayout();
                this.InvalidateLayout();
            }

        }

        private View CreateChildViewFor(object item)
        {
            this.ItemTemplate.SetValue(BindableObject.BindingContextProperty, item);
            return (View)this.ItemTemplate.CreateContent();
        }

        private static View CreateChildViewFor(DataTemplate template, object item, BindableObject container)
        {
            var selector = template as DataTemplateSelector;

            if (selector != null)
            {
                template = selector.SelectTemplate(item, container);
            }

            template.SetValue(BindableObject.BindingContextProperty, item);

            return (View)template.CreateContent();
        }
    }

}
