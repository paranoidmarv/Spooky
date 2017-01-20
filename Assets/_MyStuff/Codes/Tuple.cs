using UnityEngine;
using System.Collections;

public class Tuple<T1, T2> {
    public T1 First { get; set; }
    public T2 Second { get; set; }
    internal Tuple(T1 first, T2 second) {
        First = first;
        Second = second;
    }
    internal static bool CompareItems<T>(Tuple<T, T> tuple1, Tuple<T, T> tuple2) {
        if (tuple1.First.Equals(tuple2.First) && tuple1.Second.Equals(tuple2.Second)) { return true; }
        else { return false; }
    }
    internal static string GetItems(Tuple<T1, T2> tuple) {
        return "(" + tuple.First.ToString() + ", " + tuple.Second.ToString() + ")";
    }
}

public static class Tuple {
    public static Tuple<T1, T2> New<T1, T2>(T1 first, T2 second) {
        var tuple = new Tuple<T1, T2>(first, second);
        return tuple;
    }
}
