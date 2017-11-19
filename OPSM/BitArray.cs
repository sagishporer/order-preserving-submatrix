using System;

namespace Utils
{
    public abstract class BitArrayBase : ICloneable, IComparable
    {
        protected const int BITS_IN_ULONG = 64;
        protected static int[] _oneCount;

        public int frequency;

        static BitArrayBase()
        {
            _oneCount = new int[256];
            for (uint loop = 0; loop < 256; loop++)
            {
                int count = 0;
                uint diff = loop;
                while (diff > 0)
                {
                    if ((diff & 0x01) == 0x01)
                        count++;
                    diff >>= 1;
                }

                _oneCount[loop] = count;
            }
        }

        public abstract void Clear();

        public abstract bool Get(int i);

        public abstract void Set(int i, bool val);

        public abstract void SetAppend(int i);

        public abstract BitArrayBase And(BitArrayBase other);

        public abstract BitArrayBase Or(BitArrayBase other);

        public abstract int CountElements();

        public abstract object Clone();

        public abstract int CompareTo(object obj);
    }

    /// <summary>
    /// Implementation of bit array with up-to 64 items in the array
    /// </summary>
    public class ShortBitArray : BitArrayBase, IComparable<ShortBitArray>
    {
        ulong _bitArray;

        public ShortBitArray()
        {
            _bitArray = 0;
        }

        public ShortBitArray(ShortBitArray other)
        {
            this._bitArray = other._bitArray;
        }

        public override void Clear()
        {
            _bitArray = 0;
        }

        public override bool Get(int i)
        {
            return (_bitArray & (1UL << i)) > 0;
        }

        public override void Set(int i, bool val)
        {
            if (val == true)
                _bitArray = _bitArray | (1UL << i);
            else
                _bitArray = _bitArray & (~(1UL << i));
        }

        public override void SetAppend(int i)
        {
            _bitArray = _bitArray | (1UL << i);
        }

        public override int CountElements()
        {
            int count = 0;
            ulong diff = _bitArray;
            while (diff > 0)
            {
                count += _oneCount[(uint)(diff & 0x000000FF)];
                diff >>= 8;
            }

            return count;
        }

        public override BitArrayBase And(BitArrayBase other)
        {
            ShortBitArray shortOther = (ShortBitArray)other;
            ShortBitArray newBitArray = new ShortBitArray();

            newBitArray._bitArray = this._bitArray & shortOther._bitArray;

            return newBitArray;
        }

        public override BitArrayBase Or(BitArrayBase other)
        {
            ShortBitArray shortOther = (ShortBitArray)other;
            ShortBitArray newBitArray = new ShortBitArray();

            newBitArray._bitArray = this._bitArray | shortOther._bitArray;

            return newBitArray;
        }

        public override int GetHashCode()
        {
            return _bitArray.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            ShortBitArray other = (ShortBitArray)obj;

            return (other._bitArray == this._bitArray);
        }

        #region ICloneable Members

        public override object Clone()
        {
            return new ShortBitArray(this);
        }

        #endregion

        #region IComparable Members

        public override int CompareTo(object obj)
        {
            return CompareTo((ShortBitArray)obj);
        }

        #endregion

        #region IComparable<ShortBitArray> Members

        public int CompareTo(ShortBitArray other)
        {
            if (other == null)
                return -1;

            return this._bitArray.CompareTo(other._bitArray);
        }

        #endregion
    }

	public class SimpleBitArray : ICloneable, IComparable
	{
		static int BITS_IN_ULONG = 64;
		ulong[] bitArray;
		int size;
		public int minValue, maxValue;
		public int itemSum;

		public static int[] oneCount = null;

		public int Length
		{
			get { return size; }
		}

		public override int GetHashCode()
		{
//			ulong hash = 0;
//			for (int loop = minValue/BITS_IN_ULONG; loop < /*bitArray.Length*/maxValue/BITS_IN_ULONG + 1; loop++)
//				hash ^= (bitArray[loop] ^ (ulong)loop);

//			return (int)((0xFFFFFFFF & hash) ^ (hash >> 32));
			return itemSum;
		}

