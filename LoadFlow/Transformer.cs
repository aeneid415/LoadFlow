using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;

namespace LoadFlow
{
    public class Transformer
    {       
        //fromBus, toBus, CKT, STATUS, MVA, Z
        public int fromBus { get; set; }
        public string toBus { get; set; }
        public string ckt { get; set; }
        public string Status { get; set; }

        //Note: BusKV and IDE can only be certain values;
        public double MVA { get; set; }
        public Complex32 Z { get; set; }
    }
}
