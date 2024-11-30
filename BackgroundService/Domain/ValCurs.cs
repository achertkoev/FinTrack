using System.Xml.Serialization;

namespace BackgroundService.Domain;

[XmlRoot("ValCurs")]
public class ValCurs
{
    [XmlAttribute("Date")]
    public DateTime Date { get; set; }

    [XmlAttribute("name")]
    public string Name { get; set; }
    
    [XmlElement("Valute")]
    public List<Valute> Valutes { get; set; }
}