DRR Numbers
============

This is another interview question that I was asked. This was a test to be done at home with a 24h deadline. Here is the problem:

Find the last 5 digits of the sum of all integers between 10^1 and 10^100, that are divisors of their right rotation.

For example, consider the number 142857. We can right-rotate this number by moving the last digit (7) to the front of it, giving us 714285. It can be verified that 714285 = 5 × 142857. This demonstrates an unusual property of 142857: it is a divisor of its right-rotation.

The test was actually to develop this using the K language (http://en.wikipedia.org/wiki/K_%28programming_language%29)
which I did, but I first developed it in C# in order to better understand the problem, and then re-wrote it with K.  
Because of this goal I consciously limited myself to the simpler aspects of C# (e.g., no .NET libraries, not even LINQ).

Note: I am not posting the final K solution because I don't want to give away too much in case the company re-uses the question.

The naive solution is to loop over the number range, verify which numbers meet the condition and calculate the
modular sum of the numbers that pass the filter, however this solution requires operating on 99-digit numbers.

A "smarter search" can be done by taking into account the following:

* The rotated number is a multiple of the original, therefore there is an integer multiplier that applied to the
  original digits will produce the rotated number

* This integer multiplier is between 1 and 9, otherwise the result would have more digits than the original

* For each least significant digit and multiplier there can be only one pattern that meets the criteria. We can 
  generate a number that meets the condition based only on the Least Significant Digit and a given multiplier.
  For example, for a multiplier of 5 and a least significant digit of 7, we can calculate subsequent digits by simple
  multiplication with carry:

  7 X 5 = 35, next digit is 5 and carry is 3  
  5 x 5 = 25, plus carry 25 + 3 = 28, next digit is 8 and carry is 2  
  8 x 5 = 40, plus carry 40 + 2 = 42, next digit is 2, carry is 4  
  2 x 5 = 10, plus carry 10 + 4 = 14, next digit is 4, carry is 1  
  4 x 5 = 20, plus carry 20 + 1 = 21, next digit is 1, carry is 2  
  1 x 5 = 5, plus carry 5 + 1 = 7, next digit would be 7  

  At this point the sequence repeats again because we have exactly the same condition as the start of the sequence
  7 x 5 with a carry of 0  
  The pattern can be repeated periodically as many times as desired, if 142857 is a divisor of its right rotation,
  then 142587142587 is also a divisor of its right rotation.

* We can evaluate the 81 possible combinations of multiplier and Least Significant Digit, and calculate the
  pattern that meets the condition up to the point where it will periodically repeat.  
  For each pattern we can then calculate how many times can it be repeated for a given number of digits. E.g., 
  for 142857 and numbers up to 99 digits (n &lt; 10^100) there are 99/6 = 16 numbers that contain the pattern.  
  We can then calculate the modularized sum by taking the lower 5 digits, multiplying by 16 and taking the lower 5 
  digits again

* Two special cases to consider are
  1. Patterns that start with a 0.  
   E.g., for multiplier = 2 and least significant digit = 1 we get 052631578947368421  
   These patterns need to be discarded because the actual result would be 52631578947368421 and its rotation is
   15263157894736842 which is not a multiple of 52631578947368421.

  2. Patterns for multiplier 1.  
   When all the digits in a number are identical, rotations do not change the number
   and the result is a divisor of itself with multiplier = 1.  
   These are the only patterns shorter than 5 digits
   and we cannot apply the simplification of dividing the total number of digits by the digits in the pattern.  
   Additionally, the problem statement explicitly discards the single digits 1...9 from the solution.  
   We can skip multiplier 1 and add them separately (alternatively, it would also be possible to calculate their
   contribution and then subtract the contribution of 1...9 from the result)
