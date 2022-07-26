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
            string basePath = Directory.GetCurrentDirectory().ToString().Substring(0, Directory.GetCurrentDirectory().ToString().IndexOf("bin") - 1);
            string pathFinalData = basePath + "\\FinalData";
            string pathTrainAndTest = basePath + "\\FinalData";
            string pathDati = basePath + "\\datiC#.csv";
            string pathCorrette = pathFinalData + "\\CleanedData.csv";
            string pathTraining = pathTrainAndTest + "\\Training.csv";
            string pathTest = pathTrainAndTest + "\\Test.csv";


            Console.WriteLine("\n===========> PROJECT ALESSIO CIANINI <===========\n");
            Console.WriteLine("\n===========> Creating a model and training it <===========\n");
            utils u = new utils();

            // I LOAD THE DATA FROM THE PATH SET IN THE UTILS.CS

            string[,] matriceDati = utils.LoadData(pathDati);

            //I DECIDE WHICH COLUMN I'M GOING TO USE SINCE NOT ALL THE FEATURES I BELIEVE ARE USEFULL
            matriceDati = utils.RemoveUselessColumn(matriceDati,pathCorrette);

            // CHECKING AND CLEANING OF THE MATRIX 
            utils.CheckAndRemoveAnomalies(ref matriceDati);

            // SINCE THE MODEL IS ALWAYS THE SAME I INITIALIZE IT OUTSIDE OF THE CYCLE
            var learner = new IterativeReweightedLeastSquares<LogisticRegression>()
            {
                MaxIterations = 1000
            };
            //OTHER INITIALIZATION OUTSIDE OF THE CYCLE
            double[] scores;
            bool[] predictions;
            int i,correctNumber,count = 0;
            double accuracy;
            List<double> accuracies = new List<double>();
            Console.WriteLine("========================================================================================");
            while (count++<30)
            {                
                // SPLIT IN TRAINING AND TEST MATRIX, ALSO WRITE TO TEST.CSV AND TRAINING.CSV THE MATRIX CREATED
                // HERE IS USED ALSO A SHUFFLE ALGORITHM TO CHANGE THE ORDER FOR EACH ITERATIONS
                MLClass.SplitTrainingTest(matriceDati,pathTest,pathTraining);

                //ONCE I CREATED THE MATRIX I'M GOING GOING TO PREPARE THE DATA FOR THE MODEL, WHICH REQUIRES TO CONVERT THE 
                //STRING MATRIX IN A DOUBLE MATRIX
                double[,] miaMatriceTest = utils.LoadDataAndConvertToDouble(pathTest);
                double[,] miaMatriceTrain = utils.LoadDataAndConvertToDouble(pathTraining);

                //INITIALIZATION OF THE VARAIBLES NEEDED
                double[][] traingInputs = new double[miaMatriceTrain.GetLength(0)][];
                double[] traingOutputs = new double[miaMatriceTrain.GetLength(0)];
                double[][] testInputs = new double[miaMatriceTest.GetLength(0)][];
                double[] testOutputs = new double[miaMatriceTest.GetLength(0)];


                // I SEPARATE WITH A 80-20 TRAINING-TEST %
                MLClass.SeparateInputsAndOutputs(ref traingInputs, ref traingOutputs, miaMatriceTrain);
                MLClass.SeparateInputsAndOutputs(ref testInputs, ref testOutputs, miaMatriceTest);



                // Train a Logistic Regression model
                
                LogisticRegression logit = learner.Learn(traingInputs, traingOutputs);

                // Predict output 
                predictions = logit.Decide(testInputs);

                // Calculate the relation between the given input vector and its most strongly associated class
                scores = logit.Score(testInputs);
                

                Console.Write("WEIGHTS: ");
                foreach (var k in logit.Weights)
                {
                    Console.Write($"{Math.Round(k, 5)}, ");
                }

                // Plot the results
                i = 0;
                correctNumber = 0;
                Console.Write("\nREGRESSION COEFFICIENT: ");
                foreach (var coeff in logit.Linear.Weights)
                {
                    Console.Write($"{coeff} ");
                }
                Console.WriteLine($"\nSTANDARD ERROR: {logit.StandardErrors.Mean()}");
                foreach ( var res in predictions.ToZeroOne())
                {
                    if(res == testOutputs[i])
                    {
                        correctNumber++;
                    }
                    i++;
                }
                accuracy = Math.Round((double)correctNumber / (double)testOutputs.Length, 4) * 100;
                accuracies.Add(accuracy);
                Console.WriteLine($"ACCURACY = {accuracy}%");
                Console.WriteLine("========================================================================================");
                Thread.Sleep(100);
            }
            Console.WriteLine("THE ACCURACIES FOR EACH CYCLE ARE:");
            foreach(var x in accuracies)
            {
                Console.Write($"|| {Math.Round(x,2)} ");
            }
            Console.Write("||");
            Console.ReadKey();
        }

    }
}
