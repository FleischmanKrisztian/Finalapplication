﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BucuriaDarului.Core.Gateways
{
    public interface IEventSponsorAllocationUpdateGateway
    {
        List<Sponsor> GetListOfSponsors();
        void UpdateEvent(string EventId, Event event_);
    }
}