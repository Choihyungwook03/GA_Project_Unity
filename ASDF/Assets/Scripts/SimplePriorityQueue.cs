using System.Collections.Generic;
using System;

public class SimplePriorityQueue<T>
{
    private List<KeyValuePair<T, int>> elements = new List<KeyValuePair<T, int>>();

    public int Count => elements.Count;

    public void Enqueue(T item, int priority)
    {
        elements.Add(new KeyValuePair<T, int>(item, priority));
    }

    public T Dequeue()
    {
        int bestIndex = 0;
        for (int i = 0; i < elements.Count; i++)
        {
            if (elements[i].Value < elements[bestIndex].Value)
            {
                bestIndex = i;
            }
        }

        T bestItem = elements[bestIndex].Key;
        elements.RemoveAt(bestIndex);
        return bestItem;
    }
}