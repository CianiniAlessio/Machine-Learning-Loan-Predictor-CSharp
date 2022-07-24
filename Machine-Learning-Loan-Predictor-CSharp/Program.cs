using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Data;
using System.Threading;
using System.Linq;
using Accord;
using Accord.Controls;
using Accord.Statistics;
using Accord.Statistics.Models.Regression;
using Accord.Statistics.Models.Regression.Fitting;

namespace Machine_Learning_Loan_Predictor_CSharp
{
    class Program : utils
    {
        static void Main(string[] args)
        {
           

            Console.WriteLine("\n===========> PROJECT ALESSIO CIANINI <===========\n");
            Console.WriteLine("\n===========> Creating a model and training it <===========\n");
            utils u = new utils();

            // I LOAD THE DATA FROM THE PATH SET IN THE UTILS.CS

            string[,] matriceDati = LoadData(pathDati);
            
            //I DECIDE WHICH COLUMN I'M GOING TO USE SINCE NOT ALL THE FEATURES I BELIEVE ARE USEFULL
            matriceDati = RemoveUselessColumn(matriceDati);

            // CHECKING AND CLEANING OF THE MATRIX 
            utils.CheckAndRemoveAnomalies(ref matriceDati);
            
            // SPLIT IN TRAINING AND TEST MATRIX, ALSO WRITE TO TEST.CSV AND TRAINING.CSV THE MATRIX CREATED
            u.SplitTrainingTest(matriceDati);

            //ONCE I CREATED THE MATRIX I'M GOING GOING TO PREPARE THE DATA FOR THE MODEL, WHICH REQUIRES TO CONVERT THE 
            //STRING MATRIX IN A DOUBLE MATRIX
            double[,] miaMatriceTest = LoadDataAndConvertToDouble(pathTest);
            double[,] miaMatriceTrain = LoadDataAndConvertToDouble(pathTraining);
            
            //INITIALIZATION OF THE VARAIBLES NEEDED
            double[][] traingInputs = new double[miaMatriceTrain.GetLength(0)][];
            double[] traingOutputs = new double[miaMatriceTrain.GetLength(0)];
            double[][] testInputs = new double[miaMatriceTest.GetLength(0)][];
            double[] testOutputs = new double[miaMatriceTest.GetLength(0)];


            // I SEPARATE WITH A 80-20 TRAINING-TEST %
            u.SeparateInputsAndOutputs(ref traingInputs, ref traingOutputs, miaMatriceTrain);
            u.SeparateInputsAndOutputs(ref testInputs, ref testOutputs, miaMatriceTest);


            
            // Train a Logistic Regression model
            var learner = new IterativeReweightedLeastSquares<LogisticRegression>()
            {
                MaxIterations = 1
            };
            var logit = learner.Learn(traingInputs, traingOutputs);

            // Predict output

            bool[] predictions = logit.Decide(testInputs);
            int l = 0;

            Console.Write("The weights of my Model are: ");
            foreach (var k in logit.Weights)
            {
                Console.Write($"{Math.Round(k, 5)}, ");
            }

            // Plot the results
            int i = 0;
            int correctNumber = 0;

            Console.WriteLine($"\nThe regression used is : {logit.Linear}");
            Console.WriteLine($"\nThe standard error encountered is : {logit.StandardErrors.Mean()}");

            foreach (var k in testOutputs)
            {
                if (k == predictions[i].ToZeroOne())
                {
                    correctNumber++;
                }
                i++;
            }

            Console.WriteLine($"\nThe accuracy of my model is = {correctNumber * 100 / testOutputs.Length}%");

            Console.ReadKey();
        }

    }
}
