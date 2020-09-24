using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;

using Entitas;

namespace mmGameEngine
{
    public class mmEntity
    {
        public RenderComponent ComponentToRender;
        public bool Renderable;
        public mmEntity()
        {
            Initialize();
        }
        private void Initialize()
        {
            ComponentToRender = null;
            Renderable = false;
        }
        public T AddComponent<T>(T entComponent) where T : IComponent
        {
            if (entComponent is RenderComponent)
            {
                Renderable = true;
                ComponentToRender = entComponent as RenderComponent;

            }

            return entComponent;
        }
    }
}
