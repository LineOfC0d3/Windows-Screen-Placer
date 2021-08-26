using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Windows_Screen_Placer
{
    class ProcessWithPlacement
    {
        public Process Executeable { get; set; }
        public int X {get; set;}
        public int Y { get; set; }
        public int Width { get; set; }
        public int Heigth { get; set; }
    }
}
