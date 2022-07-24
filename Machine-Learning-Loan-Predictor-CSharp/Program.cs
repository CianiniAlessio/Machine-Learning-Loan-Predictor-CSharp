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
    class Program
    {
        static void Main(string[] args)
        {
           

            Console.WriteLine("\n===========> PROJECT ALESSIO CIANINI <===========\n");
            Console.WriteLine("\n===========> Creating a model and training it <===========\n");
            string currentDirectory = Directory.GetCurrentDirectory().ToString();
            string basePath = currentDirectory.Substring(0, currentDirectory.IndexOf("bin") - 1);
            string pathDati = basePath + "\\datiC#.csv";
            string pathCorrette = basePath + "\\UsefullData.csv";
            string pathTraining = basePath + "\\Training.csv";// @System.Configuration.ConfigurationManager.AppSettings["pathToTraining"];
            string pathTest = basePath + "\\Test.csv"; //@System.Configuration.ConfigurationManager.AppSettings["pathToTest"];
            utils u = new utils();
            string[,] matriceDati = utils.LoadData(pathDati);
            matriceDati = utils.RemoveUselessColumn(matriceDati, pathCorrette);

            //clean 
            utils.CheckAndRemoveAnomalies(ref matriceDati);
            //splitto
            u.SplitTrainingTest(matriceDati);

            double[,] miaMatriceTest = utils.LoadDataAndConvertToDouble(pathTest);
            double[,] miaMatriceTrain = utils.LoadDataAndConvertToDouble(pathTraining);
            double[][] traingInputs = new double[miaMatriceTrain.GetLength(0)][];
            double[] traingOutputs = new double[miaMatriceTrain.GetLength(0)];
            double[][] testInputs = new double[miaMatriceTest.GetLength(0)][];
            double[] testOutputs = new double[miaMatriceTest.GetLength(0)];


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
