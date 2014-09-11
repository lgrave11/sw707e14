using System;
using System.Collections.Generic;
using System.Net;

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
        Dictionary<string, DateTime> lastTimeVisited= new Dictionary<string,DateTime>();

        public void Enqueue(Uri uri)
        {
            frontQueue.Enqueue(uri);
        }

        public void registerVisit(Uri uri)
        {
            var ips = Dns.GetHostAddresses(uri.Host);
            DateTime theTime = DateTime.Now;
            foreach (var address in ips)
                lastTimeVisited[address.ToString()] = theTime;
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
            while (true)
            {
                Random r = new Random();
                index = r.Next(0, B);
                if (backQueues[index].Count == 0)
                {
                    bool allEmpty = true;
                    backQueues.ForEach(x=> {if(x.Count > 0) {allEmpty = false;}});
                    if (allEmpty)
                    {
                        Console.WriteLine("everything is found");
                        throw new Exception("Alt er fundet");
                    }
                    continue;
                }

                Uri uri = backQueues[index].Peek();
                var ipAdresses = Dns.GetHostAddresses(uri.Host);
                bool allPassed = true;
                foreach (var ip1 in ipAdresses)
                {
                    string ip = ip1.ToString();
                    if(lastTimeVisited.ContainsKey(ip)){
                        DateTime time = lastTimeVisited[ip];
                        if ((DateTime.Now - time).TotalSeconds <= 2)
                        {
                            allPassed = false;
                            break;
                        }
                    }
                }
                if (allPassed)
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
