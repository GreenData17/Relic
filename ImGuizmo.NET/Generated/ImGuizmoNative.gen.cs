using System;
using System.Numerics;
using System.Runtime.InteropServices;
using ImGuiNET;

namespace ImGuizmoNET
{
    public static unsafe partial class ImGuizmoNative
    {
        private const string DLL_NAME = "cimguizmo";

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuizmo_AllowAxisFlip(byte value);
        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void BeginFrame();
        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuizmo_DecomposeMatrixToComponents(float* matrix, float* translation, float* rotation, float* scale);
        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuizmo_DrawCubes(float* view, float* projection, float* matrices, int matrixCount);
        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuizmo_DrawGrid(float* view, float* projection, float* matrix, float gridSize);
        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuizmo_Enable(byte enable);
        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern byte ImGuizmo_IsOverNil();
        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern byte ImGuizmo_IsOverOPERATION(OPERATION op);
        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern byte IsUsing();
        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern byte Manipulate(float* view, float* projection, OPERATION operation, MODE mode, float* matrix, float* deltaMatrix, float* snap, float* localBounds, float* boundsSnap);
        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuizmo_RecomposeMatrixFromComponents(float* translation, float* rotation, float* scale, float* matrix);
        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetDrawlist(ImDrawList* drawlist);
        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuizmo_SetGizmoSizeClipSpace(float value);
        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuizmo_SetID(int id);
        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetImGuiContext(IntPtr ctx);
        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetOrthographic(byte isOrthographic);
        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetRect(float x, float y, float width, float height);
        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuizmo_ViewManipulate(float* view, float length, Vector2 position, Vector2 size, uint backgroundColor);
    }
}
