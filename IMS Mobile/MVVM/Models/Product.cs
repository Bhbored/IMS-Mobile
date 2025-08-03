using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS_Mobile.MVVM.Models
{
    [Table("Products")]
    public class Product
    {

        #region fields
        private DateTime createdDate;

        #endregion


        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public DateTime CreatedDate
        {
            get
            {
                return DateTime.Now;
            }
            set => createdDate = value;
        }
        public string CategoryName { get; set; }
        public double Cost { get; set; }
        public int stock { get; set; }
    }

    
    
}
