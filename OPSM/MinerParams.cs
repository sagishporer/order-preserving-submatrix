using System;
using System.Text;
using System.IO;

namespace OPSM
{
	public enum Algorithm 
	{
		OPSM,
		WithErrors,
		TreePattern,
		Groups,
		Layers
	}

	/// <summary>
	/// Summary description for MinerParams.
	/// </summary>
	public class MinerParams
	{
		public string DatasetFileName;
		public string OutputDirectory
		{
			get
			{
				int pos = DatasetFileName.LastIndexOf("/");
				if (pos < 0)
					pos = DatasetFileName.LastIndexOf("\\");
				if (pos < 0)
					pos = 0;

				string targetDirectory = DatasetFileName.Substring(0, pos);
				return System.IO.Path.Combine(targetDirectory, "results");
			}
		}
		public int MinSupport;

		public bool InputContainsColumnHeaders;
		public bool InputContainsRowHeaders;
		public bool WriteOutputFiles;
		public bool WriteAllResults;

		public int MinLength;
		public int MaxLength;

		public int MaxErrors;
		public int MinGroups;
		public int MaxLayerDiff;		
		public bool InCoreDualCompare;
		public Algorithm Algorithm;

		public MinerParams()
		{
			DatasetFileName = null;
			MinSupport = -1;
			InputContainsColumnHeaders = false;
			InputContainsRowHeaders = false;
			WriteOutputFiles = false;
			WriteAllResults = false;
			MinLength = 0;
			MaxLength = 999;

			MaxErrors = 0;
			MinGroups = 1;
			MaxLayerDiff = 1;
			InCoreDualCompare = true;
			Algorithm = Algorithm.OPSM;
		}


		private static bool GetIntParam(string arg, string paramName, string paramDescription, ref int val)
		{
			try
			{
				if (arg.StartsWith(paramName) == true)
				{
					val = Int32.Parse(arg.Substring(paramName.Length));
					return true;
				}
			}
			catch (FormatException )
			{
				throw new ArgumentException("Failed parsing parameter: " + paramDescription + ", invalid format");
			}
			catch (OverflowException )
			{
				throw new ArgumentException("Failed parsing parameter: " + paramDescription + ", overflow");
			}

			return false;
		}

		private static bool GetBoolParam(string arg, string paramName, string paramDescription, ref bool val)
		{
			if (arg == paramName)
			{
				val = true;
				return true;
			}

			return false;
		}

		public static MinerParams Parse(string[] args)
		{
			if (args.Length < 2)
				throw new ArgumentException("Not enough parameters");

			MinerParams minerParams = new MinerParams();
			foreach (string arg in args)
			{
				if (arg.StartsWith("/") == true)
				{
					bool foundParam = false;
					foundParam = foundParam || GetIntParam(arg, "/s=", "MinSupport", ref minerParams.MinSupport);
					foundParam = foundParam || GetIntParam(arg, "/minl=", "MinLength", ref minerParams.MinLength);
					foundParam = foundParam || GetIntParam(arg, "/maxl=", "MaxLength", ref minerParams.MaxLength);

					foundParam = foundParam || GetBoolParam(arg, "/ic", "InputContainsColumnHeaders", ref minerParams.InputContainsColumnHeaders); 
					foundParam = foundParam || GetBoolParam(arg, "/ir", "InputContainsRowHeaders", ref minerParams.InputContainsRowHeaders); 

					foundParam = foundParam || GetBoolParam(arg, "/writeAll", "WriteAllResults", ref minerParams.WriteAllResults); 
					foundParam = foundParam || GetBoolParam(arg, "/writeOut", "WriteOutputFiles", ref minerParams.WriteOutputFiles); 

					if (foundParam == false)
						throw new ArgumentException("Unhandled Parameters: " + arg);
				}
				else
				{
					// Check dataset file name
					if (minerParams.DatasetFileName != null)
						throw new ArgumentException("Two dataset files defined");

					StreamReader file = null;	
					try
					{
						file = new StreamReader(arg,System.Text.Encoding.ASCII);
					}
					catch (IOException )
					{
						throw new ArgumentException("Failed to open dataset for reading");
					}
					finally
					{
						if (file != null)
							file.Close();
					}

					minerParams.DatasetFileName = arg;
				}
			}

			if (minerParams.MinSupport == -1)
				throw new ArgumentException("MinSupport not defined");

			return minerParams;
		}

		public static string Usage()
		{
			StringBuilder str = new StringBuilder();

			str.Append("Usage: OPSM-G [/ic] [/ir] [/minl=<MinLength>] [/maxl=<MaxLength>] \n         [/writeAll] [/writeOut] /s<MinSupport> <Dataset>\n");			
			str.Append("/ic - Input contains column headers\n");
			str.Append("/ir - Input contains row headers\n");
			str.Append("/minl - Minimal pattern length, default is 0\n");
			str.Append("/maxl - Maximal pattern length, default is 999\n");
			str.Append("/writeAll - Write all results\n");
			str.Append("/writeOut - Write output files\n");

			return str.ToString();
		}


		public override string ToString()
		{
			StringBuilder str = new StringBuilder();
			if (DatasetFileName != null)
				str.Append("File name: " + DatasetFileName + "\n");
			else
				str.Append("File name: NULL\n");
			str.Append("Min Support: " + MinSupport + "\n");
			str.Append("Input Contains Column Headers: " + InputContainsColumnHeaders + "\n");
			str.Append("Input Contains Row Headers: " + InputContainsRowHeaders + "\n");
			str.Append("Write Output Files: " + WriteOutputFiles + "\n");
			str.Append("Write All Results: " + WriteAllResults + "\n");
			str.Append("Min Pattern Length: " + MinLength + "\n");
			str.Append("Max Pattern Length: " + MaxLength + "\n");

			return str.ToString();
		}

	}
}
