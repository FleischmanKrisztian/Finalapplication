﻿using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;

namespace BucuriaDarului.Contexts.VolunteerContexts
{
    public class VolunteerExporterContext
    {
        private readonly IStringLocalizer localizer;

        public VolunteerExporterContext(IStringLocalizer localizer)
        {
            this.localizer = localizer;
        }

        public VolunteerExporterResponse Execute(VolunteerExporterRequest request)
        {
            var idsAndFields = GetIdAndFieldString(request.ExportParameters);
            var header = GetHeaderForExcelPrinterVolunteer();
            var response = new VolunteerExporterResponse(CreateDictionaries(Constants.VOLUNTEERSESSION, Constants.VOLUNTEERHEADER, idsAndFields, header));
            response.IsValid = CheckForProperties(idsAndFields);
            return response;
        }

        private bool CheckForProperties(string idsAndFields)
        {
            var splitStrings = idsAndFields.Split("(((");
            if (splitStrings[1] == "")
                return false;

            return true;
        }

        private string GetIdAndFieldString(ExportParameters csv)
        {
            var idsAndOptions = csv.StringOfIDs + "(((";
            if (csv.All)
                idsAndOptions += "0";
            if (csv.Fullname)
                idsAndOptions += "1";
            if (csv.Birthdate)
                idsAndOptions += "2";
            if (csv.Address)
                idsAndOptions += "3";
            if (csv.Gender)
                idsAndOptions += "4";
            if (csv.DesiredWorkplace)
                idsAndOptions += "5";
            if (csv.CNP)
                idsAndOptions += "6";
            if (csv.FieldOfActivity)
                idsAndOptions += "7";
            if (csv.Occupation)
                idsAndOptions += "8";
            if (csv.HasId)
                idsAndOptions += "9";
            if (csv.IdInfo)
                idsAndOptions += "A";
            if (csv.IdExpirationDate)
                idsAndOptions += "B";
            if (csv.Active)
                idsAndOptions += "C";
            if (csv.HourCount)
                idsAndOptions += "D";
            if (csv.PhoneNumber)
                idsAndOptions += "E";
            if (csv.EmailAddress)
                idsAndOptions += "F";
            if (csv.DriversLicense)
                idsAndOptions += "G";
            if (csv.HasCar)
                idsAndOptions += "H";
            return idsAndOptions;
        }

        private string GetHeaderForExcelPrinterVolunteer()
        {
            var header = new string[18];
            header[0] = localizer["Fullname"];
            header[1] = localizer["Birthdate"];
            header[2] = localizer["Address"];
            header[3] = localizer["Gender"];
            header[4] = localizer["DesiredWorkplace"];
            header[5] = localizer["CNP"];
            header[6] = localizer["FieldOfActivity"];
            header[7] = localizer["Occupation"];
            header[8] = localizer["HasId"];
            header[9] = localizer["IdInfo"];
            header[10] = localizer["IdExpirationDate"];
            header[11] = localizer["Active"];
            header[12] = localizer["HourCount"];
            header[13] = localizer["PhoneNumber"];
            header[14] = localizer["EmailAddress"];
            header[15] = localizer["DriversLicense"];
            header[16] = localizer["HasCar"];

            var result = string.Empty;
            for (var i = 0; i < header.Count(); i++)
            {
                if (i == 0)
                { result = header[i]; }
                else
                { result = result + "," + header[i]; }
            }
            return result;
        }

        private Dictionary<string, string> CreateDictionaries(string key1, string key2, string idsAndFields, string header)
        {
            var result = new Dictionary<string, string>
            {
                { key1, idsAndFields },
                { key2, header }
            };

            return result;
        }
    }

    public class VolunteerExporterResponse
    {
        public Dictionary<string, string> Dictionary { get; set; }

        public bool IsValid { get; set; }

        public VolunteerExporterResponse(Dictionary<string, string> dictionary)
        {
            Dictionary = dictionary;
            IsValid = true;
        }
    }

    public class VolunteerExporterRequest
    {
        public VolunteerExporterRequest(ExportParameters exportProperties)
        {
            ExportParameters = exportProperties;
        }

        public ExportParameters ExportParameters { get; set; }
    }

    public class ExportParameters
    {
        public string StringOfIDs { get; set; }
        public bool All { get; set; }
        public bool Fullname { get; set; }
        public bool Birthdate { get; set; }
        public bool Address { get; set; }
        public bool Gender { get; set; }
        public bool DesiredWorkplace { get; set; }
        public bool CNP { get; set; }
        public bool FieldOfActivity { get; set; }
        public bool Occupation { get; set; }
        public bool HasId { get; set; }
        public bool IdInfo { get; set; }
        public bool IdExpirationDate { get; set; }
        public bool Active { get; set; }
        public bool HourCount { get; set; }
        public bool PhoneNumber { get; set; }
        public bool EmailAddress { get; set; }
        public bool DriversLicense { get; set; }
        public bool HasCar { get; set; }
    }
}