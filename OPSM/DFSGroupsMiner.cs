using System;
using Utils;
using System.Collections.Generic;

namespace OPSM
{
	/// <summary>
	/// 
	/// </summary>
	public class DFSGroupsMiner : Miner
	{
		public DFSGroupsMiner(Dataset ds, DualCompare dc)
            : base(ds, dc)
		{
		}

		override public void Mine(int support, int minLength, int maxLength, int maxGroupLength, MineResults mineResult)
		{
			Itemset head = new Itemset();
            List<int> tail = new List<int>();

			for (int loop = 0; loop < _ds.GetColumnCount(); loop++)
				tail.Add(loop);

			RecurseMining(head, tail, support, minLength, maxLength, maxGroupLength, mineResult);
		}

        void RecurseMining(Itemset head, List<int> tail, int support, int minLength, int maxLength, int maxGroupLength, MineResults mineResult)
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
					newHead.itemGroup[newHead.Count - 1] = 1;
					newHead.groupLength = 1;
					RecurseMining(newHead, newTail, support, minLength, maxLength, maxGroupLength, mineResult);
				}
				else
				{
					int currentItemGroup = head.itemGroup[head.Count - 1];
					for (int loopGroups = 0; loopGroups < 2; loopGroups++)
					{
							// Don't try to add the same item to the same group twice
						if ((loopGroups == 0)&&(i < head.GetItem(head.Count - 1)))
							continue;

							// Don't create too long group
						if ((loopGroups == 0)&&(head.groupLength >= maxGroupLength))
							continue;

							// Add 0 / 1 depends on the loop
						int newItemGroup = currentItemGroup + loopGroups;
						FastSparseBitArray bitArray = null;

							// Perform AND with all previous group members - calc support
						for (int loopHead = 0; loopHead < head.Count; loopHead++)
							if (head.itemGroup[loopHead] == newItemGroup - 1)
							{
								if (bitArray == null)
									bitArray = _dualComp.GetItemset(head.GetItem(loopHead), i).GetTransactions();
								else
									bitArray = bitArray.And(_dualComp.GetItemset(head.GetItem(loopHead), i).GetTransactions());
							}

						if (currentItemGroup > 1)
							bitArray = bitArray.And(head.GetTransactions());

						bool validSupport = false;
						if (bitArray == null)
							validSupport = true;
						else if (bitArray.CountElements() >= support)
							validSupport = true;

						if (validSupport == false)
						{
							tail.RemoveAt(loopTail);
							loopTail--;
							break;
						}
						else //if (validSupport == true)
						{
							newTail = new List<int>(tail);
							newTail.RemoveAt(loopTail);
							newHead = new Itemset(head);
							newHead.AddItem(i);
							newHead.itemGroup[newHead.Count - 1] = newItemGroup;
							if (loopGroups == 0)
								newHead.groupLength = head.groupLength + 1;
							else
								newHead.groupLength = 1;

							newHead.SetTransactions(bitArray);
							if (bitArray != null)
								newHead.support = bitArray.CountElements();
							RecurseMining(newHead, newTail, support, minLength, maxLength, maxGroupLength, mineResult);

							// Add the new item as 'found' itemset only if 1. it's above minSupport, 2. it contains more then the first group
							if ((newHead.Count >= minLength)&&(newHead.Count > newHead.groupLength))
							{
								mineResult.Add(newHead);
							}

							newHead.SetTransactions(null);
						}
					}
				}
			}
		}
	}	
}
