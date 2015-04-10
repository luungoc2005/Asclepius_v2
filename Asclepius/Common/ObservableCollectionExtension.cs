using System.Collections.Specialized;

public class ObservableCollection<T>: System.Collections.ObjectModel.ObservableCollection<T>
{
    public bool HaltUpdate { get; set; }

    protected override void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (!HaltUpdate) base.OnPropertyChanged(e);
    }

    public void AddWithoutUpdate(T item)
    {
        Items.Add(item);
    }

    public void ChangeItem(T item, int index)
    {
        Items[index] = item;
    }

    public void UpdateCollection()
    {
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }
}

