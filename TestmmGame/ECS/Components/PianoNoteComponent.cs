using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mmGameEngine;

namespace TestmmGame
{
    public class PianoNoteComponent : Component
    {
        //
        // entity for a piano key
        //
        public bool IsOn = false;
        public int NoteID = 60;             //mid C is 60

        public PianoNoteComponent()
        {

        }
    }
}
