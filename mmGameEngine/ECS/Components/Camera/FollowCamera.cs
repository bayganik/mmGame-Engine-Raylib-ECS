using System;
using System.Collections.Generic;
using System.Text;
using Raylib_cs;

namespace mmGameEngine
{
    public class FollowCamera : Component
    {
        Rectangle cameraBox;
        public FollowCamera(Rectangle _cameraBox)
        {
            cameraBox = _cameraBox;
        }
        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            if (CompEntity == null)
                return;

        }

    }
}
