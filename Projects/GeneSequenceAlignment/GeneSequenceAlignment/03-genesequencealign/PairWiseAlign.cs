using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace GeneticsLab
{
    class PairWiseAlign
    {
        int MaxCharactersToAlign;

        public PairWiseAlign()
        {
            // Default is to align only 5000 characters in each sequence.
            this.MaxCharactersToAlign = 5000;
        }

        public PairWiseAlign(int len)
        {
            // Alternatively, we can use an different length; typically used with the banded option checked.
            this.MaxCharactersToAlign = len;
        }

        /// <summary>
        /// this is the function you implement.
        /// </summary>
        /// <param name="sequenceA">the first sequence</param>
        /// <param name="sequenceB">the second sequence, may have length not equal to the length of the first seq.</param>
        /// <param name="banded">true if alignment should be band limited.</param>
        /// <returns>the alignment score and the alignment (in a Result object) for sequenceA and sequenceB.  The calling function places the result in the dispay appropriately.
        /// 
        public ResultTable.Result Align_And_Extract(GeneSequence sequenceA, GeneSequence sequenceB, bool banded)
        {
            ResultTable.Result result = new ResultTable.Result();
            int score;                                                       // place your computed alignment score here
            string[] alignment = new string[2];                              // place your two computed alignments here

			String seqA = sequenceA.Sequence.Substring(0, Math.Min(sequenceA.Sequence.Length, MaxCharactersToAlign));
			String seqB = sequenceB.Sequence.Substring(0, Math.Min(sequenceB.Sequence.Length, MaxCharactersToAlign));
			StringBuilder alignmentA = new StringBuilder();
			StringBuilder alignmentB = new StringBuilder();

			var row = seqA.Length + 1;
			var col = seqB.Length + 1;

			int[,] costmatrix = new int[row, col];
			int[,] backtrace = new int[row, col];

			var SUBST = 1;
			var INDEL = 5;
			var MATCH = -3;

            // ********* these are placeholder assignments that you'll replace with your code  *******
            //score = 0;                                                
            alignment[0] = "";
            alignment[1] = "";

			// initializationn
			costmatrix[0, 0] = 0;
			backtrace[0, 0] = 0;

			for (var i = 1; i < row; i++)
			{
				costmatrix[i, 0] = costmatrix[i - 1, 0] + INDEL; 
			}

			for (var i = 1; i < col; i++)
			{
				costmatrix[0, i] = costmatrix[0, i - 1] + INDEL;
			}

			// filling the matrixx
			for (var i = 1; i < row; i++)
			{
				for (var j = 1; j < col; j++)
				{
					var cost = 0;

					if (seqA[i - 1] == seqB[j - 1])
					{
						cost = MATCH;
					}
					else
					{
						cost = SUBST;
					}

					var choice1 = costmatrix[i - 1, j - 1] + cost;	// If characters are aligned
					var choice2 = costmatrix[i - 1, j] + INDEL;		// Gap in seqB
					var choice3 = costmatrix[i, j - 1] + INDEL;     // Gap in seqA

					var list = new[] { choice1, choice2, choice3 };

					costmatrix[i, j] = list.Min();

					if (list.Min() == choice1)
					{
						backtrace[i, j] = 0;
					}
					else if (list.Min() == choice2)
					{
						backtrace[i, j] = 1;
					}
					else
					{
						backtrace[i, j] = -1;
					}
				}
			}


			// traceback
			var a = seqA.Length;
			var b = seqB.Length;

			while (a > 0 || b > 0)
			{
				if (backtrace[a, b] == 0)
				{
					alignmentA.Append(seqA[a]);
					alignmentB.Append(seqB[b]);
					a--;
					b--;
				}
				else if (backtrace[a, b] == 1)
				{
					alignmentA.Append(seqA[a]);
					alignmentB.Append('-');
					a--;
				}
				else
				{
					alignmentA.Append('-');
					alignmentB.Append(seqB[b]);
					b--;
				}
			}

			// ***************************************************************************************

			score = costmatrix[row - 1, col - 1];
			alignment[0] = alignmentA.ToString().Substring(0, 100);
			alignment[1] = alignmentB.ToString().Substring(0, 100);

            result.Update(score,alignment[0],alignment[1]);                  // bundling your results into the right object type 
            return(result);
        }
    }
}
