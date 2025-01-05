using System.ComponentModel.DataAnnotations;

namespace Hub.Application.Models.ViewModels
{
    public class LogFieldVM
    {
        public long? Id { get; set; }

        [Display(Name = "Log")]
        public long? Log_Id { get; set; }

        [Display(Name = "Log")]
        public string Log_Name { get; set; }

        [Display(Name = "PropertyName")]
        public string PropertyName { get; set; }

        [Display(Name = "OldValue")]
        public string OldValue { get; set; }

        [Display(Name = "NewValue")]
        public string NewValue { get; set; }

        public int CountChildrens { get; set; }

        [Display(Name = "CreateDate")]
        public DateTime CreateDate { get; set; }

        public long CreateDateInTicks
        {
            get
            {
                return CreateDate.Ticks;
            }
        }
    }
}
