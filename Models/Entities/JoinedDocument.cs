using System;

namespace DocumentManagementApp.Models
{
    public class JoinedDocument
    {
        public int DocumentId { get; set; }
        public string Type { get; set; }
        public DateTime Date { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string City { get; set; }

        public int Ordinal { get; set; }
        public string Product { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public int TaxRate { get; set; }
    }
}
