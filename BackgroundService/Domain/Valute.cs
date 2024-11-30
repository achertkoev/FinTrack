using System.Xml.Serialization;

namespace BackgroundService.Domain;

public class Valute
{
    [XmlAttribute("ID")]
    public string ID { get; set; }

    [XmlElement("NumCode")]
    public string NumCode { get; set; }

    [XmlElement("CharCode")]
    public string CharCode { get; set; }

    [XmlElement("Nominal")]
    public int Nominal { get; set; }

    [XmlElement("Name")]
    public string Name { get; set; }

    [XmlElement("Value")]
    public decimal Value { get; set; }

    [XmlElement("VunitRate")]
    public decimal VunitRate { get; set; }
}