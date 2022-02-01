using Dapper;
using Microsoft.Data.Sqlite;
using OneLineNotebook.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLineNotebook.DataAccess
{
    public class SqliteDatabaseAccess
    {
        public static List<NoteModel> LoadNotes()
        {
            using (IDbConnection conn = new SqliteConnection(LoadConnectionString()))
            {
                var output = conn.Query<NoteModel>("select * from NotesTable order by Id desc limit 20", new DynamicParameters()).ToList();
                output.Reverse();
                return output;
            };
        }

        public static void SaveNote(NoteModel note)
        {
            using (IDbConnection conn = new SqliteConnection(LoadConnectionString()))
            {
                conn.Execute("insert into NotesTable (Note, SearchWord, Date) values (@Note, @SearchWord, @Date)", note);
            }
        }

        private static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }
    }
}
