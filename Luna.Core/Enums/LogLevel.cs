using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luna.Core.Enums
{
    [Flags]
    public enum LogLevel
    {
        Null = 0x0000,
        Error = 0x0001,
        Warning = 0x0002,
        RetVal = 0x0004,
        ObjState = 0x0008,
        IpcMsg = 0x0010,
        Telegram = 0x0020,
        Sequence = 0x0040,
        Function = 0x0080,
        All = 0x00FF,
        Debug = 0x0FFF
    }
}
