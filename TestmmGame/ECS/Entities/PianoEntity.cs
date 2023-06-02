using Entitas;
using mmGameEngine;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TestmmGame
{
    public class PianoEntity
    {
        /*
         * Each piano key is an Entity
         *      Entity.Tag is the note value to play
         * Each key has PianoNoteComponent
         * Each key has BoxCollider (which collides with cursor)
         */
        const int White_Key_Layer = -8;
        const int Black_Key_Layer = -7;
        //
        // Piano with 85 keys
        //
        int whitekeywidth = 0;
        int blackkeywidth = 0;
        int MaxKeys = 85;

        int MinNote = 24;
        int MaxNote = 99;

        float xpos;
        string assigned_Keyboard;
        int assigned_note;
        public PianoEntity(Vector2 PianoPos)
        {

            int note_offset = 0;
            assigned_note = MinNote - 1;
            whitekeywidth = Content.keyWhite.width;
            blackkeywidth = Content.keyBlack.width;
            //
            // White keys
            //
            for (int i = 0; i < MaxKeys; i++)
            {
                int m = (i / 12);
                int n = i % 12;
                int key_offset = -1;
                //
                // notes and keyboard assignment
                //
                switch (n)
                {
                    case 0:
                        key_offset = 0;                                         //key of C
                        assigned_note = assigned_note + 1;
                        assigned_Keyboard = "A";
                        break;
                    case 2:
                        key_offset = 1;                                         //key of D
                        assigned_note = assigned_note + 2;
                        assigned_Keyboard = "S";
                        break;
                    case 4:
                        key_offset = 2;                                         //key of E
                        assigned_note = assigned_note + 2;
                        assigned_Keyboard = "D";
                        break;
                    case 5:
                        key_offset = 3;                                         //key of F
                        assigned_note = assigned_note + 1;
                        assigned_Keyboard = "F";
                        break;
                    case 7:
                        key_offset = 4;                                         //key of G
                        assigned_note = assigned_note + 2;
                        assigned_Keyboard = "J";
                        break;
                    case 9:
                        key_offset = 5;                                         //key of A
                        assigned_note = assigned_note + 2;
                        assigned_Keyboard = "K";
                        break;
                    case 11:
                        key_offset = 6;                                         //key of B
                        assigned_note = assigned_note + 2;
                        assigned_Keyboard = "L";
                        break;
                }
                //----------------------------------------------------
                // Create WHITE key 
                //----------------------------------------------------
                if (key_offset >= 0)
                {
                    xpos = PianoPos.X + (key_offset + m * 7) * whitekeywidth;
                    Entity pkey = Global.CreateGameEntity("pkey" + assigned_note.ToString(),
                                                          new Vector2(xpos, PianoPos.Y));
                    pkey.Tag = assigned_note;

                    Sprite spr = new Sprite(Content.keyWhite);
                    spr.RenderLayer = White_Key_Layer;

                    PianoNoteComponent pcomp = new PianoNoteComponent();
                    pcomp.IsOn = false;
                    pcomp.NoteID = assigned_note;

                    pkey.AddComponent(spr);
                    pkey.AddComponent(pcomp);
                    //
                    // Each key gets a label (Keyboard key value)
                    //
                    Entity ent1 = Global.CreateGameEntity("plbl" + assigned_note.ToString(), new Vector2(-5, 100));
                    ent1.IsVisible = false;
                    Label lbl = new Label(assigned_Keyboard);
                    lbl.TextData.FontSize = 15;
                    lbl.TextData.FontColor = Color.BLACK;
                    ent1.AddComponent<PianoNoteLabelComponent>();
                    ent1.Get<TransformComponent>().Parent = pkey.Get<TransformComponent>();
                    ent1.AddComponent(lbl);
                    //
                    // stationary BoxCollider (for mouse event)
                    //
                    pkey.AddComponent(new BoxCollider( Content.keyWhite.width, Content.keyWhite.height));

                }
            }
            //
            // black keys
            //
            assigned_note = MinNote;
            int offset = 0;
            for (int i = 0; i < MaxKeys; i++)
            {
                int m = (i / 12);
                int n = i % 12;
                int key_offset = -1;
                //
                // notes and keyboard assignment
                //
                switch (n)
                {
                    case 1:
                        key_offset = 0;                                         //key of C#
                        assigned_note = assigned_note + 1 + offset;
                        assigned_Keyboard = "W";
                        break;
                    case 3:
                        key_offset = 1;                                         //key of D#
                        assigned_note = assigned_note + 2;
                        assigned_Keyboard = "E";
                        break;
                    case 6:
                        key_offset = 3;                                         //key of F#
                        assigned_note = assigned_note + 3;
                        assigned_Keyboard = "U";
                        break;
                    case 8:
                        key_offset = 4;                                         //key of G#
                        assigned_note = assigned_note + 2;
                        assigned_Keyboard = "I";
                        break;
                    case 10:
                        key_offset = 5;                                         //key of A#
                        assigned_note = assigned_note + 2;
                        assigned_Keyboard = "O";
                        offset = 2;
                        break;
                }
                //----------------------------------------------------
                // Create BLACK key 
                //----------------------------------------------------
                if (key_offset >= 0)
                {
                    xpos = (PianoPos.X + ((key_offset + m * 7) * whitekeywidth) + (whitekeywidth - blackkeywidth / 2)) - (blackkeywidth / 2);
                    Entity pkey = Global.CreateGameEntity("pkey" + assigned_note.ToString(),
                                                          new Vector2(xpos, PianoPos.Y - (Content.keyBlack.height/2 - 20)));
                    pkey.Tag = assigned_note;

                    Sprite spr = new Sprite(Content.keyBlack);
                    spr.RenderLayer = Black_Key_Layer;

                    PianoNoteComponent pcomp = new PianoNoteComponent();
                    pcomp.IsOn = false;
                    pcomp.NoteID = assigned_note;

                    pkey.AddComponent(spr);
                    pkey.AddComponent(pcomp);
                    //
                    // Each key gets a label (Keyboard key value)
                    //
                    Entity ent1 = Global.CreateGameEntity("plbl" + assigned_note.ToString(), new Vector2(-5, -70));
                    ent1.IsVisible = false;
                    Label lbl = new Label(assigned_Keyboard);
                    lbl.TextData.FontSize = 15;
                    lbl.TextData.FontColor = Color.BLACK;
                    ent1.AddComponent<PianoNoteLabelComponent>();
                    ent1.Get<TransformComponent>().Parent = pkey.Get<TransformComponent>();
                    ent1.AddComponent(lbl);
                    //
                    // stationary BoxCollider 
                    //
                    pkey.AddComponent(new BoxCollider( Content.keyBlack.width, Content.keyBlack.height));
                }
            }

        }
    }
}
