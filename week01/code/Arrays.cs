using System;
using System.Collections.Generic;
using System.Diagnostics; // Potentially needed if you use Debug.WriteLine for debugging

public static class Arrays
{
    /// <summary>
    /// This function will produce an array of size 'length' starting with 'number' followed by multiples of 'number'. For
    /// example, MultiplesOf(7, 5) will result in: {7, 14, 21, 28, 35}. Assume that length is a positive
    /// integer greater than 0.
    /// </summary>
    /// <returns>array of doubles that are the multiples of the supplied number</returns>
    public static double[] MultiplesOf(double number, int length)
    {
        // TODO Problem 1 Start
        // Remember: Using comments in your program, write down your process for solving this problem
        // step by step before you write the code. The plan should be clear enough that it could
        // be implemented by another person.

        // Plan:
        // 1. Create a new double array with the given 'length'.
        // 2. Loop from index 0 up to 'length - 1'.
        // 3. In each iteration, calculate the multiple by multiplying 'number' with (current_index + 1).
        // 4. Assign the calculated multiple to the corresponding position in the new array.
        // 5. Return the populated array.

        double[] resultArray = new double[length];

        for (int i = 0; i < length; i++)
        {
            resultArray[i] = number * (i + 1);
        }

        return resultArray;
    }

    /// <summary>
    /// Rotate the 'data' to the right by the 'amount'. For example, if the data is
    /// List<int>{1, 2, 3, 4, 5, 6, 7, 8, 9} and an amount is 3 then the list after the function runs should be
    /// List<int>{7, 8, 9, 1, 2, 3, 4, 5, 6}. The value of amount will be in the range of 1 to data.Count, inclusive.
    ///
    /// Because a list is dynamic, this function will modify the existing data list rather than returning a new list.
    /// </summary>
    public static void RotateListRight(List<int> data, int amount)
    {
        // TODO Problem 2 Start
        // Remember: Using comments in your program, write down your process for solving this problem
        // step by step before you write the code. The plan should be clear enough that it could
        // be implemented by another person.

        // Plan:
        // 1. Handle edge case: If the list is empty or has only one element, no rotation is needed.
        // 2. Calculate the actual number of elements to move from the end to the beginning.
        //    This is 'amount'.
        // 3. Determine the starting index for the elements that need to be moved: data.Count - amount.
        // 4. Use GetRange to extract these 'amount' elements from the end of the list into a temporary list (e.g., 'elementsToMove').
        // 5. Use RemoveRange to remove these 'amount' elements from the end of the original 'data' list.
        // 6. Use InsertRange to insert the 'elementsToMove' at the beginning (index 0) of the 'data' list.

        if (data == null || data.Count <= 1)
        {
            return; // No rotation needed for empty or single-element lists
        }

        // The problem states amount is in range [1, data.Count], so no need for modulo operation here
        // If amount could be > data.Count, we'd use: amount = amount % data.Count;

        // Step 3 & 4: Determine the starting index for the right part and extract it
        int startIndex = data.Count - amount;
        List<int> elementsToMove = data.GetRange(startIndex, amount);

        // Step 5: Remove the extracted elements from the original list
        data.RemoveRange(startIndex, amount);

        // Step 6: Insert the extracted elements at the beginning of the original list
        data.InsertRange(0, elementsToMove);
    }
}