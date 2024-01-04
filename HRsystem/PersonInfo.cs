using System;

namespace HRsystem
{
    public class PersonInfo
    {
        public string Names { get; set; }
        public string LastName { get; set; }
        public string NickName { get; set; }
        public string Hetu { get; set; }
        public string Street { get; set; }
        public string MailNumber { get; set; }
        public string Location { get; set; }
        public DateTime JobStart { get; set; }
        public DateTime JobEnd { get; set; }
        public string JobTitle { get; set; }
        public string Department { get; set; }

        public PersonInfo() { }

        public PersonInfo(string names, string lastname, string nickname, string hetu, string street, string mailnumber, string location, DateTime jobstart, DateTime jobend, string jobtitle, string department)
        {
            Names = names;
            LastName = lastname;
            NickName = nickname;
            Hetu = hetu;
            Street = street;
            MailNumber = mailnumber;
            Location = location;
            JobStart = jobstart;
            JobEnd = jobend;
            JobTitle = jobtitle;
            Department = department;
        }
    }
}

