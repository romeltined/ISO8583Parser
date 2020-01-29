using System;
using System.Collections.Generic;
using System.Text;

namespace ISO8583Parser
{
    public class DataElement
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string RawValue {get;set;}
        public string Value { get; set; }
        public int Length { get; set; }
        public string Type { get; set; }
        public string Format { get; set; }
        public bool Exist { get; set; }
    }
}
