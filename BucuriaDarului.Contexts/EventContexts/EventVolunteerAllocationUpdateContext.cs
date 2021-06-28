﻿using BucuriaDarului.Core;
using BucuriaDarului.Core.Gateways;
using System.Collections.Generic;
using System.Linq;

namespace BucuriaDarului.Contexts
{
    public class EventVolunteerAllocationUpdateContext
    {
        private readonly IEventVolunteerAllocationUpdateGateway dataGateway;
        private readonly ISingleEventReturnergateway singleEventReturnergateway;

        public EventVolunteerAllocationUpdateContext(IEventVolunteerAllocationUpdateGateway dataGateway, ISingleEventReturnergateway singleEventReturnergateway)
        {
            this.dataGateway = dataGateway;
            this.singleEventReturnergateway = singleEventReturnergateway;
        }

        public EventsVolunteerAllocationResponse Execute(EventsVolunteerAllocationRequest request)
        {
            bool updateCompleted = false;
            List<KeyValuePair<string, string>> messages = new List<KeyValuePair<string, string>>();
            Event event_ = dataGateway.GetEvent(request.EventId);
            List<Volunteer> volunteers = dataGateway.GetListOfVolunteers();
            volunteers = GetVolunteersByIds(volunteers, request.VolunteerIds);
            string nameOfVolunteers = GetVolunteerNames(volunteers);
            string volunteerForAllocation = GetVolunteerNames(volunteers);
            event_.AllocatedVolunteers = volunteerForAllocation;
            event_.NumberAllocatedVolunteers = VolunteersAllocatedCounter(nameOfVolunteers);
            dataGateway.UpdateEvent(request.EventId, event_);
            if (event_.AllocatedVolunteers == volunteerForAllocation)
            { updateCompleted = true;
               
            }
            else
            {
                updateCompleted = false;
                messages.Add(item: new KeyValuePair<string, string>("fail", "Update failed!Please try again!"));
            }

            return new EventsVolunteerAllocationResponse(updateCompleted, messages);
        }

        private int VolunteersAllocatedCounter(string AllocatedVolunteers)
        {
            if (AllocatedVolunteers != null)
            {
                string[] split = AllocatedVolunteers.Split(" / ");
                return split.Count() - 1;
            }
            return 0;
        }

        private string GetVolunteerNames(List<Volunteer> volunteers)
        {
            string volnames = "";
            for (int i = 0; i < volunteers.Count; i++)
            {
                var volunteer = volunteers[i];
                volnames = volnames + volunteer.Fullname + " / ";
            }
            return volnames;
        }

        private string GetAllocatedVolunteersString(Event event_, string id)
        {
            event_.AllocatedVolunteers += " / ";
            return event_.AllocatedVolunteers;
        }

        private List<Volunteer> GetVolunteersByIds(List<Volunteer> volunteers, string[] vols)
        {
            List<Volunteer> volunteerlist = new List<Volunteer>();
            for (int i = 0; i < vols.Length; i++)
            {
                Volunteer singlevolunteer = volunteers.Where(x => x._id == vols[i]).First();
                volunteerlist.Add(singlevolunteer);
            }
            return volunteerlist;
        }
    }

    public class EventsVolunteerAllocationRequest
    {
        public string EventId { get; set; }
        public string[] VolunteerIds { get; set; }

        public EventsVolunteerAllocationRequest(string[] volunteerIds, string eventId)
        {
            this.EventId = eventId;
            this.VolunteerIds = volunteerIds;
        }
    }

    public class EventsVolunteerAllocationResponse
    {
        public bool UpdateCompleted { get; set; }
        public List<KeyValuePair<string,string>> Message { get; set; }

        public EventsVolunteerAllocationResponse(bool updatedCompleted, List<KeyValuePair<string, string>> messages)
        {
            this.UpdateCompleted = updatedCompleted;
            this.Message = new List<KeyValuePair<string, string>>();
            if (messages.Count() != 0)
                this.Message = messages;
            else
                Message.Add(item: new KeyValuePair<string, string>("success", "The event has been successfuly updated!"));

        }
    }
}