using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SQLite;

namespace ShowComposer.Data
{
    public class SQLiteHelper
    {
        public const string SQLITE_LIB_NAME = "System.Data.SQLite.dll";
        private static bool ISRESOLVED = false;
        static string CREATE_TABLE_WAVE = "CREATE TABLE wave (" +
                                           "fid         INTEGER PRIMARY KEY NOT NULL, " +
                                           "min         BLOB, " +
                                           "max         BLOB, " +
                                           "rms         BLOB, " +
                                           "channels    INT, " +
                                           "compression INT, " +
                                           "FOREIGN KEY ( fid ) REFERENCES file (fid));";

        static string CREATE_TABLE_FILE = "CREATE TABLE file (" +
                                          "fid      INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, " +
                                          "location TEXT    NOT NULL, " +
                                          "subsong  INTEGER NOT NULL, " +
                                          "UNIQUE (location,subsong)); ";

        public static void InitializeDatabase(string dbFile)
        {
            ResolveSqliteAssembly();

            if (!File.Exists(dbFile))
            {
                MakeDatabase(dbFile);
            }
        }

        private static void ResolveSqliteAssembly()
        {
            if (ISRESOLVED)
                return;

            AppDomain currentDomain = AppDomain.CurrentDomain;

            currentDomain.AssemblyResolve += new ResolveEventHandler((object sender, ResolveEventArgs args) => {
                
                if (!args.Name.StartsWith(Properties.Settings.Default.SqlitePackage))
                    return args.RequestingAssembly;

                var sqliteLibDirectory = "";
                var currentAssembly = System.Reflection.Assembly.GetEntryAssembly();
                var currentDirectory = new System.IO.FileInfo(currentAssembly.Location).DirectoryName;

                if (IntPtr.Size == 4)
                    sqliteLibDirectory = System.IO.Path.Combine(currentDirectory, @"sqlite\x86", SQLITE_LIB_NAME);
                else
                    sqliteLibDirectory = System.IO.Path.Combine(currentDirectory, @"sqlite\x64", SQLITE_LIB_NAME);

                System.Reflection.Assembly assembly = System.Reflection.Assembly.LoadFrom(sqliteLibDirectory);
                return assembly;
            });

            ISRESOLVED = true;
        }

        static void MakeDatabase(string dbFile)
        {
            try
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
            catch (Exception sQLiteException)
            {
                System.Windows.MessageBox.Show(String.Format("sQLiteException {0}", sQLiteException.Message), "Error Initializing database");
            }
        }

        public static byte[] GetBytes(SQLiteDataReader reader, int field)
        {
            const int CHUNK_SIZE = 2 * 1024;
            byte[] buffer = new byte[CHUNK_SIZE];
            long bytesRead;
            long fieldOffset = 0;
            using (MemoryStream stream = new MemoryStream())
            {
                while ((bytesRead = reader.GetBytes(field, fieldOffset, buffer, 0, buffer.Length)) > 0)
                {
                    stream.Write(buffer, 0, (int)bytesRead);
                    fieldOffset += bytesRead;
                }
                return stream.ToArray();
            }
        }
    }
}
