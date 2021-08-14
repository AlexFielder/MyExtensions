using DumpiLogicRules;
using Inventor;
using System;
using log4net;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Autodesk.iLogic.Interfaces;
using System.Windows.Forms;
using System.Windows.Interop;

namespace DumpiLogicRules
{
    public partial class DumpiLogicRulesButtonActions
	{
		public static iLogicRuleType rulesListObject = null;

		public static DirtyCollection<RuleType> listofiLogicRules;

		public static string ruleslistFileName = string.Empty;
		public static bool UseListOfRules = false;

		public static readonly ILog log = LogManager.GetLogger(typeof(DumpiLogicRulesButtonActions));
		public static void Button1_Execute()
		{
			//tests our connection to our logger.
			//DumpiLogicRules.log.Debug("Button Extension button was clicked!");
			//shows the user a messagebox
			//MessageBox.Show("Hello from the first button extension!");
			//tests our connection to the Inventor application itself.
			//Reporting reporter = new Reporting();
			//reporter.UpdateStatusBar("Hello from the first button extension!");
			//DockableWindow dock = null;
			try
			{

				RunRulesDump(null);
				//MessageBox.Show("Hello world", "Central Item Numbering");
			}
			catch (Exception ex)
			{
				log.Error(ex.Message);
			}

		}

		#region "RulesDump"
		/// <summary>
		/// Dumps all iLogic rules found in an Assembly and sub-assemblies/parts.
		/// </summary>
		/// <param name="Context"></param>
		public static void RunRulesDump(NameValueMap Context)
		{
			//MessageBox.Show("Hello World!") '<-debugging!

			UseListOfRules = false;
			if (DumpiLogicRules.m_InventorApp.ActiveDocument is AssemblyDocument)
			{
				AssemblyDocument oDoc = (AssemblyDocument)DumpiLogicRules.m_InventorApp.ActiveDocument;
				if (!(oDoc.FullFileName == string.Empty))
				{
					ruleslistFileName = System.IO.Path.GetDirectoryName(oDoc.FullFileName) + "\\" + System.IO.Path.GetFileNameWithoutExtension(oDoc.FullFileName) + "-iLogicRules.xml";
					BeginCollectiLogicRules(DumpiLogicRules.m_InventorApp.ActiveDocument);
				}
				else
				{
					MessageBox.Show("The file needs to be saved somewhere before we can run this tool!");
					return;
				}
			}
			else if (DumpiLogicRules.m_InventorApp.ActiveDocument is PartDocument)
			{
				PartDocument oDoc = (PartDocument)DumpiLogicRules.m_InventorApp.ActiveDocument;
				if (!(oDoc.FullFileName == string.Empty))
				{
					ruleslistFileName = System.IO.Path.GetDirectoryName(oDoc.FullFileName) + "\\" + System.IO.Path.GetFileNameWithoutExtension(oDoc.FullFileName) + "-iLogicRules.xml";
					PartDocument thisPart = (PartDocument)DumpiLogicRules.m_InventorApp.ActiveDocument;
					BeginCollectiLogicRules(thisPart);
				}
				else
				{
					MessageBox.Show("The file needs to be saved somewhere before we can run this tool!");
					return;
				}
			}
			else if (DumpiLogicRules.m_InventorApp.ActiveDocument is DrawingDocument)
			{
				DrawingDocument oDoc = (DrawingDocument)DumpiLogicRules.m_InventorApp.ActiveDocument;
				if (!(oDoc.FullFileName == string.Empty))
				{
					ruleslistFileName = System.IO.Path.GetDirectoryName(oDoc.FullFileName) + "\\" + System.IO.Path.GetFileNameWithoutExtension(oDoc.FullFileName) + "-iLogicRules.xml";
					DrawingDocument thisDrawing = (DrawingDocument)DumpiLogicRules.m_InventorApp.ActiveDocument;
					BeginCollectiLogicRules(thisDrawing);
				}
				else
				{
					MessageBox.Show("The file needs to be saved somewhere before we can run this tool!");
					return;
				}

			}
			else if (DumpiLogicRules.m_InventorApp.ActiveDocument is PresentationDocument)
			{
				throw new Exception("NOT YET IMPLEMENTED!");
			}

			rulesListObject.SaveToFile(ruleslistFileName);
			MessageBox.Show("Rules Extraction Complete" + ruleslistFileName, "Rules filename copied to clipboard!");

			Clipboard.SetText(ruleslistFileName);
		}



