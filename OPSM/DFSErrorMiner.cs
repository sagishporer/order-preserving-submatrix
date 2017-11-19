using System;
using Utils;
using System.Collections.Generic;

namespace OPSM
{
	/// <summary>
	/// 
	/// </summary>
	public class DFSErrorMiner : Miner
	{
		public DFSErrorMiner(Dataset ds, DualCompare dc)
            : base(ds, dc)
        {
		}

		override public void Mine(int minSupport, int minLength, int maxLength, int maxMistakes, MineResults mineResult)
		{
			Itemset head = new Itemset();
            List<int> tail = new List<int>();

			for (int loop = 0; loop < _ds.GetColumnCount(); loop++)
				tail.Add(loop);

            List<int> newTail;
			Itemset newHead;
			for (int loopTail = 0; loopTail < tail.Count; loopTail++)
			{
                newTail = new List<int>(tail);
				newTail.RemoveAt(loopTail);
				newHead = new Itemset();
				newHead.AddItem(tail[loopTail]);
				RecurseMining(newHead, newTail, minSupport, minLength, maxLength, maxMistakes, mineResult);
			}
		}

			// Mine with skip-errors
		void RecurseMining(Itemset head, List<int> tail, int minSupport, int minLength, int maxLength, int maxMistakes, MineResults mineResult)
		{
			Itemset newHead;
			List<int> newTail;
			MistakesBitMask mistakes;

			if (head.Count >= maxLength)
				return;

            List<FastSparseBitArray> newSupportVectorBitMask = new List<FastSparseBitArray>();
            List<MistakesBitMask> newBitMask = new List<MistakesBitMask>();
            List<int> newSupport = new List<int>();

			for (int loopTail = 0; loopTail < tail.Count; loopTail++)
			{
				int i = tail[loopTail];

				mistakes = new MistakesBitMask(maxMistakes);

				FastSparseBitArray bitArray = _dualComp.GetItemset(head.GetLastItem(), i).GetTransactions();
				mistakes.mistakes[0] = bitArray;

				if (head.Count > 1)
				{
					FastSparseBitArray bitMaskTwoBack = _dualComp.GetItemset(head.GetItem(head.Count - 2), i).GetTransactions();

					MistakesBitMask existingMistakes = head.GetMistakes();

					mistakes.mistakes[0] = mistakes.mistakes[0].And(existingMistakes.mistakes[0]);

					// ************* Build candidate **************
					// Build the mistakes array
					for (int loopMistakes = 1; loopMistakes < maxMistakes + 1; loopMistakes++)
					{
						if (existingMistakes.mistakes[loopMistakes] != null)
						{
							FastSparseBitArray noNewErrors = bitArray.And(existingMistakes.mistakes[loopMistakes]);

							FastSparseBitArray newError = noNewErrors;// = head.GetParent().GetMistakes().mistakes[loopMistakes - 1].And(bitMaskTwoBack);						

							int upwardCount = 1;
							Itemset upwardTraversal = head;							
							upwardTraversal = upwardTraversal.GetParent();
							while ((upwardTraversal != null)&&(loopMistakes - upwardCount >= 0))
							{
								FastSparseBitArray bitTIDs = _dualComp.GetItemset(upwardTraversal.GetLastItem(), i).GetTransactions();
								newError = newError.Or(upwardTraversal.GetMistakes().mistakes[loopMistakes - upwardCount].And(bitTIDs));

								upwardTraversal = upwardTraversal.GetParent();
								upwardCount++;
							}

							mistakes.mistakes[loopMistakes] = newError;//.Or(noNewErrors);
						}
						else
						{
							FastSparseBitArray bitTIDs;
							Itemset ancestor = head;
							FastSparseBitArray newError = bitArray;
							while (ancestor.GetParent() != null)
							{
								ancestor = ancestor.GetParent();
								bitTIDs = _dualComp.GetItemset(ancestor.GetLastItem(), i).GetTransactions();
								newError = newError.Or(bitTIDs);
							}

							mistakes.mistakes[loopMistakes] = newError;//.Or(noNewErrors);
							break;
						}
					}
				}

						// *********** Calculate the support *************
				int support = 999999;

						// The support is the Union of the last 'maxMistakes' mistakes
						// vectors, to get the last mistakes vector the algorithm preforms
						// a traversal backword on the last items developed (backtracks the DFS)
				FastSparseBitArray currentSupportVector = mistakes.mistakes[maxMistakes];
				if (currentSupportVector != null)
				{
					Itemset upwardTraversal = head;
					for (int upward = 0; upward < maxMistakes; upward++)
					{
						if (upwardTraversal == null)
							break;

						currentSupportVector = currentSupportVector.Or(upwardTraversal.GetMistakes().mistakes[maxMistakes - upward - 1]);
						upwardTraversal = upwardTraversal.GetParent();
					}

					if (upwardTraversal != null)
						support = currentSupportVector.CountElements();
				}

				if (support >= minSupport)
				{
					newSupportVectorBitMask.Add(currentSupportVector);
					newBitMask.Add(mistakes);
					newSupport.Add(support);						
				}
				else // Just remove the item as it will not be a memeber later...
				{
					tail.RemoveAt(loopTail);
					loopTail--;
				}
			}

				// Do Recurse call
			if (head.Count > 0)
			{
				for (int loopTail = 0; loopTail < tail.Count; loopTail++)
				{
					int i = tail[loopTail];

					newTail = new List<int>(tail);
					newTail.RemoveAt(loopTail);
					newHead = new Itemset(head);
					newHead.AddItem(tail[loopTail]);
					newHead.support = newSupport[loopTail];
					newHead.SetTransactions((Utils.FastSparseBitArray)newSupportVectorBitMask[loopTail]);
					newHead.SetMistakes((MistakesBitMask)newBitMask[loopTail]);
					newHead.SetParent(head);

					RecurseMining(newHead, newTail, minSupport, minLength, maxLength, maxMistakes, mineResult);

					if (newHead.Count >= minLength)
					{
						mineResult.Add(newHead);
						/*
						System.IO.FileStream fs = new System.IO.FileStream("res.txt",
														System.IO.FileMode.Append);
						System.IO.StreamWriter tw = new System.IO.StreamWriter(fs);
						tw.WriteLine(newHead.ToString());
						tw.Close();
						fs.Close();
						*/
					}
					newHead.SetParent(null);
					newHead.SetMistakes(null);
					newHead.SetTransactions(null);
				}
			}
		}

/*
			// Mine with break-errors
		void RecurseMining(Itemset head, ArrayList tail, int minSupport, int minLength, int maxLength, int maxMistakes, ArrayList mineResult)
		{
			Itemset newHead;
			ArrayList newTail;
			MistakesBitMask mistakes;

			if (head.Count >= maxLength)
				return;

			ArrayList newBitMask = new ArrayList();
			ArrayList newSupport = new ArrayList();

			for (int loopTail = 0; loopTail < tail.Count; loopTail++)
			{
				int i = (int)tail[loopTail];

				mistakes = new MistakesBitMask(maxMistakes);
				FastSparseBitArray bitArray = dualComp.GetItemset(head.GetLastItem(), i).GetTransactions();
				FastSparseBitArray maxBitArray = bitArray;
				mistakes.mistakes[0] = bitArray;

				if (head.Count > 1)
				{
					MistakesBitMask existingMistakes = head.GetMistakes();
					mistakes.mistakes[0] = mistakes.mistakes[0].And(existingMistakes.mistakes[0]);
					maxBitArray = mistakes.mistakes[0];

							// Build the mistakes array
					for (int loopMistakes = 1; loopMistakes < maxMistakes + 1; loopMistakes++)
					{
						if (existingMistakes.mistakes[loopMistakes] != null)
						{
							mistakes.mistakes[loopMistakes] = bitArray.And(existingMistakes.mistakes[loopMistakes]).Or(existingMistakes.mistakes[loopMistakes - 1]);
							maxBitArray = mistakes.mistakes[loopMistakes];
						}
						else
						{
							mistakes.mistakes[loopMistakes] =  bitArray.Or(existingMistakes.mistakes[loopMistakes - 1]);
							maxBitArray = mistakes.mistakes[loopMistakes];
							break;
						}
					}

//					bitArray = bitArray.And(head.GetTransactions());
				}

				int support;
				if (mistakes.mistakes[maxMistakes] != null)
					support = mistakes.mistakes[maxMistakes].CountElements();
				else
					support = 999999;

				if (support >= minSupport)
				{
					//newBitMask.Add(bitArray);
					newBitMask.Add(mistakes);
					newSupport.Add(support);						
				}
				else // Just remove the item as it will not be a memeber later...
				{
					tail.RemoveAt(loopTail);
					loopTail--;
				}
			}

				// Do Recurse call
			if (head.Count > 0)
			{
				for (int loopTail = 0; loopTail < tail.Count; loopTail++)
				{
					int i = (int)tail[loopTail];

					newTail = (ArrayList)tail.Clone();
					newTail.RemoveAt(loopTail);
					newHead = new Itemset(head);
					newHead.AddItem((int)tail[loopTail]);
					newHead.support = (int)newSupport[loopTail];
					//newHead.SetTransactions((FastSparseBitArray)newBitMask[loopTail]);
					newHead.SetMistakes((MistakesBitMask)newBitMask[loopTail]);
					RecurseMining(newHead, newTail, minSupport, minLength, maxLength, maxMistakes, mineResult);
					newHead.SetMistakes(null);
					//newHead.SetTransactions(null);

					if (newHead.Count >= minLength)
						mineResult.Add(newHead);
				}
			}
		}

*/
	}	
}
