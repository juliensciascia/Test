using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CSProject
{
    class Staff
    {
        //fields
        private float hourlyRate;
        private int hWorked;

        //properties
        public float TotalPay { get; protected set; }
        public float BasicPay { get; protected set; }
        public string NameOfStaff { get; protected set; }
        public int HoursWorked
        {
            get
            {
                return hWorked;
            }

            set
            {
                if (value > 0)
                {
                    hWorked = value;
                }
                else
                {
                    hWorked = 0;
                }  
            }
        }

        //constructor
        public Staff(string name,float rate)
        {
            NameOfStaff = name;
            hourlyRate = rate;
        }

        //methods
        public virtual void CalculatePay()
        {
            Console.WriteLine("Calculating Pay...");
            BasicPay = hWorked * hourlyRate;
            TotalPay = BasicPay;
        }

        public override string ToString()
        {
            return "Name of Staff: " + NameOfStaff +
                "\nHourly rate: " + hourlyRate +
                "\n hWorked: " + hWorked +
                "\n Total Pay: " + TotalPay +
                "\n Basic Pay: " + BasicPay;    
        }
    }

    class Manager : Staff
    {
        private const float managerHourlyRate = 50f;

        public int Allowance { get; private set; }

        public Manager(string name) : base(name, managerHourlyRate)
        {
            
        }

        public override void CalculatePay()
        {
            //call the parent method
            base.CalculatePay();
            Allowance = 1000;
            if ( HoursWorked > 160)
            {
                TotalPay = BasicPay + Allowance;
            }
            else
            {
                TotalPay = BasicPay;
            }
        }

        public override string ToString()
        {
            return "Name of Staff: " + NameOfStaff +
                "\n Total Pay: " + TotalPay +
                "\n Basic Pay: " + BasicPay +
                "\n Allowance: " + Allowance;
        }

    }

    class Admin : Staff
    {
        private const float overtimeRate = 15.5f;
        private const float adminHourlyRate = 30.0f;

        public float Overtime { get; private set; }

        public Admin(string name) : base (name, adminHourlyRate)
        {

        }

        public override void CalculatePay()
        {
            base.CalculatePay();
            if (HoursWorked > 160)
            {
                Overtime = overtimeRate * (HoursWorked - 160);
                TotalPay = BasicPay + Overtime; 
            }
            else
            {
                TotalPay = BasicPay;
            }
        }

        public override string ToString()
        {
            return "Name of Staff: " + NameOfStaff +
                "\n Total Pay: " + TotalPay +
                "\n Basic Pay: " + BasicPay +
                "\n Overtime: " + Overtime;
        }
    }

    class FileReader
    {
        public List<Staff> ReadFile()
        {
            List<Staff> myStaff = new List<Staff>();
            string[] result = new string[2];
            string path = "staff.txt";
            string[] separator = { ", " };

            if (File.Exists(path))
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    while(sr.EndOfStream != true)
                    {
                        result = sr.ReadLine().Split(separator, StringSplitOptions.RemoveEmptyEntries);
                        if (result[1] == "Manager")
                        {
                            Manager mManager = new Manager(result[0]);
                            myStaff.Add(mManager);
                        }
                        else if (result[1] == "Admin")
                        {
                            Admin mAdmin = new Admin(result[0]);
                            myStaff.Add(mAdmin);
                        }
                    }

                    sr.Close();
                }

            }
            else
            {
                Console.WriteLine("File does not exist");
            }

            return myStaff;

        } 
    }

    class PaySlip
    {
        private int month;
        private int year;

        //enum declared in a class is by default private
        enum MonthsOfYear
        {
            JAN = 1,
            FEB = 2,
            MAR = 3,
            APR = 4,
            MAY = 5,
            JUN = 6,
            JUL = 7,
            AUG = 8,
            SEP = 9,
            OCT = 10,
            NOV = 11,
            DEC = 12
        }

        public PaySlip(int payMonth, int payYear)
        {
            month = payMonth;
            year = payYear;
        }

        public void GeneratePaySlip(List<Staff> myStaff)
        {
            string path;
            foreach (Staff f in myStaff)
            {
                path = f.NameOfStaff + ".txt";
                using (StreamWriter sw = new StreamWriter(path, false))
                {
                    //cast into enum
                    sw.WriteLine("PAYSLIP FOR {0} {1}", (MonthsOfYear)month, year);
                    sw.WriteLine("==============");
                    sw.WriteLine("Name of Staff: {0}", f.NameOfStaff);
                    sw.WriteLine("Name of Staff: {0}", f.HoursWorked);
                    sw.WriteLine("\nBasic Pay: {0:C}", f.BasicPay);
                    if (f.GetType() == typeof(Manager))
                    {
                        sw.WriteLine("Allowance: {0:C}", ((Manager)f).Allowance);
                    }
                    else if (f.GetType() == typeof(Admin))
                    {
                        sw.WriteLine("Overtime: {0:C}", ((Admin)f).Overtime);
                    }

                    sw.WriteLine("\n==============");
                    sw.WriteLine("Total pay: {0:C}", f.TotalPay);
                    sw.WriteLine("==============");

                    sw.Close();
                }
            }
        }

        public override string ToString()
        {
            return "month = " + month + "year = " + year;
 
        }

        public void GenerateSummary(List<Staff> myStaff)
        {
            var result = from s in myStaff where s.HoursWorked < 10 orderby s.NameOfStaff ascending select new { s.NameOfStaff, s.HoursWorked };
            string path = "summary.txt";

            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.WriteLine("Staff with less than 10 working hours");

                foreach(var s in result)
                {
                    sw.WriteLine("\n Name of Staff: {0} , Hours Worked: {1}",s.NameOfStaff,s.HoursWorked);
                }

                sw.Close();
            }
        }

    }

    class Program
    {
        static void Main(string[] args)
        {

            int month =0;
            int year = 0;

            List<Staff> myStaff = new List<Staff>();
            FileReader fr = new FileReader();

            while (year == 0)
            {
                Console.Write("Please enter the year: ");

                try
                {
                    year = Convert.ToInt32(Console.ReadLine());
                }
                catch (FormatException)
                {
                    Console.WriteLine("Please input a valid Year");
                    year = 0;
                }
            }

            while (month == 0)
            {
                Console.Write("Please enter the month:");

                try
                {
                    month= Convert.ToInt32( Console.ReadLine());

                   if (month < 1 || month >12)
                   {
                       Console.WriteLine("Please enter a valid month");
                        month = 0;
                   }

                }
                catch (Exception)
                {
                    Console.WriteLine("Please input a valid Month");
                    month = 0;
                }
            }

            myStaff =fr.ReadFile();

            for (int i = 0; i < myStaff.Count; i++){
                try
                {
                    Console.WriteLine("Enter hours worked for {0}", myStaff[i].NameOfStaff);
                    myStaff[i].HoursWorked = Convert.ToInt32(Console.ReadLine());
                    myStaff[i].CalculatePay();
                    Console.WriteLine(myStaff[i].ToString());
                }
                catch(Exception e)
                {
                    Console.WriteLine("Enter a correct number of hours");
                    i--;
                }
            }

            PaySlip ps = new PaySlip(month,year);
            ps.GeneratePaySlip(myStaff);
            ps.GenerateSummary(myStaff);
            Console.Read();
        }
    
    }
}
