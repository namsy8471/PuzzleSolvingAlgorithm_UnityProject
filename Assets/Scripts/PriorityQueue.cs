using System;
using System.Collections.Generic;

public class PriorityQueue<TElement, TPriority> where TPriority : IComparable<TPriority>
{
    private List<(TElement Element, TPriority Priority)> _elements = new List<(TElement Element, TPriority Priority)>();

    public int Count => _elements.Count;

    public void Enqueue(TElement element, TPriority priority)
    {
        _elements.Add((element, priority));
        int c = _elements.Count - 1;

        // Bubble up
        while (c > 0 && _elements[c].Priority.CompareTo(_elements[c / 2].Priority) < 0)
        {
            (_elements[c], _elements[c / 2]) = (_elements[c / 2], _elements[c]);
            c = c / 2;
        }
    }

    public TElement Dequeue()
    {
        int li = _elements.Count - 1;
        var frontItem = _elements[0];
        _elements[0] = _elements[li];
        _elements.RemoveAt(li);

        --li; // Last index
        int pi = 0; // Parent index

        while (true)
        {
            int ci = pi * 2 + 1; // Left child index
            if (ci > li) break; // If no children, done
            int rc = ci + 1; // Right child index
            if (rc <= li && _elements[rc].Priority.CompareTo(_elements[ci].Priority) < 0)
                ci = rc;
            if (_elements[pi].Priority.CompareTo(_elements[ci].Priority) <= 0) break;
            (_elements[pi], _elements[ci]) = (_elements[ci], _elements[pi]);
            pi = ci;
        }

        return frontItem.Element;
    }

    public void Clear()
    {
        _elements.Clear();
    }
}