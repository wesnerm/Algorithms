namespace Algorithms.Mathematics;

public interface IBitVector<T> where T : IBitVector<T>
{
    T And(T number);

    T Or(T number);

    T Not(T number);

    T Xor(T number);

    T ShiftLeft(T number);

    T ShiftRight(T number);

    T UnsignedShiftRight(T number);
}