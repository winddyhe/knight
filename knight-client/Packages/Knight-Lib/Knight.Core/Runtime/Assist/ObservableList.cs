using System;
using System.Collections.Generic;
using System.Collections;

namespace Knight.Core
{
    public interface IObservableEvent
    {
        Action ChangedHandler { get; set; }
        void Refresh();
    }

    public class ObservableList<T> : IList<T>, IList, IObservableEvent
    {
        private Action              mChangedHandler     = null;
        private readonly List<T>    mInnerList          = new List<T>();

        public ObservableList()
        {
        }

        public ObservableList(IEnumerable<T> rItems)
        {
            this.mInnerList.AddRange(rItems);
        }

        public T this[int nIndex]
        {
            get { return this.mInnerList[nIndex];       }
            set { this.mInnerList[nIndex] = value;      }
        }

        object IList.this[int nIndex]
        {
            get { return this.mInnerList[nIndex];       }
            set { this.mInnerList[nIndex] = (T)value;   }
        }

        public int Count
        {
            get { return this.mInnerList.Count;         }
        }

        public bool IsReadOnly
        {
            get { return false;                         }
        }

        public bool IsFixedSize
        {
            get { return true;                          }
        }

        public bool IsSynchronized
        {
            get { return false;                         }
        }

        public object SyncRoot
        {
            get { return this;                          }
        }

        public Action ChangedHandler
        {
            get { return mChangedHandler;               }
            set { mChangedHandler = value;              }
        }

        public void Add(T rItem)
        {
            this.mInnerList.Add(rItem);
        }

        public int Add(object rValue)
        {
            this.mInnerList.Add((T)rValue);
            return this.mInnerList.Count - 1;
        }

        public void Clear()
        {
            this.mInnerList.Clear();
        }

        public bool Contains(T rItem)
        {
            return this.mInnerList.Contains(rItem);
        }

        public bool Contains(object rItem)
        {
            return this.mInnerList.Contains((T)rItem);
        }

        public void CopyTo(T[] rArray, int nArrayIndex)
        {
            this.mInnerList.CopyTo(rArray, nArrayIndex);
        }

        public void CopyTo(Array rArray, int nIndex)
        {
            this.mInnerList.CopyTo((T[])rArray, nIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.mInnerList.GetEnumerator();
        }

        public int IndexOf(T rItem)
        {
            return this.mInnerList.IndexOf(rItem);
        }

        public int IndexOf(object rValue)
        {
            return this.mInnerList.IndexOf((T)rValue);
        }

        public void Insert(int nIndex, T nItem)
        {
            this.mInnerList.Insert(nIndex, nItem);
        }

        public void Insert(int nIndex, object rValue)
        {
            this.mInnerList.Insert(nIndex, (T)rValue);
        }

        public void Refresh()
        {
            UtilTool.SafeExecute(this.mChangedHandler);
        }

        public bool Remove(T rItem)
        {
            var bResult = this.mInnerList.Remove(rItem);
            return bResult;
        }

        public void Remove(object rValue)
        {
            this.mInnerList.Remove((T)rValue);
        }

        public void RemoveAt(int nIndex)
        {
            this.mInnerList.RemoveAt(nIndex);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.mInnerList.GetEnumerator();
        }

        public T Find(Predicate<T> match)
        {
            return this.mInnerList.Find(match);
        }

        public void Sort(Comparison<T> comparison)
        {
            this.mInnerList.Sort(comparison);
        }

        public List<T> FindAll(Predicate<T> match)
        {
            return this.mInnerList.FindAll(match);
        }

        public void AddRange(IEnumerable<T> rItems)
        {
            this.mInnerList.AddRange(rItems);
        }

        public List<T> ToList()
        {
            return new List<T>(this.mInnerList);
        }
    }
}
