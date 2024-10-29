using System;
using System.Collections.Generic;


/// <summary>
/// PriorityQueue is a custom queue structure where each item has an associated priority value.
/// Unlike a normal Queue<T>, which processes items in the order they were added (FIFO),
/// this PriorityQueue<T> dequeues the item with the highest priority (smallest priority value) first.
/// It is particularly useful for algorithms like A* pathfinding, where nodes are explored based on priority.
/// </summary>
public class PriorityQueue<T>
{
    private List<KeyValuePair<T, float>> _elements = new List<KeyValuePair<T, float>>();

    public int Count => _elements.Count;

    // Add element to queue with priority
    public void Enqueue(T item, float priority)
    {
        _elements.Add(new KeyValuePair<T, float>(item, priority));
    }

    // Delete element with the highest priority (Lowest value) and return it
    public T Dequeue()
    {
        if (_elements.Count == 0)
        {
            throw new InvalidOperationException("The priority queue is empty.");
        }

        int bestIndex = 0;

        // Finding element with the highest priority
        for (int i = 1; i < _elements.Count; i++)
        {
            if (_elements[i].Value < _elements[bestIndex].Value)
            {
                bestIndex = i;
            }
        }

        T bestItem = _elements[bestIndex].Key;
        _elements.RemoveAt(bestIndex);
        return bestItem;
    }
}
