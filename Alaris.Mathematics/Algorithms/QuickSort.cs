using System;
using System.Collections.Generic;

namespace Alaris.Mathematics.Algorithms
{
    /// <summary>
    /// Provides a generic implementation of the quick sort algorithm
    /// </summary>
    /// <typeparam name="T">Type of the array which is to be processed.</typeparam>
    public class QuickSort<T> where T : IComparable
    {
        private readonly T[] _input;

        public QuickSort(IList<T> values)
        {
            _input = new T[values.Count];

            for (var i = 0; i < values.Count; i++)
            {
                _input[i] = values[i];
            }

        }

        public T[] Output
        {
            get
            {
                return _input;
            }
        }

        public void Sort()
        {
            Sorting(0, _input.Length - 1);
        }

        private int GetPivotPoint(int begPoint, int endPoint)
        {
            var pivot = begPoint;
            var m = begPoint + 1;
            var n = endPoint;

            while ((m < endPoint) &&
                   (_input[pivot].CompareTo(_input[m]) >= 0))
            {
                m++;
            }

            while ((n > begPoint) &&
                   (_input[pivot].CompareTo(_input[n]) <= 0))
            {
                n--;
            }
            while (m < n)
            {
                var temp = _input[m];
                _input[m] = _input[n];
                _input[n] = temp;

                while ((m < endPoint) &&
                       (_input[pivot].CompareTo(_input[m]) >= 0))
                {
                    m++;
                }

                while ((n > begPoint) &&
                       (_input[pivot].CompareTo(_input[n]) <= 0))
                {
                    n--;
                }

            }
            if (pivot != n)
            {
                var temp2 = _input[n];
                _input[n] = _input[pivot];
                _input[pivot] = temp2;

            }
            return n;

        }

        private void Sorting(int beg, int end)
        {
            if (end == beg)
            {
                return;
            }

            var pivot = GetPivotPoint(beg, end);

            if (pivot > beg)
                Sorting(beg, pivot - 1);
            if (pivot < end)
                Sorting(pivot + 1, end);
        }
    }
}