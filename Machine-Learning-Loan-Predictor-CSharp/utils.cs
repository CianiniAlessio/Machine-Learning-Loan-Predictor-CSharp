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

        public static string basePath = Directory.GetCurrentDirectory().ToString().Substring(0, Directory.GetCurrentDirectory().ToString().IndexOf("bin") - 1);
        private static string pathFinalData = basePath + "\\FinalData";
        private static string pathTrainAndTest = basePath + "\\FinalData";
        public static string pathDati = basePath + "\\datiC#.csv";
        public static string pathCorrette = pathFinalData + "\\CleanedData.csv";
        public static string pathTraining = pathTrainAndTest + "\\Training.csv";
        public static string pathTest = pathTrainAndTest + "\\Test.csv"; 
        //my enum used for the choice of the column
        private enum COLUMN_FEATURES : int { ApplicantIncome = 6, CoapplicantIncome = 7, LoanAmount = 8, Credit_History = 10, Loan_Status = 12}; 
        

        /// <summary>
        /// Pass by ref a string[,] matrix in order to remove the anomalies 
        /// </summary>
        /// <param name="matriceDati"></param>
        public static void CheckAndRemoveAnomalies(ref string[,] matriceDati)
        {
            // Here I check the error in the matrix and create a list with position of the error. I didn't create an array since it changes in lenght.
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
                        if (lastErrorRow != k)
                        {
                            errori.Add(k);
                            lastErrorRow = k;
                        }
                    }
                }

            }
            // here I call the method to remove the founded anomalies
            RemoveAnomalies(ref matriceDati, errori);

        }


        // remove anomalies
        private static void RemoveAnomalies(ref string[,] matriceDati, List<int> errori)
        {
            int row = matriceDati.GetLength(0);
            int column = matriceDati.GetLength(1);
            int numberOfCurrentErrors = 0;
            // basically I create another array that has the lenght that is the lenght of the matrix not cleaned minus the number of the error
            string[,] newArray = new string[matriceDati.GetLength(0) - errori.Count, matriceDati.GetLength(1)];
            int[] PosErrori = errori.ToArray();

            bool checkPos;
            for (int k = 0; k < row; k++)
            {
                checkPos = false;
                for (int j = 0; j < column; j++)
                {
                    // i check if the row is the same for one of the error
                    for (int z = 0; z < errori.Count; z++)
                    {
                        if (PosErrori[z] == k)
                        {
                            checkPos = true;
                        }
                    }
                    // if the check is false then I'm going to fill the array 
                    if (!checkPos)
                    {
                        // the row that I'm going to fill is going to be the row in which I'm at minus the number of error counted.
                        // Basically I need to scale the position of the array with the errors minus
                        // the number of error encountered in order to not have empty spaces in the cleaned array.
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


        /// <summary>
        /// Read the data from the csv and create a matrix which I'm going to check for malformed data ex: null char, negative salary ecc
        /// </summary>
        public static string[,] LoadData(string pathToRead)
        {
            int i = 0;
            int column = 0;
            string[,] mioArray;
            // with this first cycle i get the number of column and the number of row for the new matrix that i'm going to create.
            try
            {
                using (var reader = new System.IO.StreamReader(pathToRead))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(',');
                        column = values.Length;
                        i++;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            // with this cycle i just fill the matrix with all the data 
            try
            {
                using (var reader = new System.IO.StreamReader(pathToRead))
                {
                    int l = 0;
                    mioArray = new string[i, column];
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(',');
                        if (l != 0)
                        {
                            for (int h = 0; h < column; h++)
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
            //if some problems
            return null;

        }


        /// <summary>
        /// // Convert the data read as string from the csv in double and prepare them for the Model 
        /// </summary>
        /// <param name="pathToRead"> Pass the path of the Cleaned matrix</param>
        /// <returns></returns>
        public static double[,] LoadDataAndConvertToDouble(string pathToRead)
        {
            int row = 0;
            int column = 0;
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
                        column = values.Length;
                        row++;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            // with this cycle I fill the matrix with all the data 
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
                        // here i pass the data in the matrix and i convert the last column with Y = 1 and N = 0 needed for the ML model
                        foreach (var x in values)
                        {
                            if (i <= row && i != 0)
                            {
                                if (c == values.Length - 1)
                                    mioArray[i - 1, c++] = (x.Equals("N") ? 0 : 1);
                                else
                                    mioArray[i - 1, c++] = Convert.ToDouble(x);
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
            //if some problems
            return null;

        }


        // here looking at my data for this project I decided to use only the column with the features: ApplicantIncome,CoapplicantIncome,LoanAmount,Credit_History 
        // and of course I keep the record of the result :LoanStatus which I'll use to train and test
        public static string[,] RemoveUselessColumn(string[,] matriceDati)
        {
            string[,] returnMatrix = new string[matriceDati.GetLength(0), 5];
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(pathCorrette))
            {
                file.WriteLine("ApplicantIncome,CoapplicantIncome,LoanAmount,Credit_History,Loan_Status");
                string toWrite = "";
                int i = 0;
                for (int k = 0; k < matriceDati.GetLength(0); k++)
                {
                    for (int j = 0; j < matriceDati.GetLength(1); j++)
                    {
                        // here I checked that the column that I'm looking at is the one of the column of interest decided in the enum at the beginning
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



        // here i split the matrix in test and training with a seed of 0.8 and is constant and then I save in a csv the train and the test results
        public void SplitTrainingTest(string[,] matriceDati)
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


        /// <summary>
        /// This is the save file function for the test and training output
        /// </summary>
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


        // here is a function that will be used to prepared the input and the output for the model that I chose 
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
        // NEEDED FOR THE SHUFFLE
        private List<string[]> FromMatrixToList(string[,] matrix)
        {
            List<string[]> temp = new List<string[]>(); 
            string[] row = new string[matrix.GetLength(1)];
            for(int k=0;k<matrix.GetLength(0);k++)
            {
                for (int i=0;i<matrix.GetLength(1);i++)
                {
                    row[i] = matrix[k, i];
                }
              
                temp.Add(row);
                row = new string[matrix.GetLength(1)];
            }
            return temp;
        }
        // NEEDED FOR THE SHUFFLE
        private string[,] FromListToMatrix(List<string[]> list)
        {
            int column = list[0].Length;
            int row = list.Count;
            string[,] matrix = new string[row,column];
            for(int k = 0;k<row;k++)
            {
                for(int i = 0;i<column;i++)
                {
                    matrix[k, i] = list[k][i];
                }
            }
            return matrix;
        }
        // RANDOMIZE A STARTING POINT AND CHANGING THE ORDER OF THE LIST. DOING THIS WHEN I SPLIT TRAINING AND TESTING I'M GOING TO OBTAIN
        // A DIFFERENT SET OF TEST AND TRAIN FOR EACH CYCLE. TEST RATIO TRAINING/TEST IS ALWAYS THE SAME 4:1 BUT THE ELEMENTS CHANGE
        private void Shuffle(ref List<string[]> list)
        {
            Random random = new Random();
            List<string[]> temp = new List<string[]>();
            int StartingPoint = (int)random.NextInt64(1, 100);
            for(int i = StartingPoint; i < list.Count; i++)
            {
                temp.Add(list[i]);
            }
            for(int i = 0; i<StartingPoint; i++)
            {
                temp.Add(list[i]);
            }
            list = temp;
        }

    }

}
