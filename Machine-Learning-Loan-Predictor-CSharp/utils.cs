using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Machine_Learning_Loan_Predictor_CSharp
{
    class utils
    {
        public utils() { }
        private enum COLUMN_FEATURES : int { ApplicantIncome = 6, CoapplicantIncome = 7, LoanAmount = 8, Credit_History = 10, Loan_Status = 12};
        private string basePath = Directory.GetCurrentDirectory().ToString().Substring(0,
            Directory.GetCurrentDirectory().ToString().IndexOf("bin") - 1);
        public static void CheckAndRemoveAnomalies(ref string[,] matriceDati)
        {
            int row = matriceDati.GetLength(0);
            int column = matriceDati.GetLength(1);
            List<int> errori = new List<int>();
            int lastErrorRow = -1;
            for (int k = 0; k < row; k++)
            {
                for (int j = 0; j < column; j++)
                {
                    if (matriceDati[k, j] == null || matriceDati[k, j].Equals(""))
                    {
                        Console.WriteLine($"Something wrong at line {k} and column {j}");
                        if (lastErrorRow != k)
                        {
                            errori.Add(k);
                            lastErrorRow = k;
                        }
                    }
                }

            }
            RemoveAnomalies(ref matriceDati, errori);

        }
        private static void RemoveAnomalies(ref string[,] matriceDati, List<int> errori)
        {
            int row = matriceDati.GetLength(0);
            int column = matriceDati.GetLength(1);
            int numberOfCurrentErrors = 0;
            string[,] newArray = new string[matriceDati.GetLength(0) - errori.Count, matriceDati.GetLength(1)];
            int[] PosErrori = errori.ToArray();
            for (int k = 0; k < row; k++)
            {
                bool checkPos = false;
                for (int j = 0; j < column; j++)
                {
                    for (int z = 0; z < errori.Count; z++)
                    {
                        if (PosErrori[z] == k)
                        {
                            checkPos = true;
                        }
                    }
                    if (!checkPos)
                    {
                        newArray[k - numberOfCurrentErrors, j] = matriceDati[k, j];
                    }
                }
                if (checkPos)
                {
                    numberOfCurrentErrors++;
                }
            }
            matriceDati = newArray;
        }
        public static string[,] LoadData(string pathToRead)
        {

            int i = 0;
            int colonne = 0;
            string[,] mioArray;
            // with this first cycle i get the number of column and the number of row for the file.
            try
            {
                using (var reader = new System.IO.StreamReader(pathToRead))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(',');
                        colonne = values.Length;
                        i++;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            // with this one i just fill the matrix with all the data 

            try
            {
                using (var reader = new System.IO.StreamReader(pathToRead))
                {
                    int l = 0;
                    mioArray = new string[i, colonne];
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(',');
                        if (l != 0)
                        {
                            for (int h = 0; h < colonne; h++)
                            {
                                mioArray[l - 1, h] = values[h];

                            }
                        }
                        l++;

                    }
                    
                    return mioArray;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return null;

        }       
        public static string[,] RemoveUselessColumn(string[,] matriceDati, string pathToWrite)
        {
            string[,] returnMatrix = new string[matriceDati.GetLength(0), 5];
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(pathToWrite))
            {
                file.WriteLine("ApplicantIncome,CoapplicantIncome,LoanAmount,Credit_History,Loan_Status");
                string toWrite = "";
                int i = 0;
                for (int k = 0; k < matriceDati.GetLength(0); k++)
                {
                    for (int j = 0; j < matriceDati.GetLength(1); j++)
                    {
                        if(j == (int)COLUMN_FEATURES.ApplicantIncome || j == (int)COLUMN_FEATURES.CoapplicantIncome ||
                           j == (int)COLUMN_FEATURES.LoanAmount|| j == (int)COLUMN_FEATURES.Credit_History ||
                           j == (int)COLUMN_FEATURES.Loan_Status)
                        {
                            if (j == (int)COLUMN_FEATURES.ApplicantIncome)
                            {
                                toWrite = matriceDati[k, j];
                                returnMatrix[k, i++] = matriceDati[k, j];
                            }
                            else
                            {
                                toWrite += "," + matriceDati[k, j ];
                                returnMatrix[k, i++] = matriceDati[k, j ];
                            }

                        }
                    }
                    i = 0;
                    file.WriteLine(toWrite);
                }
            }
            return returnMatrix;

        }
        public void SplitTrainingTest(string[,] matriceDati)
        {
            Random r = new Random();
            int row = matriceDati.GetLength(0);
            int column = matriceDati.GetLength(1);
            int x = Convert.ToInt32(row * 0.8);
            string[,] Training = new string[x, matriceDati.GetLength(1)];
            string[,] Test = new string[row - x, matriceDati.GetLength(1)];
            for (int i = 0; i < row; i++)
            {
                if (i < x)
                {
                    //training
                    for (int k = 0; k < column; k++)
                    {
                        Training[i, k] = matriceDati[i, k];
                    }
                }
                else
                {
                    //test
                    for (int k = 0; k < column; k++)
                    {
                        Test[i - x, k] = matriceDati[i, k];
                    }
                }
            }
            SaveToCsv(basePath + "\\Test.csv", Test);
            SaveToCsv(basePath + "\\Training.csv", Training);
            
        }
        public void SaveToCsv(string pathToWrite, string[,] matriceDati)
        {
            try
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(pathToWrite))
                {
                    file.WriteLine("ApplicantIncome,CoapplicantIncome,LoanAmount,Credit_History,Loan_Status");
                    string toWrite = "";
                    int i = 0;
                    for (int k = 0; k < matriceDati.GetLength(0); k++)
                    {
                        for (int j = 0; j < matriceDati.GetLength(1) - 1; j++)
                        {
                            if (j == 0)
                            {
                                toWrite = matriceDati[k, j];
                            }
                            toWrite += "," + matriceDati[k, j + 1];
                        }
                        file.WriteLine(toWrite);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public static double[,] LoadDataAndConvertToDouble(string pathToRead)
        {

            int row = 0;
            int colonne = 0;
            double[,] mioArray = new double[,] { };
            // with this first cycle i get the number of column and the number of row for the file.
            try
            {
                using (var reader = new System.IO.StreamReader(pathToRead))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(',');
                        colonne = values.Length;
                        row++;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            // with this one i just fill the matrix with all the data 

            try
            {
                using (var reader = new System.IO.StreamReader(pathToRead))
                {
                    int i = 0;
                    mioArray = new double[row, 5];
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(',');
                        int c = 0;

                        foreach (var x in values)
                        {
                            if (i <= row && i != 0)
                            {
                                if (c == values.Length - 1)
                                    mioArray[i - 1, c++] = (x.Equals("N") ? 0 : 1);    
                                else
                                    mioArray[i - 1, c++] =  Convert.ToDouble(x);
                            }

                        }
                        i++;

                    }
                    return mioArray;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return mioArray;

        }
        public void SeparateInputsAndOutputs(ref double[][] Inputs, ref double[] Outputs, double[,] matrix)
        {
            Inputs = new double[matrix.GetLength(0)][];
            Outputs = new double[matrix.GetLength(0)];
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                double[] d = new double[4] { matrix[i, 0], matrix[i, 1], matrix[i, 2], matrix[i, 3] };
                Inputs[i] = d;

                Outputs[i] = matrix[i, 4];

            }

        }

    }

}
