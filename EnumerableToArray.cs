using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

public static class EnumerableToArray
{
    public static T[] ToArray<T>(this T[] array)
    {
        int length = array.Length;
        if (length == 0)
        {
            return EmptyArray<T>.array;
        }
        T[] destinationArray = new T[length];
        Array.Copy(array, destinationArray, length);
        return destinationArray;
    }

    public static T[] ToArray<T>(this ICollection<T> collection)
    {
        T[] array = new T[collection.Count];
        collection.CopyTo(array, 0);
        return array;
    }

    public static T[] ToArray<T>(this IEnumerable<T> enumerable)
    {
        if (enumerable is ICollection<T>)
        {
            ICollection<T> is2 = (ICollection<T>) enumerable;
            T[] localArray = new T[is2.Count];
            is2.CopyTo(localArray, 0);
            return localArray;
        }
        using (IEnumerator<T> enumerator = enumerable.GetEnumerator())
        {
            EnumeratorToArray<T> array = new EnumeratorToArray<T>(enumerator);
            return array.array;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct EnumeratorToArray<T>
    {
        public T[] array;
        private IEnumerator<T> enumerator;
        private int len;
        public EnumeratorToArray(IEnumerator<T> enumerator)
        {
            this.array = null;
            this.enumerator = enumerator;
            this.len = 0;
            if (enumerator.MoveNext())
            {
                this.Fill();
            }
            else
            {
                this.array = EmptyArray<T>.array;
            }
            this.enumerator = null;
        }

        private void Fill()
        {
            int index = this.len++;
            T current = this.enumerator.Current;
            if (this.enumerator.MoveNext())
            {
                this.Fill();
            }
            else
            {
                this.array = new T[this.len];
            }
            this.array[index] = current;
        }
    }
}

