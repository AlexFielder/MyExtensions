using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DumpiLogicRules
{
    /// <summary>
    /// Interaction logic for ModifyRulesWindow.xaml
    /// </summary>
    public partial class ModifyRulesWindow : Window
    {
        //Public Property listofRulesToModify As New List(Of RuleType)
        //Public Property listofRulesToModify As New ObservableCollection(Of RuleType)
        private DirtyCollection<RuleType> _listofRulesToModify = new DirtyCollection<RuleType>();
        public DirtyCollection<RuleType> listofRulesToModify
        {
            get { return  _listofRulesToModify; }
            set
            {
                //if ( _listofRulesToModify != null)
                //{
                //    ((INotifyPropertyChanged)_listofRulesToModify).PropertyChanged -= ListofRulesToModify_PropertyChanged;
                //     _listofRulesToModify.PropertyChanged -= ListofRulesToModify_PropertyChanged;
                //}
                // _listofRulesToModify = value;
                //if ( _listofRulesToModify != null)
                //{
                //     -listofRulesToModify.PropertyChanged += ListofRulesToModify_PropertyChanged;
                //}
            }
        }
        public List<RuleType> listofModifiedRules { get; set; }

        public ModifyRulesWindow()
        {
            InitializeComponent();
            listofRulesToModify = DumpiLogicRules.listofiLogicRules;
            CollectionViewSource itemCollectionViewSource = FindResource("ItemCollectionViewSource") as CollectionViewSource;
            itemCollectionViewSource.Source = DumpiLogicRules.listofiLogicRules;
            //dataGrid.ItemsSource = listofRulesToModify
            // Add any initialization after the InitializeComponent() call.
        }

        private void buttonSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            if (listofRulesToModify.IsDirty)
            {

                foreach (RuleType modifiedrule in listofRulesToModify)
                {
                }
            }


            foreach (RuleType modifiedrule in listofRulesToModify)
            {
            }
            //For Each modifiedrule As RuleType In listofRulesToModify
            //    If modifiedrule.PropertyChanged Then

            //    End If
            //    'If modifiedrule.IsDirty Then
            //    '    listofModifiedRules.Add(modifiedrule)
            //    'End If
            //Next
            //listofModifiedRules = (From a As RuleType In listofRulesToModify
            //                       Where a.IsDirty = True
            //                       Select a).ToList()
            MessageBox.Show("Hello World!", "Modify Rules.");
            this.Close();
        }


        private void ListofRulesToModify_PropertyChanged(RuleType sender, PropertyChangedEventArgs e)
        {
            //something changed so add the sender to the listofmodifiedrules
            listofModifiedRules.Add(sender);
        }

    }
}
