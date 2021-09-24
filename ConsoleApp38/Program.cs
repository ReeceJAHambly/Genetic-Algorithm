using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace ACW
{

    class Program
    {

        public static Random _rand = new Random();
        //public static StreamWriter swClear = new StreamWriter("Results.txt");
        

        // populations each with 60 random weights
        static List<List<double>> populations = new List<List<double>>();
        static List<List<double>> nextPopulations = new List<List<double>>();
        static List<List<double>> currentGen = new List<List<double>>();

        // used to set and change the population size;
        static int populationSize = 500;
        static double crossover = 0.5;
        static double mutation = 0.01;
        static int runs = 20;

        public static Random randomValue = new Random();
        public static Random randomValue2 = new Random();
        static void Main(string[] args)
        {

            for (int f = 0; f < populationSize; f++)
            {
                //Create a list to store some random doubles
                List<double> randomWeights = new List<double>();

                //Add 60 random doubles to the lists (between the ranges [0,1]
                //These represent random weights 
                for (int i = 0; i < 60; i++)
                {
                    randomWeights.Add((_rand.NextDouble() * 2) - 1);
                }
                // Used to add weights to a population             
                populations.Add(randomWeights);
            }
          

            for (int passValues = 0; passValues < populationSize; passValues++)
            {
                List<double> weights = new List<double>();
                for (int t = 0; t < 60; t++)
                {
                    weights.Add(populations[passValues][t]);
                }
                nextPopulations.Add(weights);
            }

            
            int generation = 0;

            //List<double> complete = new List<double>();
          
               
                double max = 0;
                double average = 0;

            //List of top 2 populations from random selection          
            do
            {
                List<double> scores = new List<double>();
                List<List<double>> bestSamples = new List<List<double>>();
                double crossoverChance = _rand.NextDouble();
                for (int p = 0; p < populationSize; p++)
                {
                    List<double> crossWeights = new List<double>();
                    List<List<double>> values = tournament(nextPopulations);
                    for (int n = 0; n < 2; n++)
                    {
                        List<double> winWeights = new List<double>();
                        for (int b = 0; b < 60; b++)
                        {            
                            winWeights.Add(values[n][b]);                                          
                        }
                        bestSamples.Add(winWeights);
                    }
                    if (crossoverChance < crossover)
                    {
                        double[] firstHalf = new double[30];
                        double[] secondHalf = new double[30];
                        for (int l = 0; l < 30; l++)
                        {
                           
                            firstHalf[l] = bestSamples[0][l];
                           
                            secondHalf[l] = bestSamples[1][59 - l];
                            crossWeights.Add(firstHalf[l]);
                            crossWeights.Add(secondHalf[l]);
                        }                      
                    }
                    else
                    {
                        for(int u = 0; u < 60; u++)
                        {
                            double[] complete = new double[60];
                            complete[u] = bestSamples[0][u];
                            crossWeights.Add(complete[u]);
                        }
                    }
                    
                    currentGen.Add(mutationFunction(crossWeights));
                   
                    bestSamples.Clear();
                }


                for (int j = 0; j < populationSize; j++)
                {                  
                      scores.Add(GetResults(currentGen[j]));     
                     
                }
                List<double> bestWeights = new List<double>();
                bestWeights.Clear();
                for (int findBest = 0; findBest < populationSize; findBest++)
                {
                   
                    if (GetResults(currentGen[findBest]) == scores.Max())
                    {
                        for (int t = 0; t < 60; t++)
                        {
                           
                            bestWeights.Add(currentGen[findBest][t]);
                        }
                    }
                  
                }
              
                using (StreamWriter sw = new StreamWriter("best.txt"))
                {
                    for (int best = 0; best < 60; best++)
                    {
                        sw.WriteLine(bestWeights[best]);
                    }
                }
                
               
                max = scores.Max();
                
                average = scores.Average();
                //currentGen.Clear();
                //See how these weights perform on the task when applied to the network    
                string resultMessage =  max + " " + average;
                
                string[] results = new string[runs];
              
                Console.WriteLine(resultMessage);
                for(int x = 0; x < runs; x++)
                {
                    results[x] = resultMessage;
                }
                using (StreamWriter sw = new StreamWriter("Results.txt",true))
                {               
                        sw.WriteLine(results[generation]);              
                }
               

                nextPopulations.Clear();
                for (int passValues = 0; passValues < populationSize; passValues++)
                {
                    List<double> weights = new List<double>();
                    for (int t = 0; t < 60; t++)
                    {
                        weights.Add(currentGen[passValues][t]);
                    }
                    nextPopulations.Add(weights);
                }
                
                currentGen.Clear();
                generation++;
            } while(generation < runs) ;

           
            Console.WriteLine("Complete");
            Console.ReadLine();
        }
        public static List<List<double>> tournament(List<List<double>> nextPopulations)
        {
            List<List<double>> randomSample = new List<List<double>>();
            List<List<double>> winners = new List<List<double>>();
            double[] sampleResults = new double[4];
           
            for (int m = 0; m < 4; m++)
            {
                int index = _rand.Next(0, populationSize);
                List<double> weights = new List<double>();
                for (int t = 0; t < 60; t++)
                {
                    weights.Add(nextPopulations[index][t]);
                }
                randomSample.Add(weights);
                sampleResults[m] = GetResults(randomSample[m]);
            }

            for(int b = 0; b < randomSample.Count; b++)
            {
              
               sampleResults = sampleResults.OrderByDescending(w => w).ToArray();
               if(GetResults(randomSample[b]) == sampleResults[0] || GetResults(randomSample[b]) == sampleResults[1])
                {
                    List<double> weights = new List<double>();
                    for (int t = 0; t < 60; t++)
                    {
                        weights.Add(randomSample[b][t]);
                    }
                    winners.Add(weights);
                }
            }
            return winners;
        }
        /*public static List<double> crossoverFunction(List<double> winner1, List<double> winner2)
        {
            List<double> first = new List<double>();
            var crossoverChance = _rand.NextDouble();
            
                if (crossoverChance < crossover)
                {
                    for (int k = 0; k < 30; k++)
                    {
                        List<double> firstHalf = new List<double>();
                        for(int j = 0; j < 30; j++)
                        {
                        firstHalf.Add(winner1[j]);
                        }
                        first.Add(firstHalf[k]);
                    }
                    for (int q = 0; q < 30; q++)
                    {
                        List<double> secondHalf = new List<double>();
                        for (int j = 0; j < 30; j++)
                        {
                            secondHalf.Add(winner1[59-j]);
                        }
                        first.Add(secondHalf[q]);
                    }
                return mutationFunction(first);
                }
                else
                {
                for (int noCrossover = 0; noCrossover < 60; noCrossover++)
                {
                    List<double> fullWeights = new List<double>();
                    for (int j = 0; j < 60; j++)
                    {
                        fullWeights.Add(winner1[j]);
                    }
                    first.Add(fullWeights[noCrossover]);
                    }
                return mutationFunction(first);
                }             
            
           
        }*/

        // mutates(or not) the newly created child before it is passed to the next population
        public static List<double> mutationFunction(List<double> pChild)
        {
            
            var mutationChance = _rand.NextDouble();
            var randomValue = _rand.Next(0, 60);
            // replaces a weight with a new random weight
            if (mutationChance < mutation)
            {
               pChild[randomValue] = _rand.NextDouble() * 2 - 1;
               return pChild;
            }
            else
            {
               return pChild;
            }
        }
        public static double GetResults(List<double> weights)
        {
            Network net = new Network();

            net.SetWeights(weights);

            PendulumMaths p = new PendulumMaths();
            p.initialise(1);

            Network v = new Network();
            v.SetWeights(net.GetWeights());

            double[][] motor_vals = new double[p.getcrabnum()][];

            for (int i = 0; i < motor_vals.Length; i++)
            {
                motor_vals[i] = new double[2];
            }

            do
            {
                double[][] sval = (p.getSensorValues());

                double[] inputs = new double[10];

                for (int i = 0; i < p.getcrabnum(); i++)
                {

                    for (int x = 0; x < sval[0].Length; x++)
                    {
                        inputs[x] = ((sval[i][x]) / (127) * (1 - 0)) + 1;
                    }

                    v.SetInputs(inputs);

                    v.Execute();

                    double[] outputs = v.GetOutputs();

                    motor_vals[i][0] = ((outputs[0])) * 127;
                    motor_vals[i][1] = ((outputs[1])) * 127;

                }

            }
            while (p.performOneStep(motor_vals) == 1);

            return p.getFitness();
        }
    }
    
}