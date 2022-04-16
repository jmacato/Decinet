using System.Runtime.InteropServices;
using System.Text;

namespace Decinet.Backend.macOS.AudioToolbox;

[StructLayout(LayoutKind.Sequential)]
public struct AudioTimeStamp
{
    [Flags]
    public enum AtsFlags : uint
    {
        // UInt32 in AudioTimeStamp
        NothingValid = 0,
        SampleTimeValid = 1 << 0,
        HostTimeValid = 1 << 1,
        RateScalarValid = 1 << 2,
        WordClockTimeValid = 1 << 3,
        SmpteTimeValid = 1 << 4,
        SampleHostTimeValid = SampleTimeValid | HostTimeValid
    }

    public double SampleTime;
    public ulong HostTime;
    public double RateScalar;
    public ulong WordClockTime;
    public SmpteTime SMPTETime;
    public AtsFlags Flags;
    public uint Reserved;

    public override string ToString()
    {
        var sb = new StringBuilder("{");
        if ((Flags & AtsFlags.SampleTimeValid) != 0)
            sb.Append("SampleTime=" + SampleTime.ToString());

        if ((Flags & AtsFlags.HostTimeValid) != 0)
        {
            if (sb.Length > 0)
                sb.Append(',');
            sb.Append("HostTime=" + HostTime.ToString());
        }

        if ((Flags & AtsFlags.RateScalarValid) != 0)
        {
            if (sb.Length > 0)
                sb.Append(',');

            sb.Append("RateScalar=" + RateScalar.ToString());
        }

        if ((Flags & AtsFlags.WordClockTimeValid) != 0)
        {
            if (sb.Length > 0)
                sb.Append(',');
            sb.Append("WordClock=" + HostTime.ToString() + ",");
        }

        if ((Flags & AtsFlags.SmpteTimeValid) != 0)
        {
            if (sb.Length > 0)
                sb.Append(',');
            sb.Append("SmpteTime=" + SMPTETime.ToString());
        }

        sb.Append("}");

        return sb.ToString();
    }
}