﻿using Elm.Core.Parsers;
using Finalaplication.Common;
using Finalaplication.ControllerHelpers.SponsorHelpers;
using Finalaplication.ControllerHelpers.UniversalHelpers;
using Finalaplication.LocalDatabaseManager;
using Finalaplication.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JsonConvert = Newtonsoft.Json.JsonConvert;

namespace Finalaplication.Controllers
{
    public class SponsorController : Controller
    {
        private static string SERVER_NAME_LOCAL = Environment.GetEnvironmentVariable(Common.VolMongoConstants.SERVER_NAME_LOCAL);
        private static int SERVER_PORT_LOCAL = int.Parse(Environment.GetEnvironmentVariable(Common.VolMongoConstants.SERVER_PORT_LOCAL));
        private static string DATABASE_NAME_LOCAL = Environment.GetEnvironmentVariable(Common.VolMongoConstants.DATABASE_NAME_LOCAL);

        private readonly IStringLocalizer<SponsorController> _localizer;

        private EventManager eventManager = new EventManager(SERVER_NAME_LOCAL, SERVER_PORT_LOCAL, DATABASE_NAME_LOCAL);
        private ModifiedDocumentManager modifiedDocumentManager = new ModifiedDocumentManager();
        private AuxiliaryDBManager auxiliaryDBManager = new AuxiliaryDBManager(SERVER_NAME_LOCAL, SERVER_PORT_LOCAL, DATABASE_NAME_LOCAL);
        private SponsorManager sponsorManager = new SponsorManager(SERVER_NAME_LOCAL, SERVER_PORT_LOCAL, DATABASE_NAME_LOCAL);

        public SponsorController(Microsoft.AspNetCore.Hosting.IWebHostEnvironment env, IStringLocalizer<SponsorController> localizer)
        {
            _localizer = localizer;
        }

        public ActionResult FileUpload()
        {
            return View();
        }

        [HttpPost]
        public ActionResult FileUpload(IFormFile Files)
        {
            try
            {
                string path = " ";
                if (UniversalFunctions.File_is_not_empty(Files))
                {
                    path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", Files.FileName);
                    UniversalFunctions.CreateFileStream(Files, path);
                }
                else
                {
                    return View();
                }
                List<string[]> Sponsors = CSVImportParser.GetListFromCSV(path);
                for (int i = 0; i < Sponsors.Count; i++)
                {
                    Sponsor s = SponsorFunctions.GetSponsorFromString(Sponsors[i]);
                    sponsorManager.AddSponsorToDB(s);
                }
                UniversalFunctions.RemoveTempFile(path);
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("IncorrectFile", "Home");
            }
        }

        [HttpGet]
        public ActionResult CSVSaver()
        {
            string ids = HttpContext.Session.GetString(VolMongoConstants.SESSION_KEY_SPONSOR);
            HttpContext.Session.Remove(VolMongoConstants.SESSION_KEY_SPONSOR);
            string key = VolMongoConstants.SECONDARY_SESSION_KEY_SPONSOR;
            HttpContext.Session.SetString(key, ids);
            return View();
        }

        [HttpPost]
        public ActionResult CSVSaver(bool All, bool NameOfSponsor, bool Date, bool MoneyAmount, bool WhatGoods, bool GoodsAmount, bool HasContract, bool ContractDetails, bool PhoneNumber, bool MailAdress)
        {
            string IDS = HttpContext.Session.GetString(VolMongoConstants.SECONDARY_SESSION_KEY_SPONSOR);
            HttpContext.Session.Remove(VolMongoConstants.SECONDARY_SESSION_KEY_SPONSOR);
            string ids_and_fields = SponsorFunctions.GetIdAndFieldString(IDS, All, NameOfSponsor, Date, MoneyAmount, WhatGoods, GoodsAmount, HasContract, ContractDetails, PhoneNumber, MailAdress);
            string key1 = VolMongoConstants.SPONSORSESSION;
            string header = ControllerHelper.GetHeaderForExcelPrinterSponsor(_localizer);
            string key2 = VolMongoConstants.SPONSORHEADER;
            ControllerHelper.CreateDictionaries(key1, key2, ids_and_fields, header);
            string csvexporterlink = "csvexporterapp:" + key1 + ";" + key2;
            return Redirect(csvexporterlink);
        }

        public IActionResult Index(string searching, int page, string ContactInfo, DateTime lowerdate, DateTime upperdate, bool HasContract, string WhatGoods, string MoneyAmount, string GoodsAmounts)

        {
            try
            {
                if (searching != null)
                { ViewBag.Filters1 = searching; }
                if (ContactInfo != null)
                { ViewBag.Filters2 = ContactInfo; }
                if (HasContract == true)
                { ViewBag.Filters3 = ""; }
                if (WhatGoods != null)
                { ViewBag.Filters4 = WhatGoods; }
                if (MoneyAmount != null)
                { ViewBag.Filters5 = MoneyAmount; }
                if (GoodsAmounts != null)
                { ViewBag.Filters6 = GoodsAmounts; }
                DateTime date = Convert.ToDateTime("01.01.0001 00:00:00");
                if (lowerdate != date)
                { ViewBag.Filter7 = lowerdate.ToString(); }
                if (upperdate != date)
                { ViewBag.Filter8 = upperdate.ToString(); }
                ViewBag.Contact = ContactInfo;
                ViewBag.searching = searching;
                ViewBag.Upperdate = upperdate;
                ViewBag.Lowerdate = lowerdate;
                ViewBag.HasContract = HasContract;
                ViewBag.WhatGoods = WhatGoods;
                ViewBag.GoodsAmount = GoodsAmounts;
                ViewBag.MoneyAmount = MoneyAmount;

                List<Sponsor> sponsors = sponsorManager.GetListOfSponsors();
                page = UniversalFunctions.GetCurrentPage(page);
                ViewBag.page = page;
                sponsors = SponsorFunctions.GetSponsorsAfterFilters(sponsors, searching, ContactInfo, lowerdate, upperdate, HasContract, WhatGoods, MoneyAmount, GoodsAmounts);
                ViewBag.counter = sponsors.Count();
                int nrofdocs = UniversalFunctions.GetNumberOfItemPerPageFromSettings(TempData);
                ViewBag.nrofdocs = nrofdocs;
                string stringofids = SponsorFunctions.GetStringOfIds(sponsors);
                ViewBag.stringofids = stringofids;
                sponsors = SponsorFunctions.GetSponsorsAfterPaging(sponsors, page, nrofdocs);
                string key = VolMongoConstants.SESSION_KEY_SPONSOR;
                HttpContext.Session.SetString(key, stringofids);

                return View(sponsors);
            }
            catch
            {
                return RedirectToAction("Localserver", "Home");
            }
        }

