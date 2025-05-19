using System;

namespace UPZhukov.ClassFolder
{
    public class AppealClass
    {
        public int AppealId { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string CitizenLastName { get; set; }
        public string CitizenFirstName { get; set; }
        public string CitizenMiddleName { get; set; }
        public string CitizenAddress { get; set; }
        public string CitizenPhone { get; set; }
        public string CitizenEmail { get; set; }
        public string AppealText { get; set; }
        public int TypeId { get; set; }
        public int StatusId { get; set; }
        public int RegistratorId { get; set; }
        public int? ExecutorId { get; set; }
        public string ResponseText { get; set; }
        public DateTime? ResponseDate { get; set; }
    }
} 