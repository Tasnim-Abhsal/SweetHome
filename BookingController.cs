using HotelManagementSystem.Models;
using HotelManagementSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace HotelManagementSystem.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private HotelDBEntities objHotelDbEntities;
        public BookingController()
        {
            objHotelDbEntities = new HotelDBEntities();
        }
        public ActionResult Index()
        {
            BookingViewModel objBookingViewModel=new BookingViewModel();
            objBookingViewModel.listOfRooms=(from objRoom in objHotelDbEntities.Rooms where objRoom.BookingStatusId==2 select new SelectListItem()
            {
                Text=objRoom.RoomNumber,
                Value=objRoom.RoomId.ToString()
            }
            ).ToList();
            objBookingViewModel.BookingFrom=DateTime.Now;
            objBookingViewModel.BookingTo=DateTime.Now.AddDays(1);
            return View(objBookingViewModel);
        }
        [HttpPost]
        public ActionResult Index(BookingViewModel objBookingViewModel)
        {
            int numberOfDays = Convert.ToInt32((objBookingViewModel.BookingTo - objBookingViewModel.BookingFrom).TotalDays);

            // Use SingleOrDefault to avoid the exception
            Room objRoom = objHotelDbEntities.Rooms.SingleOrDefault(model => model.RoomId == objBookingViewModel.AssignRoomId);

            // Check if the room was found
            if (objRoom == null)
            {
                // Return a message or handle the case where the room is not found
                return Json(new { Message = "Room not found", success = false }, JsonRequestBehavior.AllowGet);
            }

            decimal RoomPrice = objRoom.RoomPrice;
            decimal TotalAmount = RoomPrice * numberOfDays;

            RoomBooking roomBooking = new RoomBooking()
            {
                BookingForm = objBookingViewModel.BookingFrom,
                BookingTo = objBookingViewModel.BookingTo,
                AssignRoomId = objBookingViewModel.AssignRoomId,
                CustomerAddress = objBookingViewModel.CustomerAddress,
                CustomerName = objBookingViewModel.CustomerName,
                
                NoOfMembers = objBookingViewModel.NumberOfMembers,
                TotalAmount =TotalAmount,
            };

            objHotelDbEntities.RoomBookings.Add(roomBooking);
            objHotelDbEntities.SaveChanges();

            // Update room status
            objRoom.BookingStatusId = 3;
            objHotelDbEntities.SaveChanges();

            return Json(new { Message = "Hotel booking successfully created", success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllBookingHistory()
        {
            List<BookingViewModel> listOfBookingViewModels = new List<BookingViewModel>();
            listOfBookingViewModels = (from objHotelBooking in objHotelDbEntities.RoomBookings
                                       join objRoom in objHotelDbEntities.Rooms on objHotelBooking.AssignRoomId equals objRoom.RoomId
                                       select new BookingViewModel()
                                       {
                                           BookingFrom = objHotelBooking.BookingForm,
                                           BookingTo = objHotelBooking.BookingTo,
                                           CustomerName = objHotelBooking.CustomerName,
                                           TotalAmount = objHotelBooking.TotalAmount,
                                           CustomerAddress = objHotelBooking.CustomerAddress,
                                           NumberOfMembers = objHotelBooking.NoOfMembers,
                                           AssignRoomId = objHotelBooking.AssignRoomId,
                                           

                                       }).ToList();
            return PartialView("_BookingHistoryPartial", listOfBookingViewModels);
        }

    }
}