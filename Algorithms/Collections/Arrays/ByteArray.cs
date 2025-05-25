#region Using

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
// 
// Copyright (C) 2002-2010, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////

#endregion

namespace Algorithms.Collections;

/// <summary>
///     Summary description for ByteArray.
/// </summary>
public class ByteArray
{
    #region Concat

    public static ByteArray operator +(ByteArray b1, ByteArray b2)
    {
        var result = new ByteArray(b1.Length + b2.Length);
        result.Write(b1);
        result.Write(b2);
        return result;
    }

    #endregion

    #region Variables

    int _byteLength;
    byte[] _data;
    int _offset;
    BitArray _test;

    #endregion

    #region Construction

    public ByteArray()
    {
        Data = new byte[16];
        _byteLength = 0;
    }

    public ByteArray(int length)
    {
        Data = new byte[length];
        _byteLength = 0;
    }

    public ByteArray(byte[] data) => Data = data;

    public static implicit operator ByteArray(byte[] data) => new(data);

    public static explicit operator ByteArray(int value) => new(BitConverter.GetBytes(value));

    public static explicit operator ByteArray(string value)
    {
        var array = new ByteArray();
        array.Write(value);
        return array;
    }

    public static explicit operator ByteArray(double value) => new(BitConverter.GetBytes(value));

    public static explicit operator ByteArray(long value) => new(BitConverter.GetBytes(value));

    public static explicit operator ByteArray(short value) => new(BitConverter.GetBytes(value));

    public static explicit operator byte[](ByteArray ba)
    {
        byte[] array = new byte[ba.Length];
        Array.Copy(ba._data, 0, array, 0, ba._byteLength);
        return array;
    }

    public byte[] ToByteArray() => (byte[])this;

    #endregion

    #region Save

    public void Load(string filename)
    {
        FileStream file = File.OpenRead(filename);
        Data = new byte[file.Length];
        file.Read(_data, 0, Length);
        file.Close();
    }

    public void Save(string filename)
    {
        File.Delete(filename);
        FileStream file = File.OpenWrite(filename);
        SaveToStream(file);
        file.Close();
    }

    public void SaveToStream(Stream stream)
    {
        stream.Write(_data, 0, Length);
    }

    #endregion

    #region Properties

    public byte[] Data {
        get => _data;
        set
        {
            _data = value;
            _offset = 0;
            _byteLength = _data == null ? 0 : _data.Length;
            _test = null;
        }
    }

    public byte this[int index] {
        get => _data[index];
        set => _data[index] = value;
    }

    public int Length {
        get => _byteLength;
        set
        {
            Ensure(_byteLength);
            _byteLength = value;
        }
    }

    public int Capacity {
        get => _data == null ? 0 : _data.Length;
        set
        {
            if (Capacity == value)
                return;

            byte[] newData = new byte[value];
            if (_data != null)
                Array.Copy(_data, 0, newData, 0, Math.Min(value, _data.Length));
            _data = newData;

            if (_test != null && _test.Length < value) {
                var newTest = new BitArray(value);
                int length = _test.Length;
                for (int i = 0; i < length; i++)
                    if (_test[i])
                        newTest[i] = true;
                _test = newTest;
            }
        }
    }

    public int Offset {
        get => _offset;
        set
        {
            if (_offset > _byteLength)
                throw new ArgumentOutOfRangeException("value");
            _offset = value;
        }
    }

    #endregion

    #region Serialization

    public bool Test {
        get => _test != null;
        set
        {
            if (value == Test)
                return;

            _test = value ? new BitArray(Length) : null;
        }
    }

    public void Allocate()
    {
        _data = new byte[Length];
        _offset = 0;
        _test = null;
    }

    public void Ensure(int index)
    {
        if (_data == null)
            return;

        if (index >= _data.Length)
            Capacity = index * 2;
    }

    public void CheckConsistency()
    {
        int length = Length;
        if (_test != null)
            for (int i = 0; i < length; i++)
                Debug.Assert(_test[i]);
    }

