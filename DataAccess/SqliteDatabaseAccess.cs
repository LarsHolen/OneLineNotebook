using Dapper;
using Microsoft.Data.Sqlite;
using OneLineNotebook.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
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

        public static List<NoteModel> LoadTheseNotes(int num, int off)
        {
            using (IDbConnection conn = new SqliteConnection(LoadConnectionString()))
            {
                SqliteCommand com = (SqliteCommand)conn.CreateCommand();
                //com.CommandType = System.Data.CommandType.Text;
                //com.CommandText = String.Format("select * from NoteTable order by Id desc limit ({0}) offset ({1}) ", num, off);
                string t = String.Format("select * from NotesTable order by Id desc limit @nu offset @of ");
                DynamicParameters dp = new();
                dp.Add("@nu", num);
                dp.Add("@of", off);
                var output = conn.Query<NoteModel>(t, dp).ToList();
                //var output = conn.Query<NoteModel>("select * from NotesTable order by Id desc limit 20 offset 10", new DynamicParameters()).ToList();
                output.Reverse();
                return output;
            };

        }

        public static int CountNotes()
        {
            using (IDbConnection conn = new SqliteConnection(LoadConnectionString()))
            {
                var output = conn.Query<int>("select count(*) from NotesTable", new DynamicParameters()).ToList();
                return output.First();
            };
        }

        public static int SaveNote(NoteModel note)
        {
            
            using IDbConnection conn = new SqliteConnection(LoadConnectionString());
            var result = conn.Execute("insert into NotesTable (Note, SearchWord, Date) values (@Note, @SearchWord, @Date); select last_insert_row();", note);
            return result;
        }

        private static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }

        internal static void DeleteNote(int id)
        {
            using IDbConnection conn = new SqliteConnection(LoadConnectionString());
            conn.Execute("delete from NotesTable where Id=@Id", new { Id = id });
        }
    }
}
