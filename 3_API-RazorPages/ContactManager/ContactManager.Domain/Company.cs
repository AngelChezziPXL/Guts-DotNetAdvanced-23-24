using System.ComponentModel.DataAnnotations;

namespace ContactManager.Domain
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Zip { get; set; }
        public string City { get; set; }
        public List<Contact> Contacts { get; set; }
    }
}
