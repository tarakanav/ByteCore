﻿using ByteCore.Web.Models;
using System.Web.Mvc;
using ByteCore.BusinessLogic.Attributes;
using ByteCore.BusinessLogic.Interfaces;

namespace ByteCore.Web.Controllers
{
    public class AboutUsController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