    public void Trim(bool tight)
    {
        if (!tight && Capacity - Length < 4)
            return;
        Capacity = Length;
    }

    #endregion

    #region Reading

    public byte Index1(int index) => _data[_offset + index];

    public short Index2(int index) => Read2(_offset + index * 2);

    public int Index3(int index) => Read3(_offset + index * 3);

    public int Index4(int index) => Read4(_offset + index * 4);

    public int Index(int index, int bytes)
    {
        switch (bytes) {
            case 1:
                return _data[_offset + index];
            case 2:
                return Read2(_offset + index * 2);
            case 3:
                return Read3(_offset + index * 3);
            case 4:
                return Read4(_offset + index * 4);
            default:
                throw new ArgumentOutOfRangeException("bytes");
        }
    }

    public byte Read1(int offset) => _data[offset];

    public byte Read1() => _data[_offset++];

    public short Read2(int offset)
    {
        int value = _data[offset++];
        value += _data[offset] << 8;
        return (short)value;
    }

    public short Read2()
    {
        int value = _data[_offset++];
        value += _data[_offset] << 8;
        return (short)value;
    }

    public int Read3(int offset)
    {
        int value = _data[offset++];
        value += _data[offset++] << 8;
        value += _data[offset++] << 16;
        return value;
    }

    public int Read3()
    {
        int value = _data[_offset++];
        value += _data[_offset++] << 8;
        value += _data[_offset++] << 16;
        return value;
    }

    public int Read4(int offset)
    {
        int value = _data[offset++];
        value += _data[offset++] << 8;
        value += _data[offset++] << 16;
        value += _data[offset++] << 24;
        return value;
    }

    public int Read4()
    {
        int value = _data[_offset++];
        value += _data[_offset++] << 8;
        value += _data[_offset++] << 16;
        value += _data[_offset++] << 24;
        return value;
    }

    public int ReadN(int bytes)
    {
        Debug.Assert(bytes <= 4);
        int value = 0;
        int shift = 0;
        while (bytes-- > 0) {
            value += _data[_offset++] << shift;
            shift += 8;
        }

        return value;
    }

    public long ReadLong(int bytes)
    {
        long value = 0;
        int shift = 0;
        while (bytes-- > 0) {
            value += (long)Data[_offset++] << shift;
            shift += 8;
        }

        return value;
    }

    public double ReadDouble() => BitConverter.Int64BitsToDouble(ReadLong(8));

    public unsafe float ReadFloat()
    {
        int value = Read4();
        return *(float*)&value;
    }

    public unsafe string ReadString()
    {
        byte length = Read1();
        fixed (byte* pfixed = &_data[_offset]) {
            return new string((sbyte*)pfixed, _offset, length, Encoding.UTF8);
        }
    }

    public long ReadCompressed()
    {
        long value = 0;
        int shift = 0;

        while (true) {
            byte b = Data[_offset++];
            value += (b & 0x7f) << shift;
            if ((b & 0x80) == 0)
                return value;
            shift += 7;
        }
    }

    #endregion

    #region Writing

    public void MoveToStart()
    {
        _offset = 0;
    }

    public void MoveToEnd()
    {
        _offset = _byteLength;
    }

    public void Append(ByteArray value)
    {
        MoveToEnd();
        Write(value);
    }

    public void Insert(int count)
    {
        Ensure(_byteLength + count);
        Array.Copy(_data, _offset, _data, _offset + count, _byteLength - _offset);
        Array.Clear(_data, _offset, count);
        _byteLength += count;
    }

    public void Delete(int count)
    {
        Array.Copy(_data, _offset + count, _data, _offset, _byteLength - (_offset + count));
        _byteLength -= count;
        Array.Clear(_data, _byteLength, count);
    }

    public void Write(ByteArray array)
    {
        Write(array._data, 0, array.Length);
    }

    public void Write(byte[] value)
    {
        Write(value, 0, value.Length);
    }

