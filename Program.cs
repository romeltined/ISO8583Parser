using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace ISO8583Parser
{
    class Program
    {
        static void Main(string[] args)
        {
            ISO8583 iso8583 = new ISO8583();
            string transactionFileName = System.IO.Path.GetFullPath(Directory.GetCurrentDirectory() + @"\Transaction.txt");

            var transaction = File.ReadAllText(transactionFileName).ToUpper();



            string fileName = System.IO.Path.GetFullPath(Directory.GetCurrentDirectory() + @"\Configuration.json");


            var jsonString = File.ReadAllText(fileName);
            var tpdu = transaction.Substring(0, 10);
            var mti = transaction.Substring(10, 4);
            var bitmap = transaction.Substring(14, 16);
            var binarybitmap = HexStringToBinary(bitmap);

            List<DataElement> dataElements = new List<DataElement>();
            dataElements = JsonSerializer.Deserialize<List<DataElement>>(jsonString);
            dataElements = BitmapDataElements(dataElements, binarybitmap);

            //int trxLength = transaction.Length-30;
            var trxTrimmed = transaction.Substring(30, transaction.Length - 30);
            int strLength = 0;

            foreach(DataElement de in dataElements)
            {
                if (de.Exist)
                { 
                    switch (de.Type)
                    {
                        case "FIXED":
                            if (de.Format=="z")
                            {
                                strLength = de.Length;
                                de.RawValue = trxTrimmed.Substring(0, strLength);
                            }
                            else
                            {
                                strLength = de.Length * 2;
                                de.RawValue = trxTrimmed.Substring(0, strLength);
                            }
                        
                            break;


                    }
                    trxTrimmed = trxTrimmed.Substring(strLength, (trxTrimmed.Length - strLength));
                }
            }

            iso8583.TPDU = tpdu;
            iso8583.MTI = mti;
            iso8583.Bitmap = bitmap;
            iso8583.BinaryBitmap = binarybitmap;
            iso8583.DataElements = dataElements;

        }


        static List<DataElement> BitmapDataElements(List<DataElement> dataElements, string binaryBitmap)
        {
            for (int i = 1; i < 14; i++)
            {
                if (binaryBitmap[i].ToString() == "1")
                    dataElements[i-1].Exist = true;
            }
            return dataElements;
        }

        static string HexStringToBinary(string hexString)
        {
            var lup = new Dictionary<char, string>{
            { '0', "0000"},
            { '1', "0001"},
            { '2', "0010"},
            { '3', "0011"},

            { '4', "0100"},
            { '5', "0101"},
            { '6', "0110"},
            { '7', "0111"},

            { '8', "1000"},
            { '9', "1001"},
            { 'A', "1010"},
            { 'B', "1011"},

            { 'C', "1100"},
            { 'D', "1101"},
            { 'E', "1110"},
            { 'F', "1111"}};

            var ret = string.Join("", from character in hexString
                                      select lup[character]);
            return ret;
        }

    }
}
