using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QQLib.Interface
{
    public interface IVCodeRecognizer
    {
        string Recognize(byte[] imageBuffer);
    }
}
