using System;

namespace bitconverter
{
    public class Program
    {
        private static int exp = 0;
        private static int dotPos = 0;

        public static string FloatConvert32(double input)
        {
            int sign;

            if (input < 0)
                sign = 1;
            else
                sign = 0;

            input = Math.Abs(input);
            string bits = IntBitConvert(input);
            string normalized = "";
            if (bits.Length > 1)
                normalized = bits.Insert(1, ".");
            else
                normalized = "0";
            int newExp = exp + 127;
            int newDotPos = dotPos;
            string expBits = "";
            if (input == 0)
                expBits = "00000000";
            else if (input == 1)
                expBits = "01111111";
            else
            {
                expBits = IntBitConvert(newExp);
                if (expBits.Length > 8)
                    expBits = expBits.Substring(0, 8);
                else
                {
                    while (expBits.Length < 8)
                        expBits = expBits.Insert(0, "0");
                }
            }
            string mantissa = "";
            if (normalized.Length > 2)
                mantissa = normalized.Substring(2);
            if (mantissa.Length > 23)
                mantissa = mantissa.Substring(0, 23);
            while (mantissa.Length < 23)
                mantissa += "0";
            return sign + " " + expBits + " " + mantissa;

        }

        public static string FloatConvert64(double input)
        {
            int sign;

            if (input < 0)
                sign = 1;
            else
                sign = 0;

            input = Math.Abs(input);
            string bits = IntBitConvert(input);
            string normalized = "";
            if (bits.Length > 1)
                normalized = bits.Insert(1, ".");
            int newExp = exp + 1023;
            int newDotPos = dotPos;
            string expBits = "";
            if (input == 0)
                expBits = "00000000000";
            else if (input == 1)
                expBits = "01111111111";
            else
            {
                expBits = IntBitConvert(newExp);
                if (expBits.Length > 11)
                    expBits = expBits.Substring(0, 11);
                else
                {
                    while (expBits.Length < 11)
                        expBits = expBits.Insert(0, "0");
                }
            }
            string mantissa = "";
            if (normalized.Length > 2)
                mantissa = normalized.Substring(2);
            while (mantissa.Length < 52)
                mantissa += "0";
            if (mantissa.Length > 52)
                mantissa = mantissa.Substring(0, 52);
            return sign + " " + expBits + " " + mantissa;

        }

        //Returns an unsigned bit representation of the specified number and sets global 'exp' and "dotPos".
        //Does NOT include a "." between integral and fractional parts by design.
        public static String IntBitConvert(double input)
        {
            double floor = Math.Floor(input);
            double frac = input - floor;

            int e = 0;
            while (Math.Pow(2, e) <= floor)
                e++;
            e = e - 1;                            //The largest bit lower than input is 2^(e-1)
            string bits = "";
            double temp;
            if (input > 1 || input < 0)
            {
                bits += "1";

                temp = Math.Pow(2, e);
                for (int i = e - 1; i >= 0; i--)
                {
                    if (temp + Math.Pow(2, i) <= floor)
                    {
                        temp += Math.Pow(2, i);
                        bits += "1";
                    }
                    else
                        bits += "0";
                }
                exp = bits.Length - 1;
            }

            if (frac == 0)
                return bits;
            dotPos = bits.Length;
            temp = 0;
            e = -1;
            while ( temp <= frac && e > -80)
            {
                if (temp + Math.Pow(2, e) <= frac)
                {
                    temp += Math.Pow(2, e);
                    bits += "1";
                }

                else
                    bits += "0";
                e--;
            }
            temp = 1;
            if (input < 1 && input > 0)
            {
                for (int i = dotPos; i < bits.Length; i++)
                {
                    if (bits[i] == '1')
                    {
                        exp = (int)temp * -1;
                        break;
                    }
                    temp++;
                }
                if (input < .5)
                    bits = bits.Remove(0, (int)temp-1);
            }
            return bits;
        }


        public static void Main()
        {
            Start:
            Console.WriteLine("Enter the value you'd like to convert:");
            string inp = Console.ReadLine();
            double result = 0;
            Console.WriteLine("---------------------------------------------\n");
            if (!Double.TryParse(inp, out result))
                Console.WriteLine("Invalid input.");
            else
            {
                string f32 = FloatConvert32(result);
                string f64 = FloatConvert64(result);
                Console.WriteLine("Single Precision:");
                Console.WriteLine(f32);
                Console.WriteLine("");
                Console.WriteLine("Double Precision:");
                Console.WriteLine(f64);
                Console.WriteLine("");
                Console.WriteLine("Press q + (enter) to quit, or just enter to restart the program.");
            }
            var response = Console.ReadLine();
            if (response == "q")
                Environment.Exit(0);
            else
                goto Start;
        }
    }
}
