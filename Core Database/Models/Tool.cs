using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using Realms;

namespace Core_Database.Models
{
    public partial class Tool : RealmObject, IHasPN
    {
        [PrimaryKey]
        [MapTo("pn")]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string PN { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        
        [MapTo("type")]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public ToolMainType Type { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        
        [MapTo("isStandard")]
        public bool IsStandard { get; set; } = false;


        [MapTo("shortDescription")]
        public Guid ShortDescription { get; set; }

        public override string ToString()
        {
            return PN + " " + Type.ToString();
        }

        


    }


    public partial class ToolMainType : RealmObject
    {
        [PrimaryKey]
        [MapTo("name")]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

  

    public interface IHasPN
    {
        string PN { get; set; }
    }
}
