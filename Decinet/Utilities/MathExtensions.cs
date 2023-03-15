using System.Numerics;
using System.Runtime.Serialization.Formatters.Binary;

namespace Decinet.Utilities;

public static class MathExtensions
{
    public static T MapRange<T>(this T value, T from1, T to1, T from2, T to2) where T : INumber<T>
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
    
    public static T MinVal<T>(T val1, T val2) where T : INumber<T>
    {
        return (val1 <= val2) ? val1 : val2;
    } 
    
    
}