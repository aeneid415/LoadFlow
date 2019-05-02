using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;

namespace LoadFlow
{
    public class Line
    {
        // I, J, CKT, STATUS, R, X, B2
        public int fromBus { get; set; }
        public int toBus { get; set; }
        public int status { get; set; }
        public int ckt { get; set; }
        public float R { get; set; }
        public float X { get; set; }
        public float B2 { get; set; }
        public Complex32 Z { get; set; }
        public Complex32 Y { get; set; }
    }
}
