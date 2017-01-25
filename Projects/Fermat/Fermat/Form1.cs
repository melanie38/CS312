using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Numerics;

namespace Fermat
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
		 /*
		  * Function that will call the Fermat test after click of the button "solve".
		  * The time complexity of this algorithm is O(kval*log^3(n)).
		  * Constraints: 1 <= k < (n - 1) since the random numbers generated must be greater than 2
		  * and smaller than n (n - 2 possibilities).
		  */
        private void button1_Click(object sender, EventArgs e)
        {
            int kval = Convert.ToInt32(k.Text);
            int n = Convert.ToInt32(input.Text);

            if(kval >= 1 && kval < (n - 1)) {
                if (primality(n, kval))
                {
                    double p = calculateProb(kval);
                    output.Text = "yes with probability " + p.ToString();
                }
                else
                {
                    output.Text = "no";
                }
            }
            else
            {
                output.Text = "k value out of range";
            }
        }

		/*
		 * Function that will perform the Fermat's test with a time complexity of O(kval).
		 * When random numbers are generated, they are compared to the set of numbers already
		 * tested to make sure no number is used twice.
		 * @param n: the integer to test for primality
		 * @param kval: the number of random integers to test
		 * return true if the test passed
		 *        false otherwise
		 */
        private bool primality(int n, int kval)
        {
            HashSet<int> tested = new HashSet<int>();
            Random random = new Random();
            int randomNumber = random.Next(2, n);
            while (tested.Count < kval)
            {
                if (!tested.Contains(randomNumber))
                {
                    if (modexp(randomNumber, n - 1, n) != 1) { return false; }
                    tested.Add(randomNumber);
                }
                randomNumber = random.Next(2, n - 1);
            }
            return true;
        }

		/*
		 * This function calculates x^y mod n with a complexity time of O(n^3)
		 * @param x: random number generated in primality()
		 * @param y: the exponent of x
		 * @param n: the integer to test for primality
		 * return x^y mod n
		 */
        private BigInteger modexp(int x, double y, int n)
        {
            if(y == 0) { return 1; }
            BigInteger z = modexp(x, Math.Floor(y/2), n);
            if(y % 2 == 0)
            {
                BigInteger temp = (z * z) % n;
                return temp;
            }
            else
            {
                BigInteger temp = (x * (z * z)) % n;
                return temp;
            }
        }

		/*
		 * This function calculates the probability that p is an actual prime number and not
		 * a composite with a constant time complexity.
		 * @param kval: the number of values tested
		 * return 1 - p the probability of n being a prime number
		 */
        private double calculateProb(int kval)
        {
            double p = 1 / (Math.Pow(2, kval));
            return 1 - p;
        }
        private bool isCarmicheal()
        {
            return false;
        }
    }
}
