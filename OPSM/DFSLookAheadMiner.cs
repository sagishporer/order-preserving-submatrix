using System;
using System.Collections.Generic;
using System.Text;
using Utils;

namespace OPSM
{
    /// <summary>
    /// New prunings:
    /// Look a-head pruning, if there are two patterns -
    /// Support(AB) = Support(AC) = Support(ABC)
    /// with the same support then the pattern
    /// AC
    /// Can be trimmed away
    /// </summary>
    public class DFSLookAheadMiner : Miner
    {
        public class DFSLevelItem
        {
            public ItemsetBasic Head;

            public IntList Tail;
            public List<FastSparseBitArray> TailBitArrays;

            public DFSLevelItem(ItemsetBasic head, IntList Tail)
            {
                this.Head = head;
                this.Tail = Tail;
                this.TailBitArrays = null;
            }
        }

        public DFSLookAheadMiner(Dataset ds, DualCompare dc)
            : base(ds, dc)
        {
        }

        override public void Mine(int support, int minLength, int maxLength, int maxMistakes, MineResults mineResult)
        {
            IntListPool.Instance.Clear();

            ItemsetBasic head = new ItemsetBasic(_ds.GetColumnCount());
            IntList tail = new IntList(_ds.GetColumnCount());

            // Build all items tail
            for (int loop = 0; loop < _ds.GetColumnCount(); loop++)
                tail.Add(loop);

            List<DFSLevelItem> levelItems = new List<DFSLevelItem>();

            // Build first level look-ahead
            for (int loop = 0; loop < tail.Count; loop++)
            {
                int i = tail[loop];

                IntList newTail = (IntList)tail.Clone();
                newTail.RemoveAt(loop);

                ItemsetBasic newHead = new ItemsetBasic(tail.Count);
                newHead.AddItem(i);
                newHead.support = _ds.RowCount;

                levelItems.Add(new DFSLevelItem(newHead, newTail));
            }

            RecurseMining(levelItems, support, minLength, maxLength, mineResult);
        }

        void RecurseMining(List<DFSLevelItem> levelItems, int support, int minLength, int maxLength, MineResults mineResult)
        {
            // Simple pattern cut-off: if the pattern is not long enough, or too long
            for (int i = levelItems.Count - 1; i >= 0; i--)
            {
                if (levelItems[i].Head.Count >= maxLength)
                {
                    levelItems.RemoveAt(i);
                    continue;
                }

                if (levelItems[i].Head.Count + levelItems[i].Tail.Count < minLength)
                {
                    levelItems.RemoveAt(i);
                    continue;
                }
            }

            if (levelItems.Count == 0)
                return;

            Dictionary<int, int> lookAheadPrune = new Dictionary<int, int>();
            for (int levelItemsLoop = 0; levelItemsLoop < levelItems.Count; levelItemsLoop++)
            {
                IntList tail = levelItems[levelItemsLoop].Tail;
                ItemsetBasic head = levelItems[levelItemsLoop].Head;

                List<FastSparseBitArray> bitArrays = new List<FastSparseBitArray>(tail.Count);
                for (int loopTail = 0; loopTail < tail.Count; loopTail++)
                {
                    int i = tail[loopTail];

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

                        // Prune look-ahead
                        if (bitArray.frequency == head.support)
                        {
                            int lookAheadSupport;
                            if (lookAheadPrune.TryGetValue(i, out lookAheadSupport) == false)
                                lookAheadPrune.Add(i, bitArray.frequency);
                            else
                                lookAheadPrune[i] = Math.Max(lookAheadSupport, bitArray.frequency);
                        }
                    }
                    else
                    {
                        // Don't release bit vectors from O2 matrix, or there is no
                        // O2 matrix
                        if ((head.Count > 1) || (_dualComp == null))
                            FastSparseBitArrayPool.Instance.Release(bitArray);

                        tail.RemoveAt(loopTail);
                        loopTail--;
                    }
                }

                levelItems[levelItemsLoop].TailBitArrays = bitArrays;
            }

            for (int levelItemsLoop = 0; levelItemsLoop < levelItems.Count; levelItemsLoop++)
            {
                ItemsetBasic head = levelItems[levelItemsLoop].Head;
                List<FastSparseBitArray> bitArrays = levelItems[levelItemsLoop].TailBitArrays;

                int lookAheadSupport;
                if (lookAheadPrune.TryGetValue(head.GetLastItem(), out lookAheadSupport) == true)
                {
                    if (lookAheadSupport == head.support)
                    {
                        if (head.Count > 2)
                            for (int j = 0; j < bitArrays.Count; j++)
                                FastSparseBitArrayPool.Instance.Release(bitArrays[j]);
                        continue;
                    }
                }

                IntList tail = levelItems[levelItemsLoop].Tail;

                List<DFSLevelItem> newLevelItems = new List<DFSLevelItem>();
                for (int loopTail = 0; loopTail < tail.Count; loopTail++)
                {
                    int i = tail[loopTail];

                    IntList newTail = (IntList)tail.Clone();
                    newTail.RemoveAt(loopTail);

                    ItemsetBasic newHead = new ItemsetBasic(head);
                    newHead.AddItem(i);
                    FastSparseBitArray bitArray = bitArrays[loopTail];
                    newHead.SetTransactions(bitArray);
                    newHead.support = bitArray.frequency;

                    if (newHead.Count >= minLength)
                        mineResult.Add(newHead);

                    newLevelItems.Add(new DFSLevelItem(newHead, newTail));
                }

                RecurseMining(newLevelItems, support, minLength, maxLength, mineResult);

                // Release IntList
                for (int j = 0; j < newLevelItems.Count; j++)
                    IntListPool.Instance.Release(newLevelItems[j].Tail);

                // Release FastSparseBitArray
                if (head.Count > 2)
                    for (int j = 0; j < bitArrays.Count; j++)
                        FastSparseBitArrayPool.Instance.Release(bitArrays[j]);
            }
        }
    }	
}
