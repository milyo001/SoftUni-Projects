using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;
using TeisterMask.Data.Models.Enums;

namespace TeisterMask.DataProcessor.ImportDto
{
    [XmlType("Task")]
    public class ImportTaskDto
    {

        [XmlElement("Name"), MinLength(2), MaxLength(40), Required]
        public string Name { get; set; }

        [XmlElement("OpenDate"), Required]
        public string OpenDate { get; set; }

        [XmlElement("DueDate"), Required]
        public string  DueDate { get; set; }

        [XmlElement("ExecutionType"), Required]
        public int ExecutionType { get; set; }

        [XmlElement("LabelType"), Required]
        public int LabelType { get; set; }
    }
}
