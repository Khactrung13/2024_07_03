using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SV20T1020067.BusinessLayers;
using SV20T1020607.DomainModels;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SV20T1020607.Wed.Controllers
{
    public class EmployeeController : Controller
    {
        const int PAGE_SIZE = 20;
        // GET: /<controller>/
        public IActionResult Index(int page = 1, string searchValue = "")
        {
            int rowCount = 0;
            var data = CommonDataService.ListOfEmployees(out rowCount, page, PAGE_SIZE, searchValue ?? "");

            var model = new Models.EmployeeSearchResult()
            {
                Page = page,
                PageSize = PAGE_SIZE,
                SearchValue = searchValue ?? "",
                RountCount = rowCount,
                Data = data
            };

            return View(model);
        }
        public IActionResult Create()
        {
            ViewBag.IsEdit = false;
            ViewBag.Title = "Bổ sung nhân viên";
            var model = new Employee()
            {
                EmployeeID = 0,
                Photo = "nophoto.png",
                BirthDate = new DateTime(1990,1,1),
                IsWorking = true
            };
            return View("Edit",model);
        }

        public IActionResult Edit(int id)
        {
            ViewBag.Title = "Cập nhật thông tin nhân viên";
            ViewBag.IsEdit = true;
            var model = CommonDataService.GetEmployee(id);
            if (model == null)
            {
                return RedirectToAction("Index");
            }
            if (string.IsNullOrWhiteSpace(model.Photo))
                model.Photo = "nophoto.png";
            return View(model);
        }
        public IActionResult Delete(int id=0)
        {
            var model = CommonDataService.GetEmployee(id);

            if (Request.Method == "POST")
            {
                //Xóa ảnh
                if (!string.IsNullOrEmpty(model.Photo))
                {
                    var imagePathToDelete = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "employees", model.Photo);
                    if (System.IO.File.Exists(imagePathToDelete))
                    {
                        System.IO.File.Delete(imagePathToDelete);
                    }
                }
                CommonDataService.DeleteEmployee(id);
                return RedirectToAction("Index");
            }
          
            if (model == null)
            {
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult Save(Employee model, IFormFile? uploadPhoto , string BirthDateInput="")
        {
            //Xử lý ngày sinh
            DateTime? d = BirthDateInput.ToDateTime();
            if (d.HasValue)
                model.BirthDate = d.Value;



            //Xử lý ảnh : nếu có ảnh upload thì lưu ảnh lên sever , gán tên file ảnh cho model.photo
            if (uploadPhoto != null && uploadPhoto.Length > 0)
            {
                // Lấy tên của tệp ảnh
                var FileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}";

                // Tạo đường dẫn đầy đủ cho tệp ảnh đích
                string imagePath = Path.Combine(ApplicationContext.HostEnviroment.WebRootPath, @"images/employees", FileName);
                //var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "employees", FileName);

                // Sao chép tệp ảnh từ tạm thời vị trí tải lên sang thư mục images/employees
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    uploadPhoto.CopyTo(stream);
                }
                // Lưu tên của ảnh vào model
                model.Photo = FileName;

            }
            if (model.EmployeeID == 0)
            {
                 CommonDataService.AddEmployee(model);
                
            }
            else
            {
                if (uploadPhoto != null && uploadPhoto.Length > 0)
                {
                    //Xóa ảnh ban đầu
                    var existingEmployee = CommonDataService.GetEmployee(model.EmployeeID);
                    if (!string.IsNullOrEmpty(existingEmployee.Photo))
                    {
                        string imagePathToDelete = Path.Combine(ApplicationContext.HostEnviroment.WebRootPath, @"images/employees", existingEmployee.Photo);
                        //var imagePathToDelete = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "employees", existingEmployee.Photo);
                        if (System.IO.File.Exists(imagePathToDelete))
                        {
                            System.IO.File.Delete(imagePathToDelete);
                        }
                    }
                }
                
                CommonDataService.UpdateEmployee(model);
            }
            return RedirectToAction("Index");
        }

    }
}

