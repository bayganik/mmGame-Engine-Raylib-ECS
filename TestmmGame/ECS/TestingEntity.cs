using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Entitas;
using mmGameEngine;
using Raylib_cs;

namespace TestmmGame
{
    public class TestingEntity : Entity
    {
        public TestingEntity() 
        {
            Entity ent1 = Global.CreateGameEntity(new Vector2(100, 100));
            ent1.Name = "Text";
            Text gdc = new Text("Kamran says, stay with tank", TextFontTypes.Default);
            gdc.FontSize = 20;
            gdc.TextData.FontColor = Color.DarkBlue;
            gdc.RenderLayer = 10;
            ent1.AddComponent(gdc);
            var t = ent1.GetComponent<Text>();
            t.FontSize = 25;
        }
        public void EntityComponentChanged(IEntity entity, int index, IComponent component) 
        { 
        }
    }
}
