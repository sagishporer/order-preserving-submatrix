using System;
using Utils;

namespace OPSM
{
	public class MistakesBitMask
	{
		public FastSparseBitArray[] mistakes;

		public MistakesBitMask(int maxMistakes)
		{
			mistakes = new FastSparseBitArray[maxMistakes + 1];
		}
	}

	public interface ISimpleItemset
	{
		void SetTransactions(FastSparseBitArray transactions);
		FastSparseBitArray GetTransactions();
	}
    /*
	public class ShortSimpleItemset : ISimpleItemset
	{
		ulong _bits;

		public ShortSimpleItemset()
		{
			_bits = 0;
		}

		#region ISimpleItemset Members

		public void SetTransactions(FastSparseBitArray transactions)
		{
			_bits = transactions.values[0];
		}

		public FastSparseBitArray GetTransactions()
		{
			FastSparseBitArray transactions = new FastSparseBitArray(0);
			transactions.maxUsedPos = 1;
			transactions.keys[0] = 0;
			transactions.values[0] = _bits;

			return transactions;
		}

		#endregion

	}
    */

	public class SimpleItemset : ISimpleItemset
	{
		FastSparseBitArray _transactions;
		
		public SimpleItemset()
		{
			_transactions = null;
		}

		public void SetTransactions(FastSparseBitArray transactions)
		{
			_transactions = transactions;
		}

		public FastSparseBitArray GetTransactions()
		{
			return _transactions;
		}
	}

    public class ItemsetBasic : IComparable, ICloneable
    {
		int[] items;
		public int Count;
		public int support;
		public string customStringDisplay;

		FastSparseBitArray transactions;
        MistakesBitMask mistakes;

		public ItemsetBasic(int maxItems)
		{
			customStringDisplay = "";
			Count = 0;
            items = new int[maxItems];

            mistakes = null;
			transactions = null;
		}

        public ItemsetBasic(ItemsetBasic i)
		{
			customStringDisplay = i.customStringDisplay;
			Count = i.Count;
			items = new int[Count + 2];

			for (int loop = 0; loop < Count; loop++)
				items[loop] = i.items[loop];

            mistakes = null;
            transactions = null;
        }

		public string ToSimpleString()
		{
			string res = support.ToString() + "\t" + this.Count;
			for (int loop = 0; loop < Count; loop++)
				res += "\t" + items[loop].ToString();

			return res;
		}

		public override String ToString()
		{
			String res = "Support " + support.ToString() + " : ";
			if (customStringDisplay == "")
			{
				for (int loop = 0; loop < Count; loop++)
					res += items[loop].ToString() + " ";
			}
			else
				res += customStringDisplay;

			return res;
		}

		public void SetTransactions(FastSparseBitArray trans)
		{
			transactions = trans;
		}

		public FastSparseBitArray GetTransactions()
		{
			return transactions;
		}

		public void SetMistakes(MistakesBitMask mis)
		{
			mistakes = mis;
		}

		public MistakesBitMask GetMistakes()
		{
			return mistakes;
		}

		public int GetLastItem()
		{
			return items[Count - 1];
		}

		public int GetItem(int pos)
		{
			return items[pos];
		}

		public void AddItem(int item)
		{
			items[Count++] = item;
		}

		public int RemoveLastItem()
		{
            return items[--Count];
		}

		public int CompareTo(object obj)
		{
            ItemsetBasic other = (ItemsetBasic)obj;

			for (int loop = 0; (loop < Count)&&(loop < other.Count); loop++)
			{
				int val, otherVal;
				val = GetItem(loop);
				otherVal = other.GetItem(loop);

				if (val > otherVal)
					return 1;

				if (val < otherVal)
					return -1;
			}

			if (other.Count > Count)
				return -1;

			if (other.Count < Count)
				return 1;

			return 0;
		}
		public object Clone()
		{
            return new ItemsetBasic(this);
		}
        public bool isContaining(ItemsetBasic other)
		{
			if (other.Count > Count)
				return false;

			int pos, otherPos;
			pos = 0;
			otherPos = 0;
			while ( (pos < Count)&&(otherPos < other.Count) )
			{
				if (items[pos] == other.items[otherPos])
					otherPos++;

				pos++;
			}

			if (otherPos >= other.Count)
				return true;

			return false;
		}

    }

