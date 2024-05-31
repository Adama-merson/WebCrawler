using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCrawler
{
    class PriorityComparer : IComparer<(int priority, string url, int depth)>
    {
        public int Compare((int priority, string url, int depth) x, (int priority, string url, int depth) y)
        {
            int result = x.priority.CompareTo(y.priority); // Compare by priority
            if (result == 0) // If priorities are equal, compare by URL
            {
                result = x.url.CompareTo(y.url);
            }
            return result;
        }
    }
}
