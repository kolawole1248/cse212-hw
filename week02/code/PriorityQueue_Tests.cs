using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

// TODO Problem 2 - Write and run test cases and fix the code to match requirements.

[TestClass]
public class PriorityQueueTests
{
    [TestMethod]
    // Scenario: Attempt to Dequeue from an empty PriorityQueue.
    // Expected Result: An InvalidOperationException should be thrown with the message "The queue is empty.".
    // Defect(s) Found: None. The initial `Dequeue` method correctly checks if the queue is empty and throws the specified `InvalidOperationException` with the correct message. This test should pass even before any fixes to `PriorityQueue.cs`.
    public void TestPriorityQueue_1() // Renamed from TestPriorityQueue_1_EmptyQueueException for problem
    {
        var priorityQueue = new PriorityQueue();

        // This assertion verifies that the correct exception type is thrown.
        Assert.ThrowsException<InvalidOperationException>(() => priorityQueue.Dequeue());

        // This additional try-catch block verifies the exact message, if desired.
        try
        {
            priorityQueue.Dequeue();
        }
        catch (InvalidOperationException e)
        {
            Assert.AreEqual("The queue is empty.", e.Message);
        }
        catch (Exception)
        {
            Assert.Fail("Expected InvalidOperationException but a different exception was thrown.");
        }
    }

    [TestMethod]
    // Scenario: Enqueue three items with distinct priorities ("Low":1, "Medium":5, "High":10). Then, Dequeue them to verify the highest priority item is removed first, and subsequent dequeues get the next highest.
    // Expected Result: "High", then "Medium", then "Low". The queue should be empty at the end.
    // Defect(s) Found:
    // 1. **Incorrect Highest Priority Detection (Loop Range):** The original `Dequeue` loop `for (int index = 1; index < _queue.Count - 1; index++)` *skips the last element*. If the highest priority item is added last, it won't be found. For instance, if "High" (10) is the last item, this test would fail.
    // 2. **Item Not Removed:** The `Dequeue` method retrieves the item but *does not remove it* from the internal list. This means `priorityQueue.Length` would always remain the same, and subsequent `Dequeue` calls would keep returning the same "highest" item (or an incorrect one if the list is never modified). This would cause the test to fail on subsequent `Assert.AreEqual` checks and on the final `Assert.AreEqual(0, priorityQueue.Length)`.
    public void TestPriorityQueue_2() // Renamed from TestPriorityQueue_2_DequeueHighestPriorityBasic for problem
    {
        var priorityQueue = new PriorityQueue();
        priorityQueue.Enqueue("Low", 1);
        priorityQueue.Enqueue("Medium", 5);
        priorityQueue.Enqueue("High", 10); // This is the highest priority item

        Assert.AreEqual(3, priorityQueue.Length, "Queue length should be 3 after enqueues.");

        string dequeued = priorityQueue.Dequeue();
        Assert.AreEqual("High", dequeued, "First dequeue should return the highest priority item.");
        Assert.AreEqual(2, priorityQueue.Length, "Queue length should be 2 after first dequeue.");

        dequeued = priorityQueue.Dequeue();
        Assert.AreEqual("Medium", dequeued, "Second dequeue should return the next highest priority item.");
        Assert.AreEqual(1, priorityQueue.Length, "Queue length should be 1 after second dequeue.");

        dequeued = priorityQueue.Dequeue();
        Assert.AreEqual("Low", dequeued, "Third dequeue should return the lowest priority item.");
        Assert.AreEqual(0, priorityQueue.Length, "Queue should be empty after all items are dequeued.");
    }

