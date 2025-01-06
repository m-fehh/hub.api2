using Hub.Infrastructure.Architecture.Logger.Enums;
using Hub.Infrastructure.Architecture;
using System.ComponentModel.DataAnnotations;

namespace Hub.Application.Models.ViewModels
{
    public class LogVM
    {
        public long? Id { get; set; }

        [Display(Name = "CreateDate")]
        public DateTime CreateDate { get; set; }

        public long CreateDateInTicks
        {
            get
            {
                return CreateDate.Ticks;
            }
        }

        [Display(Name = "CreateDate")]
        public string Date
        {
            get
            {
                return CreateDate.ToString("d");
            }
        }

        [Display(Name = "Hour")]
        public string CreateHour
        {
            get
            {
                return CreateDate.ToString("HH:mm:ss");
            }
        }

        [Display(Name = "PortalUser")]
        public long? CreateUser_Id { get; set; }

        [Display(Name = "PortalUser")]
        public string CreateUser_Name { get; set; }

        [Display(Name = "ObjectId")]
        public long ObjectId { get; set; }

        [Display(Name = "ObjectName")]
        public string ObjectName { get; set; }

        [Display(Name = "ObjectName")]
        public string ObjectNameDescription
        {
            get
            {
                return Engine.Get(ObjectName);
            }
        }

        [Display(Name = "Message")]
        public string Message { get; set; }

        [Display(Name = "Action")]
        public ELogAction Action { get; set; }

        [Display(Name = "Action")]
        public string ActionDescriptionLog
        {
            get
            {
                return Engine.Get(Action.ToString());
            }
        }

        [Display(Name = "LogType")]
        public ELogType LogType { get; set; }

        [Display(Name = "LogType")]
        public string LogTypeDescription
        {
            get
            {
                return Engine.Get(LogType.ToString());
            }
        }

        [Display(Name = "Father")]
        public long? LogField_Id { get; set; }

        [Display(Name = "Father")]
        public string LogField_Name { get; set; }

        public int CountChildrens { get; set; }

        [Display(Name = "Establishment")]
        public string Establishment_Name { get; set; }

        [Display(Name = "PropertyName")]
        public string PropertyName { get; set; }

        [Display(Name = "OldValue")]
        public string OldValue { get; set; }

        [Display(Name = "NewValue")]
        public string NewValue { get; set; }

        [Display(Name = "IpAddress")]
        public string IpAddress { get; set; }

        [Display(Name = "InitialHour")]
        public string InitialHour { get; set; }

        [Display(Name = "FinalHour")]
        public string FinalHour { get; set; }

        public ISet<LogFieldVM> Children { get; set; }
    }
}
