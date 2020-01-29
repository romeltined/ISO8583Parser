using System;
using System.Collections.Generic;
using System.Text;

namespace ISO8583Parser
{
    public class ISO8583
    {
        public string TPDU { get; set; }
        public string MTI { get; set; }
        public string Bitmap { get; set; }
        public string BinaryBitmap { get; set; }
        public List<DataElement> DataElements { get; set; }
    }
}
