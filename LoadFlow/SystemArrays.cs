using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;

namespace LoadFlow
{
    public class SystemArrays
    {
        
        public Matrix<Complex32> Ybus { get; set; }
        public Matrix<Complex32> Y { get; set; }
        public Matrix<Complex32> Z { get; set; }

        //public CaseID caseId { get; set; }
        
        public Bus[] bus { get; set;  }
        public Generator[] gen {get;set;}
        public Line[] line {get;set;}
        public Transformer[] trans {get;set;}
        public Load[] load {get;set;}
        public Shunt[] shunt { get; set; }

        public int[] numElements { get; set; }
        public int numBuses { get; set; }
        public int numLoads { get; set; }
        public int numShunts { get; set; }
        public int numGens { get; set; }
        public int numLines { get; set; }
        public int numTrans { get; set; }

        public double MVABase { get; set; }

        //lists
        public List<string> strBuses { get; set; }
        public List<string> strGenerators { get; set; }
        public List<string> strLines { get; set; }
        public List<string> strTransformers { get; set; }
        public List<string> strLoads { get; set; }
        public List<string> strShunts { get; set; }

    }
}
