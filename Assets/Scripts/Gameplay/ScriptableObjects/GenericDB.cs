using System;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects
{
    /// <summary>
    /// Base class for generic items keyed by string id's
    /// </summary>
    public interface GenericItem
    {
        string ItemId { get; }
    }

    /// <summary>
    /// Generic data store for any item keyed by string id's
    /// </summary>
    /// <typeparam name="T">Model class, implementing <see cref="GenericItem"/></typeparam>
    public class GenericDB<T> : ScriptableObject
        where T : GenericItem
    {
        [SerializeField] private T[] items;
        private Dictionary<string, T> itemsDictionary;

        public T Get(string key)
        {
            if (itemsDictionary == null)
            {
                InitDictionary();
            }

            if (itemsDictionary.ContainsKey(key))
            {
                return itemsDictionary[key];
            }

            throw new IndexOutOfRangeException($"Item with id {key} is not registered!");
        }

        public T[] GetAll()
        {
            return items;
        }

        public int Count()
        {
            return items.Length;
        }

        private void InitDictionary()
        {
            itemsDictionary = new Dictionary<string, T>();

            foreach (T hint in items)
            {
                itemsDictionary.Add(hint.ItemId, hint);
            }
        }
    }
}