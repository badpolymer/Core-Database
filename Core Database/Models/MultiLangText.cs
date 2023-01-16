using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Core_Database.ViewsControllers;
using Realms;

namespace Core_Database.Models
{
    class LocaleManipulator
    {
        private Realm localRealm;
        private string? ownerPN;

        public LocaleManipulator(Realm localRealm)
        {
            this.localRealm = localRealm;
            
        }


        public Guid NewGuid() 
        { 
            Guid generatedID = Guid.NewGuid();
            int existingIDAmount = 0;

            do
            {
                existingIDAmount = localRealm.All<LocaleEnglishUS>().Where(i => i.ID == generatedID).Count();

                if (existingIDAmount > 0)
                {
                    generatedID = Guid.NewGuid();
                }

            } while (existingIDAmount > 0);

            return generatedID;
        }

        public List<RealmObject> OpenEditorToGetNewTexts(string pn, string itemName, Guid id)
        {
            var a = new MultiLanguageTextEditor(pn, itemName,id);
            var b = a.ShowDialogWithOutput();

            return b;
        }

        public void ClearAllLocaleForPN(string pn)
        {
            try
            {
                localRealm.Write(() =>
                {
                    
                    var localeUS = localRealm.All<LocaleEnglishUS>().Where(locale => locale.OwnerPN == pn);
                    localRealm.RemoveRange(localeUS);
                    var localeUK = localRealm.All<LocaleEnglishUK>().Where(locale => locale.OwnerPN == pn);
                    localRealm.RemoveRange(localeUK);

                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }


    }

    interface IHasOwner
    {
        string OwnerPN { get; set; }
    }


    class LocaleEnglishUS : RealmObject
    {

        [PrimaryKey]
        [MapTo("id")]
        public Guid ID { get; set; }

        [Required]
        [MapTo("text")]
        public string? Text { get; set; }


        [Required]
        [MapTo("ownerPN")]
        public string OwnerPN { get; set; }
    }

    class LocaleEnglishUK : RealmObject
    {

        [PrimaryKey]
        [MapTo("id")]
        public Guid ID { get; set; }

        
        [MapTo("text")]
        public string? Text { get; set; }


        [Required]
        [MapTo("ownerPN")]
        public string OwnerPN { get; set; }

    }
}
