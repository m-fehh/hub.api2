using Hub.Application.Corporate.Interfaces;
using Hub.Application.Services.Enterprise;
using Hub.Infrastructure.Architecture;
using System.Runtime.InteropServices;
using TimeZoneConverter;

namespace Hub.Application.Corporate.Manager
{
    public class TimezoneManager : ICurrentTimezone
    {
        public TimeZoneInfo Get()
        {
            var establishemntTimezone = Engine.Resolve<OrganizationalStructureService>().GetCurrentEstablishmentTimeZone();

            if (establishemntTimezone != null)
            {
                return establishemntTimezone;
            }

            return TimeZoneInfo.Local;
        }

        public string GetName()
        {
            var current = Get().Id;

            if (TZConvert.TryWindowsToIana(current, out var iana))
            {
                return iana;
            }
            else
            {
                return current;
            }
        }

        public string GetServerName()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return TimeZoneInfo.Local.Id;
            }

            return TZConvert.WindowsToIana(TimeZoneInfo.Local.Id);
        }

        public DateTime? Convert(DateTime? date, TimeZoneInfo tz = null)
        {
            if (date == null) return null;

            //if (tz == null) tz = Get();

            if (tz == TimeZoneInfo.Local) return date;

            var utc = TimeZoneInfo.ConvertTimeToUtc(date.Value);

            return TimeZoneInfo.ConvertTimeFromUtc(utc, tz);
        }

        public DateTime? ConvertServer(DateTime? date, TimeZoneInfo tz = null)
        {
            if (date == null) return null;

            //if (tz == null) tz = Get();

            if (tz == TimeZoneInfo.Local) return date;

            var utc = TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(date.Value, DateTimeKind.Unspecified), tz);

            return TimeZoneInfo.ConvertTimeFromUtc(utc, TimeZoneInfo.Local);
        }
    }
}
