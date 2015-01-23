//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="Eusebio Rufian-Zilbermann">
//   Copyright (c) Eusebio Rufian-Zilbermann
// </copyright>
//-----------------------------------------------------------------------
namespace RotatedDivisor
{
   /// <summary>
   /// Consider the number 142857. We can right-rotate this number by moving the last digit (7) to the front of it, 
   /// giving us 714285. It can be verified that 714285=5×142857.
   /// This demonstrates an unusual property of 142857: it is a divisor of its right-rotation.
   /// </summary>
   public static class Program
   {
      /// <summary>
      /// Find the last 5 digits of the sum of all integers between 10^1 and 10^100, that are divisors of their right rotation.
      /// </summary>
      /// <remarks>
      /// The naive solution is to loop over the number range, verify which numbers meet the condition and calculate their
      /// modular sum, however this requires operating on 100-digit numbers. In addition to writing this in C# I want to be
      /// able to translate this program into K language so using <see cref="System.Numerics.BigInteger"/> is not an option
      /// <p></p>
      /// A "smarter search" can be done as follows:
      /// <ul>
      /// <p></p>
      /// <li>The rotated number is a multiple of the original, therefore there is an integer multiplier that applied to the
      /// original digits will produce the rotated number
      /// </li>
      /// <p></p>
      /// <li>This integer multiplier is between 1 and 9, otherwise the result would have more digits than the original
      /// </li>
      /// <p></p>
      /// <li>For each least significant digit and multiplier there can be only one pattern that meets the criteria. We can 
      /// generate a number that meets the condition based only on the Least Significant Digit and a given multiplier
      /// For example, for a multiplier of 5 and a least significant digit of 7, we can calculate subsequent digits by simple
      /// multiplication with carry:
      /// <pre>
      /// 7 X 5 = 35, next digit is 5 and carry is 3
      /// 5 x 5 = 25, plus carry 25 + 3 = 28, next digit is 8 and carry is 2
      /// 8 x 5 = 40, plus carry 40 + 2 = 42, next digit is 2, carry is 4
      /// 2 x 5 = 10, plus carry 10 + 4 = 14, next digit is 4, carry is 1
      /// 4 x 5 = 20, plus carry 20 + 1 = 21, next digit is 1, carry is 2
      /// 1 x 5 = 5, plus carry 5 + 1 = 7, next digit would be 7
      /// </pre>
      /// At this point the sequence repeats again because we have exactly the same condition as the start of the sequence
      /// 7 x 5 with a carry of 0
      /// <p></p>
      /// The pattern can be repeated periodically as many times as desired, if 142857 is a divisor of its right rotation,
      /// then 142587142587 is also a divisor of its right rotation.
      /// </li>
      /// <p></p>
      /// <li>We can loop over the 81 possible combinations of multiplier and Least Significant Digit and calculate the
      /// pattern that meets the condition, up to the point where it will periodically repeat. 
      /// For each pattern we can then calculate how many times can it be repeated for a given number of digits. E.g., 
      /// for 142857 and numbers up to 99 digits (n &lt; 10^100) we will have 99/6 = 16 patterns.
      /// We can then calculate the modularized sum by taking the lower 5 digits, multiply by 16 and taking the lower 5 digits
      /// again
      /// </li>
      /// <p></p>
      /// <li>Two special cases to consider are
      /// <ol>
      /// <p></p>
      /// <li>Patterns that start with a 0. E.g., for multiplier = 2 and least significant digit = 1 we get 052631578947368421
      /// These patterns need to be discarded because the actual result would be 52631578947368421 and its rotation is
      /// 15263157894736842 which is not a multiple of 52631578947368421.
      /// </li>
      /// <p></p>
      /// <li>Patterns for multiplier 1. When all the digits in a number are identical, rotations do not change the number
      /// and the result is a divisor of itself with multiplier = 1. These are the only patterns shorter than 5 digits
      /// and we cannot apply the simplification of dividing the total number of digits by the digits in the pattern. 
      /// Additionally, the problem statement explicitly discards the single digits 1...9 from the solution.
      /// We can skip multiplier 1 and add them separately (alternatively, it would also be possible to calculate their
      /// contribution and then subtract the contribution of 1...9 from the result)
      /// </li>
      /// </ol>
      /// </li>
      /// </ul>
      /// </remarks>
      public static void Main()
      {         
         int modularizedSum = 0;
         for (int multiplier = 2; 10 > multiplier; multiplier++)
         {
            for (int leastSignificantDigit = 1; 10 > leastSignificantDigit; leastSignificantDigit++)
            {
               int[] digits = new int[] { leastSignificantDigit, 0, 0, 0, 0 };
               int nextDigit = 0;
               int carry = 0;
               int digitIndex = 1;
               int nextDigitCandidate = multiplier * digits[0];
               while (nextDigitCandidate != leastSignificantDigit)
               {
                  nextDigit = nextDigitCandidate % 10;
                  if (5 > digitIndex)
                  {
                     digits[digitIndex] = nextDigit;
                  }

                  carry = nextDigitCandidate / 10;
                  nextDigitCandidate = (multiplier * nextDigit) + carry;
                  digitIndex++;
               }

               if (0 < nextDigit)
               {
                  modularizedSum += DigitsToInt(digits) * (99 / digitIndex);
               }
            }
         }

         for (int i = 1; 10 > i; ++i)
         {
            modularizedSum += 1233 * i; // 11 + 111 + 1111 = 1233
            modularizedSum += 11111 * i * 96;
         }

         modularizedSum %= 100000;

         System.Console.WriteLine(modularizedSum);
      }

      /// <summary>
      /// Given an array of base-10 digits, calculate the corresponding integer.
      /// </summary>
      /// <param name="a">An array of base-10 digits, starting from the Least Significant Digit</param>
      /// <returns>The integer that results from concatenating the digits</returns>
      /// <exception cref="System.OverflowException">Thrown if the array does not fit into an integer.</exception>
      /// <remarks>
      /// This can be done either by direct calculation with powers of 10 or converting into a string and
      /// parsing the result. This implementation uses powers of 10.
      /// </remarks>
      private static int DigitsToInt(int[] a)
      {
         int result = 0;
         int pow10 = 1;
         for (int i = 0; a.Length > i; i++)
         {
            result = checked(result + a[i] * pow10);
            pow10 *= 10;
         }

         return result;
      }
   }
}
