using System.Collections.Generic;

public class CircularStack<T>
{
    private readonly LinkedList<T> _list;
    private readonly int _capacity;

    public CircularStack(int capacity)
    {
        _capacity = capacity;
        _list = new LinkedList<T>();
    }

    public void Push(T item)
    {
        if (_list.Count >= _capacity)
        {
            // Elimina el elemento más antiguo (último de la lista)
            _list.RemoveLast();
        }

        _list.AddFirst(item); // Agrega en la cima (al frente)
    }

    public T Pop()
    {
        if (_list.Count == 0) throw new System.InvalidOperationException("Stack is empty");
        var value = _list.First.Value;
        _list.RemoveFirst();
        return value;
    }

    public T Peek()
    {
        if (_list.Count == 0) throw new System.InvalidOperationException("Stack is empty");
        return _list.First.Value;
    }

    public int Count => _list.Count;

    public void Clear() => _list.Clear();

    public bool Contains(T item) => _list.Contains(item);

    public T[] ToArray() => new List<T>(_list).ToArray();
}