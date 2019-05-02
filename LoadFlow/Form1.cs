using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Numerics;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;

namespace LoadFlow
{
    public partial class Form1 : Form
    {
        public static SystemArrays sysArrayMain = new SystemArrays();
        
        public Form1()
        {
            InitializeComponent();
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {




            /* disregard CSV READING for a while
            String fileContents = "";
            //read a csv file
            openFileDialog1.Filter = "CSV Files | *.csv";
            openFileDialog1.Title = "Select a CSV File";
            openFileDialog1.FileName = "";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                StreamReader sr = new StreamReader(openFileDialog1.FileName);
                fileContents = sr.ReadToEnd();
                sr.Close();
                sysArrayMain = parseCSVcontents(fileContents);
                Solver solver = new Solver();
                Vector<Complex32> Vbus =  solver.GaussSolver(sysArrayMain);
                txtResult.Text = Vbus.ToString();
            }
            */


        }

        private SystemArrays parseCSVcontents(string fileContents)
        {
            //local system array var
            SystemArrays sysArray = new SystemArrays();

            //string to parse
            string[] elements = fileContents.Split('@');

            //counter to mark each category: 1=Bus Data, 2=Load Data, 3=Shunt Data, 4=Generator Data, 5=Line Data, 6=Transformer DAta
            int ctr = 0;
            foreach (string element in elements)
            {
                ctr++;
                #region Parse Bus Data
                if (ctr == 1)
                {
                    //first element is Bus Data
                    
                    //split to get rows of buses
                    string[] busRows = element.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                    
                    //initialize system array as to the number of buses
                    sysArray.numBuses = busRows.Length;
                    sysArray.bus = new Bus[busRows.Length - 1];
                    

                    //parse array of bus rows
                    int i = -1;
                    foreach (string busRow in busRows)
                    {
                        if (busRow != "")
                        {
                            //initialize index marker
                            i++;

                            //get busData values from busRow
                            string[] busData = busRow.Split(',');

                            //parse each busData Value and save to system Array
                            //by following the order: I, 'NAME', BASKV, IDE, AREA, ZONE, OWNER, VR, VA

                            //firs element is BusNo
                            sysArray.bus[i].BusNo = Convert.ToInt32(busData[0]);

                            //2nd element is BusName                        
                            sysArray.bus[i].Name = busData[1];

                            //3rd element is KVA
                            sysArray.bus[i].BusKV = Convert.ToInt32(busData[2]);

                            //4th element is IDE -- busType?
                            sysArray.bus[i].BusType = Convert.ToInt32(busData[3]);

                            //skip elements 5th and 6th - irrelevant at this time

                            //7th element is VR 
                            //sysArray.bus[i]. = Convert.ToSingle(busData[6]);

                            //8th element is VA 
                            //sysArray.bus[i]. = Convert.ToSingle(busData[7]);
                        }                        
                    }
                }
                #endregion

                #region Parse Load Data
                else if(ctr==2)
                {
                    //second element is Load Data

                    //split to get rows of Loads
                    string[] loadRows = element.Split(new string[] { "\\n" }, StringSplitOptions.None);

                    //initialize system array as to the number of buses
                    sysArray.numLoads = loadRows.Length;
                    sysArray.load = new Load[loadRows.Length - 1];
                    //parse array of load rows
                    int i = -1;
                    foreach (string loadRow in loadRows)
                    {
                        //initialize index marker
                        i++;

                        //get loadData values from loadRow
                        string[] loadData = loadRow.Split(',');

                        //parse each loadData Value and save to system Array
                        //by following the order: I, ID, STATUS, AREA, ZONE, PL, QL

                        //firs element is load BusNo
                        sysArray.load[i].BusNo = Convert.ToInt32(loadData[0]);

                        //2nd element is ID                        
                        sysArray.load[i].ID = Convert.ToInt32(loadData[1]);

                        //3rd element is STATUS
                        sysArray.load[i].Status = Convert.ToInt32(loadData[2]);

                        //4th element is Area
                        sysArray.load[i].Area = Convert.ToInt32(loadData[3]);

                        //5th element is Zone
                        sysArray.load[i].Zone = Convert.ToInt32(loadData[4]);

                        //6th element is PL
                        sysArray.load[i].YL = Convert.ToInt32(loadData[5]);

                        //7th element is QL 
                        sysArray.load[i].ZL = Convert.ToInt32(loadData[6]);                        
                    }
                }
                #endregion

                #region Parse Shunt Data
                else if (ctr == 3)
                {
                    //3rd element is Shunt Data
                    //do nothing                    
                }
                #endregion

                #region Parse Generator Data
                else if (ctr == 4)
                {
                    //first element is Generator Data

                    //split to get rows of generators
                    string[] genRows = element.Split(new string[] { "\\n" }, StringSplitOptions.None);

                    //initialize system array as to the number of generators
                    sysArray.numGens = genRows.Length;
                    sysArray.gen = new Generator[genRows.Length - 1];
                    //parse array of gen rows
                    int i = -1;
                    foreach (string genRow in genRows)
                    {
                        //initialize index marker
                        i++;

                        //get genData values from genRow
                        string[] genData = genRow.Split(',');

                        //parse each genData Value and save to system Array
                        //by following the order: I, ID, PG, QG, QT, QB, VS, IREG, MBASE, ZR, ZX, RT, XT, GTAP, STAT

                        //firs element is BusNo
                        sysArray.gen[i].BusNo = Convert.ToInt32(genData[0]);

                        //2nd element is ID                        
                        sysArray.gen[i].ID = Convert.ToInt32(genData[1]);

                        //3rd element is PG
                        sysArray.gen[i].PG = Convert.ToInt32(genData[2]);

                        //4th element is QG
                        sysArray.gen[i].QG = Convert.ToInt32(genData[3]);

                        //5th element is QT
                        sysArray.gen[i].QT = Convert.ToInt32(genData[4]);

                        //6th element is QB
                        sysArray.gen[i].QB = Convert.ToInt32(genData[5]);

                        //7th element is VS
                        sysArray.gen[i].VS = Convert.ToInt32(genData[6]);

                        //8th element is IREG
                        sysArray.gen[i].IREG = Convert.ToInt32(genData[7]);

                        //9th element is MBASE
                        sysArray.gen[i].MBASE = Convert.ToInt32(genData[8]);

                        //10th element is ZR
                        sysArray.gen[i].ZR = Convert.ToInt32(genData[9]);

                        //11th element is ZX
                        sysArray.gen[i].ZX = Convert.ToInt32(genData[10]);

                        //12th element is RT
                        sysArray.gen[i].RT = Convert.ToInt32(genData[11]);

                        //13th element is XT
                        sysArray.gen[i].XT = Convert.ToInt32(genData[12]);

                        //14th element is GTAP
                        sysArray.gen[i].GTAP = Convert.ToInt32(genData[13]);

                        //15th element is STAT
                        sysArray.gen[i].Status = Convert.ToInt32(genData[14]);
                    }
                }
                #endregion

                #region Parse Line Data
                else if (ctr == 5)
                {
                    //FIFTH element is Line Data

                    //split to get rows of lines
                    string[] lineRows = element.Split(new string[] { "\\n" }, StringSplitOptions.None);

                    //initialize system array as to the number of lines
                    sysArray.numLines = lineRows.Length;
                    sysArray.line = new Line[lineRows.Length - 1];

                    //parse array of bus lines
                    int i = -1;
                    foreach (string lineRow in lineRows)
                    {
                        //initialize index marker
                        i++;

                        //get lineData values from lineRow
                        string[] lineData = lineRow.Split(',');

                        //parse each lineData Value and save to system Array
                        //by following the order: I, J, CKT, STATUS, R, X, B2

                        //firs element is I
                        sysArray.line[i].fromBus = Convert.ToInt32(lineData[0]);

                        //2nd element is J
                        sysArray.line[i].toBus = Convert.ToInt32(lineData[1]);

                        //3rd element is CKT
                        sysArray.line[i].ckt = Convert.ToInt32(lineData[2]);

                        //4th element is Status
                        sysArray.line[i].status = Convert.ToInt32(lineData[3]);

                        //5th element is R
                        sysArray.line[i].R = Convert.ToInt32(lineData[4]);

                        //6th element is X
                        sysArray.line[i].X = Convert.ToInt32(lineData[5]);

                        //7th element is B2
                        sysArray.line[i].B2 = Convert.ToInt32(lineData[6]);
                    }
                }
                #endregion

                #region Parse Transformer Data
                else if (ctr == 6)
                {
                    //SIXTH element is Transformer Data

                    //split to get rows of transformers
                    string[] transRows = element.Split(new string[] { "\\n" }, StringSplitOptions.None);

                    //initialize system array as to the number of transformers
                    sysArray.numTrans = transRows.Length;

                    //parse array of transformer rows
                    int i = -1;
                    foreach (string transRow in transRows)
                    {
                        //initialize index marker
                        i++;

                        //get transData values from transRow
                        string[] transData = transRow.Split(',');
                        sysArray.trans = new Transformer[transRows.Length - 1];

                        //parse each transData Value and save to system Array
                        //by following the order: fromBus, toBus, CKT, STATUS, MVA, Z

                        //first element is fromBus
                        sysArray.trans[i].fromBus = Convert.ToInt32(transData[0]);

                        //2nd element is to=Bus
                        sysArray.trans[i].toBus = transData[1];

                        //3rd element is CKT
                        sysArray.trans[i].ckt = transData[2];

                        //4th element is status
                        sysArray.trans[i].Status = transData[3];

                        //5th element is MVA
                        sysArray.trans[i].MVA = Convert.ToDouble(transData[4]);

                        //6th element is Z
                        sysArray.trans[i].Z = Convert.ToInt32(transData[5]);                                                                        
                    }
                }
            }
            #endregion

            return sysArray;

        }

        private SystemArrays initializeTestData()
        {
            SystemArrays sArray = new SystemArrays();
            
            #region INITIALIZE BUS            
            //set number of buses
            sArray.numBuses = 4;

            //sArray.Ybus
            Matrix<Complex32> Ybus = Matrix<Complex32>.Build.Dense(sArray.numBuses, sArray.numBuses);
            sArray.Ybus = Ybus;
            
            sArray.bus = new Bus[sArray.numBuses];
            for (int i = 0; i <= (sArray.numBuses - 1); i++)
            {
                sArray.bus[i] = new Bus();
            }
            

            //set bus data array for each bus element
             
            //bus 1 :   I, 'NAME', BASKV, IDE, AREA, ZONE, OWNER, VR, VA
            sArray.bus[0].BusNo = 1;
            sArray.bus[0].Name = "Bus" + sArray.bus[0].BusNo;
            sArray.bus[0].BusKV = 230;
            sArray.bus[0].BusType = 3;

            //bus 2 :   I, 'NAME', BASKV, IDE, AREA, ZONE, OWNER, VR, VA
            sArray.bus[1].BusNo = 2;
            sArray.bus[1].Name = "Bus" + sArray.bus[1].BusNo;
            sArray.bus[1].BusKV = 230;
            sArray.bus[1].BusType = 1;

            //bus 3 :   I, 'NAME', BASKV, IDE, AREA, ZONE, OWNER, VR, VA
            sArray.bus[2].BusNo = 3;
            sArray.bus[2].Name = "Bus" + sArray.bus[2].BusNo;
            sArray.bus[2].BusKV = 230;
            sArray.bus[2].BusType = 1;

            //bus 4 :   I, 'NAME', BASKV, IDE, AREA, ZONE, OWNER, VR, VA
            sArray.bus[3].BusNo = 4;
            sArray.bus[3].Name = "Bus" + sArray.bus[3].BusNo;
            sArray.bus[3].BusKV = 230;
            sArray.bus[3].BusType = 2;
            #endregion

            #region INITIALIZE LOAD
            //set number of loads
            sArray.numLoads = 4;
            sArray.load = new Load[sArray.numLoads];
            for (int i = 0; i <= sArray.numLoads - 1; i++)
            {
                sArray.load[i] = new Load();
            }

            //set load data array for each load element
            //Load 1 : I, ID, STATUS, AREA, ZONE, PL, QL
            sArray.load[0].BusNo = 1;
            sArray.load[0].ID = 1;
            sArray.load[0].Status = 1;
            sArray.load[0].Area = 0;
            sArray.load[0].Zone = 0;
            sArray.load[0].MW = 50.0F;
            sArray.load[0].MVAR = 30.99F;

            //Load 2 : I, ID, STATUS, AREA, ZONE, PL, QL
            sArray.load[1].BusNo = 2;
            sArray.load[1].ID = 1;
            sArray.load[1].Status = 1;
            sArray.load[1].Area = 0;
            sArray.load[1].Zone = 0;
            sArray.load[1].MW = 170.0F;
            sArray.load[1].MVAR = 105.35F;

            //Load 3 : I, ID, STATUS, AREA, ZONE, PL, QL
            sArray.load[2].BusNo = 3;
            sArray.load[2].ID = 1;
            sArray.load[2].Status = 1;
            sArray.load[2].Area = 0;
            sArray.load[2].Zone = 0;
            sArray.load[2].MW = 200.0F;
            sArray.load[2].MVAR = 123.94F;

            //Load 3 : I, ID, STATUS, AREA, ZONE, PL, QL
            sArray.load[3].BusNo = 4;
            sArray.load[3].ID = 1;
            sArray.load[3].Status = 1;
            sArray.load[3].Area = 0;
            sArray.load[3].Zone = 0;
            sArray.load[3].MW = 80.0F;
            sArray.load[3].MVAR = 49.58F;
            #endregion

            #region INITIALIZE SHUNT
            //set number of SHUNT
            //do nothing
            #endregion

            #region INITIALIZE GENERATOR
            sArray.numGens = 2;
            sArray.gen = new Generator[sArray.numGens];
            for (int i = 0; i <= sArray.numGens - 1; i++)
            {
                sArray.gen[i] = new Generator();
            }

            //set GENERATOR data array for each generator element
            //Generator 1 : I, ID, PG, QG, QT, QB, VS, IREG, MBASE, ZR, ZX, RT, XT, GTAP, STAT
            sArray.gen[0].BusNo = 1;
            sArray.gen[0].ID = 1;
            sArray.gen[0].PG = 0.0F;
            sArray.gen[0].QG = 0.0F;
            sArray.gen[0].QT = 0.0F;
            sArray.gen[0].QB = 0.0F;
            sArray.gen[0].VS = 1.0F;
            sArray.gen[0].IREG = 0;
            sArray.gen[0].MBASE=100;
            sArray.gen[0].ZR=0.0F;
            sArray.gen[0].ZX=0.0F;
            sArray.gen[0].RT = 0.0F;
            sArray.gen[0].XT = 0.0F;
            sArray.gen[0].GTAP = 0.0F;
            sArray.gen[0].Status = 0;

            //Generator 2 : I, ID, PG, QG, QT, QB, VS, IREG, MBASE, ZR, ZX, RT, XT, GTAP, STAT
            sArray.gen[1].BusNo = 1;
            sArray.gen[1].ID = 4;
            sArray.gen[1].PG = 318.0F;
            sArray.gen[1].QG = 0.0F;
            sArray.gen[1].QT = 0.0F;
            sArray.gen[1].QB = 0.0F;
            sArray.gen[1].VS = 1.02F;
            sArray.gen[1].IREG = 0;
            sArray.gen[1].MBASE = 100;
            sArray.gen[1].ZR = 0.0F;
            sArray.gen[1].ZX = 0.0F;
            sArray.gen[1].RT = 0.0F;
            sArray.gen[1].XT = 0.0F;
            sArray.gen[1].GTAP = 0.0F;
            sArray.gen[1].Status = 0;
            #endregion

            #region INITIALIZE LINE
            sArray.numLines = 4;
            sArray.line = new Line[sArray.numLines];
            for (int i = 0; i <= sArray.numLines - 1; i++)
            {
                sArray.line[i] = new Line();
            }

            //set LINE data array for each line element
            //Line 1 : I, J, CKT, STATUS, R, X, B2
            sArray.line[0].fromBus = 1;
            sArray.line[0].toBus = 2;
            sArray.line[0].ckt = 1;
            sArray.line[0].status = 1;
            sArray.line[0].R = 0.01008F;
            sArray.line[0].X = 0.05040F;
            sArray.line[0].B2 = 0.05125F;

            //Line 2 : I, J, CKT, STATUS, R, X, B2
            sArray.line[1].fromBus = 1;
            sArray.line[1].toBus = 3;
            sArray.line[1].ckt = 1;
            sArray.line[1].status = 1;
            sArray.line[1].R = 0.00744F;
            sArray.line[1].X = 0.03720F;
            sArray.line[1].B2 = 0.03875F;


            //Line 3 : I, J, CKT, STATUS, R, X, B2
            sArray.line[2].fromBus = 2;
            sArray.line[2].toBus = 4;
            sArray.line[2].ckt = 1;
            sArray.line[2].status = 1;
            sArray.line[2].R = 0.00744F;
            sArray.line[2].X = 0.03720F;
            sArray.line[2].B2 = 0.03875F;

            //Line 4 : I, J, CKT, STATUS, R, X, B2
            sArray.line[3].fromBus = 3;
            sArray.line[3].toBus = 4;
            sArray.line[3].ckt = 1;
            sArray.line[3].status = 1;
            sArray.line[3].R = 0.01272F;
            sArray.line[3].X = 0.06360F;
            sArray.line[3].B2 = 0.06375F;
            #endregion

            return sArray;
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            sysArrayMain = initializeTestData();
            sysArrayMain.MVABase = 230;
            
            Solver solver = new Solver();
            Vector<Complex32> Vbus = solver.GaussSolver(sysArrayMain);
            txtResult.Text = Vbus.ToString();
        }
    }
}
