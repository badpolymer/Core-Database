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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.WindowsAPICodePack.Dialogs;
using Microsoft.Win32;
using Realms;
using Realms.Exceptions;
using Core_Database.Models;
using System.Diagnostics;
using System.Xml.Serialization;
using Core_Database.ViewsControllers;

namespace Core_Database
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Realm? localRealm;
        
        string? CurrentSelectedMainItem { get { return ((ListBoxItem)MainList.SelectedItem)?.Content.ToString(); } }

        public MainWindow()
        {
            InitializeComponent();

        }

        //
        // Open A Realm File
        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new()
            {
                IsFolderPicker = true,
                InitialDirectory = "::{20D04FE0-3AEA-1069-A2D8-08002B30309D}",
            };

            CommonFileDialogResult result = dialog.ShowDialog();

            if (result == CommonFileDialogResult.Ok)
            {
                string selectedFilePath = dialog.FileName;


                RealmConfiguration config = new(selectedFilePath + "\\test.realm")
                {
                    //IsReadOnly = true,
                    SchemaVersion = 2,
                };

                try
                {
                    localRealm = Realm.GetInstance(config);

                    localRealm.Write(() =>
                    {
                        var type = localRealm.All<ToolMainType>().First();
                        var adding = new Tool { PN = "N2J", Type = type};
                        
                        //var addedType = new Test { Abc = "Asssll" };
                        // Add the instance to the realm.
                        //localRealm.Add(type);


                        //localRealm.Add(addedType);
                    });
                }
                catch (RealmFileAccessErrorException ex)
                {
                    MessageBox.Show(ex.Message, "Database error", MessageBoxButton.OK, MessageBoxImage.Error);
                }


                if (localRealm != null)
                {
                    fileLabel.Content = selectedFilePath;
                    OpenFileButton.Content = "Load Another File";
                    MainList.IsEnabled = true;
                }
                else
                {
                    MessageBox.Show("Unable to open database, try again.", "Database error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                
            }


        }

        

        
        //
        // Main list method
        private void MainList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox_SelectionChangedWithColor(sender);

            switch (CurrentSelectedMainItem)
            {
                case "Tool":
                    if (localRealm != null)
                    {
                 
                        TypeList0.ItemsSource = localRealm.All<ToolMainType>().OrderBy(t => t.Name);
                        ContentList.ItemsSource = localRealm.All<Tool>().OrderBy(t => t.PN);

                    }
                    break;
                default:
                        TypeList0.ItemsSource = null;
                        ContentList.ItemsSource = null;
                    break;
            }

            if (CurrentSelectedMainItem != null)
            {
                AddButton.IsEnabled = true;
            }
            else
            {
                AddButton.IsEnabled = false;
            }


        }

        //
        // Filter list method
        private void TypeList0_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox_SelectionChangedWithColor(sender);
            var typeList0 = sender as ListBox;
 
            if (typeList0?.SelectedItem != null)
            {
                

                switch (CurrentSelectedMainItem)
                {
                    case "Tool":
                        ContentList.ItemsSource = localRealm!.All<Tool>().Where(i =>  i.Type == typeList0.SelectedItem ).OrderBy(t => t.PN);
                        break;
                    default:
                        ContentList.ItemsSource = null;
                        break;
                }
                
            }
            else
            {
                MainList_SelectionChanged(MainList, e);

                //ContentList.ItemsSource = CurrentSelectedMainItem switch
                //{
                //    "Tool" => localRealm!.All<Tool>().OrderBy(t => t.PN),
                //    _ => null,
                //};
            }

        }

        private void ListBoxItemClick(object sender, RoutedEventArgs e)
        {
            ListBoxItem? lbi = sender as ListBoxItem;

            if (lbi != null)
            {
                if (lbi.IsSelected)
                {
                    lbi.IsSelected = false;
                    e.Handled = true;
                }
            }
        }



        //
        // ListBox UI Change Methods
        private void ListBox_SelectionChangedWithColor(object sender)
        {
            if (sender != null)
            {
                var listbox = (ListBox)sender;
                var listBoxItems = GetAllChildItems<ListBoxItem>(listbox);
                var selectedItem = GetSelectedChild(listbox);

                if (listbox != null && listBoxItems != null)
                {

                    for (int i = 0; i < listBoxItems.Length; i++)
                    {
                        ListBoxItem item = (ListBoxItem)listBoxItems[i];
                        item.Foreground = new SolidColorBrush(Colors.Black);
                        item.FontWeight = FontWeights.Normal;
                    }

                    if (selectedItem != null)
                    {
                        selectedItem.Foreground = new SolidColorBrush(Colors.Blue);
                        selectedItem.FontWeight = FontWeights.Bold;
                    }

                }
            }

        }

        private childItem? FindVisualChild<childItem>(DependencyObject obj) where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child is not null and childItem)
                    //return the first child of that type
                    return (childItem)child;
                else if (child == null)
                {
                    return null;
                }
                else
                {
                    //continue to next level seraching
                    childItem? childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }

        private childType[]? GetAllChildItems<childType>(DependencyObject element) where childType : DependencyObject
        {
            var firstChild = FindVisualChild<childType>(element);

            if (firstChild != null)
            {
                var realParent = VisualTreeHelper.GetParent(firstChild);

                var numberOfChild = VisualTreeHelper.GetChildrenCount(realParent);

                childType[] children;

                if (numberOfChild > 0)
                {
                    children = new childType[numberOfChild];
                    for (int i = 0; i < numberOfChild; i++)
                    {
                        var child = VisualTreeHelper.GetChild(realParent, i);
                        children[i] = (childType)child;
                    }

                    return children;
                }


            }

            return null;
        }

        private ListBoxItem? GetSelectedChild(ListBox listBox)
        {
            var seletedIndex = listBox.SelectedIndex;

            if (seletedIndex >= 0)
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                return GetAllChildItems<ListBoxItem>(element: listBox)[seletedIndex];
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            }

            return null;
        }



        //
        // Add Button
        private void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            if (localRealm != null)
            {
                switch (CurrentSelectedMainItem)
                {
                    case "Tool":
                        var editor = new Editor_For_Tool(localRealm);
                        editor.ShowDialog();
                        break;
                    default:
                        break;
                }
            }



        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = ContentList.SelectedItem;
            
            
            switch (selectedItem)
            {
                case Tool:
                    var editor = new Editor_For_Tool(localRealm!, selectedItem, this, false);
                    editor.ShowDialog();
                    break;
                default:
                    break;
            }

            
            ContentList.Items.Refresh();
        }

        private void ContentList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            

            
            var selectedItem = ContentList.SelectedItem;

            if (selectedItem == null)
            {
                EditButton.IsEnabled = false;
                DeleteButton.IsEnabled = false;
                CopyButton.IsEnabled = false;
            }
            else
            {
                EditButton.IsEnabled = true;
                DeleteButton.IsEnabled = true;
                CopyButton.IsEnabled = true;
            }
            
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = ContentList.SelectedItem;

            if (selectedItem == null)
            {
                EditButton.IsEnabled = false;
                DeleteButton.IsEnabled = false;
            }
            else
            {
                var result = MessageBox.Show($"Are you sure to delete the item(PN: {selectedItem})?","",MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        localRealm!.Write(() =>
                        {
                            localRealm.Remove((RealmObject)selectedItem);
                        });
                        ContentList.UnselectAll();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Database error", MessageBoxButton.OK, MessageBoxImage.Error);
                        
                    }
                }
            }
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = ContentList.SelectedItem;


            switch (selectedItem)
            {
                case Tool:
                    var editor = new Editor_For_Tool(localRealm!, selectedItem, this, true);
                    editor.ShowDialog();
                    break;
                default:
                    break;
            }


            ContentList.Items.Refresh();

        }

        private void ListBoxItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
