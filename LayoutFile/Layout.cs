using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;

namespace LayoutFileSystem
{
    public class Layout
    {
        private LayoutHeader m_LayoutHeader;
        private byte[] m_Data1;	// little endian
        private byte[] m_Data2;	// little endian
        private byte[] m_Data3;	// little endian
        private byte[] m_Data4;	// little endian
        private byte[] m_Meta;

        private string m_FileName;

        public string FileName
        {
            get { return m_FileName; }
            set { m_FileName = value; }
        }

        public int MetaSize
        {
            get
            {
                if (m_Meta == null)
                    return 0;
                return m_Meta.Length;
            }
        }

        public byte[] Data1
        {
            get { return m_Data1; }
            set { m_Data1 = value; m_LayoutHeader.SubChunk1Size = m_Data1.Length; }
        }

        public byte[] Data2
        {
            get { return m_Data2; }
            set { m_Data2 = value; m_LayoutHeader.SubChunk2Size = m_Data2.Length; }
        }

        public byte[] Data3
        {
            get { return m_Data3; }
            set { m_Data3 = value; m_LayoutHeader.SubChunk3Size = m_Data3.Length; }
        }

        public byte[] Data4
        {
            get { return m_Data4; }
            set { m_Data4 = value; m_LayoutHeader.SubChunk4Size = m_Data4.Length; }
        }

        public byte[] Meta
        {
            get { return m_Meta; }
            set { m_Meta = value; }
        }

        public Layout()
        {
            m_LayoutHeader = new LayoutHeader();

            m_Data1 = new byte[0];
            m_Data2 = new byte[0];
            m_Data3 = new byte[0];
            m_Data4 = new byte[0];
        }

