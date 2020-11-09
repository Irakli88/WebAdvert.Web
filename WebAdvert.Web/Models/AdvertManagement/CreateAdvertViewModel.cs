using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebAdvert.Web.Models.AdvertManagement
{
    public class CreateAdvertViewModel
    {
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }
        public string Description { get; set; }

        [DataType(DataType.Currency)]
        [Required(ErrorMessage = "Price is required")]
        public double Price { get; set; }
    }
}
