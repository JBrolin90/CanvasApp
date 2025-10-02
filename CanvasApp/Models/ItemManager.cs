using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Avalonia.Utilities;

namespace Sweeper.Models;

public class ItemManager<T>
{
    private readonly ObservableCollection<T> items = [];

    public ItemManager()
    {
    }
    public void Add(T item)
    {
        items.Add(item);
    }
    public void Remove(T item)
    {
        items.Remove(item);
    }

    public void Subscribe(NotifyCollectionChangedEventHandler handler)
    {
        items.CollectionChanged += handler;
    }
    public void Unubscribe(NotifyCollectionChangedEventHandler handler)
    {
        items.CollectionChanged -= handler;
    }
}
