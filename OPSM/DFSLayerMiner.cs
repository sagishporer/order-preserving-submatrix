using System;
using System.IO;
using Utils;
using System.Collections.Generic;

namespace OPSM
{
	class Candidate : IComparable, IComparable<Candidate>
	{
		public FastSparseBitArray bitArray;
		public int support;
		public int item;

		public int CompareTo(object obj)
		{
			Candidate other = (Candidate)obj;
			return this.support - other.support;
		}

        public int CompareTo(Candidate other)
        {
            return this.support - other.support;
        }
    }


	/// <summary>
	/// Summary description for DFSLayerMiner.
	/// </summary>
	public class DFSLayerMiner : Miner
	{
		int[] _itemsLayers;

		public DFSLayerMiner(Dataset ds, DualCompare dc, string path)
            : base(ds, dc)
        {
			LoadLayers(path);
		}

			// Layers file format :
			//	each line contains information about one layer.
			//	the layer is represented by a range value : X-Y
		private void LoadLayers(string path)
		{
			_itemsLayers = new int[_ds.GetColumnCount()];
			for (int loop = 0; loop < _itemsLayers.Length; loop++)
				_itemsLayers[loop] = -1;

			StreamReader sr = new StreamReader(File.OpenRead(path + ".layers"), System.Text.Encoding.ASCII);
			string line = sr.ReadLine();
			int layer = 0;
			while (line != null)
			{
					// parse X-Y
				int pos = line.IndexOf('-');
				int firstIndex, lastIndex;

				firstIndex = Int32.Parse(line.Substring(0, pos));
				lastIndex = Int32.Parse(line.Substring(pos + 1));

				for (int loop = firstIndex; loop <= lastIndex; loop++)
					_itemsLayers[loop] = layer;

				layer++;
				line = sr.ReadLine();
			}
			sr.Close();

				// Verify that all items got a layer definition
			for (int loop = 0; loop < _itemsLayers.Length; loop++)
				if (_itemsLayers[loop] == -1)
					throw new Exception("Item #"+loop+" has no layer definition");
		}


		override public void Mine(int support, int minLength, int maxLength, int maxLayerDiff, MineResults mineResult)
		{
            List<int> tail = new List<int>();

			int numberOfLayers = 0;
			for (int loopLayers = 0; loopLayers < _itemsLayers.Length; loopLayers++)
				numberOfLayers = Math.Max(numberOfLayers, _itemsLayers[loopLayers]);
			numberOfLayers++;
			Itemset.numberOfLayers = numberOfLayers; // Set number of layers

			for (int loop = 0; loop < _ds.GetColumnCount(); loop++)
				tail.Add(loop);

				// TODO : Sort the tail in some smart way...
			Itemset head;
			while (tail.Count > 0)
			{
				head = new Itemset();
				head.AddItem(tail[0]);
				head.layersItemsSum[_itemsLayers[tail[0]]]++;

				tail.RemoveAt(0);

				RecurseMining(head, tail, support, minLength, maxLength, mineResult, maxLayerDiff);
			}

			Itemset.numberOfLayers = -1;
		}

        void RecurseMining(Itemset head, List<int> tail, int support, int minLength, int maxLength, MineResults mineResult, int maxLayerDiff)
		{
			if (head.Count >= maxLength)
				return;

            List<Candidate> candidates = new List<Candidate>();

					// Find the layer with the minimal number of items
			int minLayerSize = head.layersItemsSum[0];
			for (int loopLayer = 1; loopLayer < head.layersItemsSum.Length; loopLayer++)
				minLayerSize = Math.Min(minLayerSize, head.layersItemsSum[loopLayer]);

			for (int loopTail = 0; loopTail < tail.Count; loopTail++)
			{
				int i = tail[loopTail];

					// TODO : Calculation of the support should be according to the items from the
					// lower and upper layers.
				FastSparseBitArray bitArray = null;
				for (int loop = 0; loop < head.Count; loop++)
				{
					int headItem = head.GetItem(loop);

					if (_itemsLayers[headItem] < _itemsLayers[i])
					{
						if (bitArray == null)
							bitArray = _dualComp.GetItemset(headItem, i).GetTransactions();
						else
							bitArray = bitArray.And(_dualComp.GetItemset(headItem, i).GetTransactions());
					}
					else if (_itemsLayers[headItem] > _itemsLayers[i])
					{
						if (bitArray == null)
							bitArray = _dualComp.GetItemset(i, headItem).GetTransactions();
						else
							bitArray = bitArray.And(_dualComp.GetItemset(i, headItem).GetTransactions());
					}
				}

				if (head.GetTransactions() != null)
				{
					if (bitArray == null)
						bitArray = head.GetTransactions();
					else
						bitArray = bitArray.And(head.GetTransactions());
				}
				int currentSupport = Int32.MaxValue;
				if (bitArray != null)
					currentSupport = bitArray.CountElements();
/*
				FastSparseBitArray bitArray = dualComp.GetItemset(head.GetLastItem(), i).GetTransactions();

				if (head.Count > 1)
					bitArray = bitArray.And(head.GetTransactions());

				int currentSupport = bitArray.CountElements();
*/
				if (currentSupport >= support)
				{
					Candidate cand = new Candidate();
					cand.item = i;

					cand.support = currentSupport;

						// If the new tail creates and itemset with unbalanced layers
						// set support to infinity so the itemset will not be traversed
					if (head.layersItemsSum[_itemsLayers[i]]+1 > minLayerSize + maxLayerDiff)
						cand.support = Int32.MaxValue;

					cand.bitArray = bitArray;

					candidates.Add(cand);
				}				
			}

				// Dynamic Reordering
			candidates.Sort();

				// Rebuild tail
            List<int> newTail = new List<int>();
			for (int loop = 0; loop < candidates.Count; loop++)
				newTail.Add(candidates[loop].item);

			Itemset newHead;
			for (int loopTail = 0; loopTail < candidates.Count; loopTail++)
			{
					// Stop recursing when all the tail left are MaxInt (e.g. from the same layer)
				if (candidates[loopTail].support == Int32.MaxValue)
					break;

				int i = newTail[0];
				newTail.RemoveAt(0);

				newHead = new Itemset(head);
				newHead.AddItem(i);
				newHead.layersItemsSum[_itemsLayers[i]]++;

				FastSparseBitArray bitArray = candidates[loopTail].bitArray;

				newHead.SetTransactions(bitArray);
				newHead.support = candidates[loopTail].support;
				RecurseMining(newHead, newTail, support, minLength, maxLength, mineResult, maxLayerDiff);

				if (newHead.Count >= minLength)
					mineResult.Add(newHead);

				newHead.SetTransactions(null);
			}
		}
	}
}