		public SimpleBitArray(int size)
		{
			this.size = size;
			bitArray = new ulong[size/BITS_IN_ULONG + 1];
			maxValue = -1;
			minValue = Int32.MaxValue;
			itemSum = 0;

			if (oneCount == null)
			{
				oneCount = new int[256];
				for (uint loop = 0; loop < 256; loop++)
				{
					int count = 0;
					uint diff = loop;
					while (diff > 0)
					{
						if ( (diff & 0x01) == 0x01)
							count++;
						diff >>= 1;
					}	

					oneCount[loop] = count;
				}
			}
		}

		public SimpleBitArray(SimpleBitArray copy)
		{
			bitArray = (ulong[])copy.bitArray.Clone();
			maxValue = copy.maxValue;
			minValue = copy.minValue;
			size = copy.size;
			itemSum = copy.itemSum;
		}

		public object Clone()
		{
			return new SimpleBitArray(this);
		}


		public int FindFirstItemReverse(int lastValue)
		{
			int loop;

			lastValue = Math.Min(lastValue, maxValue);
			loop = lastValue / BITS_IN_ULONG;

			for (; loop >= 0; loop--)
			{
				if (bitArray[loop] > 0)
				{
					ulong bitMask = 0x01UL << (BITS_IN_ULONG - 1);
					int bitPos = BITS_IN_ULONG - 1;

					while (true)
					{
						if ( (bitMask & bitArray[loop]) != 0)
							return bitPos + (loop * BITS_IN_ULONG);

						bitPos--;
						bitMask >>= 1;
					}
				}
			}

			return -1;
		}

		public int FindFirstItem(int startIndex)
		{
			int maxIndex = maxValue/BITS_IN_ULONG + 1;
			int loop;
			int initShift;

			startIndex = Math.Max(startIndex, minValue);
			loop = startIndex / BITS_IN_ULONG;
			initShift = startIndex % BITS_IN_ULONG;

			for (; loop < maxIndex; loop++)
			{
				ulong diff = bitArray[loop] >> initShift;
				int bitPos = initShift;
				initShift = 0;

				while (diff > 0)
				{
					if ( (diff & 0x01) == 0x01)
						return bitPos + (loop * BITS_IN_ULONG);

					bitPos++;
					diff >>= 1;
				}
			}

			return -1;
		}
		public bool IsOneItemDiff(SimpleBitArray other)
		{
			bool foundOneBit = false;
			for (int loop = 0; loop < bitArray.Length; loop++)
			{
				ulong diff = bitArray[loop] ^ other.bitArray[loop];
				if (diff == 0)
					continue;
	
				if (foundOneBit == true)
					return false;

				// Find the first '1' in the number
				while ((diff & 0x01) == 0x00) // <==> (n & 0x01) != 0x01
					diff = diff >> 1;

				// The first bit is '1' (n & 0x01 == 0x01), if it's the only bit than
				// n == 1
				// Another option is to check with XOR : (n ^ 0x01) == 0
				if (diff == 1)
					foundOneBit = true;
				else
					return false; // More then one bit on in this ulong
			}

			return true;
		}

		public void Set(int i, bool val)
		{
			// Find the ulong & bit of 'i'
			int arrayPos = i/BITS_IN_ULONG;
			int bitPos = i%BITS_IN_ULONG;

			if (val == true)
			{
				minValue = Math.Min(minValue,i);
				maxValue = Math.Max(maxValue,i);

				bitArray[arrayPos] = bitArray[arrayPos] | (1UL << bitPos);
				itemSum += i;
			}
			else
			{
				bitArray[arrayPos] = bitArray[arrayPos] ^ (1UL << bitPos);
				if (i == maxValue)
 					maxValue = this.FindFirstItemReverse(maxValue - 1);
				if (i == minValue)
				{
					minValue = this.FindFirstItem(minValue + 1);
					if (minValue == -1)
						minValue = Int32.MaxValue;
				}

				itemSum -= i;
			}
		}

		public bool Get(int i)
		{
			// Find the ulong & bit of 'i'
			int arrayPos = i/BITS_IN_ULONG;
			int bitPos = i%BITS_IN_ULONG;
			
			return (bitArray[arrayPos] & (1UL << bitPos)) > 0;
		}


		public SimpleBitArray Or(SimpleBitArray other)
		{
			SimpleBitArray res = new SimpleBitArray(size);

			res.minValue = Math.Min(minValue,other.minValue);
			res.maxValue = Math.Max(maxValue,other.maxValue);

			for (int loop = 0; loop < bitArray.Length; loop++)
				res.bitArray[loop] = bitArray[loop] | other.bitArray[loop];

			return res;			
		}