		#endregion
		#region "ExciseRules"
		/// <summary>
		/// Will eventually perform an extraction of rules in sub-parts/assemblies to an external rules location
		/// </summary>
		/// <param name="Context"></param>
		public static void ExciseRules(NameValueMap Context)
		{
			MessageBox.Show("Hello World", "Rule Excisor");
		}
		#endregion
		#region "Rules Collection"

		/// <summary>
		/// Starts the collection of the iLogic rules present in this Assembly, Sub-assemblies and Sub-Parts.
		/// </summary>
		/// <param name="oDoc"></param>
		public static void BeginCollectiLogicRules(Document oDoc)
		{
			//refactor here
			IiLogicAutomation iLogicAuto = ProcessFiles.GetiLogicAutomation(DumpiLogicRules.m_InventorApp);
			rulesListObject = new iLogicRuleType();
			BuildRuleColl(oDoc, iLogicAuto);
			// Loop through all referenced docs in assembly
			AssemblyDocument assyDoc = (AssemblyDocument)oDoc;
			AssemblyComponentDefinition asmCompDef = assyDoc.ComponentDefinition;
			LevelOfDetailRepresentation currentLoD = asmCompDef.RepresentationsManager.ActiveLevelOfDetailRepresentation;
			//need to set the level of detail to Master and then back to whatever it was!
			ProcessFiles.UpdateStatusBar("Beginning iLogic Rules Dump");

			asmCompDef.RepresentationsManager.LevelOfDetailRepresentations["Master"].Activate();

			double percent = 0;
			int rulesInt = 0;
			foreach (Document oSubDoc in oDoc.AllReferencedDocuments)
			{
				// Build collection of rules
				percent = (Convert.ToDouble(rulesInt) / oDoc.AllReferencedDocuments.Count);
				ProcessFiles.UpdateStatusBar(percent, "iLogic Rules Excision Progress = ");

				BuildRuleColl(oSubDoc, iLogicAuto);
				rulesInt += 1;
			}
			//should be the "iLogic" level of detail that we need to reactivate here:
			asmCompDef.RepresentationsManager.LevelOfDetailRepresentations[currentLoD.Name].Activate();
		}

		/// <summary>
		/// This overload allows the tool to function on part files.
		/// </summary>
		/// <param name="oPartDoc"></param>
		public static void BeginCollectiLogicRules(PartDocument oPartDoc)
		{
			IiLogicAutomation iLogicAuto = ProcessFiles.GetiLogicAutomation(DumpiLogicRules.m_InventorApp);
			rulesListObject = new iLogicRuleType();
			BuildRuleColl((Document)oPartDoc, iLogicAuto);
			ProcessFiles.UpdateStatusBar("Beginning iLogic Rules Dump");
		}

		/// <summary>
		/// Starts running the collect rules method for any Inventor Drawing.
		/// </summary>
		/// <param name="thisDrawing"></param>
		public static void BeginCollectiLogicRules(DrawingDocument thisDrawing)
		{
			IiLogicAutomation iLogicAuto = ProcessFiles.GetiLogicAutomation(DumpiLogicRules.m_InventorApp);
			rulesListObject = new iLogicRuleType();
			BuildRuleColl((Document)thisDrawing, iLogicAuto);
			ProcessFiles.UpdateStatusBar("Beginning iLogic Rules Dump");
		}

		/// <summary>
		/// Adds iLogic rules from each document passed to it to the global rulelist.
		/// </summary>
		/// <param name="oDoc"></param>
		/// <param name="iLogicAuto"></param>
		public static void BuildRuleColl(Document oDoc, IiLogicAutomation iLogicAuto)
		{
			string ruleName = string.Empty;
			IEnumerable rules = iLogicAuto.get_Rules(oDoc);

			if ((rules != null))
			{
				foreach (iLogicRule rule in rules)
				{
					RuleType thisrule = new RuleType();
					thisrule.Name = rule.Name;
					thisrule.IsActive = rule.IsActive;
					thisrule.FireDependentImmediately = rule.FireDependentImmediately;
					thisrule.AutomaticOnParameterChange = rule.AutomaticOnParamChange;
					thisrule.Silentoperation = rule.SilentOperation;
					thisrule.Text = rule.Text;
					thisrule.Text.Replace("&gt;", ">");
					thisrule.Text.Replace("&lt;", "<");
					thisrule.Text.Replace("&amp;", "&");
					thisrule.ParentFileName = oDoc.FullFileName;
					//'set this to false otherwise the collection is completely dirty when we try to modify anything.
					//thisrule.IsDirty = False
					//this is removing files where the file already exists in the .xml output.
					//what we should do instead is look for rules with the same name and then remove the duplicates
					RuleType foundexistingrule = (from anEntry in rulesListObject.Rule
												  where anEntry.ParentFileName == oDoc.FullFileName & anEntry.Name == thisrule.Name
												  select anEntry).FirstOrDefault();
					if ((foundexistingrule != null))
					{
						//remove existing entries for this document
						rulesListObject.Rule.RemoveAll(X => X.ParentFileName.Contains(oDoc.FullFileName));
						if (UseListOfRules)
						{
							listofiLogicRules.Add(thisrule);
						}
						else
						{
							rulesListObject.Rule.Add(thisrule);
						}
					}
					else
					{
						if (UseListOfRules)
						{
							listofiLogicRules.Add(thisrule);
						}
						else
						{
							rulesListObject.Rule.Add(thisrule);
						}
					}
				}
			}
		}

