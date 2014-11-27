using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrawlerNamespace
{
    public class QueueHashSet : Queue<Uri>
    {

        // Use a queue because we have to, and a mirror hashSet for constant time look up.
        public Queue<Uri> queue = new Queue<Uri>();
        public HashSet<Uri> hashSet = new HashSet<Uri>();
        public Uri currentUri;

        public QueueHashSet(List<string> seedUrls) 
        {
            foreach (String s in seedUrls)
            {
                // If this fails it won't make an Uri, and thus skip it.
                if (Uri.TryCreate(s, UriKind.RelativeOrAbsolute, out currentUri))
                {
                    queue.Enqueue(currentUri);
                    hashSet.Add(currentUri);
                }
            }
        }

        public void Enqueue(Uri uri) 
        {
            queue.Enqueue(uri);
            hashSet.Add(uri);
        }

        public Uri Dequeue() 
        {
            Uri item = queue.Dequeue();
            hashSet.Remove(item);
            return item;
        }
    }
}
