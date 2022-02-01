using OneLineNotebook.DataAccess;
using OneLineNotebook.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OneLineNotebook
{
    public partial class OneLineNotebookForm : Form
    {
        List<NoteModel> notes = new();
        public OneLineNotebookForm()
        {
            InitializeComponent();
            ActiveControl = textBox2;
            LoadNotes();
        }
        /// <summary>
        /// Loading notes from sqlite
        /// Currently mocked list
        /// </summary>
        private void LoadNotes()
        {
            /*
            notes.Add(new NoteModel {Id = 0, Message = "Mitt første notat", SearchWord = "Mitt", Date = DateTime.Now.ToString()});
            notes.Add(new NoteModel { Id = 0, Message = "Mitt andre notat", SearchWord = "Mitt", Date = DateTime.Now.ToString()});
            notes.Add(new NoteModel { Id = 0, Message = "Mitt tredje notat", SearchWord = "Mitt", Date = DateTime.Now.ToString()});
            notes.Add(new NoteModel { Id = 0, Message = "Mitt fjerde notat", SearchWord = "Mitt", Date = DateTime.Now.ToString()});
            notes.Add(new NoteModel { Id = 0, Message = "Mitt femte notat", SearchWord = "Mitt", Date = DateTime.Now.ToString()});
            */
            notes = SqliteDatabaseAccess.LoadNotes();
            
            ShowNotes();
        }
        /// <summary>
        /// Showing notes from the noteslist in textbox
        /// </summary>
        private void ShowNotes()
        {
            foreach (NoteModel note in notes)
            {
                textBox1.AppendText(note.Note + "\r\n");
            }
            foreach (NoteModel note in notes)
            {
                Debug.WriteLine(note.Note);
            }


        }


        // textBox.Font = new Font(textBox.Font.FontFamily, 16);


        private void Form1_Load(object sender, EventArgs e)
        {

        }


        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Todo Load", "Load notes");
           
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                NoteModel n = new() { Note = textBox1.Lines[^1], Date = DateTime.Now.ToString() };
                notes.Add(n);
                SqliteDatabaseAccess.SaveNote(n);
            }
        }
    }
}
