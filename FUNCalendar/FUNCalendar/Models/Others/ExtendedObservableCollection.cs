using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNCalendar.Models
{
    public class ExtendedObservableCollection<T> : ObservableCollection<T>
    {

        public void Replace(IEnumerable<T> collection)
        {
            Items.Clear();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            var count = collection.Count();
            for (int i = 0; i < count; i++)
            {
                var x = collection.ElementAt(i);
                Items.Add(x);
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,x,i));
            }

        }
    }
}