		public SimpleBitArray And(SimpleBitArray other)
		{
			SimpleBitArray res = new SimpleBitArray(size);

			res.minValue = Math.Max(minValue,other.minValue);
			res.maxValue = Math.Min(maxValue,other.maxValue);

			for (int loop = 0; loop < bitArray.Length; loop++)
				res.bitArray[loop] = bitArray[loop] & other.bitArray[loop];

			return res;	
		}

		public SimpleBitArray Xor(SimpleBitArray other)
		{
			SimpleBitArray res = new SimpleBitArray(size);
		
			for (int loop = 0; loop < bitArray.Length; loop++)
				res.bitArray[loop] = bitArray[loop] ^ other.bitArray[loop];

			return res;	
		}

		public int CountElements()
		{
			int loopMin = minValue / BITS_IN_ULONG;
			int loopMax = maxValue / BITS_IN_ULONG + 1;
			int count = 0;
			for (int loop = loopMin; loop < loopMax; loop++)
			{
				ulong diff = bitArray[loop];
				while (diff > 0)
				{
//					if ( (diff & 0x01) == 0x01)
//						count++;
//					diff >>= 1;

					count += oneCount[(uint)(diff & 0x000000FF)];
					diff >>= 8;
				}
			}

			return count;
		}

		public bool IsContaining(SimpleBitArray other)
		{
			for (int loop = 0; loop < bitArray.Length; loop++)
				if ( (bitArray[loop] & other.bitArray[loop]) != other.bitArray[loop])
					return false;

			return true;
		}

		public override bool Equals(object obj)
		{
			return CompareTo(obj) == 0;
		}

//		public static bool reverseSort = false;
		public int CompareTo(object obj)
		{
			SimpleBitArray i = (SimpleBitArray)obj;
//			for (int loop = 0; loop < bitArray.Length; loop++)
/*
			if (reverseSort)
			{
				for (int loop = 0; loop < bitArray.Length * BITS_IN_ULONG; loop++)
				{					
					if ((!Get(loop)) && i.Get(loop))
						return 1;
					if (Get(loop) && (!i.Get(loop)))
						return -1;
				}
			}
			else
*/			{
				for (int loop = bitArray.Length - 1; loop >= 0; loop--)
				{
					if (bitArray[loop] > i.bitArray[loop])
						return 1;
					if (bitArray[loop] < i.bitArray[loop])
						return -1;
				}
			}

			return 0;
		}

	}


	public class FastSparseBitArrayPool
	{
		public static FastSparseBitArrayPool Instance = new FastSparseBitArrayPool();
		FastSparseBitArray[] pool;
		int maxPosUsed;

		private FastSparseBitArrayPool()
		{
			pool = new FastSparseBitArray[10000];
			maxPosUsed = 0;
		}

		public FastSparseBitArray Allocate()
		{
			if (maxPosUsed == 0)
				return new FastSparseBitArray();

			maxPosUsed--;
			return pool[maxPosUsed];
		}

		public void Release(FastSparseBitArray array)
		{
			if (maxPosUsed == pool.Length)
				return;

			array.Clear();
			pool[maxPosUsed] = array;
			maxPosUsed++;
		}
	}

/*
	public class SimpleArray : ICloneable, IComparable
	{
		int[] array;
		int maxPosUsed;

		public SimpleArray(int size)
		{
			array = new int[2 * (int)(Math.Log(size)+1)];
			maxPosUsed = 0;
		}

		public SimpleArray(SimpleArray other)
		{
			array = other.array.Clone();
			maxPosUsed = other.maxPosUsed;
		}

		public int CompareTo(object obj)
		{
			SimpleArray other = (SimpleArray)obj;

			int pos = 0;
			while (pos <
		}

		public object Clone()
		{
			return new SimpleArray(this);
		}
	}
*/

	public class FastSparseBitArray : ICloneable, IComparable
	{
		public int maxUsedPos;
		public int[] keys;
		public ulong[] values;

		static int BITS_IN_ULONG = 64;
		static int ARRAY_INIT_SIZE = 35;
		public int itemSum;

		public int frequency = -1;

		public static int[] oneCount = null;

		public int minValue
		{
			get 
			{
				if (maxUsedPos == 0)
					return Int32.MaxValue;

				ulong diff = values[0];
				int pos = 0;
				while(true)
				{
					if ( (diff & 0x01) > 0 )
						return pos + keys[0] * BITS_IN_ULONG;

					pos++;
					diff >>= 1;
				}
			}
		}

