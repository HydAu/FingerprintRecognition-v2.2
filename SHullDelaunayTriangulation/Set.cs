using System;
using System.Collections.Generic;

/*
 S-hull is free software and may be obtained from www.s-hull.org. It may be freely copied, modified, and redistributed under the following conditions which might loosely be termed a contribtors beerware license:

1. All copyright notices must remain intact in all files.

2. A copy of this text file must be distributed along with any copies of S-hull that you redistribute; this includes copies that you have modified, or copies of programs or other software products that include S-hull where distributed as source.

3. If you modify S-hull, you must include a notice giving the name of the person performing the modification, the date of modification, and the reason for such modification.

4. If you are distributing a binary or compiled version of s-hull it is not necessary to include any acknowledgement or reference to s-hull.

5. There is no warranty or other guarantee of fitness for S-hull, it is provided solely "as is". Bug reports or fixes may be sent to bugs@s-hull.org; the authors may or may not act on them as they desire.

6. By copying or compliing the code for S-hull you explicitly indemnify the copyright holder against any liability he may incur as a result of you copying the code.

7. If you meet any of the contributors to the code you used from s-hull.org in a pub or a bar, and you think the source code they contributed to is worth it, you can buy them a beer. If your principles run against beer a bacon-double-cheeseburger would do just as nicely or you could email david@s-hull.org and arrange to make a donation of 10 of your local currancy units to support s-hull.org.   
  
 */

namespace DelaunayTriangulator
{
    class Set<T> : IEnumerable<T>
    {
        SortedList<T, int> list;

        public Set()
        {
            list = new SortedList<T, int>();
        }

        public void Add(T k)
        {
            if (!list.ContainsKey(k))
                list.Add(k, 0);
        }

        public int Count
        {
            get { return list.Count; }
        }

        public void DeepCopy(Set<T> other)
        {
            list.Clear();
            foreach(T k in other.list.Keys)
                Add(k);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return list.Keys.GetEnumerator();
        }

        public void Clear()
        {
            list.Clear();
        }


        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
