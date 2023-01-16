using Core_Database.Models;
using Realms;
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

namespace Core_Database.ViewsControllers
{
    /// <summary>
    /// Interaction logic for Add_Tool_Main_Type.xaml
    /// </summary>
    public partial class Add_Tool_Main_Type : Window
    {
        private Realm localRealm;
        private RealmObject? output;

        public Add_Tool_Main_Type(Realm localRealm)
        {
            this.localRealm = localRealm;

            InitializeComponent();
            
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string typeName = TypeNameTextBox.Text.Trim();

            if (typeName == "")
            {
                MessageBox.Show("Please enter a name for the type.", "Editor Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                localRealm.Write(() =>
                {
                    ToolMainType newToolType= new() { Name = typeName };
                    localRealm.Add(newToolType);
                    output = newToolType;
                });

                
                MessageBox.Show($"Successfully create a new tool type({typeName}).", "Operation Info", MessageBoxButton.OK, MessageBoxImage.Information);

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public RealmObject? ShowDialogWithOutput()
        {
            this.ShowDialog();
            if (output != null)
            {
                return output;
            }
            return null;
        }

    }
}
