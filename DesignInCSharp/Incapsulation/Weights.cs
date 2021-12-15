using System;

namespace Incapsulation.Weights
{
    class Indexer
    {
        private static double[] array;
        private static readonly bool Init = false;
        public int Length { get; }
        public int Start { get; }

        public double this[int index]
        {
            get
            {
                if (index < 0 || index > Length - 1)
                    throw new IndexOutOfRangeException();

                return array[Start + index];
            }
            set
            {
                if (index < 0 || index > Length - 1)
                    throw new IndexOutOfRangeException();

                array[Start + index] = value;
            }
        }

        public Indexer(double[] ar, int start, int length)
        {
            if (start < 0 || length < 0 || length > ar.Length || length > ar.Length - start)
                throw new ArgumentException();

            Length = length;
            Start = start;
            if (!Init) array = ar;
        }
    }
}