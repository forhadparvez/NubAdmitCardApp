using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathCaptcha
{
    internal class CaptchaNumber
    {
        public static List<int> GetNumber()
        {
            var l=new List<int>();
            var r= new Random();
            var n1 = r.Next(1, 10);
            var n2 = r.Next(1, 10);

            l.Add(n1);
            l.Add(n2);
            return l;
        }

        public static string GetOperator()
        {
            var l=new List<string>()
            {
                "+","-","X"
            };

            var r = new Random();
            var n = r.Next(0, 2);

            return l[n];
        }

        public static int GetAnswer(int x, int y, string o)
        {
            switch (o)
            {
                case "+":
                    return x + y;
                case "-":
                    return x - y;
                case "X":
                    return x * y;
            }

            return 0;
        }
        
    }
}
