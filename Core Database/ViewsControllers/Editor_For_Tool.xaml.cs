using Core_Database.Models;
using Core_Database.ViewsControllers;
using Realms;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
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
using System.Windows.Shapes;

namespace Core_Database.ViewsControllers
{
    /// <summary>
    /// Interaction logic for Editor_For_Tool.xaml
    /// </summary>
    public partial class Editor_For_Tool : Window
    {
        private Realm localRealm;
        private bool isEditing = false;
        private Tool? editingTool;
        private MainWindow? mainWindow;
        private bool isCopying = false;
        private LocaleManipulator localeManipulator;
        private List<RealmObject> localeTexts = new List<RealmObject>();
        private Guid shortDescID;
        


        //
        //For Add New Tool Only
        public Editor_For_Tool(Realm realm)
        {
            localRealm = realm;
            localeManipulator = new(localRealm); 
            shortDescID = localeManipulator.NewGuid();

            InitializeComponent();

            DeleteButton.IsEnabled = false;

            LoadToolTypes();

            ShortDescTextBlock.Text = "Not Created Yet.";
            DetailDescTextBlock.Text = "Not Created Yet.";


        }


        //For Editing Existing Record or Copy An Existing Record
        public Editor_For_Tool(Realm realm, Object editingTool, MainWindow mainWindow, bool isCopying)
        {
            localRealm = realm;
            isEditing = true;
            this.editingTool = editingTool as Tool;
            this.mainWindow = mainWindow;
            this.isCopying = isCopying;
            shortDescID = this.editingTool!.ShortDescription;

            InitializeComponent();

            if (!this.isCopying)
            {
                DeleteButton.IsEnabled = true;
            }
            
            LoadToolTypes();

            PNTextBox.Text = this.editingTool?.PN ?? "";
            TypeSelector.SelectedItem = this.editingTool?.Type;
            IsStandardCheckBox.IsChecked = this.editingTool?.IsStandard;
            ShortDescTextBlock.Text = localRealm.Find<LocaleEnglishUS>(shortDescID).Text;

        }

        private void LoadToolTypes()
        {
            TypeSelector.ItemsSource = localRealm.All<ToolMainType>();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string pn = PNTextBox.Text.Trim().ToUpper();
            var type = TypeSelector.SelectedItem;
            bool isStandard = IsStandardCheckBox.IsChecked ?? false;


            if (pn == "")
            {
                MessageBox.Show("Please enter a part number for the tool.", "Editor Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (type == null)
            {
                MessageBox.Show("Please select the type of the tool.", "Editor Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!isEditing)
            {
                //
                //Create New Tool
                //

                Tool newTool = new() { PN = pn, Type = (ToolMainType)type, IsStandard = isStandard, ShortDescription = shortDescID };

                foreach (var localeText in localeTexts)
                {
                    (localeText as IHasOwner).OwnerPN = pn;
                }

                try
                {
                    localRealm.Write(() => {
                        localRealm.Add(newTool);
                        foreach (var localeText in localeTexts)
                        {
                            localRealm.Add(localeText);
                        }
                    });

                    MessageBox.Show($"Successfully create a new tool(PN: {pn}).", "Operation Info", MessageBoxButton.OK, MessageBoxImage.Information);

                    this.Close();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            else if(!isCopying)
            {

                //
                //Editing Existing Tool
                //
                try
                {

                    localRealm.Write(() => {


                        if (editingTool?.PN == pn)
                        {
                            editingTool!.Type = (ToolMainType)type;
                            editingTool!.IsStandard = isStandard;
                        }
                        else
                        {
                            Tool newTool = new() { PN = pn, Type = (ToolMainType)type, IsStandard = isStandard };
                            localRealm.Remove(editingTool);
                            localRealm.Add(newTool);
                            mainWindow!.ContentList.UnselectAll();
                        }

                        

                    });

                    MessageBox.Show($"Successfully update a tool(PN: {pn}).", "Operation Info", MessageBoxButton.OK, MessageBoxImage.Information);

                    this.Close();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            else
            {
                //
                //Copy From Existing Tool
                //
                try
                {

                    localRealm.Write(() => {
      
                        Tool newTool = new() { PN = pn, Type = (ToolMainType)type, IsStandard = isStandard };
                        localRealm.Add(newTool);
                        mainWindow!.ContentList.UnselectAll();
                    });

                    MessageBox.Show($"Successfully save a tool(PN: {pn}).", "Operation Info", MessageBoxButton.OK, MessageBoxImage.Information);

                    this.Close();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddTypeButton_Click(object sender, RoutedEventArgs e)
        {
            var editor = new Add_Tool_Main_Type(localRealm);
            var newType = editor.ShowDialogWithOutput();
            TypeSelector.SelectedItem = newType;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (editingTool != null)
            {
                var result = MessageBox.Show($"Are you sure to delete the item(PN: {editingTool})?", "", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        string pn = editingTool.PN;

                        localRealm!.Write(() =>
                        {
                            localRealm.Remove(editingTool);
                        });

                        mainWindow?.ContentList.UnselectAll();
                        mainWindow?.ContentList.Items.Refresh();

                        MessageBox.Show($"Successfully save a tool(PN: {pn}).", "Operation Info", MessageBoxButton.OK, MessageBoxImage.Information);
                        
                        this.Close();

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Database error", MessageBoxButton.OK, MessageBoxImage.Error);

                    }
                }
            }
            
        }

        private void ShortDescEditButton_Click(object sender, RoutedEventArgs e)
        {
            PNTextBox.IsEnabled = false;
            var pn = PNTextBox.Text.Trim().ToUpper();
            


            if (!isEditing)
            {
                //
                //Create New Tool
                //
                var newTexts = localeManipulator.OpenEditorToGetNewTexts(pn, "Short Description",shortDescID);

                localeTexts.Concat(newTexts);


            }
            else if (!isCopying)
            {

                //
                //Editing Existing Tool
                //
                
            }
            else
            {
                //
                //Copy From Existing Tool
                //
                
            }


        }
    }
}
