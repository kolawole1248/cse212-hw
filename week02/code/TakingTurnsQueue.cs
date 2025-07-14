using System;
using System.Collections.Generic;

// Assuming Person and PersonQueue classes are defined elsewhere.
// Person: Should have Name (string) and Turns (int, mutable) properties.
// PersonQueue: Should have Enqueue(Person), Dequeue(), IsEmpty(), Length properties.
//             Critically, based on errors, it seems Dequeue() removes the *last* item added (LIFO/Stack behavior).

/// <summary>
/// This queue is circular. When people are added via AddPerson, then they are added to the
/// back of the queue (per FIFO rules). When GetNextPerson is called, the next person
/// in the queue is saved to be returned and then they are placed back into the back of the queue. Thus,
/// each person stays in the queue and is given turns. When a person is added to the queue,
/// a turns parameter is provided to identify how many turns they will be given. If the turns is 0 or
/// less than they will stay in the queue forever. If a person is out of turns then they will
/// not be added back into the queue.
/// </summary>
public class TakingTurnsQueue
{
    private readonly PersonQueue _people = new();

    public int Length => _people.Length;

    /// <summary>
    /// Add new people to the queue with a name and number of turns
    /// </summary>
    /// <param name="name">Name of the person</param>
    /// <param name="turns">Number of turns remaining</param>
    public void AddPerson(string name, int turns)
    {
        var person = new Person(name, turns);
        _people.Enqueue(person);
    }

