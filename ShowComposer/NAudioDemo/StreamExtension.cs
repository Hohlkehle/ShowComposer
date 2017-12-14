using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowComposer.NAudioDemo
{


    public static class StreamExtension
    {
        public static byte[] ToArray(this Stream stream)
        {
            byte[] buffer = new byte[4096];
            int reader = 0;
            MemoryStream memoryStream = new MemoryStream();
            while ((reader = stream.Read(buffer, 0, buffer.Length)) != 0)
                memoryStream.Write(buffer, 0, reader);
            return memoryStream.ToArray();
        }
    }


}
