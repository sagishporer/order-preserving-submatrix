using System;
using System.Collections.Generic;

namespace OPSM
{
	/// <summary>
	/// Summary description for Miner.
	/// </summary>
	public abstract class Miner
	{
		protected Dataset _ds;
		protected DualCompare _dualComp;

        public Miner(Dataset ds, DualCompare dualComp)
		{
            this._ds = ds;
            this._dualComp = dualComp;
		}

		abstract public void Mine(int support, int minLength, int maxLength, int maxMistakes, MineResults mineResults);

        public static List<Itemset> CalculateMFI(List<Itemset> FI)
		{
            List<Itemset> MFI = new List<Itemset>();

			foreach (Itemset a in FI)
			{
				bool foundContaining = false;

				foreach (Itemset b in FI)
				{
					if (b.Count > a.Count)
						if (b.isContaining(a))
							foundContaining = true;							
				}

				if (foundContaining == false)
					MFI.Add(a);
			}

			return MFI;
		}
	}
}
