using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CaffWebApp.BLL.Exceptions;

[Serializable]
public class ParserException : Exception
{
    public ParserException(string? message) : base(message)
    {
    }

    protected ParserException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
