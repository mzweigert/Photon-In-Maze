using System;
using System.Collections;
using System.Collections.Generic;

namespace PhotonInMaze.Common {
    public class OrderedSet<T> : ICollection<T> {
        private readonly IDictionary<T, LinkedListNode<T>> dictionary;
        private readonly LinkedList<T> linkedList;

        public OrderedSet()
            : this(EqualityComparer<T>.Default) {
        }

        public OrderedSet(IEqualityComparer<T> comparer) {
            dictionary = new Dictionary<T, LinkedListNode<T>>(comparer);
            linkedList = new LinkedList<T>();
        }

        public int Count {
            get { return dictionary.Count; }
        }

        public virtual bool IsReadOnly {
            get { return dictionary.IsReadOnly; }
        }

        public LinkedListNode<T> First { get { return linkedList.First; } }

        void ICollection<T>.Add(T item) {
            AddLast(item);
        }

        public LinkedListNode<T> Find(T item) {
            bool found = dictionary.TryGetValue(item, out LinkedListNode<T> node);
            if(found) {
                return node;
            }
            return null;
        }

        public int IndexOf(T item) {
            int index = 0;
            LinkedListNode<T> current = First;
            while(current != null) {
                if(current.Value.Equals(item)) {
                    return index;
                }
                index++;
                current = current.Next;
            }
            return -1;
        }

        public void Clear() {
            linkedList.Clear();
            dictionary.Clear();
        }

        public bool Remove(T item) {
            bool found = dictionary.TryGetValue(item, out LinkedListNode<T> node);
            if(!found) {
                return false;
            }
            dictionary.Remove(item);
            linkedList.Remove(node);
            return true;
        }

        public bool RemoveAllBackwards(T item) {
            LinkedListNode<T> node = Find(item);
            if(node == null) {
                return false;
            }
            while(!First.Equals(node)) {
                RemoveFirst();
            }
            return true;
        }

        public bool RemoveFirst() {
            if(First == null) {
                return false;
            }
            bool found = dictionary.TryGetValue(First.Value, out LinkedListNode<T> node);
            if(!found) {
                return false;
            }
            dictionary.Remove(First.Value);
            linkedList.RemoveFirst();
            return true;
        }


        public IEnumerator<T> GetEnumerator() {
            return linkedList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public bool Contains(T item) {
            return dictionary.ContainsKey(item);
        }

        public void CopyTo(T[] array, int arrayIndex) {
            linkedList.CopyTo(array, arrayIndex);
        }

        public bool AddLast(T item) {
            if(dictionary.ContainsKey(item)) return false;
            LinkedListNode<T> node = linkedList.AddLast(item);
            dictionary.Add(item, node);
            return true;
        }

        public bool AddFirst(T item) {
            if(dictionary.ContainsKey(item)) return false;
            LinkedListNode<T> node = linkedList.AddFirst(item);
            dictionary.Add(item, node);
            return true;
        }

    }
}