using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exercise1
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Angiv string 1: ");
                string s1 = Console.ReadLine();
                Console.WriteLine("Angiv string 2: ");
                string s2 = Console.ReadLine();

                Console.WriteLine("Without sketches:\n" + NearDuplicatesWithoutSketches.NearDuplicate(s1, s2, 4, 0.9f));

                Console.WriteLine("With sketches: \n" + NearDuplicatesWithSketches.NearDuplicate(s1, s2, 4, 0.9f));
                Console.ReadLine();
                Console.Clear();
            }
        }


    }
}
