using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayoutFileSystem
{
    public class LayoutHeader
    {
        public static readonly int HEADER_BYTE_LENGTH = 20;

        public static readonly string FILE_HEADER = "SPIF";
        public static readonly string CHUNK1_HEADER = "xfr1";
        public static readonly string CHUNK2_HEADER = "xfr2";
        public static readonly string CHUNK3_HEADER = "ifr1";
        public static readonly string CHUNK4_HEADER = "ifr2";

        private string m_ChunkId = "SPIF";	// 4 bytes
        private string m_SubChunk1Id = "xfr1";	// 4 bytes
        private string m_SubChunk2Id = "xfr2";	// 4 bytes
        private string m_SubChunk3Id = "ifr1";	// 4 bytes
        private string m_SubChunk4Id = "ifr2";	// 4 bytes

        private int m_ChunkSize = 12; // unsigned 4 bytes, little endian
        private int m_SubChunk1Size = 0; // unsigned 4 bytes, little endian
        private int m_SubChunk2Size = 0; // unsigned 4 bytes, little endian
        private int m_SubChunk3Size = 0; // unsigned 4 bytes, little endian
        private int m_SubChunk4Size = 0; // unsigned 4 bytes, little endian

        private short m_FileVersion = 1; // unsigned 2 bytes, little endian
        private short m_DataFormat = 1; // unsigned 2 bytes, little endian
  
        public string ChunkId
        {
            get { return m_ChunkId; }
            set { m_ChunkId = value; }
        }

        public string SubChunk1Id
        {
            get { return m_SubChunk1Id; }
            set { m_SubChunk1Id = value; }
        }

        public string SubChunk2Id
        {
            get { return m_SubChunk2Id; }
            set { m_SubChunk2Id = value; }
        }

        public string SubChunk3Id
        {
            get { return m_SubChunk3Id; }
            set { m_SubChunk3Id = value; }
        }

        public string SubChunk4Id
        {
            get { return m_SubChunk4Id; }
            set { m_SubChunk4Id = value; }
        }
       
        public int ChunkSize
        {
            get { return m_ChunkSize; }
            set { m_ChunkSize = value; }
        }

        public int SubChunk4Size
        {
            get { return m_SubChunk4Size; }
            set { m_SubChunk4Size = value; }
        }

        public int SubChunk3Size
        {
            get { return m_SubChunk3Size; }
            set { m_SubChunk3Size = value; }
        }

        public int SubChunk2Size
        {
            get { return m_SubChunk2Size; }
            set { m_SubChunk2Size = value; }
        }

        public int SubChunk1Size
        {
            get { return m_SubChunk1Size; }
            set { m_SubChunk1Size = value; }
        }

        public short FileVersion
        {
            get { return m_FileVersion; }
            set { m_FileVersion = value; }
        }

        public short DataFormat
        {
            get { return m_DataFormat; }
            set { m_DataFormat = value; }
        }


        public bool IsValid { get; set; }

        public LayoutHeader()
        {
            IsValid = true;
        }

        public LayoutHeader(Stream inputStream)
        {
            IsValid = true;
            LoadHeader(inputStream);
        }

        private bool LoadHeader(Stream inputStream)
        {
            var br = new BinaryReader(inputStream);
            try
            {
                inputStream.Seek(0, SeekOrigin.Begin);
                // SPIFF Header
                m_ChunkId = Encoding.ASCII.GetString(br.ReadBytes(4));//4
                m_ChunkSize = (int)br.ReadUInt32();//4
                m_FileVersion = br.ReadInt16();//2
                m_DataFormat = br.ReadInt16();//2

                if (!IsSupportedVersion(m_FileVersion))
                    throw new NotSupportedException("Unsuported file format.");

                m_SubChunk1Id = Encoding.ASCII.GetString(br.ReadBytes(4));//4
                m_SubChunk1Size = (int)br.ReadInt32();//4
                br.BaseStream.Seek(m_SubChunk1Size, SeekOrigin.Current);

                m_SubChunk2Id = Encoding.ASCII.GetString(br.ReadBytes(4));//4
                m_SubChunk2Size = (int)br.ReadInt32();//4
                br.BaseStream.Seek(m_SubChunk2Size, SeekOrigin.Current);

                m_SubChunk3Id = Encoding.ASCII.GetString(br.ReadBytes(4));//4
                m_SubChunk3Size = (int)br.ReadInt32();//4
                br.BaseStream.Seek(m_SubChunk3Size, SeekOrigin.Current);

                m_SubChunk4Id = Encoding.ASCII.GetString(br.ReadBytes(4));//4
                m_SubChunk4Size = (int)br.ReadInt32();//4
                br.BaseStream.Seek(m_SubChunk4Size, SeekOrigin.Current);

                //// Skip zero bytes.
                //while ((br.ReadByte()) == 0) ;
                //br.BaseStream.Seek(-1, SeekOrigin.Current);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                IsValid = false;
                throw new FileLoadException(e.Message);
            }
            //finally
            //{
            //    if (br != null)
            //        br.Close();
            //}
            
            // check the format is support
            if (m_ChunkId.ToUpper().Equals(FILE_HEADER) && m_DataFormat.Equals(CONSTS.DATA_FORMAT))
            {
                return true;
            }
            else
            {
                Console.WriteLine("Layout file: Unsupported header format.");
            }

            return false;
        }

        private bool IsSupportedVersion(short version)
        {
            return version == CONSTS.FILE_VERSION;
        }


       
    }
}