		public int maxValue
		{
			get
			{
				if (maxUsedPos == 0)
					return -1;

				ulong diff = values[maxUsedPos - 1];
				ulong bitMask = 0x01UL << (BITS_IN_ULONG - 1);
				int pos = BITS_IN_ULONG - 1;
				while (true)
				{
					if ( (diff & bitMask) > 0 )
						return pos + keys[maxUsedPos - 1] * BITS_IN_ULONG;

					pos--;
					bitMask >>= 1;
				}
			}
		}


		public FastSparseBitArray(int initSize)
		{
			if (oneCount == null)
			{
				oneCount = new int[256];
				for (uint loop = 0; loop < 256; loop++)
				{
					int count = 0;
					uint diff = loop;
					while (diff > 0)
					{
						if ( (diff & 0x01) == 0x01)
							count++;
						diff >>= 1;
					}	

					oneCount[loop] = count;
				}
			}

			keys = new int[initSize/BITS_IN_ULONG + 1];
			values = new ulong[initSize/BITS_IN_ULONG + 1];
			maxUsedPos = 0;
			itemSum = 0;
		}

		public FastSparseBitArray()
		{
			if (oneCount == null)
			{
				oneCount = new int[256];
				for (uint loop = 0; loop < 256; loop++)
				{
					int count = 0;
					uint diff = loop;
					while (diff > 0)
					{
						if ( (diff & 0x01) == 0x01)
							count++;
						diff >>= 1;
					}	

					oneCount[loop] = count;
				}
			}

			keys = new int[ARRAY_INIT_SIZE];
			values = new ulong[ARRAY_INIT_SIZE];
			maxUsedPos = 0;
			itemSum = 0;
		}

		protected FastSparseBitArray(FastSparseBitArray other)
		{
			keys = (int[])other.keys.Clone();
			values = (ulong[])other.values.Clone();
			maxUsedPos = other.maxUsedPos;
			itemSum = other.itemSum;
		}

		public void Clear()
		{
			maxUsedPos = 0;
			itemSum  = 0;
		}

		public override String ToString()
		{
			String s = "";
			int count = 0;
			for (int i = 0; i <= maxValue; i++)
			{
				if (Get(i) == true)
				{
					s = s + " " + i.ToString();
					count++;
					if (count%25 == 0)
						s+='\n';
				}
			}

			return s;
		}

		public object Clone()
		{
			return new FastSparseBitArray(this);
		}

		public override int GetHashCode()
		{
/*			ulong xorVal = 0;
			int res = 0;
			for (int loop = 0; loop < maxUsedPos; loop++)
				xorVal ^= values[loop];

			res = (int)((xorVal & 0xFFFFFFFF) ^ (xorVal >> 32));
			return 1;			
*/		
			return itemSum;
		}

		public override bool Equals(object obj)
		{
			return CompareTo(obj) == 0;
		}

		public int CompareTo(object obj)
		{
			FastSparseBitArray other = (FastSparseBitArray)obj;

			int otherPos = other.maxUsedPos - 1;
			int pos = maxUsedPos - 1;
			
			while ( (pos >= 0)&&(otherPos >= 0) )
			{
				if (keys[pos] > other.keys[otherPos])
					return 1;

				if (keys[pos] < other.keys[otherPos])
					return -1;

				if (values[pos] > other.values[otherPos])
					return 1;
				if (values[pos] < other.values[otherPos])
					return -1;

				pos--;
				otherPos--;
			}

			if ((pos == -1)&&(otherPos == -1))
				return 0;

			if (otherPos == -1)
				return 1;

			return -1;
		}

		public bool Get(int i)
		{
			int pos = Array.BinarySearch(keys,0,maxUsedPos, i/BITS_IN_ULONG);
			if (pos < 0)
				return false;

			int bitPos = i%BITS_IN_ULONG;

			return (values[pos] & (1UL << bitPos)) > 0;
		}

		private void Insert(int pos, int key)
		{
			if (keys.Length <= maxUsedPos + 1)
			{
				int[] newKeys = new int[keys.Length * 2];
				ulong[] newValues = new ulong[keys.Length * 2];

				keys.CopyTo(newKeys, 0);
				values.CopyTo(newValues, 0);
				keys = newKeys;
				values = newValues;
			}

			for (int loop = maxUsedPos - 1; loop >= pos; loop--)
			{
				keys[loop + 1] = keys[loop];
				values[loop + 1] = values[loop];
			}

			keys[pos] = key;
			values[pos] = 0;
			maxUsedPos++;
		}

