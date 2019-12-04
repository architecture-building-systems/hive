using System;

namespace ConsoleTester
{
    class Program
    {
        static void Main(string[] args)
        {
            double[] testy = new double[2] { 1, 2 };
            Test test = new Test(testy);
            foreach(double a in test.Testy) Console.WriteLine(a);
            Console.ReadKey();
        }


        struct Test
        {
            internal double[] Testy;
            internal Test(double[] testy)
            {
                Testy = testy;
            }
        }
    }
}
