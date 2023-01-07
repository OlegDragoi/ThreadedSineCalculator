namespace ThreadedSineCalculator
{
    public static class Calculator
    {
        // Decided to calculate sine through cosine, because cos seems to be a bit more efficient
        // sin(x) = cos(pi/2 - x)
        public static decimal Cos(decimal x)
        {
            decimal cos = 0;
            decimal sqrX = x * x;

            // cos(x) as a continued fraction https://en.wikipedia.org/wiki/Sine_and_cosine#Continued_fraction_definitions
            /*
             *           1      x^2         1*2*(x^2)          (2*(k-1))*(2*(k-1)+1)*(x^2)      (2*k)*(2*k+1)*(x^2)
             * cos(x) = ——— ————————————— ————————————— ... ————————————————————————————————— ———————————————————————, (k>0)
             *           1+  1*2 - x^2 +   3*4 - x^2 +       (2*(k-1)+2)*(2*(k-1)+3) - x^2 +   (2*k+2)*(2*k+3) - x^2
             */

            int nOfIterations = 100;

            for (int i = nOfIterations * 2 - 1; i > 0; i -= 2)
            {
                int ci = i;
                cos = ((ci) * ++ci * sqrX) /              //             (i * (i+1) * x^2) /
                       ((++ci) * (++ci) - sqrX + cos);    // ((i+2) * (i+3) - x^2 + previousIteration)
            }

            cos = sqrX /
                  (2 - sqrX + cos);
            cos = 1 /
                  (1 + cos);

            return cos;
        }

        public static decimal Sin(decimal x)
        {
            return Cos(PI / 2 - x);
        }

        public static decimal PI { get { return 3.1415926535897932384626433832m; } }
    }
}
