﻿using Relic.Engine;
using System;
using System.Numerics;
using Relic.DataTypes;

namespace Relic.Editor
{
    public class GuiViewPort : Gui
    {
        private Vector2 viewportSize = new(1024, 768);
        public System.Numerics.Vector2 imageSize;

        public GuiViewPort() : base("ViewPort", true) { }

        public override void OnGui()
        {
            SetStyleVar(ImGuiStyleVar.WindowPadding, System.Numerics.Vector2.Zero);
            Begin("Viewport");
            DetectHovering();
            RemoveStyleVar();

            if (Window.mainCam is null) { End(); return; }
            if (viewportSize.X != GetContentRegionAvail().X || viewportSize.Y != GetContentRegionAvail().Y)
            {
                if(GetContentRegionAvail().X == Window.mainCam.bufferTexture.BufferSize.X) return;
                if(GetContentRegionAvail().X == Window.mainCam.bufferTexture.BufferSize.Y) return;

                viewportSize = new Vector2(GetContentRegionAvail().X, GetContentRegionAvail().Y);

                if (viewportSize.Y < GetNewBufferY()) ResizeX();
                else ResizeY();

            }

            if (viewportSize.Y < GetNewBufferY())
            {
                SolidLabel("##Space", new Vector2(GetXSpace(), 20), new Color(0));
                SameLine();
            }
            
            Image((IntPtr)Window.mainCam.bufferTexture.frameBufferName + 1, imageSize, new System.Numerics.Vector2(0, 1), new System.Numerics.Vector2(1, 0));

            End();
        }

        float GetNewBufferX() => (float)Window.mainCam.bufferTexture.BufferSize.X / (float)Window.mainCam.bufferTexture.BufferSize.Y *
                                 (float)GetContentRegionAvail().Y;

        float GetNewBufferY() => (float)Window.mainCam.bufferTexture.BufferSize.Y / (float)Window.mainCam.bufferTexture.BufferSize.X * 
                                 (float)GetContentRegionAvail().X;

        void ResizeX() => imageSize = new System.Numerics.Vector2(GetNewBufferX(), GetContentRegionAvail().Y);
        void ResizeY() => imageSize = new System.Numerics.Vector2(GetContentRegionAvail().X, GetNewBufferY());
        
        float GetXSpace() => (GetContentRegionAvail().X - GetNewBufferX()) / 2;
    }
}
