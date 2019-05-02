using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;

namespace LoadFlow
{
    public class Load
    {
        // I, ID, STATUS, AREA, ZONE, PL, QL
        private int mBusNo;
        public int BusNo { get { return this.mBusNo; } set { this.mBusNo = value; } }
        //public int BusNo { get; set; }
        public int ID {get;set;}
        public int Status { get; set; }
        public int Area { get; set; }
        public int Zone { get; set; }
        public float MW { get; set; }
        public float MVAR { get; set; }
        public Complex32 ZL { get; set; }
        public Complex32 YL { get; set; }
    }
}
