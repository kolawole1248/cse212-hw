using System;
using System.Collections.Generic;

// Assuming Person and PersonQueue classes are defined elsewhere in your project:
// public class Person
// {
//     public string Name { get; set; }
//     public int Turns { get; set; } // This property needs to be mutable for decrementing turns
//
//     public Person(string name, int turns)
//     {
//         Name = name;
//         Turns = turns;
//     }
//
//     public override string ToString()
//     {
//         return $"({Name}, {Turns})";
//     }
// }

// Assuming PersonQueue is a custom queue implementation that has Enqueue, Dequeue, IsEmpty, and Length
// For example, it might internally use a System.Collections.Generic.Queue<Person>
// public class PersonQueue
// {
//     private Queue<Person> _queue = new Queue<Person>();
//
//     public int Length => _queue.Count;
//
//     public bool IsEmpty() => _queue.Count == 0;
//
//     public void Enqueue(Person person) => _queue.Enqueue(person);
//
//     public Person Dequeue() => _queue.Dequeue();
//
//     public override string ToString() => string.Join(", ", _queue);
// }


/// <summary>
/// This queue is circular.  When people are added via AddPerson, then they are added to the
/// back of the queue (per FIFO rules).  When GetNextPerson is called, the next person
/// in the queue is saved to be returned and then they are placed back into the back of the queue.  Thus,
/// each person stays in the queue and is given turns.  When a person is added to the queue,
/// a turns parameter is provided to identify how many turns they will be given.  If the turns is 0 or
/// less than they will stay in the queue forever.  If a person is out of turns then they will
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
    /// have no more turns left.  Note that a turns value of 0 or less means the
    /// person has an infinite number of turns.  An error exception is thrown
    /// if the queue is empty.
    /// </summary>
    public Person GetNextPerson()
    {
        // Requirement: "If the queue is empty, then an error exception shall be thrown."
        if (_people.IsEmpty())
        {
            throw new InvalidOperationException("No one in the queue.");
        }
        else
        {
            Person person = _people.Dequeue();

            // Requirement: "If the turns is 0 or less than they will stay in the queue forever."
            // This means if 'person.Turns' was *initially* 0 or less, it's infinite.
            // We don't decrement infinite turns.
            if (person.Turns <= 0)
            {
                // Requirement: "When a person is dequeued and has an infinite number of turns (...), they shall be enqueued again."
                _people.Enqueue(person);
            }
            // If turns are finite (greater than 0 initially)
            else
            {
                // Consume one turn
                person.Turns -= 1;

                // Requirement: "If a person is out of turns then they will not be added back into the queue."
                // "When a person is dequeued and still has turns left, they shall be enqueued again."
                // "Still has turns left" means turns > 0 *after* decrementing.
                if (person.Turns > 0)
                {
                    _people.Enqueue(person);
                }
                // If person.Turns is now 0 (or less, though that shouldn't happen with finite turns),
                // they are not re-enqueued as they are out of turns.
            }

            return person;
        }
    }

    public override string ToString()
    {
        return _people.ToString();
    }
}