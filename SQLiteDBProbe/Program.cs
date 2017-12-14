using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteDBProbe
{
    class Program
    {
        static string dbFile = "waveform.db3";
        static string CREATE_TABLE_WAVE = "CREATE TABLE wave ("+
                                           "fid         INTEGER PRIMARY KEY NOT NULL, "+
                                           "min         BLOB, "+
                                           "max         BLOB, "+
                                           "rms         BLOB, "+
                                           "channels    INT, "+
                                           "compression INT, "+
                                           "FOREIGN KEY ( fid ) REFERENCES file (fid));";

        static string CREATE_TABLE_FILE = "CREATE TABLE file (" +
                                          "fid      INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, " +
                                          "location TEXT    NOT NULL, " +
                                          "subsong  INTEGER NOT NULL, " +
                                          "UNIQUE (location,subsong)); ";
        static void Main(string[] args)
        {

            if (!File.Exists(dbFile))
            {
                MakeDatabase();
            }


            DbProviderFactory fact = DbProviderFactories.GetFactory("System.Data.SQLite");
            using (DbConnection cnn = fact.CreateConnection())
            {
                cnn.ConnectionString = "Data Source=" + dbFile;
                cnn.Open();
            }



        }

        static void MakeDatabase()
        {
            using (var connection = new SQLiteConnection("Data Source=" + dbFile + ";Version=3"))
            {
                connection.Open();
                using (var command = new SQLiteCommand(CREATE_TABLE_WAVE, connection))
                {
                    command.ExecuteNonQuery();
                }
                using (var command = new SQLiteCommand(CREATE_TABLE_FILE, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        static byte[] GetBytes(SQLiteDataReader reader)
        {
            const int CHUNK_SIZE = 2 * 1024;
            byte[] buffer = new byte[CHUNK_SIZE];
            long bytesRead;
            long fieldOffset = 0;
            using (MemoryStream stream = new MemoryStream())
            {
                while ((bytesRead = reader.GetBytes(0, fieldOffset, buffer, 0, buffer.Length)) > 0)
                {
                    stream.Write(buffer, 0, (int)bytesRead);
                    fieldOffset += bytesRead;
                }
                return stream.ToArray();
            }
        }
    }
}
