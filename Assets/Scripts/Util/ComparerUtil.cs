using System;
using System.Collections.Generic;



public class ComparerUtil<T> : Comparer<T> {
    private Comparison<T> _comparison;

    public Comparer<T> Default
    {
        get { return Comparer<T>.Default; }
    }

    public ComparerUtil(Comparison<T> comparison) {
        _comparison = comparison;
    }

    public static Comparer<T> Create(Comparison<T> comparison) {
        return new ComparerUtil<T>(comparison);
    }

    public override int Compare(T x, T y) {
        return _comparison(x, y);
    }
}