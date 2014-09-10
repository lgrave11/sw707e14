using System;
using System.Collections.Generic;

namespace MiniProject1
{
    /// <summary>
    /// Description of Mercator
    /// </summary>
    public sealed class Mercator
    {
        #region singleton part
        private static Mercator instance = new Mercator();

        public static Mercator Instance
        {
            get
            {
                return instance;
            }
        }

        private Mercator()
        {
            for (int i = 0; i < B; i++)
            {
                backQueues.Add(new Queue<Uri>());
            }
        }
        #endregion

        public const int B = 10;
        private Queue<Uri> frontQueue = new Queue<Uri>();
        private List<Queue<Uri>> backQueues = new List<Queue<Uri>>();
        Dictionary<string, int> backQueueLookupTable = new Dictionary<string, int>();
        Dictionary<string, DateTime> lastTimevisited= new Dictionary<string,DateTime>();

        public void Enqueue(Uri uri)
        {
            frontQueue.Enqueue(uri);
        }

        public void registerVisit(Uri uri)
        {
            lastTimevisited[uri.Host] = DateTime.Now;
        }

        public Uri Dequeue()
        {
            bool predicate = true;
            List<int> emptyBackQueues = new List<int>();
            //first time only
            while (frontQueue.Count > 0 && backQueues.Exists(x => x.Count == 0))
            {
                for (int i = 0; i < B && frontQueue.Count > 0; i++)
                {
                    if (backQueues[i].Count == 0)
                    {
                        Uri uri = frontQueue.Dequeue();
                        if (!backQueueLookupTable.ContainsKey(uri.Host))
                        {
                            backQueueLookupTable[uri.Host] = i;
                            backQueues[i].Enqueue(uri);
                            break;
                        }
                        else
                        {
                            if (!backQueues[backQueueLookupTable[uri.Host]].Contains(uri))
                                backQueues[backQueueLookupTable[uri.Host]].Enqueue(uri);
                        }
                    }
                }
            }

            //get a uri from a backqueue
            int index;
            while (true) { 
                Random r = new Random();
                index = r.Next(0, B);
                if (backQueues[index].Count == 0)
                {
                    
                }
                else if (!lastTimevisited.ContainsKey(backQueues[index].Peek().Host))
                    break;
                else if ((DateTime.Now - lastTimevisited[backQueues[index].Peek().Host]).TotalSeconds >= 2)
                {
                    break;
                }
            }
            Uri returnUri = backQueues[index].Dequeue();

            while (backQueues[index].Count == 0 && frontQueue.Count > 0)
            {
                Uri uri = frontQueue.Dequeue();
                if (backQueueLookupTable.ContainsKey(uri.Host))
                {
                    backQueues[backQueueLookupTable[uri.Host]].Enqueue(uri);
                }
                else
                {
                    backQueues[index].Enqueue(uri);
                }
            }

            return returnUri;
        }
    }
}