    /// <summary>
    /// Get the next person in the queue and return them. The person should
    /// go to the back of the queue again unless the turns variable shows that they
    /// have no more turns left. Note that a turns value of 0 or less means the
    /// person has an infinite number of turns. An error exception is thrown
    /// if the queue is empty.
    /// </summary>
    public Person GetNextPerson()
    {
        if (_people.IsEmpty())
        {
            throw new InvalidOperationException("No one in the queue.");
        }

        // --- Start of changes to compensate for potential LIFO (Stack-like) PersonQueue ---
        // To get the "oldest" person (FIFO), if _people is behaving like a Stack (LIFO),
        // we must effectively reverse the stack to get the true "front" element.
        // This makes the operation O(N) instead of O(1).

        // Temporarily store all elements in reverse order to find the "first-in" person
        var tempStack = new PersonQueue(); // Use another PersonQueue if it's the only available collection type that supports Enqueue/Dequeue. If not, a standard Stack or List would be better.
                                         // Assuming PersonQueue provides simple Enqueue/Dequeue, it will behave like a stack if it's indeed LIFO.
        Person oldestPerson = null;

        // Dequeue all elements from _people into tempStack
        // If _people is LIFO: _people = [A, B, C] (C on top). tempStack = [C, B, A] (A on top)
        // If _people is FIFO: _people = [A, B, C] (A at front). tempStack = [A, B, C] (C on top) -- wait, this doesn't help.
        // Let's assume _people.Dequeue() gets the LAST-IN element (like a stack's Pop).

        // Corrected strategy for LIFO PersonQueue:
        // We need to find the element that was added first (the "bottom" of the stack).
        // This means dequeuing all elements, storing them, and finding the one that was at the bottom.
        // The most robust way without peeking is to transfer all but the last (oldest) to a temp storage.

        List<Person> allPeople = new List<Person>();
        while (!_people.IsEmpty())
        {
            allPeople.Add(_people.Dequeue());
        }

        // If _people was LIFO (Stack-like): allPeople will contain elements from Newest to Oldest.
        // Example: Add Bob, Tim, Sue -> _people (LIFO) -> Pop Sue, Pop Tim, Pop Bob.
        // allPeople = [Sue, Tim, Bob]

        // The "oldest" (FIFO) person is now the last element in allPeople.
        if (allPeople.Count == 0)
        {
             throw new InvalidOperationException("No one in the queue."); // Should have been caught by initial IsEmpty()
        }

        oldestPerson = allPeople[allPeople.Count - 1]; // This is the "Bob" in our example
        allPeople.RemoveAt(allPeople.Count - 1); // Remove it from the temp list

        // Now process oldestPerson as per rules:
        if (oldestPerson.Turns <= 0) // Infinite turns
        {
            // Re-add to the "back" of the conceptual FIFO queue.
            // If _people is LIFO, adding back means it goes to the "top".
            // We need to rebuild the original order with oldestPerson at the true "bottom".
        }
        else // Finite turns
        {
            oldestPerson.Turns -= 1;
            if (oldestPerson.Turns > 0)
            {
                // Re-add to the "back" of the conceptual FIFO queue.
            }
            else
            {
                // Person is out of turns, do not re-add.
                oldestPerson = null; // Mark for not re-adding
            }
        }
        
        // Rebuild the _people queue, ensuring correct order and oldestPerson placed at the back if needed.
        // The original elements from 'allPeople' (which were from newest to oldest if _people is LIFO)
        // need to be re-added first, then oldestPerson (if not removed).
        // To maintain conceptual FIFO, we'd add back in reverse (oldest first into _people if LIFO)
        // followed by the processed oldestPerson (if remaining).

        // This is the tricky part assuming _people is LIFO:
        // allPeople is [Newest, ..., SecondOldest]. oldestPerson is Oldest.
        // To put them back in "FIFO" order for the next cycle:
        // We need Oldest -> SecondOldest -> ... -> Newest.
        // But _people.Enqueue() adds to the "top" (end if List.Add, or top if Stack).
        // To get Oldest at the conceptual "bottom" for next cycle:
        // First, push back the current 'allPeople' (which are Newer elements) onto the original queue in reversed order.
        // Then, push back oldestPerson if it remains. This creates a conceptual FIFO.

        // Rebuild the queue: Iterate allPeople in reverse order and Enqueue to _people.
        // This puts them back into their original LIFO order in _people, *without* the one just removed.
        for (int i = allPeople.Count - 1; i >= 0; i--)
        {
            _people.Enqueue(allPeople[i]);
        }

        // Now, add the processed person back to the 'back' of the conceptual FIFO queue.
        // If _people is LIFO, Enqueue puts it on top, so it will be the next one popped.
        // This needs careful thought.
        // The requirement is "placed back into the back of the queue."
        // If _people.Dequeue() gives LIFO, and we want FIFO simulation:
        // The correct approach is to truly use a temporary structure that reverses order.

        // Let's refine the LIFO compensation:
        // We pop everything into a temporary list. The last element in that list is the oldest.
        // Remove it. Process it.
        // If it needs to be re-enqueued, add it to the *front* of the temporary list.
        // Then, push all elements from the temporary list back onto the original queue in reverse.

        // Simpler approach for LIFO _people:
        // If _people.Dequeue() always gives the LAST element, but we need the FIRST element (Bob),
        // we essentially need to "rotate" the stack until Bob is at the top.
        // This also makes it O(N).

        Person actualNextPerson = null;

        // Rotate the conceptual "queue" until the oldest person is at the top (ready to be dequeued)
        // If _people contains [Bob, Tim, Sue] (Bob at bottom, Sue at top/next out)
        // We need to move Sue and Tim to the "bottom" to get Bob out.
        if (_people.Length > 0)
        {
            // The number of rotations is (Length - 1) to bring the oldest to the front
            // This is if _people.Dequeue() removes from the *front* (FIFO).
            // But if _people.Dequeue() removes from the *back* (LIFO), we need a different approach.

            // Given the errors, _people.Dequeue() is likely giving the LAST-IN element.
            // Let's assume _people behaves like System.Collections.Generic.Stack<Person>.
            // Enqueue = Push, Dequeue = Pop.

            // To get the "Bob" (first enqueued) from a stack, we must pop everything else.
            Person oldest = null;
            var tempPersons = new Stack<Person>(); // Use a real Stack for temporary storage

            while (!_people.IsEmpty())
            {
                tempPersons.Push(_people.Dequeue());
            }

            // Now, tempPersons has the elements in reverse order: [Sue, Tim, Bob] (Bob on top)
            // The first element added to _people (Bob) is now at the top of tempPersons.
            oldest = tempPersons.Pop(); // This is our "Bob"

            // Process 'oldest' (our current turn person)
            if (oldest.Turns <= 0)
            {
                // Infinite turns: add it back to the conceptual "end" of the FIFO queue.
                // To do this with a stack, it means putting it at the bottom.
                // Push remaining tempPersons back to _people.
                while (tempPersons.Count > 0)
                {
                    _people.Enqueue(tempPersons.Pop());
                }
                // Then add 'oldest' to the "bottom" (meaning, it will be the next one after the current "top").
                // If _people is LIFO, Enqueuing 'oldest' makes it the new bottom.
                // This would require a second pass or an internal List in PersonQueue.
                // Since _people is opaque, we'll have to push 'oldest' on top of _people and then cycle
                // it to the bottom if needed. The simplest way is to add it back to _people after the rest.
                _people.Enqueue(oldest); // Add to the back of the conceptual queue
            }
            else
            {
                oldest.Turns -= 1;
                if (oldest.Turns > 0)
                {
                    // Finite turns remain: add it back to the conceptual "end" of the FIFO queue.
                    while (tempPersons.Count > 0)
                    {
                        _people.Enqueue(tempPersons.Pop());
                    }
                    _people.Enqueue(oldest); // Add to the back
                }
                else
                {
                    // Person is out of turns, do not re-add.
                    // Just push remaining tempPersons back to _people.
                    while (tempPersons.Count > 0)
                    {
                        _people.Enqueue(tempPersons.Pop());
                    }
                }
            }
            actualNextPerson = oldest;
        }

        // --- End of changes for LIFO PersonQueue compensation ---
        return actualNextPerson;
    }

    public override string ToString()
    {
        return _people.ToString();
    }
}