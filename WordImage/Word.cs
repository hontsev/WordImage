using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordImage
{
    class Word : IComparable
    {
        public string word;
        public int num;

        public Word(string w, int n)
        {
            word = w;
            num = n;
        }


        public int CompareTo(object obj)
        {
            int result;
            try
            {
                Word info = obj as Word;
                if (this.num > info.num)
                {
                    result = -1;
                }
                else
                    result = 1;
                return result;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
    }
}
