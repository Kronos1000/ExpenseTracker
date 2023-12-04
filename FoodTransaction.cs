using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker
{
    public class FoodTransaction
    {
        public string date;

        public string Date
        {
            get { return date; }
            set { date = value; }
        }

        public string vendor;

        public string Vendor
        {
            get { return vendor; }
            set { vendor = value; }

        }
        public string transAmount;

        public string TransAmount
        {
            get { return transAmount; }
            set { transAmount = value; }
        }

        public FoodTransaction(string date, string vendor, string transAmount)
        {
            Date = date;
            Vendor = vendor;
            TransAmount = transAmount;
        }



    }
}
