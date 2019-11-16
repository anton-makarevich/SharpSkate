// ==========================================================================
// Copyright (c) 2011-2016, dlg.krakow.pl
// All Rights Reserved
//
// NOTICE: dlg.krakow.pl permits you to use, modify, and distribute this file
// in accordance with the terms of the license agreement accompanying it.
// ==========================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sanet.SmartSkating.Tools.GpxComposer.Models.Gpx
{
    public class GpxProperties
    {
        private class GpxListWrapper<T> : IList<T>
        {
            readonly GpxProperties _properties;
            readonly string _name;
            IList<T> _items;

            public GpxListWrapper(GpxProperties properties, string name)
            {
                _properties = properties;
                _name = name;
                _items = properties.GetObjectProperty<IList<T>>(name);
            }

            public int IndexOf(T item)
            {
                return (_items != null) ? _items.IndexOf(item) : -1;
            }

            public void Insert(int index, T item)
            {
                if (_items == null && index != 0) throw new ArgumentOutOfRangeException();

                if (_items == null)
                {
                    _items = new List<T>();
                    _properties.SetObjectProperty(_name, _items);
                }

                _items.Insert(index, item);
            }

            public void RemoveAt(int index)
            {
                if (_items == null) throw new ArgumentOutOfRangeException();
                _items.RemoveAt(index);
            }

            public T this[int index]
            {
                get
                {
                    if (_items == null) throw new ArgumentOutOfRangeException();
                    return _items[index];
                }
                set
                {
                    if (_items == null) throw new ArgumentOutOfRangeException();
                    _items[index] = value;
                }
            }

            public void Add(T item)
            {
                if (_items == null)
                {
                    _items = new List<T>();
                    _properties.SetObjectProperty(_name, _items);
                }

                _items.Add(item);
            }

            public void Clear()
            {
                if (_items != null)
                {
                    _items.Clear();
                    _items = null;
                }
            }

            public bool Contains(T item)
            {
                return _items != null ? _items.Contains(item) : false;
            }

            public void CopyTo(T[] array, int arrayIndex)
            {
                _items?.CopyTo(array, arrayIndex);
            }

            public int Count => _items != null ? _items.Count : 0;

            public bool IsReadOnly => false;

            public bool Remove(T item)
            {
                return _items != null ? _items.Remove(item) : false;
            }

            public IEnumerator<T> GetEnumerator()
            {
                return (_items != null ? _items : Enumerable.Empty<T>()).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        Dictionary<string, object> _properties;

        public T? GetValueProperty<T>(string name) where T : struct
        {
            if (_properties == null) return null;

            object value;
            if (!_properties.TryGetValue(name, out value)) return null;

            return (T)value;
        }

        public T GetObjectProperty<T>(string name) where T : class
        {
            if (_properties == null) return null;

            object value;
            if (!_properties.TryGetValue(name, out value)) return null;

            return (T)value;
        }

        public IList<T> GetListProperty<T>(string name)
        {
            return new GpxListWrapper<T>(this, name);
        }

        public void SetValueProperty<T>(string name, T? value) where T : struct
        {
            if (value != null)
            {
                if (_properties == null) _properties = new Dictionary<string, object>();
                _properties[name] = value.Value;
            }
            else
            {
                _properties?.Remove(name);
            }
        }

        public void SetObjectProperty<T>(string name, T value) where T : class
        {
            if (value != null)
            {
                if (_properties == null) _properties = new Dictionary<string, object>();
                _properties[name] = value;
            }
            else
            {
                _properties?.Remove(name);
            }
        }
    }
}
