namespace Cinema.DataProcessor.ImportDto
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    [XmlType("Customer")]
    public class ImportCustomerDto
    {
        [MinLength(3), MaxLength(20), Required]
        [XmlElement(ElementName = "FirstName")]
        public string FirstName { get; set; }

        [MinLength(3), MaxLength(20), Required]
        [XmlElement(ElementName = "LastName")]
        public string LastName { get; set; }

        [Range(12, 110), Required]
        [XmlElement(ElementName = "Age")]
        public int Age { get; set; }

        [Range(0.01, double.MaxValue), Required]
        [XmlElement(ElementName = "Balance")]
        public decimal Balance { get; set; }

        [XmlArray(ElementName = "Tickets")]
        public ImportTicketDto[] Tickets { get; set; }
    }
    
}