        public ActionResult ContractExp()
        {
            try
            {
                List<Sponsor> sponsors = sponsorManager.GetListOfSponsors();
                sponsors = SponsorFunctions.GetExpiringContracts(sponsors);
                return View(sponsors);
            }
            catch
            {
                return RedirectToAction("Localserver", "Home");
            }
        }

        public ActionResult Details(string id)
        {
            try
            {
                Sponsor detailssponsor = sponsorManager.GetOneSponsor(id);
                return View(detailssponsor);
            }
            catch
            {
                return RedirectToAction("Localserver", "Home");
            }
        }

        public ActionResult Create()
        {
            try
            {
                return View();
            }
            catch
            {
                return RedirectToAction("Localserver", "Home");
            }
        }

        [HttpPost]
        public ActionResult Create(Sponsor incomingsponsor)
        {
            try
            {
                string sponsorasstring = JsonConvert.SerializeObject(incomingsponsor);
                if (UniversalFunctions.ContainsSpecialChar(sponsorasstring))
                {
                    ModelState.AddModelError("Cannot contain semi-colons", "Cannot contain semi-colons");
                }

                ModelState.Remove("Contract.RegistrationDate");
                ModelState.Remove("Contract.ExpirationDate");
                ModelState.Remove("Sponsorship.Date");
                if (ModelState.IsValid)
                {
                    incomingsponsor._id = Guid.NewGuid().ToString();
                    incomingsponsor.Contract.RegistrationDate = incomingsponsor.Contract.RegistrationDate.AddHours(5);
                    incomingsponsor.Contract.ExpirationDate = incomingsponsor.Contract.ExpirationDate.AddHours(5);
                    sponsorManager.AddSponsorToDB(incomingsponsor);
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.containsspecialchar = UniversalFunctions.ContainsSpecialChar(sponsorasstring);
                    return View();
                }
            }
            catch
            {
                return RedirectToAction("Localserver", "Home");
            }
        }

        public ActionResult Edit(string id)
        {
            try
            {
                Sponsor sponsor = sponsorManager.GetOneSponsor(id);
                ViewBag.id = id;
                return View(sponsor);
            }
            catch
            {
                return RedirectToAction("Localserver", "Home");
            }
        }

        [HttpPost]
        public ActionResult Edit(string id, Sponsor sponsor)
        {
            try
            {
                string sponsorasstring = JsonConvert.SerializeObject(sponsor);
                if (UniversalFunctions.ContainsSpecialChar(sponsorasstring))
                {
                    ModelState.AddModelError("Cannot contain semi-colons", "Cannot contain semi-colons");
                }

                Sponsor currentsavedsponsor = sponsorManager.GetOneSponsor(id);
                ModelState.Remove("Contract.RegistrationDate");
                ModelState.Remove("Contract.ExpirationDate");
                ModelState.Remove("Sponsorship.Date");
                if (ModelState.IsValid)
                {
                    sponsor.Contract.RegistrationDate = sponsor.Contract.RegistrationDate.AddHours(5);
                    sponsor.Contract.ExpirationDate = sponsor.Contract.ExpirationDate.AddHours(5);
                    sponsor.Sponsorship.Date = sponsor.Sponsorship.Date.AddHours(5);

                    List<ModifiedIDs> modifiedidlist = modifiedDocumentManager.GetListOfModifications();
                    string modifiedids = JsonConvert.SerializeObject(modifiedidlist);
                    if (!modifiedids.Contains(id))
                    {
                        Sponsor currentsponsor = sponsorManager.GetOneSponsor(id);
                        string currentsponsorasstring = JsonConvert.SerializeObject(currentsponsor);
                        auxiliaryDBManager.AddDocumenttoDB(currentsponsorasstring);
                    }
                    sponsorManager.UpdateSponsor(sponsor, id);
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.id = id;
                    ViewBag.containsspecialchar = UniversalFunctions.ContainsSpecialChar(sponsorasstring);
                    return View();
                }
            }
            catch
            {
                return RedirectToAction("Localserver", "Home");
            }
        }

        public ActionResult Delete(string id)
        {
            try
            {
                Sponsor showsponsor = sponsorManager.GetOneSponsor(id);
                return View(showsponsor);
            }
            catch
            {
                return RedirectToAction("Localserver", "Home");
            }
        }

        [HttpPost]
        public ActionResult Delete(string id, IFormCollection collection)
        {
            try
            {
                sponsorManager.DeleteSponsor(id);
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Localserver", "Home");
            }
        }
    }
}