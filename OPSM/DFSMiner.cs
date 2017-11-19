using System;
using Utils;
using System.Collections.Generic;

namespace OPSM
{
	/// <summary>
	/// The most basic miner, in most cases you should use DFSLookAheadMiner
	/// </summary>
	public class DFSMiner : Miner
	{
		public DFSMiner(Dataset ds, DualCompare dc)
            : base(ds, dc)
        {
		}

		override public void Mine(int support, int minLength, int maxLength, int maxMistakes, MineResults mineResult)
		{
            IntListPool.Instance.Clear();

            ItemsetBasic head = new ItemsetBasic(_ds.GetColumnCount());
            IntList tail = new IntList(_ds.GetColumnCount());

			for (int loop = 0; loop < _ds.GetColumnCount(); loop++)
				tail.Add(loop);

			RecurseMining(head, tail, support, minLength, maxLength, mineResult);
		}

        void RecurseMining(ItemsetBasic head, IntList tail, int support, int minLength, int maxLength, MineResults mineResult)
		{
			if (head.Count >= maxLength)
				return;

			if (head.Count + tail.Count < minLength)
				return;

            List<FastSparseBitArray> bitArrays = new List<FastSparseBitArray>(tail.Count);
			for (int loopTail = 0; loopTail < tail.Count; loopTail++)
			{
				int i = tail[loopTail];

				if (head.Count == 0)
				{
                    IntList newTail = (IntList)tail.Clone(); // new IntList(tail); 
					newTail.RemoveAt(loopTail);
                    ItemsetBasic newHead = new ItemsetBasic(tail.Count);
					newHead.AddItem(i);
					RecurseMining(newHead, newTail, support, minLength, maxLength, mineResult);

                    IntListPool.Instance.Release(newTail);
				}
				else
				{
					FastSparseBitArray bitArray = null;
					
					if (_dualComp != null)
					{
						ISimpleItemset dualItemset = _dualComp.GetItemset(head.GetLastItem(), i);
						if (dualItemset == null)
						{
							tail.RemoveAt(loopTail);
							loopTail--;
							continue;
						}

						bitArray = dualItemset.GetTransactions();
					}
					else
					{
						bitArray = FastSparseBitArrayPool.Instance.Allocate();
						_ds.BuildBitVector(head.GetLastItem(), i, bitArray);
					}

					if (head.Count > 1)
						bitArray = bitArray.And(head.GetTransactions());

					bitArray.frequency = bitArray.CountElements();
					if (bitArray.frequency >= support)
					{
						bitArrays.Add(bitArray);
					}
					else
					{
							// Don't release bit vectors from O2 matrix, or there is no
							// O2 matrix
						if ((head.Count > 1)||(_dualComp == null))
							FastSparseBitArrayPool.Instance.Release(bitArray);

						tail.RemoveAt(loopTail);
						loopTail--;
					}
				}				
			}

			if (head.Count > 0)
				for (int loopTail = 0; loopTail < tail.Count; loopTail++)
				{
					int i = tail[loopTail];

                    IntList newTail = (IntList)tail.Clone(); // new IntList(tail);
					newTail.RemoveAt(loopTail);

                    // Create 'head' restore point
                    FastSparseBitArray restoreBitArray = head.GetTransactions();
                    int restoreSupport = head.support;

                    head.AddItem(i);
                    FastSparseBitArray bitArray = bitArrays[loopTail];
                    head.SetTransactions(bitArray);
                    head.support = bitArray.frequency;
                    RecurseMining(head, newTail, support, minLength, maxLength, mineResult);

                    IntListPool.Instance.Release(newTail);

                    if (head.Count >= minLength)
                        mineResult.Add(head);

                    head.SetTransactions(null);
                    if (head.Count > 2)
                        FastSparseBitArrayPool.Instance.Release(bitArray);

                    // Restore 'head'
                    head.RemoveLastItem();
                    head.SetTransactions(restoreBitArray);
                    head.support = restoreSupport;
				}
		}
	}
}
