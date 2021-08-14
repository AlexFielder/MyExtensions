using System.Linq;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace DumpiLogicRules
{
    public class DirtyCollection<T> : ObservableCollection<T> where T : INotifyPropertyChanged
    {
        private bool isDirty = false;

        public bool IsDirty
        {
            get { return this.isDirty; }
        }

        public void Clean()
        {
            this.isDirty = false;
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            // We aren't concerned with how the collection changed, just that it did.
            this.isDirty = true;

            // But we do need to add the handlers to detect property changes on each item.
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    this.AddPropertyChanged(e.NewItems);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    this.RemovePropertyChanged(e.OldItems);
                    break;

                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Reset:
                    this.RemovePropertyChanged(e.OldItems);
                    this.AddPropertyChanged(e.NewItems);
                    break;
            }

            base.OnCollectionChanged(e);
        }

        private void AddPropertyChanged(IEnumerable items)
        {
            if (items != null)
            {
                foreach (var obj in items.OfType<INotifyPropertyChanged>())
                {
                    obj.PropertyChanged += OnItemPropertyChanged;
                }
            }
        }

        private void RemovePropertyChanged(IEnumerable items)
        {
            if (items != null)
            {
                foreach (var obj in items.OfType<INotifyPropertyChanged>())
                {
                    obj.PropertyChanged -= OnItemPropertyChanged;
                }
            }
        }

        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // A property of a contained item has changed.
            this.isDirty = true;
        }
    }
    //public class DirtyCollection<T> : ObservableCollection<T> where T : INotifyPropertyChanged
    //{

    //    private bool m_isDirty = false;
    //    public bool IsDirty
    //    {
    //        get { return m_isDirty; }
    //    }

    //    public void Clean()
    //    {
    //        m_isDirty = false;
    //    }

    //    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    //    {
    //        // We aren't concerned with how the collection changed, just that it did.
    //        m_isDirty = true;

    //        // But we do need to add the handlers to detect property changes on each item.
    //        switch (e.Action)
    //        {
    //            case NotifyCollectionChangedAction.Add:
    //                AddPropertyChanged(e.NewItems);
    //                break; // TODO: might not be correct. Was : Exit Select


    //                break;
    //            case NotifyCollectionChangedAction.Remove:
    //                RemovePropertyChanged(e.OldItems);
    //                break; // TODO: might not be correct. Was : Exit Select


    //                break;
    //            case NotifyCollectionChangedAction.Replace:
    //            case NotifyCollectionChangedAction.Reset:
    //                RemovePropertyChanged(e.OldItems);
    //                AddPropertyChanged(e.NewItems);
    //                break; // TODO: might not be correct. Was : Exit Select

    //                break;
    //        }

    //        base.OnCollectionChanged(e);
    //    }

    //    private void AddPropertyChanged(IEnumerable items)
    //    {
    //        if (items != null)
    //        {
    //            foreach (INotifyPropertyChanged obj in items.OfType<INotifyPropertyChanged>())
    //            {
    //                obj.PropertyChanged += OnItemPropertyChanged;
    //            }
    //        }
    //    }

    //    private void RemovePropertyChanged(IEnumerable items)
    //    {
    //        if (items != null)
    //        {
    //            foreach (INotifyPropertyChanged obj in items.OfType<INotifyPropertyChanged>())
    //            {
    //                obj.PropertyChanged -= OnItemPropertyChanged;
    //            }
    //        }
    //    }

    //    private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
    //    {
    //        // A property of a contained item has changed.
    //        if (ItemPropertyChanged != null)
    //        {
    //            ItemPropertyChanged((RuleType)sender, e);
    //        }
    //        m_isDirty = true;
    //    }
    //    //This is the event is what we will use to expose the property change to our code
    //    public event ItemPropertyChangedEventHandler ItemPropertyChanged;
    //    public delegate void ItemPropertyChangedEventHandler(RuleType sender, System.ComponentModel.PropertyChangedEventArgs e);

    //}
}
