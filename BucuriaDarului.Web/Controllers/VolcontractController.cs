﻿using BucuriaDarului.Contexts.VolunteerContractContext;
using BucuriaDarului.Core;
using BucuriaDarului.Gateway.VolContractGateways;
using Finalaplication.ControllerHelpers.UniversalHelpers;
using Microsoft.AspNetCore.Mvc;

namespace BucuriaDarului.Web.Controllers
{
    public class VolcontractController : Controller
    {
        [HttpGet]
        public IActionResult Index(string idOfVolunteer)
        {
            var nrOfDocs = UniversalFunctions.GetNumberOfItemPerPageFromSettings(TempData);
            var contractsMainDisplayIndexContext = new VolunteerContractIndexDisplayContext(new VolunteerContractIndexDisplayGateway());
            var model = contractsMainDisplayIndexContext.Execute(new VolunteerContractsMainDisplayIndexRequest(idOfVolunteer, nrOfDocs));
            return View(model);
        }

        public ActionResult ContractExp()
        {
            var contractExpirationContext = new VolunteerContractsExpirationContext(new VolunteerContractExpirationGateway());
            var contracts = contractExpirationContext.Execute();
            return View(contracts);
        }

        [HttpGet]
        public ActionResult Create(string idOfVolunteer, string message)
        {
            ViewBag.message = message;
            ViewBag.idOfVol = idOfVolunteer;
            return View();
        }

        [HttpPost]
        public ActionResult Create(VolunteerContract volunteerContract, string idOfVolunteer)
        {
            var contractCreateContext = new VolunteerContractCreateContext(new VolunteerContractCreateGateway());
            var contractCreateResponse = contractCreateContext.Execute(volunteerContract, idOfVolunteer);

            if (!contractCreateResponse.IsValid)
            {
                return RedirectToAction("Create", new { idOfVolunteer = idOfVolunteer, message = contractCreateResponse.Message });
            }
            return RedirectToAction("Index", new { idOfVolunteer = idOfVolunteer });
        }

        [HttpGet]
        public ActionResult Print(string id)
        {
            var model = SingleVolunteerContractReturnerGateway.GetVolunteerContract(id);
            return View(model);
        }

        [HttpGet]
        public ActionResult Delete(string id)
        {
            var model = SingleVolunteerContractReturnerGateway.GetVolunteerContract(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(string id, string idOfVol)
        {
            VolunteerContractDeleteGateway.Delete(id);
            return RedirectToAction("Index", new { idOfVolunteer = idOfVol });
        }
    }
}