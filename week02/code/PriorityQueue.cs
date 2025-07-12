using System;
using System.Collections.Generic;

public class PriorityQueue
{
    private List<PriorityItem> _queue = new();

    /// <summary>
    /// Add a new value to the queue with an associated priority. The
    /// node is always added to the back of the queue regardless of
    /// the priority.
    /// </summary>
    /// <param name="value">The value</param>
    /// <param name="priority">The priority</param>
    public void Enqueue(string value, int priority)
    {
        var newNode = new PriorityItem(value, priority);
        _queue.Add(newNode);
    }

    /// <summary>
    /// Remove the item with the highest priority and return its value.
    /// If there are more than one item with the highest priority,
    /// then the item closest to the front of the queue will be removed and its value returned.
    /// Throws InvalidOperationException if the queue is empty.
    /// </summary>
    public string Dequeue()
    {
        // Requirement: "If the queue is empty, then an error exception shall be thrown."
        if (_queue.Count == 0) // Verify the queue is not empty
        {
            throw new InvalidOperationException("The queue is empty.");
        }

        // Find the index of the item with the highest priority to remove
        // Initialize with the first item's priority and index
        // This implicitly makes the first item the "highest" until a truly higher one is found.
        int highPriorityIndex = 0;
        int highestPriority = _queue[0].Priority;

        // Iterate through the rest of the list to find the item with the highest priority.
        // We start from index 1 because index 0 is our initial candidate.
        // The loop condition `index < _queue.Count` ensures all elements are checked.
        for (int index = 1; index < _queue.Count; index++)
        {
            // Requirement: "The Dequeue function shall remove the item with the highest priority and return its value."
            // Requirement: "If there are more than one item with the highest priority,
            // then the item closest to the front of the queue will be removed and its value returned."
            //
            // If the current item's priority is GREATER THAN the current highestPriority found so far,
            // then it becomes the new highest. This ensures we always find the absolute highest.
            if (_queue[index].Priority > highestPriority)
            {
                highestPriority = _queue[index].Priority;
                highPriorityIndex = index;
            }
            // If the current item's priority is EQUAL to the current highestPriority,
            // we DO NOT update highPriorityIndex. This implicitly ensures FIFO tie-breaking,
            // as the item found first (with a lower 'highPriorityIndex') will be kept.
        }

        // Remove the item with the highest priority from the list
        // and store its value before removal.
        var valueToReturn = _queue[highPriorityIndex].Value;
        _queue.RemoveAt(highPriorityIndex); // This is the crucial step to actually remove it

        return valueToReturn;
    }

    public override string ToString()
    {
        return $"[{string.Join(", ", _queue)}]";
    }

    // You also need the Length property if it's not already defined for PriorityQueue (it is in your example).
    // public int Length => _queue.Count;
}

internal class PriorityItem
{
    internal string Value { get; set; } // Property changed from Data to Value based on your provided code
    internal int Priority { get; set; }

    internal PriorityItem(string value, int priority)
    {
        Value = value;
        Priority = priority;
    }

    public override string ToString()
    {
        return $"{Value} (Pri:{Priority})";
    }
}