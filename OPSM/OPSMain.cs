using System;
using System.Windows.Forms;

namespace OPSM
{
	/// <summary>
	/// Summary description for OPSM.
	/// </summary>
	public class OPSMMain
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args) 
		{
			if (args.Length == 0)
			{
				Form1 form = new Form1();
				form.ShowDialog();
			}
			else
			{
				MinerParams minerParams = null;

				try
				{
					minerParams = MinerParams.Parse(args);

					DateTime timeBefore = DateTime.Now;
					MineResults mineResult = null;

					mineResult = DoMine(minerParams);

					if (mineResult != null)
					{
						try
						{
							System.Console.WriteLine("Mining time : " + (DateTime.Now - timeBefore));
							System.Console.WriteLine("Total found " + mineResult.Count.ToString() + " patterns");
							System.Console.WriteLine(mineResult.ToString());
						}
						catch (Exception ex)
						{
							MessageBox.Show("Exception during writing result: " + ex.ToString(), "Exception",
								MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						}				
						finally
						{
							mineResult.Dispose();
							mineResult = null;
						}
					}
				}
				catch (ArgumentException ex)
				{
					System.Console.WriteLine("Failed to parse command line argument: " + ex.Message);
					System.Console.WriteLine();
					System.Console.WriteLine(MinerParams.Usage());
				}
			}
		}

		public static MineResults DoMine(MinerParams minerParams)
		{
			int parameter1 = 0;

			OPSM.Dataset dataset = null;
			MineResults mineResult = null;

			try
			{									
				dataset = new Dataset(
					minerParams.DatasetFileName,
					minerParams.InputContainsColumnHeaders, 
					minerParams.InputContainsRowHeaders);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Failed to load dataset: " + ex.Message, "Exception",
					MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}

			if (dataset != null)
			{
				try
				{
					if (minerParams.Algorithm == Algorithm.WithErrors)
						parameter1 = minerParams.MaxErrors;
					else if (minerParams.Algorithm == Algorithm.Groups)
						parameter1 = minerParams.MinGroups;
					else if (minerParams.Algorithm == Algorithm.Layers)
						parameter1 = minerParams.MaxLayerDiff;

					OPSM.DualCompare dualCompare = null;
					int dualSupport = -1;
					if (minerParams.Algorithm == Algorithm.OPSM)
						dualSupport = minerParams.MinSupport;

					if (minerParams.InCoreDualCompare == true)
						dualCompare = new DualCompare(dataset, dualSupport);
					OPSM.Miner dfsMiner = null;

					if (minerParams.Algorithm == Algorithm.OPSM)
                        dfsMiner = new DFSLookAheadMiner(dataset, dualCompare);
					else if (minerParams.Algorithm == Algorithm.TreePattern)
						dfsMiner = new DFSPatternMiner(dataset, dualCompare);
					else if (minerParams.Algorithm == Algorithm.WithErrors)
						dfsMiner = new DFSErrorMiner(dataset, dualCompare);
					else if (minerParams.Algorithm == Algorithm.Groups)
						dfsMiner = new DFSGroupsMiner(dataset, dualCompare);
					else if (minerParams.Algorithm == Algorithm.Layers)
						dfsMiner = new DFSLayerMiner(dataset, dualCompare, minerParams.DatasetFileName);

					string targetDirectory = minerParams.OutputDirectory;

					mineResult = new MineResults(
						15, 
						targetDirectory,
						minerParams.WriteAllResults && minerParams.WriteOutputFiles);

					dfsMiner.Mine(minerParams.MinSupport, minerParams.MinLength, minerParams.MaxLength, parameter1, mineResult);

					if (mineResult != null)
						mineResult.WriteResults(dataset);
				}
				catch (Exception ex)
				{
					mineResult = null;
					MessageBox.Show("Exception during mining: " + ex.ToString(), "Exception",
						MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}				
			}

			return mineResult;
		}
	}
}
