using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniProject2
{
    class Classifier
    {
        double posProb = 0.0;
        double negProb = 0.0;

        Dictionary<string, WordProbWrapper> wordDependentProb = new Dictionary<string, WordProbWrapper>();
        public Classifier(List<Review> reviews)
        {
            LaplaceSmooth(reviews);
        }

        private double countPos = 0;
        private double countNeg = 0;
        private void LaplaceSmooth(List<Review> reviews)
        {
            

            foreach(Review r in reviews)
            {
                if(r.score > 3.0)
                {
                    countPos++;
                }
                else if(r.score < 3.0)
                {
                    countNeg++;
                }
            }
            posProb = (countPos + 1) / (countPos + countNeg + 2.0);
            negProb = (countNeg + 1) / (countPos + countNeg + 2.0);
        }

        private void wordDependentProbInit(List<Review> reviews)
        {
            foreach(Review r in reviews)
            {
                foreach(string s in r.tokenStream)
                {
                    wordDependentProb[s] = null;
                }
            }

            foreach(string s in wordDependentProb.Keys)
            {
                double countPos = 0.0, countNeg = 0.0;

                foreach(Review r in reviews)
                {
                    if(r.tokenStream.Contains(s))
                    {
                        if (r.score > 3.0)
                            countPos++;
                        else if (r.score < 3.0)
                            countNeg++;
                    }
                }
                double probpos = (countPos + 1)/()
                wordDependentProb[s] = new WordProbWrapper();
            }
        }
    }
}