		#endregion
		#region "Modify Rules"
		/// <summary>
		/// Modifies the rules present in this part or assembly (including sub parts!)
		/// once the rules have been modified it *should* push the changes back into the parts but doesn't display correctly currently. AF 2016-05-11
		/// </summary>
		/// <param name="Context"></param>
		private static void RunModifyRules(NameValueMap Context)
		{
			try
			{
				Document oDoc = DumpiLogicRules.m_InventorApp.ActiveDocument;
				UseListOfRules = true;
				listofiLogicRules = new DirtyCollection<RuleType>();
				//listofiLogicRules = New List(Of RuleType)
				BeginCollectiLogicRules(oDoc);
				ModifyRulesWindow modifyRulesForm = new ModifyRulesWindow();
				WindowInteropHelper helper = new WindowInteropHelper(modifyRulesForm);
				helper.Owner = new IntPtr(DumpiLogicRules.m_InventorApp.MainFrameHWND);
				modifyRulesForm.ShowDialog();
				//update any modified rules.
				if (modifyRulesForm.listofModifiedRules.Count > 0)
				{
					UpdateModifiedRules(modifyRulesForm.listofModifiedRules);
				}

			}
			catch (Exception ex)
			{
			}
		}

		/// <summary>
		/// Updates a given list of modified iLogic rules.
		/// </summary>
		/// <param name="listofrulestoUpdate"></param>
		public static void UpdateModifiedRules(List<RuleType> listofrulestoUpdate)
		{
			IiLogicAutomation iLogicAuto = ProcessFiles.GetiLogicAutomation(DumpiLogicRules.m_InventorApp);
			//MessageBox.Show("Count of modified rules is: " & listofrulestoUpdate.Count.ToString())
			//need to sort the list
			listofrulestoUpdate.OrderBy(x => x.ParentFileName);
			List<string> listoffilestoedit = (from a in listofrulestoUpdate
											  select a.ParentFileName).Distinct().ToList();
			foreach (string filetoedit in listoffilestoedit)
			{
				Document oDoc = null;
				if (filetoedit == DumpiLogicRules.m_InventorApp.ActiveDocument.FullFileName)
				{
					oDoc = DumpiLogicRules.m_InventorApp.ActiveDocument;
				}
				else
				{
					oDoc = DumpiLogicRules.m_InventorApp.Documents.Open(filetoedit);
				}

				IEnumerable rules = iLogicAuto.get_Rules(oDoc);
				List<RuleType> rulestomodify = (from a in listofrulestoUpdate
												where a.ParentFileName == filetoedit
												select a).ToList();
				foreach (iLogicRule rule in rules)
				{
					foreach (RuleType modifiedrule in rulestomodify)
					{
						if (modifiedrule.Name == rule.Name)
						{
							rule.IsActive = modifiedrule.IsActive;
							rule.FireDependentImmediately = modifiedrule.FireDependentImmediately;
							rule.SilentOperation = modifiedrule.Silentoperation;
							rule.AutomaticOnParamChange = modifiedrule.AutomaticOnParameterChange;
						}
					}
				}
				oDoc.Close(false);
			}
			//Dim grouped = listofrulestoUpdate.GroupBy(Function(x) x.ParentFileName)
			//For Each filenameToEdit As Object In grouped
			//    DumpiLogicRules.m_InventorApp.Documents.Open(filenameToEdit.ToString())
			//    For Each ruletomodify As RuleType In filenameToEdit

			//    Next

			//Next
		}
		#endregion
		#region "Simplify Rules"
		/// <summary>
		/// Will one day peruse a set of rules and refactor them to simplify things.
		/// </summary>
		/// <param name="Context"></param>
		private static void SimplifyRules(NameValueMap Context)
		{
			MessageBox.Show("Hello World", "Rule Simplification");
		}
		#endregion

	}
}