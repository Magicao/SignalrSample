using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            List<int> collection1 = new List<int>() { 3,4 };
            List<int> collection2 = new List<int>() { 1,2,3};

            var ExecptResult = collection1.Except(collection2);//差集
            var IntersectResult = collection1.Intersect(collection2);//交集
            var UnionResult = collection1.Union(collection2);//并集

            foreach (var singleResult in ExecptResult)
            {
                Console.WriteLine(singleResult.ToString());
            }
            Console.ReadLine();
        }
    }
}
