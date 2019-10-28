﻿using Finalaplication.App_Start;
using Finalaplication.Common;
using Finalaplication.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System;
using System.Linq;

namespace Finalaplication.Controllers
{
    public class SettingsController : Controller
    {
        private MongoDBContextOffline dbcontextoffline;
        private IMongoCollection<Settings> settingcollection;

        public SettingsController()
        {
            dbcontextoffline = new MongoDBContextOffline();
            settingcollection = dbcontextoffline.databaseoffline.GetCollection<Settings>("Settings");
        }

        public IActionResult Settings()
        {
            try
            {
                int nrofdocs = ControllerHelper.getNumberOfItemPerPageFromSettings(TempData);
                ViewBag.env = TempData.Peek(VolMongoConstants.CONNECTION_ENVIRONMENT);
                return View();
            }
            catch
            {
                return RedirectToAction("Localserver", "Home");
            }
        }

        [HttpPost]
        public ActionResult Settings(string lang, string env, int quantity)
        {
            Settings set = settingcollection.AsQueryable<Settings>().SingleOrDefault();
            set.Env = env;
            set.Quantity = quantity;
            set.Lang = lang;
            ViewBag.Lang = lang;
            TempData["environment"] = set.Env;
            TempData["numberofdocuments"] = set.Quantity;
            Response.Cookies.Append(
          CookieRequestCultureProvider.DefaultCookieName,
          CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(lang)),
          new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
      );
            settingcollection.ReplaceOne(y => y.Env.Contains("i"), set);
            return RedirectToAction("Index", "Home");
        }
    }
}
