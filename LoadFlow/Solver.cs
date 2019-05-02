using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;


namespace LoadFlow
{
    public class Solver
    {
        public int iterations { get; set; }
        public double tolerance { get; set; }

        public Vector<Complex32> GaussSolver(SystemArrays array)
        {
            #region "Initialize Vectors & Complex Numbers"
            Vector<Complex32> Vbus = Vector<Complex32>.Build.Dense(array.numBuses);
            Vector<Complex32> PQbus = Vector<Complex32>.Build.Dense(array.numBuses);

            double tolerance = 1.0e-6;

            Complex32 accel = new Complex32(1.6f, 0.0f); //remove
            Complex32 sumIRem = new Complex32(0.0f, 0.0f);
            Complex32 sumPVRem = new Complex32(0.0f, 0.0f);

            #endregion
            //assume that this is MakeYBus
            #region Call the method to Generate Vbus complex vector flat start nd fixed voltages
            Vbus=makeVBus(array);
            #endregion
            
            #region Calculate PQbus which defines net apparent scheduled power into each bus
            PQbus=makePQBus(array);
            #endregion
            
            #region "Iterate & Calculate Vbus at all buses"
            
            #region Initialize arrays & variables
            //make new array of PV bus fixed setpoint magnitudes
            Complex32[] VbusSet = new Complex32[array.numBuses];

            //generate a vector for the previous oteratopm calculated values of Vbus
            //to subtract from latest calculated, and compare to tolerance
            Vector<Complex32> VbusOld = Vector<Complex32>.Build.Dense(array.numBuses);
            Vector<Complex32> VbusDiff = Vector<Complex32>.Build.Dense(array.numBuses);

            //initialize Ybus
            array = InitializeYBus(array);

            //initialize the 'previous' Vbus values to the initial flat start values
            Vbus.CopyTo(VbusOld);

            //Define the maximum, absolute magnitude of bus voltage difference of all buses 
            //since previous iteration
            double diff = 1.0;
            #endregion

            while(diff > tolerance)
            {
                //SOLVE V for each bus once
                //parallel for(0,array.numBuses, 1=>
                for(int i=0;i<array.numBuses;i++)
                {
                    #region Solve for PQ (load) buses (Type 1)
                    if (array.bus[i].BusType==1)
                    {
                        //calculate all remote  bus current components
                        for (int j=0; j<array.numBuses;j++)
                        {
                            if(j==1) continue;
                            //sum of components at remote bus numbers greater than i
                            sumIRem += array.Ybus[i,j] * Vbus[j];
                        }
                        //calculate local bus current components
                        //Vbus[i] = (i/array.Ybus[i,i]) * ((PQbus[i].Conjugate() / Vbus[i].Conjugate()) - sumIRem);
                        Vbus[i] = (i / array.Ybus[i, i]) * ((PQbus[i] / Vbus[i].Conjugate()) - sumIRem);

                        //calculate acceleration and update Vbus with accelerated values
                        VbusDiff[i] = Vbus[i] - VbusOld[i];
                        Vbus[i] = VbusOld[i] + VbusDiff[i] * accel;
                    }
                    sumIRem=0;
                    sumPVRem=0;
                    #endregion

                    #region Solve for PV (generator) buses (Type 2)
                    if (array.bus[i].BusType == 2)
                    {
                        VbusSet[i] = Vbus[i].Magnitude;
                    
                        //calculate all remote bus current components
                        for (int j=0; j<array.numBuses; j++)
                        {
                            sumPVRem += array.Ybus[i,j] * Vbus[j];
                        }

                        //calculate first VARs at the local bus
                        float Qnew = -(Vbus[i].Conjugate() * sumPVRem).Imaginary;
                            
                        sumPVRem = 0;
                        //calculate local voltage usigng new value of Q
                        PQbus[i] = new Complex32(PQbus[i].Real,Qnew);

                        for (int j=0; j<array.numBuses; j++)
                        {
                            if (j==i) continue;
                            sumPVRem += array.Ybus[i,j] * Vbus[j];
                        }

                        Vbus[i] = (1/array.Ybus[i,i]) * ((PQbus[i].Conjugate()) - sumPVRem);
                        //force Vbus[i] magnitude to voltage setpoint, and angle to newly calculated Vbus[i] voltage
                        Vbus[i] = VbusSet[i] * Vbus[i]/Vbus[i].Magnitude;
                    }
                    #endregion

                
                    //goto next bus
                    sumIRem=0;
                    sumPVRem=0;
                }
            
                //ALL BUS voltages have been calculated for this iteration
                //start next iteration (k)
                iterations++;

                //give the magnitude of greatest value in VbusDiff vector
                diff=VbusDiff.AbsoluteMaximum().Real;

                //note: you cannot set one vector equal to the other or they will be considered the same
                //You must instead copy the values from one to the other
                //copy the latest Vbus Vector elements to VbusOld vector
                Vbus.CopyTo(VbusOld);
            }
            #endregion

            return Vbus;
        }  

        
        public Vector<Complex32> makeVBus(SystemArrays array)
        {
            //BUS TYPES
            //TYPE 1 = PQ Bus (load bus)
            //Type 2 = PV Bus (generator bus)
            //Type 3 = Slack Bus
            //NOTE: IF PQ Bus, provide default voltage which is 1.0+j0; ELSE provide the given Voltage + j0; 

            Vector<Complex32> thisVbus = Vector<Complex32>.Build.Dense(array.numBuses);
            for (int i = 0; i < array.numBuses; i++)
            {
                Complex32 defaultV = new Complex32(1.0F, 0.0F);
                //if PQBus
                if (array.bus[i].BusType == 1) 
                {
                    thisVbus[i] = defaultV;                    
                }
                else
                //PV bus
                {
                    if (array.bus[i].BusType == 2)
                    {
                        //if PV bus
                        //find the generator number that corresponds to the bus number
                        int b=getBusNumberOfGenerator(array, array.bus[i].BusNo);
                        float real = array.gen[b].VS;
                        Complex32 givenV = new Complex32(real, 0.0F);
                        thisVbus[i] = givenV;
                    }                    
                }
            }
            return thisVbus;
        }

