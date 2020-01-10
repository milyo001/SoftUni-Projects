


namespace TeisterMask.DataProcessor.ExportDto
{
    using Data.Models;
    using System.Xml.Serialization;

    [XmlType(TypeName = "Task")]
    public class ExportTaskDto
    {
        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("Label")]
        public string Label { get; set; }
    }
}
