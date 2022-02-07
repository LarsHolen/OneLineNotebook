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

        /// <summary>
        /// Loads the 20 first Notes
        /// </summary>
        /// <returns></returns>
        public static List<NoteModel> LoadNotes()
        {
            using (IDbConnection conn = new SqliteConnection(LoadConnectionString()))
            {
                var output = conn.Query<NoteModel>("select * from NotesTable order by Id desc limit 20", new DynamicParameters()).ToList();
                output.Reverse();
                return output;
            };
        }

        /// <summary>
        /// Loads notes with pagesize and offset
        /// </summary>
        /// <param name="num"></param>
        /// <param name="off"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Count the total number of notes in DB
        /// </summary>
        /// <returns></returns>
        public static int CountNotes()
        {
            using (IDbConnection conn = new SqliteConnection(LoadConnectionString()))
            {
                var output = conn.Query<int>("select count(*) from NotesTable", new DynamicParameters()).ToList();
                return output.First();
            };
        }

        /// <summary>
        /// Save a new note to the DB
        /// </summary>
        /// <param name="note"></param>
        /// <returns></returns>
        public static int SaveNote(NoteModel note)
        {
            
            using IDbConnection conn = new SqliteConnection(LoadConnectionString());
            var result = conn.Execute("insert into NotesTable (Note, SearchWord, Date) values (@Note, @SearchWord, @Date); select last_insert_rowid();", note);
            return result;
        }

        /// <summary>
        /// Load the connection string from App.config
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }

        /// <summary>
        /// Delete note with ID.
        /// </summary>
        /// <param name="id"></param>
        internal static void DeleteNote(int id)
        {
            using IDbConnection conn = new SqliteConnection(LoadConnectionString());
            conn.Execute("delete from NotesTable where Id=@Id", new { Id = id });
        }


        /// <summary>
        /// Search the Searchword col for text 
        /// </summary>
        /// <param name="text"></param>
        /// <returns> return a list of Notes with the searchword</returns>
        internal static List<NoteModel> Search(string text)
        {
            using (IDbConnection conn = new SqliteConnection(LoadConnectionString()))
            {
                SqliteCommand com = (SqliteCommand)conn.CreateCommand();
                string t = String.Format("select * from NotesTable where SearchWord = @sword");
                DynamicParameters dp = new();
                dp.Add("@sword", text);
                var output = conn.Query<NoteModel>(t, dp).ToList();
                output.Reverse();
                
                return output;
            };
        }
    }
}
