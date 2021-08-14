using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Inventor;
using System.Windows.Forms;
using DRYHelpers;
using System.IO;
using Autodesk.iLogic.Interfaces;

namespace DumpiLogicRulesExtension
{
    

public class ProcessFiles
{
	/// <summary>
	/// Prompts the user to select a folder then processes the part files contained within
	/// </summary>
	/// <param name="Context"></param>
	public static void ProcessPartsinFolder(NameValueMap Context)
	{
		MessageBox.Show("Remember to log out of Vault or you're gonna have a bad day!\r\nRemember to unload the Feature recognizer addin Too!");
        IiLogicAutomation iLogicAuto = GetiLogicAutomation(DumpiLogicRulesExtension.m_InventorApp);
		System.Windows.Forms.FolderBrowserDialog folderbrowser = new System.Windows.Forms.FolderBrowserDialog();

		folderbrowser.RootFolder = System.Environment.SpecialFolder.UserProfile;
		//folderbrowser.RootFolder = getOneDriveFolderPath()
		folderbrowser.Description = "Select Folder to look for files to process.";
		folderbrowser.ShowDialog();
		string foldername = folderbrowser.SelectedPath;
		if (!(foldername == string.Empty)) {
			//now we can get/create a list of part files
			string rulename = SelectRuleToProcessPartsWith(CloudStorageHelpers.getDropBoxFolderPath() + "\\iLogic\\");
			if (!(rulename == string.Empty)) {
				System.IO.DirectoryInfo directory = new System.IO.DirectoryInfo(foldername);
				System.Collections.Generic.IEnumerable<FileInfo> partlisttoprocess = directory.GetFilesByExtensions(".ipt");

				int progressint = 1;
				double percent = 0;

				if ((partlisttoprocess != null)) {

					foreach (FileInfo partfile in partlisttoprocess) {
						percent = (Convert.ToDouble(progressint) / partlisttoprocess.Count());
						progressint += 1;

                            PartDocument partdoc = (PartDocument)DumpiLogicRulesExtension.m_InventorApp.Documents.Open(partfile.FullName);

						UpdateStatusBar(percent, "Processing: " + System.IO.Path.GetFileNameWithoutExtension(partdoc.File.FullFileName));
						//insert the name of the external rule you wish to run here:
						//iLogicAuto.RunExternalRule(partdocument, rulename)
						NameValueMap Args = DumpiLogicRulesExtension.m_InventorApp.TransientObjects.CreateNameValueMap();
						Args.Value["Filename"] = partdoc.FullFileName;
						iLogicAuto.RunExternalRuleWithArguments((Document)partdoc, rulename, Args);
						partdoc.Close();
					}
				}
			}
		}
	}

	/// <summary>
	/// Gets the iLogic Automation interface from the current Inventor application.
	/// </summary>
	/// <param name="app"></param>
	/// <returns></returns>
	public static IiLogicAutomation GetiLogicAutomation(Inventor.Application app)
	{

		Inventor.ApplicationAddIn addIn = null;
		try {
			addIn = app.ApplicationAddIns.ItemById["{3bdd8d79-2179-4b11-8a5a-257b1c0263ac}"];
		} catch (Exception ex) {
			return null;
		}

		return addIn.Automation;
	}

	public static string selectedRule = string.Empty;
	/// <summary>
	/// Displays to the user a Windows form containing a list of iLogic Rules.
	/// </summary>
	/// <param name="InventorExternalRulesFolderPath"></param>
	/// <returns></returns>
	public static string SelectRuleToProcessPartsWith(string InventorExternalRulesFolderPath)
	{
		System.Collections.Generic.IEnumerable<FileInfo> partlisttoprocess = default(System.Collections.Generic.IEnumerable<FileInfo>);
		//For Each foldername As String In InventorExternalRulesFolderPath
		System.IO.DirectoryInfo directory = new System.IO.DirectoryInfo(InventorExternalRulesFolderPath);
		partlisttoprocess = directory.GetFilesByExtensions(".iLogicVb");
		//Next
		//Dim RulesList As New ArrayList
		RulesList ruleslistform = new RulesList();

		foreach (System.IO.FileInfo file in partlisttoprocess) {
			ruleslistform.ListBox1.Items.Add(System.IO.Path.GetFileNameWithoutExtension(file.FullName));
			//RulesList.Add(System.IO.Path.GetFileNameWithoutExtension(file.FullName))
		}

		ruleslistform.ShowDialog(new WindowWrapper(new IntPtr(DumpiLogicRulesExtension.m_InventorApp.MainFrameHWND)));

		//Dim selectedfile As String = InputListBox("Prompt", RulesList, "", Title:="Select Rule from this list", ListName:="Available External Rules")


		//Try
		//    'oFileDlg.ShowOpen()
		//    selectedRule = System.IO.Path.GetFileNameWithoutExtension(selectedRule)
		//Catch
		//    Return Nothing 'operation was cancelled by the user
		//End Try
		return selectedRule;
	}
	#region "Status bar"
	/// <summary>
	/// updates the statusbar with a string value.
	/// </summary>
	/// <param name="Message"></param>
	/// <remarks></remarks>
	public static void UpdateStatusBar(string Message)
	{
		DumpiLogicRulesExtension.m_InventorApp.StatusBarText = Message;
	}

	/// <summary>
	/// Updates the statusbar with a percentage value
	/// </summary>
	/// <param name="percent"></param>
	/// <param name="Message"></param>
	/// <remarks></remarks>
	public static void UpdateStatusBar(double percent, string Message)
	{
		DumpiLogicRulesExtension.m_InventorApp.StatusBarText = Message + " (" + percent.ToString("P1") + ")";
	}
	#endregion
}



}