    public void Write(byte[] value, int start, int length)
    {
        int end = start + length;
        if (_data == null) {
            _offset += length;
        } else {
            Ensure(_offset + length);
            for (int i = start; i < end; i++) {
                if (_test != null) {
                    Debug.Assert(!_test[_offset]);
                    _test[_offset] = true;
                }

                _data[_offset++] = value[i];
            }
        }

        if (_offset > _byteLength)
            _byteLength = _offset;
    }

    public void Write(byte value)
    {
        if (_data != null) {
            if (_test != null) {
                Debug.Assert(!_test[_offset]);
                _test[_offset] = true;
            }

            _data[_offset] = value;
        }

        _offset++;
    }

    public void Write(short value)
    {
        Write(value, 2);
    }

    public void Write(int value)
    {
        Write(value, 4);
    }

    public void Write(long value)
    {
        Write(value, 8);
    }

    public unsafe void Write(double value)
    {
        Write(*(long*)&value, 8);
    }

    public unsafe void Write(float value)
    {
        Write(*(int*)&value, 4);
    }

    public void Write(string value)
    {
        int length = value[0];
        if (length > 255)
            length = 255;

        if (_data == null) {
            _offset += length + 1;
        } else {
            Ensure(_offset + length + 1);
            _data[_offset++] = (byte)length;
            for (int i = 0; i < length; i++) {
                if (_test != null) {
                    Debug.Assert(!_test[_offset]);
                    _test[_offset] = true;
                }

                _data[_offset++] = (byte)value[i];
            }
        }

        if (_offset > length)
            length = _offset;
    }

    public static int CompressedSize(long value)
    {
        int count = 0;
        Debug.Assert(value >= 0);
        do {
            byte b = (byte)(value & 0x7f);
            value >>= 7;
            count++;
        } while (value != 0);

        return count;
    }

    public void WriteCompressed(long value)
    {
        Ensure(_offset + 8);

        int oldOffset = _offset;
        long oldValue = value;

        Debug.Assert(value >= 0);
        do {
            byte b = (byte)(value & 0x7f);
            value >>= 7;
            if (value != 0)
                b |= 0x80;

            if (_data != null) {
                if (_test != null) {
                    Debug.Assert(!_test[_offset]);
                    _test[_offset] = true;
                }

                _data[_offset] = b;
            }

            _offset++;
        } while (value > 0);

        if (_data != null) {
            int offsetSave = _offset;
            _offset = oldOffset;
            Debug.Assert(ReadCompressed() == oldValue);
            Debug.Assert(_offset == offsetSave);
        }

        if (_offset > _byteLength)
            _byteLength = _offset;
    }

    public void Write(long value, int bytes)
    {
        Ensure(_offset + bytes);

        if (_data != null) {
            while (bytes-- > 0) {
                if (_test != null) {
                    Debug.Assert(!_test[_offset]);
                    _test[_offset] = true;
                }

                _data[_offset++] = (byte)(value & 0xff);
                value = value >> 8;
            }

            Debug.Assert(value == 0 || value == -1, "Database value truncated");
        } else {
            _offset += bytes;
        }

        if (_offset > _byteLength)
            _byteLength = _offset;
    }

    #endregion

    #region Comparison

    public override bool Equals(object x)
    {
        byte[] data1 = _data;
        byte[] data2 = ((ByteArray)x)._data;
        if (data1 == data2)
            return true;
        int cmp = data1.Length - data2.Length;
        if (cmp != 0)
            return false;
        for (int i = 0; i < data1.Length; i++) {
            cmp = data1[i] - data2[i];
            if (cmp != 0)
                return false;
        }

        return true;
    }

    public override int GetHashCode()
    {
        // ReSharper disable NonReadonlyFieldInGetHashCode
        int hash = _data.Length;
        for (int i = 0; i < _data.Length; i++) {
            int tmp = _data[i].GetHashCode();
            tmp |= tmp << 8;
            tmp |= tmp << 16;
            hash = unchecked(hash + tmp);
        }

        return hash;
        // ReSharper restore NonReadonlyFieldInGetHashCode
    }

    #endregion
}