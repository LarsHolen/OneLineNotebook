﻿using OneLineNotebook.DataAccess;
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
        int noteCount = 0;
        int firstNote = 0;
        int lastNote = 0;
        int page = 0;
   
        public OneLineNotebookForm()
        {
            InitializeComponent(); 
            ActiveControl = textBox2;
            LoadNotes();
            LoadCount();
        }

        private void LoadCount()
        {
            noteCount = SqliteDatabaseAccess.CountNotes();
            firstNote = noteCount - notes.Count + (notes.Count * page);
            lastNote = noteCount - (notes.Count * page);
            string s = "Showing nr" + firstNote + " to " + lastNote;
            label1.Text = s;
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
            if(Screen.AllScreens.Length > 1)
            {
                this.Location = new Point(Screen.AllScreens[1].WorkingArea.Right - this.Width, Screen.AllScreens[1].WorkingArea.Top);
            }
            else
            {
                this.Location = new Point(Screen.FromPoint(this.Location).WorkingArea.Right - this.Width, 0);
            } 
        }


        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Todo Load", "Load notes");
           
        }

        /// <summary>
        /// On clicking Exit, close app
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void ListBox1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null) return;
            textBox2.Text = listBox1.SelectedItem.ToString();
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
                SqliteDatabaseAccess.SaveNote(n);
                textBox2.Text = "";
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
            notes.Clear();
            notes = SqliteDatabaseAccess.LoadTheseNotes(10,100);
            foreach(NoteModel note in notes)
            {
                Debug.WriteLine("Note:" + note);
            }
            Debug.WriteLine("Empty?"  + notes.Count);
            ShowNotes();
            //page++;
        }

        private void Page_Down_Click(object sender, EventArgs e)
        {
            notes.Clear();
            notes = SqliteDatabaseAccess.LoadTheseNotes(5, 0);
            ShowNotes();
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void LoadToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
