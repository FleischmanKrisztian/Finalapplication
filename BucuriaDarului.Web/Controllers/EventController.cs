﻿using BucuriaDarului.Contexts;
using BucuriaDarului.Gateway;
using Finalaplication.Common;
using Finalaplication.ControllerHelpers.UniversalHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;

namespace Finalaplication.Controllers
{
    public class EventController : Controller
    {
        private readonly IStringLocalizer<EventController> _localizer;

        public EventController(Microsoft.AspNetCore.Hosting.IWebHostEnvironment env, IStringLocalizer<EventController> localizer)
        {
            _localizer = localizer;
        }

        public ActionResult Import()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Import(IFormFile file)
        {
            try
            {
                var eventsImportContext = new EventsImportContext(new EventsImportDataGateway());
                eventsImportContext.Execute(file.OpenReadStream());
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("IncorrectFile", "Home");
            }
        }

        public ActionResult Index(string searching, int page, string searchingPlace, string searchingActivity, string searchingType, string searchingVolunteers, string searchingSponsor, DateTime lowerDate, DateTime upperDate)
        {
            try
            {
                int nrOfDocs = UniversalFunctions.GetNumberOfItemPerPageFromSettings(TempData);
                var eventsMainDisplayIndexContext = new EventsMainDisplayIndexContext(new EventsMainDisplayIndexGateway());
                var model = eventsMainDisplayIndexContext.Execute(new EventsMainDisplayIndexRequest(searching, page, nrOfDocs, searchingPlace, searchingActivity, searchingType, searchingVolunteers, searchingSponsor, lowerDate, upperDate));
                return View(model);
            }
            catch
            {
                return RedirectToAction("Localserver", "Home");
            }
        }

        [HttpGet]
        public ActionResult CSVExporter(string stringOfIDs)
        {
            var exportParamenters = new ExportParamenters();
            exportParamenters.StringOfIDs = stringOfIDs;
            return View();
        }

        [HttpPost]
        public ActionResult CSVExporter(ExportParamenters csvExportProperties)
        {
            var eventsExporterContext = new EventsExporterContext(_localizer);
            var eventsExportData = eventsExporterContext.Execute(new EventsExporterRequest(csvExportProperties));
            DictionaryHelper.d = eventsExportData.Dictionary;
            return Redirect("csvexporterapp:eventSession;eventHeader");
        }

        public ActionResult VolunteerAllocation(string id, int page, string searching)
        {
            try
            {
                int nrofdocs = UniversalFunctions.GetNumberOfItemPerPageFromSettings(TempData);
                var allocatedVolunteerContext = new EventsVolunteerAllocationContext(new EventsVolunteerAllocationDataGateway());
                var model = allocatedVolunteerContext.Execute(new EventsVolunteerAllocationDisplayRequest(id, page, nrofdocs, searching));
                return View(model);
            }
            catch
            {
                return RedirectToAction("Localserver", "Home");
            }
        }

        [HttpPost]
        public ActionResult VolunteerAllocation(string[] volunteerids, string Evid)
        {
            try
            {
                var allocatedVolunteerContext = new EventsVolunteerAllocationContext(new EventsVolunteerAllocationDataGateway());
                allocatedVolunteerContext.UdateAllocationToEvent(new EventsVolunteerAllocationRequest(volunteerids, Evid));
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Localserver", "Home");
            }
        }

        public ActionResult SponsorAllocation(string id, int page, string searching)
        {
            try
            {
                int nrofdocs = UniversalFunctions.GetNumberOfItemPerPageFromSettings(TempData);

                var allocatedSponsorContext = new EventsSponsorAllocationContext(new EventsSponsorAllocationDataGateway());
                var model = allocatedSponsorContext.Execute(new EventsSponsorsAllocationDisplayRequest(id, page, nrofdocs, searching));
                return View(model);
            }
            catch
            {
                return RedirectToAction("Localserver", "Home");
            }
        }

        [HttpPost]
        public ActionResult SponsorAllocation(string[] sponsorids, string Evid)
        {
            try
            {
                var allocatedSponsorContext = new EventsSponsorAllocationContext(new EventsSponsorAllocationDataGateway());
                allocatedSponsorContext.UdateAllocationToEvent(new EventsSponsorAllocationRequest(sponsorids, Evid));
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Details(string id)
        {
            try
            {
                var eventDetailGateway = new EventDetailGateway();
                var model = eventDetailGateway.ReturnEvent(id);
                return View(model);
            }
            catch
            {
                return RedirectToAction("Localserver", "Home");
            }
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(EventCreateRequest request)
        {
            try
            {
                var eventCreateContext = new EventCreateContext(new EventCreateGateway());
                var eventCreateResponse = eventCreateContext.Execute(request);
                if (eventCreateResponse.ContainsSpecialChar)
                {
                    ViewBag.ContainsSpecialChar = true;
                    ModelState.Remove("NumberOfVolunteersNeeded");
                    ModelState.Remove("DateOfEvent");
                    return View();
                }
                else if (!eventCreateResponse.IsValid)
                {
                    ModelState.Remove("NumberOfVolunteersNeeded");
                    ModelState.Remove("DateOfEvent");
                    ModelState.AddModelError("NameOfEvent", "Name Of Event must not be empty");
                    return View();
                }
                else
                {
                    return RedirectToAction("Index");
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
                var eventEditGateway = new EventEditGateway();
                var model = eventEditGateway.ReturnEvent(id);
                return View(model);
            }
            catch
            {
                return RedirectToAction("Localserver", "Home");
            }
        }

        [HttpPost]
        public ActionResult Edit(EventEditRequest request)
        {
            try
            {
                var eventEditContext = new EventEditContext(new EventEditGateway());
                var eventEditResponse = eventEditContext.Execute(request);
                if (eventEditResponse.ContainsSpecialChar)
                {
                    ViewBag.ContainsSpecialChar = true;
                    ModelState.Remove("NumberOfVolunteersNeeded");
                    ModelState.Remove("DateOfEvent");
                    return View(eventEditResponse.Event);
                }
                else if (!eventEditResponse.IsValid)
                {
                    ModelState.Remove("NumberOfVolunteersNeeded");
                    ModelState.Remove("DateOfEvent");
                    ModelState.AddModelError("NameOfEvent", "Name Of Event must not be empty");
                    return View(eventEditResponse.Event);
                }
                else
                {
                    return RedirectToAction("Index");
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
                var eventDeleteGateway = new EventDeleteGateway();
                var model = eventDeleteGateway.ReturnEvent(id);
                return View(model);
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
                var eventDeleteGateway = new EventDeleteGateway();
                var model = eventDeleteGateway.DeleteEvent(id);
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Localserver", "Home");
            }
        }
    }
}