using System;
using System.Collections.Generic;
using System.Text;

namespace OPSM
{
    public class IntList : ICloneable
    {
        int[] _data;
        int _head;

        public int Count
        {
            get { return _head; }
        }

        public int this[int idx]
        {
            get { return _data[idx]; }
        }

        public IntList(int size)
        {
            _data = new int[size];
            _head = 0;
        }

        /*
        public IntList(IntList other)
        {
            other.CopyTo(this);
        }
        */
        private void CopyTo(IntList other)
        {
            other._head = this._head;
            Array.Copy(this._data, other._data, this._head);
        }

        public void Clear()
        {
            _head = 0;
        }

        public void Add(int val)
        {
            //if (_head == _data.Length)
            //    throw new IndexOutOfRangeException();

            _data[_head++] = val;
        }

        public void RemoveAt(int pos)
        {
            _data[pos] = _data[--_head];
        }

        public void InsertAt(int pos, int val)
        {
            // Add
            _data[_head++] = _data[pos];
            _data[pos] = val;
        }

        #region ICloneable Members

        public object Clone()
        {
            IntList intList = IntListPool.Instance.Allocate(this._data.Length);
            this.CopyTo(intList);

            return intList;
        }

        #endregion
    }

    public class IntListPool
    {
        private const int POOL_SIZE = 10000;
        public static IntListPool Instance = new IntListPool();

        IntList[] _pool;
        int _poolPos;

        private IntListPool()
        {
            _pool = new IntList[POOL_SIZE];
            _poolPos = 0;
        }

        public void Clear()
        {
            while (_poolPos > 0)
                _pool[--_poolPos] = null;
        }

        public IntList Allocate(int size)
        {
            if (_poolPos == 0)
                return new IntList(size);

            return _pool[--_poolPos];
        }

        public void Release(IntList item)
        {
            if (_poolPos == _pool.Length)
                return;

            item.Clear();
            _pool[_poolPos++] = item;
        }
    }
}
