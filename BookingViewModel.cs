using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace HotelManagementSystem.ViewModel
{
    public class BookingViewModel
    {
        [Display(Name ="Customer Name")]
        [Required(ErrorMessage = "Customer Name is required")]
        public string CustomerName {  get; set; }
        [Display(Name = "Customer Address")]
        [Required(ErrorMessage = "Customer Address is required")]
        public string CustomerAddress {  get; set; }
        [Display(Name = "Customer Phone")]
        [Required(ErrorMessage = "Customer Phone is required")]
        public int CustomerPhone { get; set; }
        [Display(Name = "Booking From")]
        [Required(ErrorMessage = "Booking From is required")]
        [DisplayFormat(DataFormatString="{0:dd-MM-yyyy}",ApplyFormatInEditMode =true)]
        public DateTime BookingFrom { get; set; }
        [Display(Name = "Booking To")]
        [Required(ErrorMessage = "Booking To is required")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime BookingTo { get; set; }
        [Display(Name = "Assign Room")]
        public int AssignRoomId {  get; set; }
        [Display(Name = "Number Of Members")]
        [Required(ErrorMessage = "Number Of Members is required")]
        public int NumberOfMembers { get; set; }
        public decimal TotalAmount {  get; set; }
        public IEnumerable<SelectListItem>listOfRooms { get; set; }
    }
}