using System;
using System.IO;
using System.Collections;

namespace OPSM
{
	class MineResult : IComparable 
	{
        ItemsetBasic _itemset;

		public int Length
		{
			get { return _itemset.Count; }
		}

		public int Support
		{
			get { return _itemset.support; }
		}

		public MineResult(ItemsetBasic itemset)
		{
            _itemset = (ItemsetBasic)itemset.Clone();
			_itemset.SetTransactions((Utils.FastSparseBitArray)itemset.GetTransactions().Clone());
			_itemset.support = itemset.support;
		}

		public override string ToString()
		{
			return _itemset.ToString();
		}
		
		public int CompareTo(object o)
		{
			MineResult other = (MineResult)o;

			if (this.Length < other.Length)
				return -1;
			else if (this.Length > other.Length)
				return 1;
			else if (this.Support < other.Support)
				return -1;
			else if (this.Support > other.Support)
				return 1;

			return 0;
		}		

		public void WriteResults(string dataFileName, Dataset dataset)
		{
			// Get columns
			int[] columns = new int[_itemset.Count];
			for (int i = 0; i < _itemset.Count; i++)
				columns[i] = _itemset.GetItem(i);

			// Get rows
			Utils.FastSparseBitArray rowsBits = _itemset.GetTransactions();
			int[] rows = new int[_itemset.support];
			int rowsPos = 0;
			for (int i = 0; i < dataset.RowCount; i++)
			{
				if (rowsBits.Get(i) == true)
				{
					rows[rowsPos] = i;
					rowsPos++;
				}
			}

			Dataset subMatrix = dataset.GetSubMatrix(columns, rows);
			StreamWriter sw = null;
			try
			{
				sw = new StreamWriter(dataFileName, false);
				subMatrix.SaveFile(sw);
			}
			finally
			{
				if (sw != null)
					sw.Close();
			}
		}
	}

	/// <summary>
	/// Summary description for MineResult.
	/// </summary>
	public class MineResults : IDisposable
	{
		string _targetDirectory;
		SortedList _results;
		int _maxSavedItems;

		int _totalItemsetsCount;
		bool _writeAllResults;
		StreamWriter _allResultsStream;

		public int Count
		{
			get { return _totalItemsetsCount; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="maxSavedItems">Number of 'top' itemsets saved for each pattern length, used for display</param>
		/// <param name="targetDirectory"></param>
		/// <param name="writeAllResults"></param>
		public MineResults(int maxSavedItems, string targetDirectory, bool writeAllResults)
		{
			_totalItemsetsCount = 0;
			_maxSavedItems = maxSavedItems;
			_results = new SortedList();

			_targetDirectory = targetDirectory;
			_writeAllResults = writeAllResults;

			if (Directory.Exists(_targetDirectory) == false)
				Directory.CreateDirectory(_targetDirectory);

			_allResultsStream = null;
			if (_writeAllResults == true)
			{
				string allResultsFileName = Path.Combine(_targetDirectory, "mining-results-all.txt");
				_allResultsStream = new StreamWriter(allResultsFileName, false);
			}
		}
		
		~MineResults()
		{
			Dispose();	
		}

		public void Add(ItemsetBasic itemset)
		{
			_totalItemsetsCount++;

			if (_writeAllResults == true)
				_allResultsStream.WriteLine(itemset.ToSimpleString());
			
			if (_results.ContainsKey(itemset.Count) == false)
				_results.Add(itemset.Count, new SortedList());

			SortedList lengthResult = (SortedList)_results[itemset.Count];

			// If the queue of current length is full - check if the new item
			// should be inseted
			if (lengthResult.Count >= _maxSavedItems)
			{
				MineResult mineResult = (MineResult)lengthResult.GetByIndex(0);
				if (itemset.support <= mineResult.Support)
					return;
			}			

			MineResult result = new MineResult(itemset);
			double key = (double)itemset.support;

			while (lengthResult.ContainsKey(key))
				key += 0.00001;
			lengthResult.Add(key, result);

			while (lengthResult.Count > _maxSavedItems)
				lengthResult.RemoveAt(0);
			
		}

		public override string ToString()
		{
			string str = "";
			foreach (SortedList lengthResult in _results.Values)
			{
				MineResult first = (MineResult)lengthResult.GetByIndex(0);
				str += "**********************\n";
				str += "Length " + first.Length + "\n";
				foreach (MineResult mineResult in lengthResult.Values)
					str += mineResult.ToString() + "\n";
			}

			return str;
		}

		public void WriteResults(Dataset dataset)
		{
			string targetFileName = Path.Combine(_targetDirectory, "mining-results.txt");
			StreamWriter sr = null;
			try
			{
				if (Directory.Exists(_targetDirectory) == false)
					Directory.CreateDirectory(_targetDirectory);

				sr = new StreamWriter(targetFileName, false);

				foreach (SortedList lengthResult in _results.Values)
				{
					MineResult first = (MineResult)lengthResult.GetByIndex(0);
					int patternLength = first.Length;
					sr.WriteLine("**********************");
					sr.WriteLine("Length " + patternLength);

					int lengthCount = 0;
					foreach (MineResult mineResult in lengthResult.Values)
					{
						lengthCount++;
						string dataFileName = string.Format("len-{0}-serial-{1}.txt",
							patternLength,
							lengthCount);
						dataFileName = Path.Combine(_targetDirectory, dataFileName);

						sr.WriteLine(mineResult.ToString() + ", Data file: " + dataFileName);

						mineResult.WriteResults(dataFileName, dataset);
					}
				}
			}
			finally
			{
				if (sr != null)
					sr.Close();
			}
		}
		#region IDisposable Members

		public void Dispose()
		{
			if (_allResultsStream != null)
			{
				_allResultsStream.Close();
				_allResultsStream = null;
			}
		}

		#endregion
	}
}
