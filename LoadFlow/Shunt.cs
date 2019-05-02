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
    public class Shunt
    {
        public int BusNo { get; set; }
        public string ID { get; set; }        
        public string Status { get; set; }
        public float GL { get; set; }
        public float BL { get; set; }
        public Complex32 Y { get; set; }
    }
}
