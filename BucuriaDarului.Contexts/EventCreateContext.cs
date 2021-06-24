﻿using BucuriaDarului.Core;
using BucuriaDarului.Core.Gateways;
using Newtonsoft.Json;
using System;

namespace BucuriaDarului.Contexts
{
    public class EventCreateContext
    {
        private readonly IEventCreateGateway dataGateway;
        EventCreateResponse response = new EventCreateResponse("", false, true);

        public EventCreateContext(IEventCreateGateway dataGateway)
        {
            this.dataGateway = dataGateway;
        }

        public EventCreateResponse Execute(EventCreateRequest request)
        {
            
            var noNullRequest = ChangeNullValues(request);

            if (ContainsSpecialchar(noNullRequest))
            {
                response.ContainsSpecialChar = true;
                response.Message = "The Object Cannot contain Semi-Colons! ";
            }

            var @event = ValidateRequest(noNullRequest);

            if (response.ContainsSpecialChar == false && response.IsValid)
            {
                dataGateway.Insert(@event);
            }
            return response;
        }

        private Event ValidateRequest(EventCreateRequest request)
        {
            if(request.NameOfEvent=="")
            {
                response.Message += "The Event must have a name! ";
                response.IsValid = false;
            }

            var validatedEvent = new Event();
            validatedEvent._id = Guid.NewGuid().ToString();
            validatedEvent.NameOfEvent = request.NameOfEvent;
            validatedEvent.PlaceOfEvent = request.PlaceOfEvent;
            validatedEvent.DateOfEvent = request.DateOfEvent.AddHours(5);            
            validatedEvent.NumberOfVolunteersNeeded = request.NumberOfVolunteersNeeded;
            validatedEvent.TypeOfActivities = request.TypeOfActivities;
            validatedEvent.TypeOfEvent = request.TypeOfEvent;
            validatedEvent.Duration = request.Duration;
            validatedEvent.AllocatedVolunteers = request.AllocatedVolunteers;
            validatedEvent.NumberAllocatedVolunteers = request.NumberAllocatedVolunteers;
            validatedEvent.AllocatedSponsors = request.AllocatedSponsors;

            return validatedEvent;
        }

        private bool ContainsSpecialchar(object @event)
        {
            string eventString = JsonConvert.SerializeObject(@event);
            bool containsSpecialChar = false;
            if (eventString.Contains(";"))
            {
                containsSpecialChar = true;
            }
            return containsSpecialChar;
        }

        private EventCreateRequest ChangeNullValues(EventCreateRequest request)
        {
            foreach (var property in request.GetType().GetProperties())
            {
                var propertyType = property.PropertyType;
                var value = property.GetValue(request, null);
                if (propertyType == typeof(string) && value == null)
                {
                    property.SetValue(request, string.Empty);
                }
            }
            return request;
        }
    }

    public class EventCreateResponse
    {
        public string Message { get; set; }

        public bool IsValid { get; set; }

        public bool ContainsSpecialChar { get; set; }

        public EventCreateResponse(string message, bool containsSpecialChar, bool isValid)
        {
            Message = message;
            ContainsSpecialChar = containsSpecialChar;
            IsValid = isValid;
        }
    }

    public class EventCreateRequest
    {
        public string NameOfEvent { get; set; }

        public string PlaceOfEvent { get; set; }

        public DateTime DateOfEvent { get; set; }

        public int NumberOfVolunteersNeeded { get; set; }

        public string TypeOfActivities { get; set; }

        public string TypeOfEvent { get; set; }

        public string Duration { get; set; }

        public string AllocatedVolunteers { get; set; }

        public int NumberAllocatedVolunteers { get; set; }

        public string AllocatedSponsors { get; set; }
    }
}