    // Additional test cases for comprehensive coverage, as previously designed:
    [TestMethod]
    // Scenario: Enqueue multiple items with the same highest priority to specifically test the FIFO tie-breaking rule.
    //           Items: "First_High" (10), "Mid_Low" (5), "Second_High" (10), "Last_High" (10).
    // Expected Result: The dequeued order should be "First_High", then "Second_High", then "Last_High" (all priority 10), and finally "Mid_Low".
    // Defect(s) Found:
    // 1. **Incorrect FIFO Tie-Breaking:** The original `Dequeue` uses `if (_queue[index].Priority >= _queue[highPriorityIndex].Priority)`. This condition means that if a later item has *equal* priority to the current highest, it will overwrite the index, effectively choosing the *last* item with that priority, violating the "closest to the front" (FIFO) rule for ties.
    // 2. **Item Not Removed:** As in Test 2, items are not actually removed, which would lead to incorrect subsequent dequeues and length assertions.
    public void TestPriorityQueue_3_DequeueHighestPriorityTieBreakerFIFO()
    {
        var priorityQueue = new PriorityQueue();
        priorityQueue.Enqueue("First_High", 10);  // Highest priority, enqueued first
        priorityQueue.Enqueue("Mid_Low", 5);
        priorityQueue.Enqueue("Second_High", 10); // Highest priority, enqueued second
        priorityQueue.Enqueue("Last_High", 10);   // Highest priority, enqueued third

        Assert.AreEqual(4, priorityQueue.Length, "Queue length should be 4 initially.");

        string dequeued = priorityQueue.Dequeue();
        Assert.AreEqual("First_High", dequeued, "First dequeue should be the first enqueued item with highest priority.");
        Assert.AreEqual(3, priorityQueue.Length, "Length should decrement correctly.");

        dequeued = priorityQueue.Dequeue();
        Assert.AreEqual("Second_High", dequeued, "Second dequeue should be the second enqueued item with highest priority.");
        Assert.AreEqual(2, priorityQueue.Length, "Length should decrement correctly.");

        dequeued = priorityQueue.Dequeue();
        Assert.AreEqual("Last_High", dequeued, "Third dequeue should be the last enqueued item with highest priority.");
        Assert.AreEqual(1, priorityQueue.Length, "Length should decrement correctly.");

        dequeued = priorityQueue.Dequeue();
        Assert.AreEqual("Mid_Low", dequeued, "Final dequeue should be the remaining item.");
        Assert.AreEqual(0, priorityQueue.Length, "Queue should be empty.");
    }

    [TestMethod]
    // Scenario: A comprehensive test combining various priorities and testing multiple dequeues to ensure both highest priority selection and FIFO tie-breaking work correctly in sequence.
    //           Items: "A"(5), "B"(10), "C"(3), "D"(10), "E"(7).
    // Expected Result: Dequeue sequence: "B", then "D", then "E", then "A", then "C".
    // Defect(s) Found:
    // 1. This test will likely fail due to a combination of all previous defects: incorrect loop range in `Dequeue`, faulty FIFO tie-breaking logic, and the failure to actually remove items from the internal list. If not removed, the queue's state would be incorrect for subsequent dequeues.
    public void TestPriorityQueue_4_DequeueComplexScenario()
    {
        var priorityQueue = new PriorityQueue();
        priorityQueue.Enqueue("A", 5);
        priorityQueue.Enqueue("B", 10); // First item with priority 10
        priorityQueue.Enqueue("C", 3);
        priorityQueue.Enqueue("D", 10); // Second item with priority 10
        priorityQueue.Enqueue("E", 7);

        Assert.AreEqual(5, priorityQueue.Length, "Initial queue length should be 5.");

        Assert.AreEqual("B", priorityQueue.Dequeue(), "Dequeue 1 failed.");
        Assert.AreEqual(4, priorityQueue.Length, "Length after dequeue 1 is incorrect.");

        Assert.AreEqual("D", priorityQueue.Dequeue(), "Dequeue 2 failed.");
        Assert.AreEqual(3, priorityQueue.Length, "Length after dequeue 2 is incorrect.");

        Assert.AreEqual("E", priorityQueue.Dequeue(), "Dequeue 3 failed.");
        Assert.AreEqual(2, priorityQueue.Length, "Length after dequeue 3 is incorrect.");

        Assert.AreEqual("A", priorityQueue.Dequeue(), "Dequeue 4 failed.");
        Assert.AreEqual(1, priorityQueue.Length, "Length after dequeue 4 is incorrect.");

        Assert.AreEqual("C", priorityQueue.Dequeue(), "Dequeue 5 failed.");
        Assert.AreEqual(0, priorityQueue.Length, "Length after dequeue 5 is incorrect.");

        Assert.ThrowsException<InvalidOperationException>(() => priorityQueue.Dequeue(), "Should throw exception when empty.");
    }
}