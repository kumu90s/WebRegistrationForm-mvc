using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.UI;
using WebRegistrationForm.Model;
using WebRegistrationForm.Service;

namespace WebRegistrationForm.Controllers
{
    public class HomeController : Controller
    {
        readonly IWebRegistrationFormService _webRegistrationFormService;
        readonly ISignUpFormServices _signUpFormServices;
        readonly ILogInFormService _logInFormService;
        readonly IDashboardService _dashboardService;
        public HomeController()
        {

        }
        public HomeController(IWebRegistrationFormService registrationFormService, ISignUpFormServices UpFormServices, ILogInFormService InFormService, IDashboardService dashboardService)
        {
            _webRegistrationFormService = registrationFormService;
            _signUpFormServices = UpFormServices;
            _logInFormService = InFormService;
            _dashboardService = dashboardService;
        }
        public void init()
        {
            var stateclass = /*await*/ _webRegistrationFormService.stateInformation();
            ViewBag.State = new SelectList(stateclass, "stateId", "stateName");

            var districtClass = /*await*/ _webRegistrationFormService.districtInformation();
            ViewBag.District = new SelectList(districtClass, "districtId", "districtName");

            var municipalityClass = /*await*/ _webRegistrationFormService.municipalityInformation();
            ViewBag.Municipality = new SelectList(municipalityClass, "municipalityId", "municipalityName");
        }
        public /*async Task<*/ ActionResult/*>*/ Index()
        {
            return RedirectToAction("Registration");
        }

        //// To get the district of the selected state only
        [HttpGet]       
        public JsonResult stateDistrict(string stateId)
        {
            var districtClass = _webRegistrationFormService.districtInformation(stateId);
            //ViewBag.District = new SelectList(districtClass, "districtId", "districtName");
            return Json(districtClass,JsonRequestBehavior.AllowGet);
        }

        //// To get the municipality of the selected district only
        [HttpGet]
        public JsonResult districtMunicipality(string districtId)
        {
            var districtClass = _webRegistrationFormService.municipalityInformation(districtId);
            //ViewBag.District = new SelectList(districtClass, "districtId", "districtName");
            return Json(districtClass, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetDistricts()
        {
            //var districtClass = _webRegistrationFormService.districtInformation();
            //ViewBag.District = new SelectList(districtClass, "districtId", "districtName");
            return View();
        }

        /// To registure the user
        [HttpGet]
        public ActionResult Registration()
        {
            init();
            return View();
        }
        
        [HttpPost]
        public ActionResult Registration(RegistrationDto registrationDto)
        {
            if (ModelState.IsValid)
            {
                var status = _webRegistrationFormService.RegistrationForm(registrationDto);
                if (status == "00")
                {
                    return RedirectToAction("signUp", "Home", new { registrationDto.email });
                }
            }
            return RedirectToAction("Index", "Home");

        }

        /// To check and signup of the userName
        [HttpGet]
        public ActionResult signUp(string email)
        {
            ViewBag.email = email;
            return View();
        }

        [HttpPost]
        public ActionResult signUp(signUpDto _signUpDto)
        {
            if (ModelState.IsValid)
            {
                var status = _signUpFormServices.signUpForm(_signUpDto);
                if (status == "00")
                {
                    return RedirectToAction("logIn", "Home");
                }
            }
            return View();
        }

        /// To login the valid User
        [HttpGet]
        public ActionResult logIn()
        {
            ViewBag.TaskStatus = TempData["TaskStatus"];
            ViewBag.TaskMessage = TempData["TaskMessage"];
            return View();
        }

        [HttpPost]
        public ActionResult logIn(logInDto logUpFormDto)
        {
            if (ModelState.IsValid)
            {
                statusDto _statusDto = new statusDto();
                _statusDto = _logInFormService.LogInForm(logUpFormDto);

                if (_statusDto.status.Equals("00"))
                {
                    TempData["TaskStatus"] = _statusDto.status;
                    TempData["TaskMessage"] = _statusDto.message;
                }
                else
                {
                    TempData["TaskStatus"] = _statusDto.status;
                    TempData["TaskMessage"] = _statusDto.message;
                    ModelState.AddModelError("", "Invalid userName or Password");
                    //return View(logUpFormDto);
                }
            }

            ViewBag.TaskStatus = TempData["TaskStatus"];
            ViewBag.TaskMessage = TempData["TaskMessage"];
            return View();
        }

        // To search the userDetails
        [HttpGet]
        public ActionResult dashboard()
        {
            init();

            return View();
        }
        [HttpPost]
        public Action dashboard(RegistrationDto _registrationDto)
        {
            return null;
        }

        /// To reterive the data of users
        [HttpGet]
        public ActionResult dashboardResult(dashboardDto dashboard)
        {
            List<RegistrationDto> _registrationDtos = _dashboardService.userDetail(dashboard);
            return PartialView(_registrationDtos);

        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}