using System;
using System.IO;
using Utils;
using System.Collections.Generic;

namespace OPSM
{
	/// <summary>
	/// 
	/// </summary>
	public class Dataset
	{
		static string SEPERATOR = "\t";
		List<List<double>> dataset;

		List<string> _columnHeaders;
		List<string> _rowHeaders;

		#region Properties
		public List<string> ColumnHeaders
		{
            get { return _columnHeaders; }
		}

		public List<string> RowHeaders
		{
			get { return _rowHeaders; }
		}

		public int RowCount
		{
			get { return RowHeaders.Count; } 
		}

		#endregion

		private Dataset()
		{
			dataset = null;
			_columnHeaders = null;
			_rowHeaders = null;
		}

		public Dataset(String path, bool columnHeaders, bool rowHeaders)
		{
			StreamReader file;
			dataset = null;			
			file = new StreamReader((System.IO.Stream)File.OpenRead(path),System.Text.Encoding.ASCII);

			LoadFile(file, columnHeaders, rowHeaders);
		}

		void LoadFile(StreamReader srFile, bool columnHeaders, bool rowHeaders)
		{
			String nextLine = srFile.ReadLine();
			dataset = new List<List<double>>();

			_rowHeaders = new List<string>();

			// Build column headers
			try
			{
				_columnHeaders = new List<string>();
				if (nextLine != null)
				{
					nextLine = nextLine.Trim();
					string[] row = nextLine.Split(SEPERATOR.ToCharArray());
					int firstDataColumn = 0;
					if (rowHeaders == true)
						firstDataColumn = 1;

					if (columnHeaders == true)
					{
						for (int i = firstDataColumn; i < row.Length; i++)
							_columnHeaders.Add(row[i]);

						nextLine = srFile.ReadLine();
					}
					else
					{
						for (int i = firstDataColumn; i < row.Length; i++)
						{
							int colNumber = i;
							if (firstDataColumn == 0)
								colNumber = colNumber + 1;

							_columnHeaders.Add("Column #" + colNumber.ToString());
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw new FormatException("Failed parsing column headers: " + ex.Message);
			}

			int rowCount = 0;
			try
			{
				while (nextLine != null)
				{
					nextLine = nextLine.Trim();
					if (nextLine != "")
					{
						List<double> singleRow = new List<double>();
						string[] row = nextLine.Split(SEPERATOR.ToCharArray());
						int firstDataColumn = 0;
				
						rowCount++;

						if (rowHeaders == true)
						{
							firstDataColumn = 1;
							_rowHeaders.Add(row[0]);
						}
						else
						{
							_rowHeaders.Add("Row #" + rowCount.ToString());
						}

						for (int i = firstDataColumn; i < row.Length; i++)
							singleRow.Add(Double.Parse(row[i]));
			
						dataset.Add(singleRow);
					}

					nextLine = srFile.ReadLine();
				}
			}
			catch 
			{
				throw new FormatException("Failed parsing line #" + rowCount + " (Not counting header rows)"); 
			}
		}


		public void SaveFile(StreamWriter sw)
		{
			System.Text.StringBuilder strBuilder = new System.Text.StringBuilder();

			// Column headers
			strBuilder.Append("Row");
			for (int cols = 0; cols < ColumnHeaders.Count; cols++)
				strBuilder.Append(SEPERATOR + ColumnHeaders[cols]);

			sw.WriteLine(strBuilder.ToString());

			// Rows
			for (int row = 0; row < RowHeaders.Count; row++)
			{
				strBuilder = new System.Text.StringBuilder();
				strBuilder.Append(RowHeaders[row]);

				for (int col = 0; col < ColumnHeaders.Count; col++)
					strBuilder.Append(SEPERATOR + GetItem(row, col));

				sw.WriteLine(strBuilder.ToString());
			}
		}

		public int GetRowCount()
		{
			return dataset.Count;
		}

		public int GetColumnCount()
		{
			return dataset[0].Count;
		}

		public double GetItem(int row, int column)
		{
			return dataset[row][column];
		}

		public void BuildBitVector(int columnA, int columnB, FastSparseBitArray bitArray)
		{
			for (int loop = 0; loop < GetRowCount(); loop++)
			{
				if (GetItem(loop, columnA) < GetItem(loop, columnB))
					bitArray.SetAppend(loop);
			}
		}

		public FastSparseBitArray BuildBitVector(int columnA, int columnB)
		{
			FastSparseBitArray bitArray = new FastSparseBitArray();
			BuildBitVector(columnA, columnB, bitArray);

			return bitArray;
		}


		public Dataset GetSubMatrix(int[] columns, int[] rows)
		{
			Dataset newDataset = new Dataset();

			// Column Headers
			newDataset._columnHeaders = new List<string>();
			for (int i = 0; i < columns.Length; i++)
				newDataset._columnHeaders.Add(this.ColumnHeaders[columns[i]]);

			// Row Headers
			newDataset._rowHeaders = new List<string>();
			for (int i = 0; i < rows.Length; i++)
				newDataset._rowHeaders.Add(this.RowHeaders[rows[i]]);

			// Data
			newDataset.dataset = new List<List<double>>();
			for (int row = 0; row < rows.Length; row++)
			{
				List<double> singleRow = new List<double>();
				for (int column = 0; column < columns.Length; column++)
					singleRow.Add(this.GetItem(rows[row], columns[column]));

				newDataset.dataset.Add(singleRow);
			}

			return newDataset;
		}
	}
}
