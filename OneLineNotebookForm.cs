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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OneLineNotebook
{
    public partial class OneLineNotebookForm : Form
    {
        List<NoteModel> notes = new();


        int offset = 0;
        readonly int pageSize = 20;
        int countNotes = 0;

        public OneLineNotebookForm()
        {
            InitializeComponent();
            ActiveControl = textBox2;
            LoadNotes();
            LoadCount();
        }

        private void LoadCount()
        {
            countNotes = SqliteDatabaseAccess.CountNotes();
        }



        /// <summary>
        /// Loading notes from sqlite
        /// Currently mocked list
        /// </summary>
        private void LoadNotes()
        {
            notes = SqliteDatabaseAccess.LoadNotes();
            ShowNotes();
        }

        /// <summary>
        /// Showing notes from the noteslist in textbox
        /// Setting it to null first, so it is refreshed when called later.
        /// </summary>
        private void ShowNotes()
        {
            listBox1.DataSource = null;
            listBox1.DataSource = notes;
            label1.Text = (offset / 20 + 1).ToString();
        }


        // textBox.Font = new Font(textBox.Font.FontFamily, 16);

        /// <summary>
        /// Onload code.  
        /// 1. Positioning the app on screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            // Tests if you have multiple screens.
            // If you do, app opens in top right corner of second screen,
            // else it open in top right corner.
            // Because I want my OneLineNotebook out of the way :P
            if (Screen.AllScreens.Length > 1)
            {
                this.Location = new Point(Screen.AllScreens[1].WorkingArea.Right - this.Width, Screen.AllScreens[1].WorkingArea.Top);
            }
            else
            {
                this.Location = new Point(Screen.FromPoint(this.Location).WorkingArea.Right - this.Width, 0);
            }
        }


        private void ListBox1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null) return;
            textBox2.Text = listBox1.SelectedItem.ToString() + "\t" + notes[listBox1.SelectedIndex].Date;
            ActiveControl = textBox2;
        }


        /// <summary>
        /// Saving note when one use enter in textBox2
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(textBox2.Text, @"\w") == false) return;
                NoteModel n = new() { Note = textBox2.Lines[^1], Date = DateTime.Now.ToString() };
                notes.Add(n);
                ShowNotes();
                Debug.WriteLine("New ID: " + n.Id);
                n.Id = SqliteDatabaseAccess.SaveNote(n);
                textBox2.Text = "";
                Debug.WriteLine("New ID: " + n.Id);
            }
        }

        /// <summary>
        /// Deleting the selected note
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonDelete_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem is NoteModel)
            {
                NoteModel toBeRemoved = listBox1.SelectedItem as NoteModel;
                Debug.WriteLine(toBeRemoved, toBeRemoved.Id.ToString());
                Debug.WriteLine(toBeRemoved.Id);
                textBox2.Text = "";
                notes.Remove(toBeRemoved);
                SqliteDatabaseAccess.DeleteNote(toBeRemoved.Id);
                ShowNotes();
            }
            ActiveControl = textBox2;
        }

        private void Page_Up_Click(object sender, EventArgs e)
        {
            offset += 20;
            if (offset > countNotes) offset -= 20;
            notes.Clear();
            notes = SqliteDatabaseAccess.LoadTheseNotes(pageSize, offset);
            foreach (NoteModel note in notes)
            {
                Debug.WriteLine("Note:" + note);
            }
            Debug.WriteLine("Empty?" + notes.Count);
            ShowNotes();
            label1.Text = (offset / 20 + 1).ToString();
        }

        private void Page_Down_Click(object sender, EventArgs e)
        {
            offset -= 20;
            if (offset < 0) offset = 0;
            notes.Clear();
            notes = SqliteDatabaseAccess.LoadTheseNotes(pageSize, offset);
            ShowNotes();
            label1.Text = (offset / 20 + 1).ToString();
        }



        private void Textbox1_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (e.KeyChar == (char)13)
            {
                Debug.WriteLine(".!.");
                Debug.WriteLine("." + textBox1.Text + ".");
                if (System.Text.RegularExpressions.Regex.IsMatch(textBox1.Text, @"\w") == false) return;
                textBox1.Text = Regex.Replace(textBox1.Text, @"\t|\n|\r", "");
                notes = SqliteDatabaseAccess.Search(textBox1.Text);
                Debug.WriteLine(notes.Count);
                textBox1.Text = "";
                ShowNotes();
            }
        }

        private void HelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            const string message = "Type your one line note in the bottom textfield.  Enter saves and clears the message.  \nThe 20 last notes are shown on " +
                "screen.  Use page up/down to see other notes.  \nSearch for notes by the first word in the note, so use keywords for the first word in your notes!" +
                "\nClick on a note and click delete to remove notes from the book. \nEnjoy! \nMade by Lars Holen.";
            const string caption = "Help";
            MessageBox.Show(message, caption, MessageBoxButtons.OK);

        }

        private void Button_reload_Click(object sender, EventArgs e)
        {
            ActiveControl = textBox2;
            offset = 0;
            countNotes = 0;
            LoadNotes();
            LoadCount();
            ShowNotes();
        }

        private void TextBox1_Enter(object sender, EventArgs e)
        {
            Debug.WriteLine("fire");
            textBox1.Text = "";
        }

    }
}
