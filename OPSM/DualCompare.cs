using System;

namespace OPSM
{
	/// <summary>
	/// 
	/// </summary>
	public class DualCompare
	{
		//SortedList dualStruct;
		ISimpleItemset[,] dualStruct;

		public DualCompare(Dataset ds, int support)
		{
			int maxItem = ds.GetColumnCount();

			//dualStruct = new SortedList();
			dualStruct = new ISimpleItemset[maxItem,maxItem];

			for (int loopA = 0; loopA < maxItem; loopA++)
				for (int loopB = 0; loopB < maxItem; loopB++)
					if (loopA != loopB)
					{
						Utils.FastSparseBitArray bitArray = ds.BuildBitVector(loopA, loopB);
						int countElements = bitArray.CountElements();

						if (countElements >= support)
						{
							ISimpleItemset itemset;
							//if (ds.RowCount <= 64)
							//	itemset = new ShortSimpleItemset();
							//else
								itemset = new SimpleItemset();

							//itemset.AddItem(loopA);
							//itemset.AddItem(loopB);
							itemset.SetTransactions(bitArray);
							//itemset.support = countElements;

							dualStruct[loopA,loopB] = itemset;
						}
					}
		}

		public ISimpleItemset GetItemset(Itemset i)
		{
			//return (Itemset)dualStruct[i];
			return dualStruct[i.GetItem(0), i.GetItem(1)];
		}

		public ISimpleItemset GetItemset(int a, int b)
		{
			return dualStruct[a,b];
		}

	}
}