		private void RemoveAt(int pos)
		{
			for (int loop = pos; loop < maxUsedPos; loop++)
			{
				keys[loop] = keys[loop + 1];
				values[loop] = values[loop + 1];
			}

			maxUsedPos--;
		}

		public void Set(int i, bool val)
		{
			int pos = Array.BinarySearch(keys,0,maxUsedPos, i/BITS_IN_ULONG);
			if (pos < 0)
			{
				// The item dosen't exist, dont' add it if all you got to do is to 
				// remove it...
				if (val == false)
					return;

				if (maxUsedPos == 0)
				{
					pos = 0;
					Insert(pos, i/BITS_IN_ULONG);
				}
				else
				{
						// BinarySearch & Insert
					int minItem = 0;
					int maxItem = maxUsedPos - 1;

					int mid;
					int comp;
					while (Math.Abs(minItem - maxItem) > 0)
					{
						mid = (minItem + maxItem)/2;
						comp = keys[mid] - (i/BITS_IN_ULONG);

						if (comp > 0)
						{
							if (maxItem == mid)
								mid--;
							maxItem = mid;
						}
						else if (comp < 0)
						{
							if (minItem == mid)
								mid++;
							minItem = mid;
						}
						else //(comp == 0)
							comp = 0; // HA?
					}
				
					comp = keys[minItem] - (i/BITS_IN_ULONG);					
					if (comp > 0)
						pos = minItem;
					else // comp < 0
						pos = minItem + 1;

					Insert(pos, i/BITS_IN_ULONG);
				}
			}

			if (val == true)
			{
				values[pos] |= 1UL<<(i%BITS_IN_ULONG);
				itemSum += i;
			}
			else
			{
				// Assumes the value exists
				values[pos] ^= 1UL<<(i%BITS_IN_ULONG);
				if (values[pos] == 0)
					RemoveAt(pos);
				itemSum -= i;
			}
		}

		public void SetAppend(int i)
		{
			if (maxUsedPos == 0)
			{
				Insert(0, i/BITS_IN_ULONG);
			}
			else if (keys[maxUsedPos - 1] != i/BITS_IN_ULONG)
			{
				Insert(maxUsedPos, i/BITS_IN_ULONG);
				itemSum += i/BITS_IN_ULONG;
			}

			values[maxUsedPos - 1] |= 1UL<<(i%BITS_IN_ULONG);
		}

		public bool IsContaining(FastSparseBitArray other)
		{
			int pos = 0;
			int otherPos = 0;
			int count = maxUsedPos;
			int otherCount = other.maxUsedPos;
			
			while ( (pos < count)&&(otherPos < otherCount) )
			{
				if (keys[pos] > other.keys[otherPos])
					return false;

				if (keys[pos] < other.keys[otherPos])
				{
					pos++;
					continue;
				}

				if ( (values[pos] & other.values[otherPos]) != other.values[otherPos])
					return false;

				pos++;
				otherPos++;
			}

			if ((pos == count)&&(otherPos < otherCount))
				return false;

			return true;
		}

			// a.AndNot(b) == a And !b
		public FastSparseBitArray AndNot(FastSparseBitArray other)
		{
			FastSparseBitArray res = new FastSparseBitArray();

			int pos = 0;
			int otherPos = 0;
			int itemCount = maxUsedPos;
			int otherItemCount = other.maxUsedPos;

			while ((pos < itemCount)&&(otherPos < otherItemCount))
			{
				if (keys[pos] < other.keys[otherPos])
				{
					res.Insert(res.maxUsedPos, keys[pos]);
					res.values[res.maxUsedPos-1] = values[pos];
					pos++;
				}
				else if (keys[pos] > other.keys[otherPos])
					otherPos++;
				else
				{
					if ( (values[pos] & (~other.values[otherPos])) != 0)
					{
						res.Insert(res.maxUsedPos, keys[pos]);
						res.values[res.maxUsedPos-1] = (values[pos] & (~other.values[otherPos]));
					}
					pos++;
					otherPos++;
				}
			}

			while (pos < itemCount)
			{
				res.Insert(res.maxUsedPos, keys[pos]);
				res.values[res.maxUsedPos-1] = values[pos];
				pos++;
			}

			return res;
		}

