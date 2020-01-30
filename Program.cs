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

            while (true) // Loop indefinitely
            {
                Console.WriteLine("Enter input:"); // Prompt
                string line = Console.ReadLine(); // Get string from user
                if (line == "exit") // Check string
                {
                    break;
                }
                ParseISO8583(line);
            }
 

        }

        static void ParseISO8583(string line)
        {

            ISO8583 iso8583 = new ISO8583();
            string transactionFileName = System.IO.Path.GetFullPath(Directory.GetCurrentDirectory() + @"\Transaction.txt");

            var transaction = line.ToUpper().Replace(" ", ""); //File.ReadAllText(transactionFileName).ToUpper().Replace(" ", "");

            string fileName = System.IO.Path.GetFullPath(Directory.GetCurrentDirectory() + @"\Configuration.json");

            var jsonString = File.ReadAllText(fileName);
            var tpdu = transaction.Substring(0, 10);
            var mti = transaction.Substring(10, 4);
            var bitmap = transaction.Substring(14, 16);
            var binarybitmap = HexStringToBinary(bitmap);

            Console.WriteLine("TPDU  " + tpdu);
            Console.WriteLine("MTI   " + mti);
            Console.WriteLine("BitMap  " + bitmap);
            Console.WriteLine("BinBitMap   " + binarybitmap);

            List<DataElement> dataElements = new List<DataElement>();
            dataElements = JsonSerializer.Deserialize<List<DataElement>>(jsonString);
            dataElements = BitmapDataElements(dataElements, binarybitmap);

            var trxTrimmed = transaction.Substring(30, transaction.Length - 30);
            int strLength = 0;

            foreach (DataElement de in dataElements)
            {
                if (de.Exist)
                {
                    switch (de.Type)
                    {
                        case "FIXED":
                            if (de.Format == "z")
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

                        case "LLVAR":
                            if (de.Format == "z")
                            {
                                strLength = int.Parse(trxTrimmed.Substring(0, 2));
                                if (IsOdd(strLength))
                                {
                                    strLength++;
                                }
                                de.RawValue = trxTrimmed.Substring(2, strLength);
                                strLength = strLength + 2;
                            }
                            else
                            {
                                strLength = int.Parse(trxTrimmed.Substring(0, 2));
                                strLength = strLength * 2;
                                de.RawValue = trxTrimmed.Substring(4, strLength);
                                strLength = strLength + 4;
                            }
                            break;

                        case "LLLVAR":
                            if (de.Format == "z")
                            {
                                strLength = int.Parse(trxTrimmed.Substring(0, 4));
                                if (IsOdd(strLength))
                                {
                                    strLength++;
                                }
                                de.RawValue = trxTrimmed.Substring(4, strLength);
                                strLength = strLength + 4;
                            }
                            else
                            {
                                strLength = int.Parse(trxTrimmed.Substring(0, 4));
                                strLength = strLength * 2;
                                de.RawValue = trxTrimmed.Substring(4, strLength);
                                strLength = strLength + 4;
                            }
                            break;

                    }
                    Console.WriteLine("[" + de.Id.ToString() + "]  " + de.RawValue);
                    trxTrimmed = trxTrimmed.Substring(strLength, (trxTrimmed.Length - strLength));
                }
            }


        }

        static List<DataElement> BitmapDataElements(List<DataElement> dataElements, string binaryBitmap)
        {
            for (int i = 0; i < 64; i++)
            {
                if (binaryBitmap[i].ToString() == "1")
                    dataElements[i].Exist = true;
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

        static bool IsOdd(int input)
        {
            if(input%2 !=0)
            {
                return true;
            }
            return false;
        }

    }
}
