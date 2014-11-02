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
        Pair<double> emptyScore;


        Dictionary<string, WordProbWrapper> wordDependentProb = new Dictionary<string, WordProbWrapper>();
        public Classifier(List<Review> reviews)
        {
            Console.WriteLine("Laplace");
            LaplaceSmooth(reviews);
            Console.WriteLine("Dependent");
            wordDependentProbInit(reviews);
            Console.WriteLine("Empty score");
            emptyScore = calcEmptyScore();
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

            Console.WriteLine("beginning to count dependent");
            Dictionary<string, Pair<int>> dictionayCount = new Dictionary<string, Pair<int>>();
            foreach(Review r in reviews)
            {
                foreach(string s in r.tokenStream)
                {
                    if(!dictionayCount.ContainsKey(s))
                    {
                        dictionayCount[s] = new Pair<int>(0, 0);
                    }
                    if(r.score > 3.0)
                    {
                        dictionayCount[s].left++;
                    }
                    else if(r.score < 3.0)
                    {
                        dictionayCount[s].right++;
                    }
                }
            }
            Console.WriteLine("beginning to calc dependent");
            foreach(var x in dictionayCount)
            {
                double probpos = (x.Value.left + 1) / (this.countPos + wordDependentProb.Keys.Count);
                double probNeg = (x.Value.right + 1) / (this.countNeg + wordDependentProb.Keys.Count);

                wordDependentProb[x.Key] = new WordProbWrapper(probpos, probNeg);
            }

        }

        public string WantsToBuy(Review review)
        {
            double scorePos = 0.0, scoreNeg = 0.0;
            foreach(string token in review.tokenStream)
            {
                if(wordDependentProb.ContainsKey(token))
                {
                    scorePos += Math.Log((wordDependentProb[token].dependentPos) / (wordDependentProb[token].notdependentPos));
                    scoreNeg += Math.Log((wordDependentProb[token].dependentNeg) / (wordDependentProb[token].notdependentNeg));
                }
            }
            scorePos += emptyScore.left; // these have already gotten the logarithm applied
            scoreNeg += emptyScore.right;

            if(scorePos >= scoreNeg)
            {
                return "yes";
            }
            else 
            {
                return "no";
            }

        }

        // Left is the empty score for positive category, Right is empty score for negative category
        private Pair<double> calcEmptyScore()
        {
            double sumPos = 0.0, sumNeg = 0.0;

            foreach(var x in wordDependentProb.Values)
            {
                sumPos += Math.Log(x.notdependentPos);
                sumNeg += Math.Log(x.notdependentNeg);
            }

            sumPos += Math.Log(posProb);
            sumNeg += Math.Log(negProb);

            return new Pair<double>(sumPos, sumNeg);
        }

    }
}
