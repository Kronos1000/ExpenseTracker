using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace ExpenseTracker
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            string filePath;



            DateTime currentDate = DateTime.Now;

            // Append the current date to the file names
            string formattedDate = currentDate.ToString("MM-yyyy");
            

            // Define file names 
            string FoodtxtFileName = $"FoodTransactions_{formattedDate}.txt";
            string FoodcsvFileName = $"FoodTransactions_{formattedDate}.csv";

            string PetroltxtFileName = $"PetrolTransactions_{formattedDate}.txt";
            string PetrolcsvFileName = $"PetrolTransactions_{formattedDate}.csv";




            if (args.Length > 0)
            {
                // Use the file path provided as a command-line argument
                filePath = args[0];
            }
            else
            {
                Console.Write("Please drag and drop the file here and press Enter: ");
                filePath = Console.ReadLine().Trim('\"');
            }

            if (File.Exists(filePath))
            {
                // Get data 
                List<FoodTransaction> FoodTransactions = GetFoodTransactions(filePath);
               FoodTransaction[] FoodTransArray = FoodTransactions.ToArray();


                // read array data from external files


                List<PetrolTransaction> PetrolTransactions = GetPetrolTransactions(filePath);
                PetrolTransaction[] PetrolTransArray = PetrolTransactions.ToArray();

                // Initialize total amounts
                double totalFoodAmount = 0;
                double totalPetrolAmount = 0;

                // For loop to iterate through  food transactions array and print to console 
                Console.Clear();
                Console.WriteLine("-- Food  Expenditure  for the month -- ");
                for (int i = 0; i < FoodTransArray.Length; i++)
                {
                    // Access the vendor property of each transaction
                    Console.WriteLine(FoodTransArray[i].Date + " " + FoodTransArray[i].Vendor + " " + FoodTransArray[i].TransAmount);
                    totalFoodAmount += Convert.ToDouble(FoodTransArray[i].TransAmount);
                }

                // For loop to iterate through  Petrol  transactions array and print to console 
                
                Console.WriteLine("\n\n\n -- Petrol Expenditure  for the month -- ");
                for (int i = 0; i < PetrolTransArray.Length; i++)
                {
                    // Access the vendor property of each transaction
                    Console.WriteLine(PetrolTransArray[i].Date + " " + PetrolTransArray[i].Vendor + " " + PetrolTransArray[i].TransAmount);
                    totalPetrolAmount += Convert.ToDouble(PetrolTransArray[i].TransAmount);
                
                 }

                // write food transaction data to file

                using (StreamWriter writer = new StreamWriter(FoodtxtFileName))
                {
                    // Write headers to the file
                    writer.WriteLine("Date,Vendor,Transaction Amount");

                    // Populate data
                    foreach (FoodTransaction trans in FoodTransArray)
                    {
                        writer.WriteLine($"{trans.Date} {trans.TransAmount}");
                    }
                }

                using (StreamWriter writer = new StreamWriter(FoodcsvFileName))
                {
                    // Write headers to the file
                    writer.WriteLine("Date,Vendor,Transaction Amount");

                    // Populate data
                    foreach (FoodTransaction trans in FoodTransArray)
                    {
                        string VendorWithComma = trans.Vendor.Replace(';', ',');
                        writer.WriteLine($"{trans.Date},{VendorWithComma} {trans.TransAmount}");
                    }
                }

                // write petrol transaction data to file 

                using (StreamWriter writer = new StreamWriter(PetroltxtFileName))
                {
                    // Write headers to the file
                    writer.WriteLine("Date,Transaction Amount,Vendor");

                    // Populate data
                    foreach (PetrolTransaction trans in PetrolTransArray)
                    {
                        writer.WriteLine($"{trans.Date} {trans.Vendor} {trans.TransAmount}");
                    }
                }

                using (StreamWriter writer = new StreamWriter(PetrolcsvFileName))
                {
                    // Write headers to the file
                    writer.WriteLine("Date,Vendor,Transaction Amount");

                    // Populate data
                    foreach (PetrolTransaction trans in PetrolTransArray)
                    {
                        writer.WriteLine($"{trans.Date},{trans.Vendor},{trans.TransAmount}");
                    }
                }

                Console.WriteLine($"\nTotal Food Expenditure: ${totalFoodAmount:N2}");
                Console.WriteLine($"Total Petrol Expenditure: ${totalPetrolAmount}");
                Console.WriteLine("\n\n\n Data written to" + FoodcsvFileName);
                Console.WriteLine("Data written to" +FoodtxtFileName);

                Console.WriteLine("Data written to" + PetrolcsvFileName);
                Console.WriteLine("Data written to" + PetroltxtFileName);
            }
            else
            {
                Console.WriteLine("File not found.");
            }

            Console.ReadKey();
        }

        // Get transaction data method 
        public static List<FoodTransaction> GetFoodTransactions(string filePath)
        {


            string[] foodVendors = File.ReadAllLines("FoodVendors.txt");

            string  [] ignoreVendorList = File.ReadAllLines("VendorsToIgnore.txt");

            // Create a list to store transactions
            List<FoodTransaction> FoodTransactionList = new List<FoodTransaction>();

            // Read in file using stream reader 
            using (StreamReader reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    // Read in each line and store inside a variable for further processing
                    string line = reader.ReadLine();
                    string[] parts = line.Split(',');

                    string transDate = parts[1];
                    string vendor = parts[2];

                    string transAMT = parts[13];

                    // Create a new transaction using this data
                    FoodTransaction trans = new FoodTransaction(transDate, vendor ,transAMT);
                   
                    if (foodVendors.Any(foodVendor => vendor.Contains(foodVendor, StringComparison.OrdinalIgnoreCase))
                        && !ignoreVendorList.Any(ignoreVendor => vendor.Contains(ignoreVendor, StringComparison.OrdinalIgnoreCase))
                        && !vendor.Contains("FUEL", StringComparison.OrdinalIgnoreCase))
                    {
                        FoodTransactionList.Add(trans);
                    }

                }
            }

            // Return the data 
            return FoodTransactionList;
        }



        public static List<PetrolTransaction> GetPetrolTransactions(string filePath)
        {
           
            string[] petrolVendors = File.ReadAllLines("PetrolVendors.txt");
            string  [] ignoreVendorList = File.ReadAllLines("VendorsToIgnore.txt");


            // Create a list to store transactions
            List<PetrolTransaction> PetrolTransactionList = new List<PetrolTransaction>();

            // Read in file using stream reader 
            using (StreamReader reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    // Read in each line and store inside a variable for further processing
                    string line = reader.ReadLine();
                    string[] parts = line.Split(',');

                    string transDate = parts[1];
                    string vendor = parts[2];
                    string transAMT = parts[13];

                    // Create a new transaction using this data
                    PetrolTransaction trans = new PetrolTransaction(transDate, vendor, transAMT);
                    if (petrolVendors.Any(petrolVendor => vendor.Contains(petrolVendor, StringComparison.OrdinalIgnoreCase))
                  && !ignoreVendorList.Any(ignoreVendor => vendor.Contains(ignoreVendor, StringComparison.OrdinalIgnoreCase)))
                    {
                        PetrolTransactionList.Add(trans);
                    }
                }
            }

            // Return the data 
            return PetrolTransactionList;
        }
    }
}