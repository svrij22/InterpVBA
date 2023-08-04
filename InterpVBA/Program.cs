using System;
using System.Collections.Generic;
using System.Threading;

namespace InterpVBA
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            List<double> xs = new() { 2, 5, 9, 13, 21 };
            List<double> ys = new() { 2, 7, 1, 7, 2 };

            for (int i = 4; i <= 19; i++)
            {
                var bez = BezierInterpolation.Bezier(xs, ys, i);
                Console.WriteLine(bez);
            }

            while (true)
            {
                Thread.Sleep(100);
            }
        }
    }
}
