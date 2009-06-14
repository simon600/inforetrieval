using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WikipediaSearchEngine
{
    public class Pair : IComparable<Pair>
    {
        public Pair(int x, int y)
        {
            first = x;
            second = y;
        }

        public int this[int index]
        {
            get
            {
                if (index == 0)
                    return first;
                else if (index == 1)
                    return second;

                throw new IndexOutOfRangeException();
            }
        }

        public int First
        {
            get { return first; }
        }
        public int Second
        {
            get { return second; }
        }
        

        #region IComparable<Pair> Members

        public int CompareTo(Pair other)
        {
            if (this.second < other.second)
                return -1;
            else if (this.second == other.second)
                return 0;
            else return 1;
        }

        #endregion

        private int first;
        private int second;
    }
}
