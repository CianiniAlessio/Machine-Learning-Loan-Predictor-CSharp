using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Machine_Learning_Loan_Predictor_CSharp
{
    class MLClass
    {
        public MLClass() { }
        // here is a function that will be used to prepared the input and the output for the model that I chose 
        public static void SeparateInputsAndOutputs(ref double[][] Inputs, ref double[] Outputs, double[,] matrix)
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
        // here i split the matrix in test and training with a seed of 0.8 and is constant and then I save in a csv the train and the test results
        public static void SplitTrainingTest(string[,] matriceDati,string pathTest, string pathTraining)
        {
            List<string[]> temp = FromMatrixToList(matriceDati);
            //HERE I USED THE SHUFFLE FUNCTION IN ORDER TO CHANGE THE ORDER OF THE ARRAY
            Shuffle(ref temp);
            matriceDati = FromListToMatrix(temp);
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
            SaveToCsv(pathTest, Test);
            SaveToCsv(pathTraining, Training);

        }

        // RANDOMIZE A STARTING POINT AND CHANGING THE ORDER OF THE LIST. DOING THIS WHEN I SPLIT TRAINING AND TESTING I'M GOING TO OBTAIN
        // A DIFFERENT SET OF TEST AND TRAIN FOR EACH CYCLE. TEST RATIO TRAINING/TEST IS ALWAYS THE SAME 4:1 BUT THE ELEMENTS CHANGE
        private static void Shuffle(ref List<string[]> list)
        {
            Random random = new Random();
            List<string[]> temp = new List<string[]>();
            int StartingPoint = (int)random.NextInt64(1, 100);
            for (int i = StartingPoint; i < list.Count; i++)
            {
                temp.Add(list[i]);
            }
            for (int i = 0; i < StartingPoint; i++)
            {
                temp.Add(list[i]);
            }
            list = temp;
        }
        // NEEDED FOR THE SHUFFLE
        private static List<string[]> FromMatrixToList(string[,] matrix)
        {
            List<string[]> temp = new List<string[]>();
            string[] row = new string[matrix.GetLength(1)];
            for (int k = 0; k < matrix.GetLength(0); k++)
            {
                for (int i = 0; i < matrix.GetLength(1); i++)
                {
                    row[i] = matrix[k, i];
                }

                temp.Add(row);
                row = new string[matrix.GetLength(1)];
            }
            return temp;
        }
        // NEEDED FOR THE SHUFFLE
        private static string[,] FromListToMatrix(List<string[]> list)
        {
            int column = list[0].Length;
            int row = list.Count;
            string[,] matrix = new string[row, column];
            for (int k = 0; k < row; k++)
            {
                for (int i = 0; i < column; i++)
                {
                    matrix[k, i] = list[k][i];
                }
            }
            return matrix;
        }
        /// <summary>
        /// This is the save file function for the test and training output
        /// </summary>
        private static void SaveToCsv(string pathToWrite, string[,] matriceDati)
        {
            try
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(pathToWrite))
                {
                    file.WriteLine("ApplicantIncome,CoapplicantIncome,LoanAmount,Credit_History,Loan_Status");
                    string toWrite = "";
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
    }
    
    
}
