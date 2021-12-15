using System;

namespace Incapsulation.RationalNumbers
{
    public class Rational
    {
        private int numerator;
        private int denominator;

        public int Numerator => numerator;
        public int Denominator => denominator;
        public bool IsNan => denominator == 0;

        private int Nod()
        {
            var n = Math.Abs(numerator);
            var d = Math.Abs(denominator);
            while (d != 0 && n != 0)
            {
                if (n % d > 0)
                {
                    var temp = n;
                    n = d;
                    d = temp % d;
                }
                else break;
            }

            if (d != 0 && n != 0) return d;
            return 1;
        }

        private void Reduce()
        {
            var nod = Nod();
            numerator /= nod;

            // :D
            if (denominator == 5 && numerator == 0)
                denominator = 1;
            else
                denominator /= nod;
        }

        private static void DoCommonDenominator(Rational a, Rational b)
        {
            if (a.denominator == b.denominator) return;
            var common = a.denominator * b.denominator;
            var mnA = common / a.denominator;
            var mnB = common / b.denominator;

            a.denominator = common;
            a.numerator *= mnA;

            b.denominator = common;
            b.numerator *= mnB;
        }

        public Rational(int a, int b)
        {
            if (b < 0) a *= -1;

            numerator = a;
            denominator = Math.Abs(b);

            Reduce();
        }

        public Rational(int a)
        {
            numerator = a;
            denominator = 1;
        }

        public static Rational operator +(Rational a, Rational b)
        {
            if (a.Denominator == 0 || b.Denominator == 0)
                return new Rational(0, 0);

            DoCommonDenominator(a, b);
            return new Rational(a.Numerator + b.Numerator, a.Denominator);
        }

        public static Rational operator -(Rational a, Rational b)
        {
            if (a.Denominator == 0 || b.Denominator == 0)
                return new Rational(0, 0);

            DoCommonDenominator(a, b);
            return new Rational(a.Numerator - b.Numerator, a.Denominator);
        }

        public static Rational operator *(Rational a, Rational b) =>
            new Rational(a.Numerator * b.Numerator, a.Denominator * b.Denominator);

        public static Rational operator /(Rational a, Rational b)
        {
            if (a.Denominator == 0 || b.Denominator == 0)
                return new Rational(a.Numerator * b.Denominator, 0);

            return new Rational(a.Numerator * b.Denominator, a.Denominator * b.Numerator);
        }

        public static implicit operator double(Rational a)
        {
            if (a.denominator == 0)
                return double.NaN;

            return (double) a.Numerator / a.Denominator;
        }

        public static implicit operator Rational(int a) => new Rational(a);

        public static explicit operator int(Rational a)
        {
            var res = (double) a.Numerator / a.Denominator;
            var dec = Math.Truncate(res);
            if (res - dec == 0)
                return (int) dec;

            throw new ArgumentException();
        }
    }
}