        public Layout(string filename)
        {
            m_FileName = filename;
            Stream inputStream = null;
            try
            {
                using (ZipInputStream s = new ZipInputStream(new FileStream(filename, FileMode.Open, FileAccess.Read)))
                {
                    ZipEntry theEntry;
                    while ((theEntry = s.GetNextEntry()) != null)
                    {
                        if (theEntry.Name != String.Empty)
                        {
                            using (var streamWriter = new MemoryStream())
                            {
                                int size = 2048;
                                byte[] data = new byte[2048];
                                while (true)
                                {
                                    size = s.Read(data, 0, data.Length);
                                    if (size > 0)
                                    {
                                        streamWriter.Write(data, 0, size);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                initWithInputStream(streamWriter);
                            } 
                        }
                    }
                }
            }
            catch (FileNotFoundException e)
            {
                Trace.TraceError(e.ToString());
                throw new FileLoadException(e.Message);
            }
            catch (IOException e)
            {
                Trace.TraceError(e.ToString());
                throw new FileLoadException(e.Message);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw new FileLoadException(e.Message);
            }
            finally
            {
                if (inputStream != null)
                    inputStream.Close();
            }
        }

        public Layout(LayoutHeader header)
        {
            m_LayoutHeader = header;
        }

        private void initWithInputStream(Stream inputStream)
        {
            // reads the first 44 bytes for header
            m_LayoutHeader = new LayoutHeader(inputStream);

            if (m_LayoutHeader.IsValid)
            {
                // load data
                try
                {
                    inputStream.Seek(LayoutHeader.HEADER_BYTE_LENGTH, SeekOrigin.Begin);
                    m_Data1 = new byte[m_LayoutHeader.SubChunk1Size];
                    inputStream.Read(m_Data1, 0, m_LayoutHeader.SubChunk1Size);
                    inputStream.Seek(8, SeekOrigin.Current);//skip header

                    m_Data2 = new byte[m_LayoutHeader.SubChunk2Size];
                    inputStream.Read(m_Data2, 0, m_LayoutHeader.SubChunk2Size);
                    inputStream.Seek(8, SeekOrigin.Current);//skip header

                    m_Data3 = new byte[m_LayoutHeader.SubChunk3Size];
                    inputStream.Read(m_Data3, 0, m_LayoutHeader.SubChunk3Size);
                    inputStream.Seek(8, SeekOrigin.Current);//skip header

                    m_Data4 = new byte[m_LayoutHeader.SubChunk4Size];
                    inputStream.Read(m_Data4, 0, m_LayoutHeader.SubChunk4Size);

                    //ReadMetaData(inputStream);
                }
                catch (IOException e)
                {
                    Trace.TraceError(e.ToString());
                    throw new FileLoadException(e.Message);
                }
                // end load data
            }
            else
            {
                Trace.TraceError("Invalid File Header");
                throw new ArgumentException("Invalid File Header");
            }
        }

        private void ReadMetaData(Stream inputStream)
        {
            var eln = inputStream.Length - inputStream.Position;
            if (eln > 0)
            {
                m_Meta = new byte[eln];
                inputStream.Read(m_Meta, 0, m_Meta.Length);
            }
        }

        public void _Save(string filePath)
        {
            using (ZipOutputStream s = new ZipOutputStream(new FileStream(filePath, FileMode.Create)))
            {
                // 0 - store only to 9 - means best compression
                s.SetLevel(9);
             
                using (var writer = new BinaryWriter(s))
                {
                    s.PutNextEntry(new ZipEntry("dchSPIF.bin") { DateTime = DateTime.Now });

                    // Write the header
                    writer.Write(m_LayoutHeader.ChunkId.ToCharArray());
                    writer.Write(LayoutHeader.HEADER_BYTE_LENGTH);
                    writer.Write(m_LayoutHeader.FileVersion);
                    writer.Write(m_LayoutHeader.DataFormat);

                    s.PutNextEntry(new ZipEntry("dchxfr1.bin") { DateTime = DateTime.Now });

                    // Write the data1 chunk
                    writer.Write(m_LayoutHeader.SubChunk1Id.ToCharArray());
                    writer.Write(m_LayoutHeader.SubChunk1Size);
                    foreach (var dataPoint in m_Data1)
                        writer.Write(dataPoint);

                    s.PutNextEntry(new ZipEntry("dchxfr2.bin") { DateTime = DateTime.Now });

                    // Write the data2 chunk
                    writer.Write(m_LayoutHeader.SubChunk2Id.ToCharArray());
                    writer.Write(m_LayoutHeader.SubChunk2Size);
                    foreach (var dataPoint in m_Data2)
                        writer.Write(dataPoint);

                    s.PutNextEntry(new ZipEntry("dchifr1.bin") { DateTime = DateTime.Now });

                    // Write the data3 chunk
                    writer.Write(m_LayoutHeader.SubChunk3Id.ToCharArray());
                    writer.Write(m_LayoutHeader.SubChunk3Size);
                    foreach (var dataPoint in m_Data3)
                        writer.Write(dataPoint);

                    s.PutNextEntry(new ZipEntry("dchifr2.bin") { DateTime = DateTime.Now });

                    // Write the data4 chunk
                    writer.Write(m_LayoutHeader.SubChunk4Id.ToCharArray());
                    writer.Write(m_LayoutHeader.SubChunk4Size);
                    foreach (var dataPoint in m_Data4)
                        writer.Write(dataPoint);

                    if (m_Meta != null)
                    {
                        s.PutNextEntry(new ZipEntry("dchmt.bin") { DateTime = DateTime.Now });
                        foreach (var dataPoint in m_Meta)
                            writer.Write(dataPoint);
                    }
                }

                s.Finish();
                s.Close();
            }

            
        }

        public void Save(string filePath)
        {
            using (ZipOutputStream s = new ZipOutputStream(new FileStream(filePath, FileMode.Create)))
            {
                // 9 - means best compression
                s.SetLevel(9);
                var entry = new ZipEntry("data.bin");
                entry.DateTime = DateTime.Now;
                s.PutNextEntry(entry);

                using (var writer = new BinaryWriter(s))
                {
                    // Write the header
                    writer.Write(m_LayoutHeader.ChunkId.ToCharArray());
                    writer.Write(LayoutHeader.HEADER_BYTE_LENGTH);
                    writer.Write(m_LayoutHeader.FileVersion);
                    writer.Write(m_LayoutHeader.DataFormat);

                    // Write the data1 chunk
                    writer.Write(m_LayoutHeader.SubChunk1Id.ToCharArray());
                    writer.Write(m_LayoutHeader.SubChunk1Size);
                    foreach (var dataPoint in m_Data1)
                        writer.Write(dataPoint);

                    // Write the data2 chunk
                    writer.Write(m_LayoutHeader.SubChunk2Id.ToCharArray());
                    writer.Write(m_LayoutHeader.SubChunk2Size);
                    foreach (var dataPoint in m_Data2)
                        writer.Write(dataPoint);

                    // Write the data3 chunk
                    writer.Write(m_LayoutHeader.SubChunk3Id.ToCharArray());
                    writer.Write(m_LayoutHeader.SubChunk3Size);
                    foreach (var dataPoint in m_Data3)
                        writer.Write(dataPoint);

                    // Write the data4 chunk
                    writer.Write(m_LayoutHeader.SubChunk4Id.ToCharArray());
                    writer.Write(m_LayoutHeader.SubChunk4Size);
                    foreach (var dataPoint in m_Data4)
                        writer.Write(dataPoint);

                    if (m_Meta != null)
                        foreach (var dataPoint in m_Meta)
                            writer.Write(dataPoint);
                }

                s.Finish();
                s.Close();
            }

            #region MyRegion
            //using (var fileStream = new FileStream(filePath, FileMode.Create))
            //using (var writer = new BinaryWriter(fileStream))
            //{
            //    // Write the header
            //    writer.Write(m_LayoutHeader.ChunkId.ToCharArray());
            //    writer.Write(LayoutHeader.HEADER_BYTE_LENGTH);
            //    writer.Write(m_LayoutHeader.FileVersion);
            //    writer.Write(m_LayoutHeader.DataFormat);

            //    // Write the data1 chunk
            //    writer.Write(m_LayoutHeader.SubChunk1Id.ToCharArray());
            //    writer.Write(m_LayoutHeader.SubChunk1Size);
            //    foreach (var dataPoint in m_Data1)
            //        writer.Write(dataPoint);

            //    // Write the data2 chunk
            //    writer.Write(m_LayoutHeader.SubChunk2Id.ToCharArray());
            //    writer.Write(m_LayoutHeader.SubChunk2Size);
            //    foreach (var dataPoint in m_Data2)
            //        writer.Write(dataPoint);

            //    // Write the data3 chunk
            //    writer.Write(m_LayoutHeader.SubChunk3Id.ToCharArray());
            //    writer.Write(m_LayoutHeader.SubChunk3Size);
            //    foreach (var dataPoint in m_Data3)
            //        writer.Write(dataPoint);

            //    // Write the data4 chunk
            //    writer.Write(m_LayoutHeader.SubChunk4Id.ToCharArray());
            //    writer.Write(m_LayoutHeader.SubChunk4Size);
            //    foreach (var dataPoint in m_Data4)
            //        writer.Write(dataPoint);

            //    if (m_Meta != null)
            //        foreach (var dataPoint in m_Meta)
            //            writer.Write(dataPoint);
            //} 
            #endregion
        }

        public void Signed(string filePath)
        {

        }
    }
}
