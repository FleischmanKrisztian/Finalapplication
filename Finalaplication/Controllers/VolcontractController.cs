﻿using Finalaplication.ControllerHelpers.UniversalHelpers;
using Finalaplication.ControllerHelpers.VolcontractHelpers;
using Finalaplication.LocalDatabaseManager;
using Finalaplication.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Finalaplication.Controllers
{
    public class VolcontractController : Controller
    {
        private static string SERVER_NAME_LOCAL = Environment.GetEnvironmentVariable(Common.VolMongoConstants.SERVER_NAME_LOCAL);
        private static int SERVER_PORT_LOCAL = int.Parse(Environment.GetEnvironmentVariable(Common.VolMongoConstants.SERVER_PORT_LOCAL));
        private static string DATABASE_NAME_LOCAL = Environment.GetEnvironmentVariable(Common.VolMongoConstants.DATABASE_NAME_LOCAL);

        private VolunteerManager volunteerManager = new VolunteerManager(SERVER_NAME_LOCAL, SERVER_PORT_LOCAL, DATABASE_NAME_LOCAL);
        private VolContractManager volContractManager = new VolContractManager(SERVER_NAME_LOCAL, SERVER_PORT_LOCAL, DATABASE_NAME_LOCAL);

        [HttpGet]
        public IActionResult Index(string idofvol)
        {
            try
            {
                int nrofdocs = UniversalFunctions.GetNumberOfItemPerPageFromSettings(TempData);
                List<Volcontract> volcontracts = volContractManager.GetListOfVolunteersContracts();
                Volunteer vol = volunteerManager.GetOneVolunteer(idofvol);
                volcontracts = volcontracts.Where(z => z.OwnerID.ToString() == idofvol).ToList();
                ViewBag.nameofvol = vol.Fullname;
                ViewBag.idofvol = idofvol;
                return View(volcontracts);
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
                List<Volcontract> volcontracts = volContractManager.GetListOfVolunteersContracts();
                volcontracts = VolcontractFunctions.GetExpiringContracts(volcontracts);
                return View(volcontracts);
            }
            catch
            {
                return RedirectToAction("Localserver", "Home");
            }
        }

        [HttpGet]
        public ActionResult Create(string id)
        {
            try
            {
                ViewBag.idofvol = id;
                return View();
            }
            catch
            {
                return RedirectToAction("Localserver", "Home");
            }
        }

        [HttpPost]
        public ActionResult Create(Volcontract volcontract, string idofvol)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Volunteer vol = volunteerManager.GetOneVolunteer(idofvol);
                    volcontract._id = Guid.NewGuid().ToString();
                    volcontract.ExpirationDate = volcontract.ExpirationDate.AddDays(1);
                    volcontract.RegistrationDate = volcontract.RegistrationDate.AddDays(1);
                    volcontract.Birthdate = vol.Birthdate;
                    volcontract.Fullname = vol.Fullname;
                    volcontract.CNP = vol.CNP;
                    volcontract.CIseria = vol.CIseria;
                    volcontract.CINr = vol.CINr;
                    volcontract.CIEliberat = vol.CIEliberat;
                    volcontract.Nrtel = vol.ContactInformation.PhoneNumber;
                    volcontract.Hourcount = vol.HourCount;
                    volcontract.CIeliberator = vol.CIeliberator;
                    string address = string.Empty;
                    if (vol.Address.District != null && vol.Address.District != "-")
                    { address = vol.Address.District; }
                    if (vol.Address.City != null && vol.Address.City != "-")
                    { address = address + "," + vol.Address.City; }
                    if (vol.Address.Street != null && vol.Address.Street != "-")
                    { address = vol.Address.District; }
                    if (vol.Address.Number != null && vol.Address.Number != "-")
                    { address = address + "," + vol.Address.City; }
                    volcontract.Address = address;
                    volcontract.OwnerID = idofvol;
                    volContractManager.AddVolunteerContractToDB(volcontract);
                    return RedirectToAction("Index", new { idofvol });
                }
            }
            catch
            {
                ModelState.AddModelError("", "Unable to save changes! ");
            }
            return RedirectToAction("Create", new { idofvol });
        }

        [HttpGet]
        public ActionResult Print(string id)
        {
            try
            {
                var contract = volContractManager.GetVolunteerContract(id);
                return View(contract);
            }
            catch
            {
                return RedirectToAction("Localserver", "Home");
            }
        }

        [HttpGet]
        public ActionResult Delete(string id)
        {
            try
            {
                var contract = volContractManager.GetVolunteerContract(id);
                return View(contract);
            }
            catch
            {
                return RedirectToAction("Localserver", "Home");
            }
        }

        [HttpPost]
        public ActionResult Delete(string id, string idofvol)
        {
            try
            {
                volContractManager.DeleteAVolContract(id);
                return RedirectToAction("Index", new { idofvol });
            }
            catch
            {
                return RedirectToAction("Localserver", "Home");
            }
        }
    }
}