        public Vector<Complex32> makePQBus(SystemArrays array)
        {
            Vector<Complex32> thisPQbus = Vector<Complex32>.Build.Dense(array.numBuses);
            for (int i = 0; i < array.numBuses; i++)
            {
                float real = 0.0F;
                float imaginary = 0.0F;
                if (array.bus[i].BusType == 1) //PQBus
                {                  
                    //NOTE : THIS IS ONLY INJECTED LOAD...wala pang drawn load
                    real = array.load[i].MW;
                    imaginary = array.load[i].MVAR;
                    Complex32 PQ = new Complex32(real, imaginary);
                    thisPQbus[i] = PQ;
                }
                else
                {
                    real = array.load[i].MW;
                    imaginary = 0.0F;
                    Complex32 PQ = new Complex32(real, imaginary);
                    thisPQbus[i] = PQ;
                }
            }
            return thisPQbus;
        }

        private int getBusNumberOfGenerator(SystemArrays array, int busNo)
        {
            int retVal = 0;
            for (int i = 0; i < array.numGens; i++)
            {
                if (array.gen[i].BusNo == busNo)
                {
                    retVal=i;
                    i=array.numGens;
                }
            }
            return retVal;
        }

        private SystemArrays InitializeYBus(SystemArrays array)
        {
            
            for (int i = 0; i < array.numBuses; i++)
            {
                for (int j = 0; j < array.numBuses; j++)
                {
                    if (i == j)
                    {
                        //diagonal matrix, solve for total admittance
                        array.Ybus[i,j] = TotalAdmittance(i, array);
                    }
                    else
                    {
                        //off diagonal
                        int index = GetLineIndex(i, j, array);
                        Complex32 y = new Complex32(array.line[index].R, array.line[index].X);
                        array.Ybus[i, j] = y;
                    }
                }
            }
            return array;
        }

        private Complex32 TotalAdmittance(int bus, SystemArrays array)
        {            
            Complex32 retVal = new Complex32(0.0F, 0.0F);
            for (int i = 0; i < array.numBuses; i++)
            {
                if (array.line[i].fromBus == bus || array.line[i].toBus == bus)
                {
                    Complex32 temp = new Complex32(array.line[i].R, array.line[i].X);
                    retVal += (1 / temp);
                }
            }
            return retVal;
        }

        private int GetLineIndex(int fromBus, int toBus, SystemArrays array) 
        {
            int retVal=0;
            for (int i = 0; i < array.numBuses; i++)
            {
                if (array.line[i].fromBus == fromBus && array.line[i].toBus == toBus)
                {
                    retVal = i;
                    i = array.numBuses;
                }
            }
            return retVal;
        }
        

    }
}
