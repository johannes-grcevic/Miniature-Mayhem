using System.Text;

public static class StringBuilderExtensions
{
    public static StringBuilder AppendSpace(this StringBuilder sb)
    {
        sb.Append(' ');
        return sb;
    }

    public static StringBuilder AppendWithSpace(this StringBuilder sb, char value)
    {
        sb.Append(value);
        sb.Append(' ');
        return sb;
    }

    public static StringBuilder AppendWithSpace(this StringBuilder sb, string value)
    {
        sb.Append(value);
        sb.Append(' ');
        return sb;
    }
}
