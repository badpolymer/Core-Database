using System;
using System.Collections.Generic;
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
using Core_Database.Models;
using Realms;

namespace Core_Database.ViewsControllers
{
    /// <summary>
    /// Interaction logic for MultiLanguageTextEditor.xaml
    /// </summary>
    public partial class MultiLanguageTextEditor : Window
    {
        Realm? localRealm;
        private readonly string PN;
        private string columnName;
        private Guid ID;
        private bool isEditingExistingItem = false;
        private List<RealmObject> texts = new();

        
        /// <summary>
        /// Create a new MultiLanguage Text Editor for Creating New Texts, it will return a list of texts when the user click Save.
        /// </summary>
        public MultiLanguageTextEditor(string pn, string columnName, Guid id)
        {
            
            this.PN = pn;
            this.columnName = columnName;
            this.ID = id;

            InitializeComponent();

            PNTextBlock.Text = pn;
            ColumnTextBlock.Text = columnName;
            IDTextBlock.Text = ID.ToString().ToUpper();
            
        }

        //
        //For Existing Text
        //
        public MultiLanguageTextEditor(Realm localRealm, string ColumnName, IHasPN editingItem, Guid ID)
        {
            this.localRealm = localRealm;
            this.columnName = ColumnName;
            this.ID = ID;
            this.PN = editingItem.PN;
            this.isEditingExistingItem= true;

            InitializeComponent();
        }


        public List<RealmObject> ShowDialogWithOutput()
        {
            this.ShowDialog();


            return texts;
        }


        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string? ENUSText = ENUSTextBox.Text.Trim() == "" ? null : ENUSTextBox.Text.Trim();
            string? ENUKText = ENUKTextBox.Text.Trim() == "" ? null : ENUKTextBox.Text.Trim();
            string? CTNText = CTNTextBox.Text.Trim() == "" ? null : CTNTextBox.Text.Trim();
            string? TMDRText = TMDRTextbox.Text.Trim() == "" ? null : TMDRTextbox.Text.Trim();
            string? SMDRText = SMDRTextbox.Text.Trim() == "" ? null : SMDRTextbox.Text.Trim();

            if (ENUSText == null)
            {
                MessageBox.Show("Please at least enter a text for default", "Database error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!isEditingExistingItem)
            {
                //
                //Generate new texts
                //
                LocaleEnglishUS defaultText = new() { ID = ID, OwnerPN = PN, Text = ENUSText };
                texts.Add(defaultText);

                if (ENUKText == null)
                {
                    LocaleEnglishUK translate = new() { ID = ID, OwnerPN = PN };
                    texts.Add(translate);
                    
                }
                else
                {
                    LocaleEnglishUK translate = new() { ID = ID, OwnerPN = PN, Text = ENUKText };
                    texts.Add(translate);
                }

                this.Close();

            }
            

        }
    }
}
