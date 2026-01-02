using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ExpenseTracker
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;

            string filePath;

            DateTime currentDate = DateTime.Now;
            string formattedDate = currentDate.ToString("MM-yyyy");

            if (args.Length > 0)
            {
                filePath = args[0];
            }
            else
            {
                Console.Write("Please drag and drop the file here and press Enter: ");
                filePath = Console.ReadLine().Trim('\"');
            }

            if (!File.Exists(filePath))
            {
                Console.WriteLine("File not found.");
                return;
            }

            List<FoodTransaction> foodTransactions = GetFoodTransactions(filePath);
            List<PetrolTransaction> petrolTransactions = GetPetrolTransactions(filePath);

            double totalFoodAmount = 0;
            double totalPetrolAmount = 0;

            Console.Clear();
            Console.WriteLine("-- Food Expenditure for the month --");

            foreach (var trans in foodTransactions)
            {
                DateTime date = DateTime.Parse(trans.Date);
                double amount = Math.Abs(Convert.ToDouble(trans.TransAmount));

                Console.WriteLine($"{date:dd/MM/yyyy} {trans.Vendor} {amount:N2}");
                totalFoodAmount += amount;
            }

            Console.WriteLine("\n-- Petrol Expenditure for the month --");

            foreach (var trans in petrolTransactions)
            {
                DateTime date = DateTime.Parse(trans.Date);
                double amount = Math.Abs(Convert.ToDouble(trans.TransAmount));

                Console.WriteLine($"{date:dd/MM/yyyy} {trans.Vendor} {amount:N2}");
                totalPetrolAmount += amount;
            }

            Console.WriteLine($"\nTotal Food Expenditure: ${totalFoodAmount:N2}");
            Console.WriteLine($"Total Petrol Expenditure: ${totalPetrolAmount:N2}");
            Console.ReadKey();
        }

        // ---------------- FOOD ----------------
        public static List<FoodTransaction> GetFoodTransactions(string filePath)
        {
            string[] foodVendors = File.ReadAllLines("FoodVendors.txt");

            List<FoodTransaction> list = new List<FoodTransaction>();

            using (StreamReader reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] parts = line.Split(',');

                    if (parts.Length < 14)
                        continue;

                    string transDate = parts[1];
                    string vendor = parts[3];
                    string transAMT = parts[13];

                    bool isFoodVendor =
                        foodVendors.Any(v => vendor.Contains(v, StringComparison.OrdinalIgnoreCase));

                    // 🔑 FOOD OVERRIDES IGNORE LIST
                    if (isFoodVendor && !vendor.Contains("FUEL", StringComparison.OrdinalIgnoreCase))
                    {
                        list.Add(new FoodTransaction(transDate, vendor, transAMT));
                    }
                }
            }

            return list;
        }

        // ---------------- PETROL ----------------
        public static List<PetrolTransaction> GetPetrolTransactions(string filePath)
        {
            string[] petrolVendors = File.ReadAllLines("PetrolVendors.txt");
            string[] ignoreVendorList = File.ReadAllLines("VendorsToIgnore.txt");

            List<PetrolTransaction> list = new List<PetrolTransaction>();

            using (StreamReader reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] parts = line.Split(',');

                    if (parts.Length < 14)
                        continue;

                    string transDate = parts[1];
                    string vendor = parts[3];
                    string transAMT = parts[13];

                    if (
                        petrolVendors.Any(v => vendor.Contains(v, StringComparison.OrdinalIgnoreCase)) &&
                        !ignoreVendorList.Any(v => vendor.Contains(v, StringComparison.OrdinalIgnoreCase))
                    )
                    {
                        list.Add(new PetrolTransaction(transDate, vendor, transAMT));
                    }
                }
            }

            return list;
        }
    }
}