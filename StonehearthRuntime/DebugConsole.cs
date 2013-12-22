using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace StonehearthRuntime
{
    public class DebugConsole : MonoBehaviour
    {
        private Rect mWindowRect = new Rect(20, 20, Screen.width - 40, (int)(Screen.height * 0.2));
        private Vector2 mScrollPosition = new Vector2();
        private List<string> mLines = new List<string>();

        public void Update()
        {
        }

        public void OnGUI()
        {
            GUILayout.Window(0xFFFF, mWindowRect, CreateDebugConsoleWindow, "Debug Console");
        }

        private void CreateDebugConsoleWindow(int pWindowID)
        {
            mScrollPosition = GUILayout.BeginScrollView(mScrollPosition);
            mLines.ForEach(s => GUILayout.Label(s));
            GUILayout.EndScrollView();
        }

        public void WriteLine(string pFormat, params object[] pArgs)
        {
            mLines.Insert(0, string.Format(pFormat, pArgs));
            while (mLines.Count > 1000) mLines.RemoveAt(mLines.Count - 1);
        }
    }
}
