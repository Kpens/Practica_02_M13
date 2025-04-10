using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace Gestio_Botiga_Calcat.model
{
    class ExtendedObservableCollection
    {
    }

    public  class ExtendedObservableCollection<T> : ObservableCollection<T> where T : INotifyPropertyChanged
    {
        private IEnumerable<Prod_select> enumerable;

        public event PropertyChangedEventHandler? ListChanged;

        public ExtendedObservableCollection(List<T> list)
        {
            foreach (var item in list)
            {
                item.PropertyChanged += ItemPropertyChanged;
            }
            foreach (var item in list) Add(item);
        }

        public ExtendedObservableCollection(IEnumerable<T> enumerable)
        {
            foreach (var item in enumerable)
            {
                item.PropertyChanged += ItemPropertyChanged;
            }
            foreach (var item in enumerable)
            {
                 Add(item);
            }
        }

        public ExtendedObservableCollection()
        {
        }

        protected override void ClearItems()
        {
            foreach (var item in Items) item.PropertyChanged -= ItemPropertyChanged;
            base.ClearItems();
        }

        private void ItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            ListChanged?.Invoke(sender, e);
        }

        protected override void InsertItem(int index, T item)
        {
            item.PropertyChanged += ItemPropertyChanged;
            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            this[index].PropertyChanged -= ItemPropertyChanged;
            base.RemoveItem(index);
        }

        protected override void SetItem(int index, T item)
        {
            this[index].PropertyChanged -= ItemPropertyChanged;
            item.PropertyChanged += ItemPropertyChanged;
            base.SetItem(index, item);
        }

    }
}
