using System;
using Utils;
using System.Collections.Generic;

namespace OPSM
{
	/// <summary>
	/// 
	/// </summary>
	public class DFSPatternMiner : Miner
	{
			// Tree-Pattern specific information
		int MAX_IN_RANK = 2;
		int MAX_OUT_RANK = 2;

		public DFSPatternMiner(Dataset ds, DualCompare dc)
            : base(ds, dc)
        {
		}

		override public void Mine(int support, int minLength, int maxLength, int maxMistakes, MineResults mineResult)
		{
			Itemset head = new Itemset();
            List<int> tail = new List<int>();

			for (int loop = 0; loop < _ds.GetColumnCount(); loop++)
				tail.Add(loop);

			RecurseMining(head, tail, support, minLength, maxLength, mineResult);
		}

        void RecurseMining(Itemset head, List<int> tail, int support, int minLength, int maxLength, MineResults mineResult)
		{
			Itemset newHead;
            List<int> newTail;

			if (head.Count >= maxLength)
				return;

			for (int loopTail = 0; loopTail < tail.Count; loopTail++)
			{
				int i = tail[loopTail];

				if (head.Count == 0)
				{
					newTail = new List<int>(tail);
					newTail.RemoveAt(loopTail);
					newHead = new Itemset();
					newHead.AddItem(i);
					RecurseMining(newHead, newTail, support, minLength, maxLength, mineResult);
				}
				else
				{
					for (int loopHead = 0; loopHead < head.Count; loopHead++)
					{
						for (int loopReverseOrder = 0; loopReverseOrder < 1; loopReverseOrder++)
						{
							FastSparseBitArray bitArray;
							if (loopReverseOrder == 0)
							{
								if (head.outDegree[loopHead] >= MAX_OUT_RANK)
									break;

								bitArray = _dualComp.GetItemset(head.GetItem(loopHead), i).GetTransactions();
							}
							else
							{
								if (head.inDegree[loopHead] >= MAX_IN_RANK)
									break;

								bitArray = _dualComp.GetItemset(i, head.GetItem(loopHead)).GetTransactions();
							}

							if (head.Count > 1)
								bitArray = bitArray.And(head.GetTransactions());

							if (bitArray.CountElements() >= support)
							{
								newTail = new List<int>(tail);
								newTail.RemoveAt(loopTail);
								newHead = new Itemset(head);
								newHead.AddItem(i);
								newHead.SetTransactions(bitArray);
								newHead.support = bitArray.CountElements();
								if (loopReverseOrder == 0)
								{
									newHead.inDegree[newHead.Count - 1] = 1;
									newHead.outDegree[loopHead]++;
									newHead.customStringDisplay +=  "" + head.GetItem(loopHead) + "=>" + i + "; ";
								}
								else
								{
									newHead.inDegree[loopHead]++;
									newHead.outDegree[newHead.Count - 1] = 1;
									newHead.customStringDisplay +=  "" + i + "=>" + head.GetItem(loopHead) + "; ";
								}

								RecurseMining(newHead, newTail, support, minLength, maxLength, mineResult);

								if (newHead.Count >= minLength)
								{
									mineResult.Add(newHead);
									/*
									System.IO.FileStream fs = new System.IO.FileStream("res.txt",
										System.IO.FileMode.Append);
									System.IO.StreamWriter tw = new System.IO.StreamWriter(fs);
									tw.WriteLine(newHead.ToString());
									tw.Close();
									fs.Close();*/
								}
								newHead.SetTransactions(null);
							}
						}
					}
				}
			}
		}
	}	
}
