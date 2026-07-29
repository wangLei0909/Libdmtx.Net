namespace Libdmtx;

public class DmtxException : Exception
{
    public DmtxException() { }
    public DmtxException(string message) : base(message) { }
    public DmtxException(string message, Exception inner) : base(message, inner) { }
}