	/// <summary>
	/// 
	/// </summary>
    public class Itemset : ItemsetBasic
	{
		public int[] inDegree;
		public int[] outDegree;

		public int[] itemGroup;
		public int groupLength;

		public static int numberOfLayers = -1;
		public int[] layersItemsSum;

        Itemset parent;

		public Itemset()
            : base(3)
		{
			inDegree = new int[3];
			outDegree = new int[3];

			itemGroup = new int[3];

			if (numberOfLayers > 0)
			{
				layersItemsSum = new int[numberOfLayers];
			}
			else
			{
				layersItemsSum = null;
			}

            parent = null;
		}

		public Itemset(Itemset i)
            : base(i)
		{
			inDegree = new int[Count + 2];
			outDegree = new int[Count + 2];

			itemGroup = new int[Count + 2];

			for (int loop = 0; loop < Count; loop++)
			{
				inDegree[loop] = i.inDegree[loop];
				outDegree[loop] = i.outDegree[loop];
				itemGroup[loop] = i.itemGroup[loop];
			}

			if (i.layersItemsSum != null)
			{
				layersItemsSum = new int[i.layersItemsSum.Length];
				i.layersItemsSum.CopyTo(layersItemsSum, 0);
			}

            parent = null;
		}

		public override String ToString()
		{
			String res = "Support " + support.ToString() + " : ";
			if (customStringDisplay == "")
			{
				int currentItemGroup = -1;
				for (int loop = 0; loop < Count; loop++)
				{
					if (currentItemGroup == -1)
						currentItemGroup = itemGroup[loop];

					if (itemGroup[loop] != currentItemGroup)
					{
						res += " || ";
						currentItemGroup = itemGroup[loop];
					}

					res += this.GetItem(loop).ToString() + " ";
				}
			}
			else
				res += customStringDisplay;

			return res;
		}

		public object Clone()
		{
			return new Itemset(this);
		}

        public Itemset GetParent()
        {
            return parent;
        }

        public void SetParent(Itemset newParent)
        {
            parent = newParent;
        }
	}

/*
	public class Itemset : IComparable, ICloneable
	{
		ArrayList items;
		FastSparseBitArray transactions;

		public int Count
		{
			get
			{
				if (items == null) return 0;
				return items.Count;
			}
		}


		public Itemset()
		{
			items = new ArrayList();
		}

		private Itemset(ArrayList i)
		{
			items = (ArrayList)i.Clone();
		}

		public override String ToString()
		{
			String res = "";
			for (int loop = 0; loop < items.Count; loop++)
				res += ((int)items[loop]).ToString() + " ";

			return res;
		}

		public void SetTransactions(FastSparseBitArray trans)
		{
			transactions = trans;
		}

		public FastSparseBitArray GetTransactions()
		{
			return transactions;
		}

		public int GetItem(int pos)
		{
			return (int)(items[pos]);
		}

		public void AddItem(int item)
		{
			items.Add(item);
		}

		public int RemoveLastItem()
		{
			int val = GetItem(items.Count - 1);
			items.RemoveAt(items.Count - 1);

			return val;
		}


		public int CompareTo(object obj)
		{
			Itemset other = (Itemset)obj;

			for (int loop = 0; (loop < Count)&&(loop < other.Count); loop++)
			{
				int val, otherVal;
				val = GetItem(loop);
				otherVal = other.GetItem(loop);

				if (val > otherVal)
					return 1;

				if (val < otherVal)
					return -1;
			}

			if (other.Count > Count)
				return -1;

			if (other.Count < Count)
				return 1;

			return 0;
		}
		public object Clone()
		{
			return new Itemset(items);
		}
	}
*/	
}
