using HotelManagementSystem.Models;
using HotelManagementSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HotelManagementSystem.Controllers
{
    //[Authorize]
    public class RoomController : Controller
    {
        //[AllowAnonymous]
        // GET: Room
        private HotelDBEntities objHotelDBEntities;
        public RoomController()
        {
            objHotelDBEntities = new HotelDBEntities();
        }
        public ActionResult Index()
        {
            RoomViewModel objRoomViewModel=new RoomViewModel();
            objRoomViewModel.ListOfBookingStatus = (from obj in objHotelDBEntities.BookingStatus
            select new SelectListItem()
            { 
                Text=obj.BookingStatus,
                Value=obj.BookingStatusId.ToString()
             }).ToList();
            objRoomViewModel.ListOfRoomType= (from obj in objHotelDBEntities.RoomTypes
            select new SelectListItem()
             {
                Text = obj.RoomTypeName,
                Value = obj.RoomTypeId.ToString()
            }).ToList();
            return View(objRoomViewModel);
        }
        [HttpPost]
        public ActionResult Index(RoomViewModel objRoomViewModel)
        {
            if (objRoomViewModel == null)
            {
                return Json(new { success = false, message = "RoomViewModel is null." }, JsonRequestBehavior.AllowGet);
            }

            if (objRoomViewModel.Image == null || objRoomViewModel.Image.ContentLength == 0)
            {
                return Json(new { success = false, message = "No image uploaded." }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                // Generate unique image name and get extension

                string message = string.Empty;
               
                if (objRoomViewModel.RoomId == 0)
                {
                    string imageUniqueName = Guid.NewGuid().ToString();
                    string fileExtension = Path.GetExtension(objRoomViewModel.Image.FileName);
                    string actualImageName = imageUniqueName + fileExtension;
                    if (string.IsNullOrEmpty(fileExtension))
                    {
                        return Json(new { success = false, message = "Image file extension is invalid." }, JsonRequestBehavior.AllowGet);
                    }
                    // Ensure directory exists
                    string imagePath = Server.MapPath("~/RoomImages/");
                    if (!Directory.Exists(imagePath))
                    {
                        Directory.CreateDirectory(imagePath);
                    }

                    string fullImagePath = Path.Combine(imagePath, actualImageName);

                    // Save the image
                    objRoomViewModel.Image.SaveAs(fullImagePath);

                    // Create Room object
                    Room objRoom = new Room
                    {
                        RoomNumber = objRoomViewModel.RoomNumber,
                        RoomDescription = objRoomViewModel.RoomDescription,
                        RoomPrice = objRoomViewModel.RoomPrice,
                        BookingStatusId = objRoomViewModel.BookingStatusId,
                        IsActive = true,
                        RoomImage = actualImageName,
                        RoomCapacity = objRoomViewModel.RoomCapacity,
                        RoomTypeId = objRoomViewModel.RoomTypeId,
                    };
                    objHotelDBEntities.Rooms.Add(objRoom);
                    message = "Added";
                }
                
                else
                {
                    Room objRoom = objHotelDBEntities.Rooms.Single(model => model.RoomId == objRoomViewModel.RoomId);
                    objRoom.RoomNumber = objRoomViewModel.RoomNumber;
                    objRoom.RoomDescription = objRoomViewModel.RoomDescription;
                    objRoom.RoomPrice = objRoomViewModel.RoomPrice;
                    objRoom.BookingStatusId = objRoomViewModel.BookingStatusId;
                    objRoom.IsActive = true;
                    //objRoom.RoomImage = actualImageName;
                    objRoom.RoomCapacity = objRoomViewModel.RoomCapacity;
                    objRoom.RoomTypeId = objRoomViewModel.RoomTypeId;
                }

                objHotelDBEntities.SaveChanges();

                // Save the room object to database here (not shown in the original code)

                return Json(new {message="Room Successfully Added" +message,success=true}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Log exception details (optional)
                // LogException(ex);

                return Json(new { success = false, message = "An error occurred while adding the room. Please try again later." }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetAllRooms()
        {
            IEnumerable<RoomDetailsViewModel> listOfRoomDetailsViewModels=(from objRoom in objHotelDBEntities.Rooms join objBooking in objHotelDBEntities.BookingStatus on objRoom.BookingStatusId equals objBooking.BookingStatusId join objRoomType in objHotelDBEntities.RoomTypes on objRoom.RoomTypeId equals objRoomType.RoomTypeId
               where objRoom.IsActive== true
               select new RoomDetailsViewModel()
               {
                   RoomNumber= objRoom.RoomNumber,
                   RoomDescription= objRoom.RoomDescription,
                   RoomCapacity= objRoom.RoomCapacity,
                   RoomPrice= objRoom.RoomPrice,
                   BookingStatus=objBooking.BookingStatus,
                   RoomType=objRoomType.RoomTypeName,
                   RoomImage= objRoom.RoomImage,
                   RoomID=objRoom.RoomId

               }).ToList();
               
            return PartialView("_RoomDetailsPartial",listOfRoomDetailsViewModels);
        }
        [HttpGet]
        public JsonResult EditRoomDetails(int roomId)
        {
            var result=objHotelDBEntities.Rooms.Single(model=>model.RoomId==roomId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult DeleteRoomDetails(int roomId)
        {
            Room objRoom = objHotelDBEntities.Rooms.Single(model => model.RoomId == roomId);
            objRoom.IsActive = false;
            objHotelDBEntities.SaveChanges();
            return Json(new {message="Record Successfully Deleted",success=true},JsonRequestBehavior.AllowGet);
        }

    }
}