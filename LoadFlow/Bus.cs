using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
//using CsvHelper;
namespace LoadFlow
{
    public class Bus
    {
        // I, 'NAME', BASKV, IDE, AREA, ZONE, OWNER, VR, VA
        private int mBusNo=0;
        private string mName = "";
        private float mBusKV = 0.0F;
        private int mBusType = 0;
        //public int BusNo { get; set; }
        public int BusNo { get { return this.mBusNo; } set { this.mBusNo = value; } }
        public string Name { get { return this.mName; } set { this.mName = value; } }

        //Note: BusKV and IDE can only be certain values;
        public float BusKV { get { return this.mBusKV; } set { this.mBusKV = value; } }
        public int BusType { get { return this.mBusType; } set { this.mBusType = value; } }        
    }
}
