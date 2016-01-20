using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockExchange.Models
{
    public class Stock
    {
        [Display(AutoGenerateField= false)]
        [Editable(false) ]
        public Int32 StockId { get; set; }

        [Required]
        [StringLength(8,MinimumLength = 1 , ErrorMessage= "A Stock Quote is allowed to between 1 and 8 characters inclusive in length")]
        [Display(Name ="Stock Quote name", Description="Identifies the name of the quote you wish to add")]
        public String Code { get; set; }

        [Editable(false)]
        public Double Price { get; set; }

        public string ApplicationUser_id { get; set; }

        [Editable(false)]
       [Required]
        [Display(AutoGenerateField = false)]

        [ForeignKey("ApplicationUser_id")]
        public virtual ApplicationUser Owner { get; set; }

    }
}
