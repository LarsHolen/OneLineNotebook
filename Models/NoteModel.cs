﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLineNotebook.Models
{
    public class NoteModel
    {
        public int Id { get; set; }
        public string Note { get; set; }
        public string SearchWord 
        { get
            {
                return Note.Split(" ")[0];
            } 
        }
        public string Date { get; set; }
    }
}