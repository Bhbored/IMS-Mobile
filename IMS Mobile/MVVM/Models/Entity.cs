using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS_Mobile.MVVM.Models
{
    public class Entity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

    }
}