		public FastSparseBitArray And(FastSparseBitArray other)
		{
			FastSparseBitArray res = FastSparseBitArrayPool.Instance.Allocate(); //new FastSparseBitArray(Math.Min(maxUsedPos,other.maxUsedPos)*BITS_IN_ULONG);

			int pos = 0;
			int otherPos = 0;
			int itemCount = maxUsedPos;
			int otherItemCount = other.maxUsedPos;

			while ((pos < itemCount)&&(otherPos < otherItemCount))
			{
				if (keys[pos] < other.keys[otherPos])
					pos++;
				else if (keys[pos] > other.keys[otherPos])
					otherPos++;
				else
				{
					if ( (values[pos] & other.values[otherPos]) != 0)
					{
						if (res.keys.Length <= res.maxUsedPos + 1)
						{
							int[] newKeys = new int[res.keys.Length * 2];
							ulong[] newValues = new ulong[res.keys.Length * 2];

							res.keys.CopyTo(newKeys, 0);
							res.values.CopyTo(newValues, 0);
							res.keys = newKeys;
							res.values = newValues;
						}

						res.keys[res.maxUsedPos] = keys[pos];
						res.maxUsedPos++;
//						res.Insert(res.maxUsedPos, keys[pos]);
						res.values[res.maxUsedPos-1] = (values[pos] & other.values[otherPos]);
					}
					pos++;
					otherPos++;
				}
			}

			return res;
		}

		public FastSparseBitArray Or(FastSparseBitArray other)
		{
			FastSparseBitArray res = new FastSparseBitArray();
			int pos = 0;
			int otherPos = 0;

			while ((pos < maxUsedPos)||(otherPos < other.maxUsedPos))
			{
				int key;
				ulong val;

				if (otherPos == other.maxUsedPos)
				{
					key = keys[pos];
					val = values[pos];
					pos++;
				}
				else if (pos == maxUsedPos)
				{
					key = other.keys[otherPos];
					val = other.values[otherPos];
					otherPos++;
				}
				else if (keys[pos] < other.keys[otherPos])
				{
					key = keys[pos];
					val = values[pos];
					pos++;
				}
				else if (keys[pos] > other.keys[otherPos])
				{
					key = other.keys[otherPos];
					val = other.values[otherPos];
					otherPos++;
				}
				else
				{
					key = keys[pos];
					val = values[pos] | other.values[otherPos];
					pos++;
					otherPos++;
				}

				res.Insert(res.maxUsedPos, key);
				res.values[res.maxUsedPos - 1] = val;
			}

			return res;
		}

		public FastSparseBitArray Xor(FastSparseBitArray other)
		{
			FastSparseBitArray res = new FastSparseBitArray();
			int pos = 0;
			int otherPos = 0;

			while ((pos < maxUsedPos)||(otherPos < other.maxUsedPos))
			{
				int key;
				ulong val;

				if (otherPos == other.maxUsedPos)
				{
					key = keys[pos];
					val = values[pos];
					pos++;
				}
				else if (pos == maxUsedPos)
				{
					key = other.keys[otherPos];
					val = other.values[otherPos];
					otherPos++;
				}
				else if (keys[pos] < other.keys[otherPos])
				{
					key = keys[pos];
					val = values[pos];
					pos++;
				}
				else if (keys[pos] > other.keys[otherPos])
				{
					key = other.keys[otherPos];
					val = other.values[otherPos];
					otherPos++;
				}
				else
				{
					key = keys[pos];
					val = values[pos] ^ other.values[otherPos];
					pos++;
					otherPos++;
				}

				res.Insert(res.maxUsedPos, key);
				res.values[res.maxUsedPos - 1] = val;
			}

			return res;
		}
		

		public int CountElements()
		{
			int count = 0;
			int bitArrayCount = maxUsedPos;

			for (int loop = 0; loop < bitArrayCount; loop++)
			{
				ulong diff = values[loop];

				while (diff > 0)
				{
					//					if ( (diff & 0x01) == 0x01)
					//						count++;
					//					diff >>= 1;
					
					count += oneCount[(uint)(diff & 0x000000FF)];
					diff >>= 8;
				}
			}

			frequency = count;
			return count;
		}

		public bool IsOneItemDiff(FastSparseBitArray other)
		{
			return Xor(other).CountElements() == 1;
		}

	}


}
