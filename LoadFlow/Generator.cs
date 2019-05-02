using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;

namespace LoadFlow
{
    public class Generator        
    {
        //I, ID, PG, QG, QT, QB, VS, IREG, MBASE, ZR, ZX, RT, XT, GTAP, STAT
        public int BusNo { get; set; }
        public int ID { get; set; }
        public float PG { get; set; }
        public float QG { get; set; }
        public float QT { get; set; }
        public float QB { get; set; }
        public float VS { get; set; }
        public int IREG { get; set; }
        public double MBASE { get; set; }
        public float ZR { get; set; }
        public float ZX { get; set; }
        public float RT { get; set; }
        public float XT { get; set; }
        public float GTAP { get; set; }
        public int Status { get; set; }
        public Complex32 Z { get; set; }
        public Complex32 Y { get; set; }
    }
}
