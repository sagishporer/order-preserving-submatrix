using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace OPSM
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button buttonBrowseDataset;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RichTextBox richTextBoxResults;
		private System.Windows.Forms.TextBox textBoxDatasetPath;
		private System.Windows.Forms.TextBox textBoxSupport;
		private System.Windows.Forms.TextBox textBoxMinLength;
		private System.Windows.Forms.TextBox textBoxMaxLength;
		private System.Windows.Forms.Button buttonMine;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textBoxMaxErrors;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.RadioButton radioButtonOPSM;
		private System.Windows.Forms.RadioButton radioButtonWithError;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox textBoxMinGroups;
		private System.Windows.Forms.RadioButton radioButtonTreePattern;
		private System.Windows.Forms.RadioButton radioButtonGroups;
		private System.Windows.Forms.RadioButton radioButtonLayers;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox textBoxMaxLayerDiff;
		private System.Windows.Forms.CheckBox checkBoxUseInCoreDualCompare;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.CheckBox checkBoxInputColumnHeaders;
		private System.Windows.Forms.CheckBox checkBoxInputRowHeaders;
		private System.Windows.Forms.CheckBox checkBoxWriteOutput;
		private System.Windows.Forms.CheckBox checkBoxWriteAllResults;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			EnableTextBoxes();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.richTextBoxResults = new System.Windows.Forms.RichTextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.buttonBrowseDataset = new System.Windows.Forms.Button();
			this.textBoxDatasetPath = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textBoxSupport = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.textBoxMinLength = new System.Windows.Forms.TextBox();
			this.textBoxMaxLength = new System.Windows.Forms.TextBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.textBoxMaxLayerDiff = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.textBoxMinGroups = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.textBoxMaxErrors = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.buttonMine = new System.Windows.Forms.Button();
			this.radioButtonOPSM = new System.Windows.Forms.RadioButton();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.radioButtonGroups = new System.Windows.Forms.RadioButton();
			this.radioButtonTreePattern = new System.Windows.Forms.RadioButton();
			this.radioButtonWithError = new System.Windows.Forms.RadioButton();
			this.radioButtonLayers = new System.Windows.Forms.RadioButton();
			this.checkBoxUseInCoreDualCompare = new System.Windows.Forms.CheckBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.checkBoxWriteOutput = new System.Windows.Forms.CheckBox();
			this.checkBoxInputRowHeaders = new System.Windows.Forms.CheckBox();
			this.checkBoxInputColumnHeaders = new System.Windows.Forms.CheckBox();
			this.checkBoxWriteAllResults = new System.Windows.Forms.CheckBox();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// richTextBoxResults
			// 
			this.richTextBoxResults.Location = new System.Drawing.Point(8, 232);
			this.richTextBoxResults.Name = "richTextBoxResults";
			this.richTextBoxResults.ReadOnly = true;
			this.richTextBoxResults.Size = new System.Drawing.Size(512, 160);
			this.richTextBoxResults.TabIndex = 0;
			this.richTextBoxResults.Text = "";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(56, 23);
			this.label1.TabIndex = 1;
			this.label1.Text = "Dataset";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// buttonBrowseDataset
			// 
			this.buttonBrowseDataset.Location = new System.Drawing.Point(432, 16);
			this.buttonBrowseDataset.Name = "buttonBrowseDataset";
			this.buttonBrowseDataset.Size = new System.Drawing.Size(64, 23);
			this.buttonBrowseDataset.TabIndex = 2;
			this.buttonBrowseDataset.Text = "Browse...";
			this.buttonBrowseDataset.Click += new System.EventHandler(this.buttonBrowseDataset_Click);
			// 
			// textBoxDatasetPath
			// 
			this.textBoxDatasetPath.Location = new System.Drawing.Point(72, 16);
			this.textBoxDatasetPath.Name = "textBoxDatasetPath";
			this.textBoxDatasetPath.Size = new System.Drawing.Size(360, 20);
			this.textBoxDatasetPath.TabIndex = 3;
			this.textBoxDatasetPath.Text = "";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 16);
			this.label2.Name = "label2";
			this.label2.TabIndex = 4;
			this.label2.Text = "Support";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textBoxSupport
			// 
			this.textBoxSupport.Location = new System.Drawing.Point(80, 16);
			this.textBoxSupport.Name = "textBoxSupport";
			this.textBoxSupport.Size = new System.Drawing.Size(48, 20);
			this.textBoxSupport.TabIndex = 5;
			this.textBoxSupport.Text = "9999";
			this.textBoxSupport.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(136, 16);
			this.label3.Name = "label3";
			this.label3.TabIndex = 6;
			this.label3.Text = "Min. Length";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(312, 16);
			this.label4.Name = "label4";
			this.label4.TabIndex = 7;
			this.label4.Text = "Max. Length";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textBoxMinLength
			// 
			this.textBoxMinLength.Location = new System.Drawing.Point(208, 16);
			this.textBoxMinLength.Name = "textBoxMinLength";
			this.textBoxMinLength.Size = new System.Drawing.Size(96, 20);
			this.textBoxMinLength.TabIndex = 8;
			this.textBoxMinLength.Text = "0";
			this.textBoxMinLength.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textBoxMaxLength
			// 
			this.textBoxMaxLength.Location = new System.Drawing.Point(400, 16);
			this.textBoxMaxLength.Name = "textBoxMaxLength";
			this.textBoxMaxLength.Size = new System.Drawing.Size(96, 20);
			this.textBoxMaxLength.TabIndex = 9;
			this.textBoxMaxLength.Text = "999";
			this.textBoxMaxLength.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.textBoxMaxLayerDiff);
			this.groupBox1.Controls.Add(this.label7);
			this.groupBox1.Controls.Add(this.textBoxMinGroups);
			this.groupBox1.Controls.Add(this.label6);
			this.groupBox1.Controls.Add(this.textBoxMinLength);
			this.groupBox1.Controls.Add(this.textBoxMaxLength);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.textBoxSupport);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.textBoxMaxErrors);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Location = new System.Drawing.Point(8, 152);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(512, 72);
			this.groupBox1.TabIndex = 10;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Requierments";
			// 
			// textBoxMaxLayerDiff
			// 
			this.textBoxMaxLayerDiff.Location = new System.Drawing.Point(400, 40);
			this.textBoxMaxLayerDiff.Name = "textBoxMaxLayerDiff";
			this.textBoxMaxLayerDiff.Size = new System.Drawing.Size(96, 20);
			this.textBoxMaxLayerDiff.TabIndex = 15;
			this.textBoxMaxLayerDiff.Text = "0";
			this.textBoxMaxLayerDiff.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(312, 40);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(88, 23);
			this.label7.TabIndex = 14;
			this.label7.Text = "Max. Layer Diff.";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textBoxMinGroups
			// 
			this.textBoxMinGroups.Location = new System.Drawing.Point(224, 40);
			this.textBoxMinGroups.Name = "textBoxMinGroups";
			this.textBoxMinGroups.Size = new System.Drawing.Size(80, 20);
			this.textBoxMinGroups.TabIndex = 13;
			this.textBoxMinGroups.Text = "0";
			this.textBoxMinGroups.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(136, 40);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(88, 23);
			this.label6.TabIndex = 12;
			this.label6.Text = "Max Group Len";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textBoxMaxErrors
			// 
			this.textBoxMaxErrors.Location = new System.Drawing.Point(80, 40);
			this.textBoxMaxErrors.Name = "textBoxMaxErrors";
			this.textBoxMaxErrors.Size = new System.Drawing.Size(48, 20);
			this.textBoxMaxErrors.TabIndex = 11;
			this.textBoxMaxErrors.Text = "0";
			this.textBoxMaxErrors.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(8, 40);
			this.label5.Name = "label5";
			this.label5.TabIndex = 10;
			this.label5.Text = "Max. Errors";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// buttonMine
			// 
			this.buttonMine.Location = new System.Drawing.Point(528, 216);
			this.buttonMine.Name = "buttonMine";
			this.buttonMine.Size = new System.Drawing.Size(120, 23);
			this.buttonMine.TabIndex = 11;
			this.buttonMine.Text = "Mine";
			this.buttonMine.Click += new System.EventHandler(this.buttonMine_Click);
			// 
			// radioButtonOPSM
			// 
			this.radioButtonOPSM.Checked = true;
			this.radioButtonOPSM.Location = new System.Drawing.Point(8, 16);
			this.radioButtonOPSM.Name = "radioButtonOPSM";
			this.radioButtonOPSM.TabIndex = 13;
			this.radioButtonOPSM.TabStop = true;
			this.radioButtonOPSM.Text = "OPSM";
			this.radioButtonOPSM.CheckedChanged += new System.EventHandler(this.radioButtonOPSM_CheckedChanged);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.radioButtonGroups);
			this.groupBox2.Controls.Add(this.radioButtonTreePattern);
			this.groupBox2.Controls.Add(this.radioButtonWithError);
			this.groupBox2.Controls.Add(this.radioButtonOPSM);
			this.groupBox2.Controls.Add(this.radioButtonLayers);
			this.groupBox2.Controls.Add(this.checkBoxUseInCoreDualCompare);
			this.groupBox2.Location = new System.Drawing.Point(528, 8);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(120, 200);
			this.groupBox2.TabIndex = 14;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Algorithm";
			// 
			// radioButtonGroups
			// 
			this.radioButtonGroups.Location = new System.Drawing.Point(8, 88);
			this.radioButtonGroups.Name = "radioButtonGroups";
			this.radioButtonGroups.TabIndex = 16;
			this.radioButtonGroups.Text = "Groups";
			this.radioButtonGroups.CheckedChanged += new System.EventHandler(this.radioButtonGroups_CheckedChanged);
			// 
			// radioButtonTreePattern
			// 
			this.radioButtonTreePattern.Location = new System.Drawing.Point(8, 64);
			this.radioButtonTreePattern.Name = "radioButtonTreePattern";
			this.radioButtonTreePattern.TabIndex = 15;
			this.radioButtonTreePattern.Text = "Tree Pattern";
			this.radioButtonTreePattern.CheckedChanged += new System.EventHandler(this.radioButtonTreePattern_CheckedChanged);
			// 
			// radioButtonWithError
			// 
			this.radioButtonWithError.Location = new System.Drawing.Point(8, 40);
			this.radioButtonWithError.Name = "radioButtonWithError";
			this.radioButtonWithError.TabIndex = 14;
			this.radioButtonWithError.Text = "With Error";
			this.radioButtonWithError.CheckedChanged += new System.EventHandler(this.radioButtonWithError_CheckedChanged);
			// 
			// radioButtonLayers
			// 
			this.radioButtonLayers.Location = new System.Drawing.Point(8, 112);
			this.radioButtonLayers.Name = "radioButtonLayers";
			this.radioButtonLayers.TabIndex = 17;
			this.radioButtonLayers.Text = "Layers";
			this.radioButtonLayers.CheckedChanged += new System.EventHandler(this.radioButtonLayers_CheckedChanged);
			// 
			// checkBoxUseInCoreDualCompare
			// 
			this.checkBoxUseInCoreDualCompare.Checked = true;
			this.checkBoxUseInCoreDualCompare.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxUseInCoreDualCompare.Location = new System.Drawing.Point(8, 144);
			this.checkBoxUseInCoreDualCompare.Name = "checkBoxUseInCoreDualCompare";
			this.checkBoxUseInCoreDualCompare.Size = new System.Drawing.Size(104, 40);
			this.checkBoxUseInCoreDualCompare.TabIndex = 15;
			this.checkBoxUseInCoreDualCompare.Text = "Use in-core dual compare";
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.checkBoxWriteAllResults);
			this.groupBox3.Controls.Add(this.checkBoxWriteOutput);
			this.groupBox3.Controls.Add(this.checkBoxInputRowHeaders);
			this.groupBox3.Controls.Add(this.checkBoxInputColumnHeaders);
			this.groupBox3.Controls.Add(this.label1);
			this.groupBox3.Controls.Add(this.textBoxDatasetPath);
			this.groupBox3.Controls.Add(this.buttonBrowseDataset);
			this.groupBox3.Location = new System.Drawing.Point(8, 8);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(512, 144);
			this.groupBox3.TabIndex = 16;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Input / Output";
			// 
			// checkBoxWriteOutput
			// 
			this.checkBoxWriteOutput.Checked = true;
			this.checkBoxWriteOutput.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxWriteOutput.Location = new System.Drawing.Point(16, 88);
			this.checkBoxWriteOutput.Name = "checkBoxWriteOutput";
			this.checkBoxWriteOutput.Size = new System.Drawing.Size(184, 24);
			this.checkBoxWriteOutput.TabIndex = 6;
			this.checkBoxWriteOutput.Text = "Write output files";
			this.checkBoxWriteOutput.CheckedChanged += new System.EventHandler(this.checkBoxWriteOutput_CheckedChanged);
			// 
			// checkBoxInputRowHeaders
			// 
			this.checkBoxInputRowHeaders.Location = new System.Drawing.Point(16, 64);
			this.checkBoxInputRowHeaders.Name = "checkBoxInputRowHeaders";
			this.checkBoxInputRowHeaders.Size = new System.Drawing.Size(184, 24);
			this.checkBoxInputRowHeaders.TabIndex = 5;
			this.checkBoxInputRowHeaders.Text = "Input contains row headers";
			// 
			// checkBoxInputColumnHeaders
			// 
			this.checkBoxInputColumnHeaders.Location = new System.Drawing.Point(16, 40);
			this.checkBoxInputColumnHeaders.Name = "checkBoxInputColumnHeaders";
			this.checkBoxInputColumnHeaders.Size = new System.Drawing.Size(184, 24);
			this.checkBoxInputColumnHeaders.TabIndex = 4;
			this.checkBoxInputColumnHeaders.Text = "Input contains column headers";
			// 
			// checkBoxWriteAllResults
			// 
			this.checkBoxWriteAllResults.Location = new System.Drawing.Point(16, 112);
			this.checkBoxWriteAllResults.Name = "checkBoxWriteAllResults";
			this.checkBoxWriteAllResults.Size = new System.Drawing.Size(184, 24);
			this.checkBoxWriteAllResults.TabIndex = 7;
			this.checkBoxWriteAllResults.Text = "Write all results";
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(658, 399);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.buttonMine);
			this.Controls.Add(this.richTextBoxResults);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Form1";
			this.Text = "OPSM Miner";
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonExit_Click(object sender, System.EventArgs e)
		{
			Close();
		}

		private void buttonBrowseDataset_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "All files (*.*)|*.*" ;
			openFileDialog.FilterIndex = 2 ;

			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				textBoxDatasetPath.Text = openFileDialog.FileName;
			}
		}

		private void EnabledCheckBoxes()
		{
			if (this.checkBoxWriteOutput.Checked == false)
				this.checkBoxWriteAllResults.Enabled = false;
			else
				this.checkBoxWriteAllResults.Enabled = true;
		}

		#region radioButtons
		private void radioButtonOPSM_CheckedChanged(object sender, System.EventArgs e)
		{
			EnableTextBoxes();
		}

		private void radioButtonWithError_CheckedChanged(object sender, System.EventArgs e)
		{
			EnableTextBoxes();
		}

		private void radioButtonTreePattern_CheckedChanged(object sender, System.EventArgs e)
		{
			EnableTextBoxes();
		}

		private void radioButtonGroups_CheckedChanged(object sender, System.EventArgs e)
		{
			EnableTextBoxes();
		}

		private void radioButtonLayers_CheckedChanged(object sender, System.EventArgs e)
		{
			EnableTextBoxes();
		}

		private void EnableTextBoxes()
		{
			if (this.radioButtonGroups.Checked == true)
			{
				this.textBoxMaxErrors.Enabled = false;
				this.textBoxMaxLayerDiff.Enabled = false;
				this.textBoxMinGroups.Enabled = true;
			}
			else if (this.radioButtonLayers.Checked == true)
			{
				this.textBoxMaxErrors.Enabled = false;
				this.textBoxMaxLayerDiff.Enabled = true;
				this.textBoxMinGroups.Enabled = false;
			}
			else if ((this.radioButtonOPSM.Checked == true)||(this.radioButtonTreePattern.Checked == true))
			{
				this.textBoxMaxErrors.Enabled = false;
				this.textBoxMaxLayerDiff.Enabled = false;
				this.textBoxMinGroups.Enabled = false;
			}
			else if (this.radioButtonWithError.Checked == true)
			{
				this.textBoxMaxErrors.Enabled = true;
				this.textBoxMaxLayerDiff.Enabled = false;
				this.textBoxMinGroups.Enabled = false;
			}
		}


		#endregion

		private void buttonMine_Click(object sender, System.EventArgs e)
		{
			MinerParams minerParams = new MinerParams();

			if (textBoxDatasetPath.Text == "")
			{
				MessageBox.Show("No dataset selected", "Dataset Error",
					MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}

			minerParams.DatasetFileName = textBoxDatasetPath.Text;
			minerParams.InputContainsColumnHeaders = this.checkBoxInputColumnHeaders.Checked;
			minerParams.InputContainsRowHeaders = this.checkBoxInputRowHeaders.Checked;

			minerParams.WriteOutputFiles = this.checkBoxWriteOutput.Checked;
			minerParams.WriteAllResults = this.checkBoxWriteAllResults.Checked;

			minerParams.MinSupport = Int32.Parse(textBoxSupport.Text);
			minerParams.MinLength = Int32.Parse(textBoxMinLength.Text);
			minerParams.MaxLength = Int32.Parse(textBoxMaxLength.Text);

			minerParams.MaxErrors = Int32.Parse(textBoxMaxErrors.Text);
			minerParams.MinGroups = Int32.Parse(textBoxMinGroups.Text);
			minerParams.MaxLayerDiff = Int32.Parse(textBoxMaxLayerDiff.Text);

			minerParams.InCoreDualCompare = this.checkBoxUseInCoreDualCompare.Checked;

			if (radioButtonOPSM.Checked == true)
				minerParams.Algorithm = Algorithm.OPSM;
			else if (radioButtonTreePattern.Checked == true)
				minerParams.Algorithm = Algorithm.TreePattern;
			else if (radioButtonWithError.Checked == true)
				minerParams.Algorithm = Algorithm.WithErrors;
			else if (radioButtonGroups.Checked == true)
				minerParams.Algorithm = Algorithm.Groups;
			else if (radioButtonLayers.Checked == true)
				minerParams.Algorithm = Algorithm.Layers;
			else
				throw new ArgumentException("Unsupported algorithm");

			buttonMine.Enabled = false;

			DateTime timeBefore = DateTime.Now;
			MineResults mineResult = null;
			richTextBoxResults.Clear();		
			
			mineResult = OPSMMain.DoMine(minerParams);

			if (mineResult != null)
			{
				try
				{
					richTextBoxResults.AppendText("Mining time : " + (DateTime.Now - timeBefore) + "\n");
					richTextBoxResults.AppendText("Total found " + mineResult.Count.ToString() + " patterns\n");
					richTextBoxResults.AppendText(mineResult.ToString());
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

			buttonMine.Enabled = true;
		}

		private void checkBoxWriteOutput_CheckedChanged(object sender, System.EventArgs e)
		{
			EnabledCheckBoxes();
		}
	}
}
