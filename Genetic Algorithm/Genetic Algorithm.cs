using System;
using System.IO;
using System.Diagnostics;

namespace Genetic_Algorithm
{
    class Population
    {
        public TimeSpan Elapsed { get; }
        public string[] chromosome;
        public int positionX;
        public int positionY;
        public double fitness;
        public double fPercentage;

        public Population(Random random, int steps)
        {
            chromosome = new string[3 * steps];
            for (int i = 0; i < chromosome.Length; i++)
            {
                chromosome[i] = Convert.ToString(random.Next(0, 2));
            }
        }

        public void Set_Fitness(Node End_Node, Node Start_Node)
        {
            int DistanceX = Math.Abs(End_Node.positionX - positionX);
            int DistanceY = Math.Abs(End_Node.positionY - positionY);
            double distanceEnd;
            if (DistanceX > DistanceY)
            {
                distanceEnd = ((14 * DistanceY) + (10 * (DistanceX - DistanceY)));
            }
            else
            {
                distanceEnd = ((14 * DistanceX) + (10 * (DistanceY - DistanceX)));
            }
            int distanceStartX = Math.Abs(Start_Node.positionX - positionX);
            int distanceStartY = Math.Abs(Start_Node.positionY - positionY);
            double distanceStart;
            if (distanceStartX > distanceStartY)
            {
                distanceStart = ((14 * distanceStartY) + (10 * (distanceStartX - distanceStartY)));
            }
            else
            {
                distanceStart = ((14 * distanceStartX) + (10 * (distanceStartY - distanceStartX)));     //14 and 10 used for diagonal and straight manoeuvrability between nodes
            }
            fitness = Math.Abs(1 / (distanceEnd - distanceStart + 1));
        }
    }

    class Node
    {
        public int positionX;
        public int positionY;
        public bool Walls;
    }
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            Population[] Agents = new Population[4];
            int Columns = 0;
            int Rows = 0;
            string mazeString = "";
            Console.WriteLine("Please enter the filename");
            string FileName = Console.ReadLine();
                using (StreamReader sr = new StreamReader(FileName + ".txt"))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        mazeString = line;
                    }
                }



                if (mazeString[2] == ' ' && mazeString[4] == ' ')
                {
                    string number = (mazeString[0] + "" + mazeString[1]);
                    Columns = Convert.ToInt32(number);
                    Rows = Convert.ToInt32(Convert.ToString(mazeString[3]));
                }
                else if (mazeString[2] == ' ' && mazeString[5] == ' ')
                {
                    string number = (mazeString[0] + "" + mazeString[1]);
                    Columns = Convert.ToInt32(number);
                    string number2 = (mazeString[3] + "" + mazeString[4]);
                    Rows = Convert.ToInt32(number2);
                }
                else if (mazeString[1] == ' ' && mazeString[4] == ' ')
                {
                    string number3 = (mazeString[2] + "" + mazeString[3]);
                    Columns = Convert.ToInt32(Convert.ToString(mazeString[0]));
                    Rows = Convert.ToInt32(number3);
                }
                else
                {
                    Columns = Convert.ToInt32(Convert.ToString(mazeString[0]));
                    Rows = Convert.ToInt32(Convert.ToString(mazeString[2]));
                }
                
                Node[,] maze = new Node[Rows, Columns];
                for (int i = 0; i < Rows; i++)
                {
                    for (int j = 0; j < Columns; j++)
                    {
                        
                        maze[i, j] = new Node();
                        maze[i, j].positionX = j;
                        maze[i, j].positionY = i;
                    }

                }     
                int index;
                
                //these IF and Else statements are used to set the "index" value which is later used to map the mazestring onto the 2D array
                //these statements once again account for double digit widths/lengths mazes
                
            if (Columns > 9)
                {
                    if (Rows > 9)
                    {
                        index = 6;
                    }
                    else
                    {
                        index = 5;
                    }
                }
                else if (Rows > 9)
                {
                    index = 5;
                }
                else
                {
                    index = 4;
                }

                int startX = 0;
                int startY = 0;
                int targetX = 0;
                int targetY = 0;
                
                for (int i = 0; i < Rows; i++)
                {
                    for (int j = 0; j < Columns; j++)
                    {
                        if (mazeString[index] == '1')
                        {
                            maze[i, j].Walls = true;
                        }
                        else if (mazeString[index] == '2')
                        {
                            startX = j;
                            startY = i;
                        }
                        else if (mazeString[index] == '3')
                        {
                            targetX = j;
                            targetY = i;
                        }
                        index += 2;
                    }
                }
                Random r = new Random();
                int n_moves = 8;
                int bitsMutated = 0;
               
                for (int i = 0; i < 4; i++)
                {
                    Agents[i] = new Population(r, n_moves);
                    Agents[i].positionX = startX;
                    Agents[i].positionY = startY;
                }
                bool finish = false;
                string[] cFitness = { "" };
                int count = 0;



                while (finish == false)
                {
                    count++;
                    if (count == 25)
                    {
                        n_moves += 2;
                        for (int i = 0; i < 4; i++)
                        {
                            Agents[i] = new Population(r, n_moves);
                            Agents[i].positionX = startX;
                            Agents[i].positionY = startY;
                        }
                        
                        Console.WriteLine();
                        count = 0;
                    }

                    bitsMutated = n_moves * 3;
                    double tFitness = 0;
                  


                    for (int i = 0; i < Agents.Length; i++)
                    {
                        for (int j = 0; j < Agents[i].chromosome.Length; j++)
                        {
                            string stepDir;
                            stepDir = Agents[i].chromosome[j] + Agents[i].chromosome[j + 1] + Agents[i].chromosome[j + 2];
                            if (stepDir == "000" && Agents[i].positionY != 0 && maze[Agents[i].positionY - 1, Agents[i].positionX].Walls == false)
                            {
                                Agents[i].positionY -= 1;
                            }
                            else if (stepDir == "001" && Agents[i].positionY != 0 && Agents[i].positionX < Columns - 1 && maze[Agents[i].positionY - 1, Agents[i].positionX + 1].Walls == false)
                            {
                                Agents[i].positionY -= 1;
                                Agents[i].positionX += 1;
                            }
                            else if (stepDir == "010" && Agents[i].positionX < Columns - 1 && maze[Agents[i].positionY, Agents[i].positionX + 1].Walls == false)
                            {
                                Agents[i].positionX += 1;
                            }
                            else if (stepDir == "011" && Agents[i].positionY < Rows - 1 && Agents[i].positionX < Columns - 1 && maze[Agents[i].positionY + 1, Agents[i].positionX + 1].Walls == false)
                            {
                                Agents[i].positionY += 1;
                                Agents[i].positionX += 1;
                            }
                            else if (stepDir == "100" && Agents[i].positionY < Rows - 1 && maze[Agents[i].positionY + 1, Agents[i].positionX].Walls == false)
                            {
                                Agents[i].positionY += 1;
                            }
                            else if (stepDir == "101" && Agents[i].positionY < Rows - 1 && Agents[i].positionX != 0 && maze[Agents[i].positionY + 1, Agents[i].positionX - 1].Walls == false)
                            {
                                Agents[i].positionY += 1;
                                Agents[i].positionX -= 1;
                            }
                            else if (stepDir == "110" && Agents[i].positionX != 0 && maze[Agents[i].positionY, Agents[i].positionX - 1].Walls == false)
                            {
                                Agents[i].positionX -= 1;
                            }
                            else if (stepDir == "111" && Agents[i].positionY != 0 && Agents[i].positionX != 0 && maze[Agents[i].positionY - 1, Agents[i].positionX - 1].Walls == false)
                            {
                                Agents[i].positionY -= 1;
                                Agents[i].positionX -= 1;
                            }
                           
                            if (Agents[i].positionY == targetY && Agents[i].positionX == targetX)
                            {
                                finish = true;
                                Console.WriteLine(Agents[i] + " has found the finish");
                                cFitness = Agents[i].chromosome;
                                j = Agents[i].chromosome.Length;
                            }
                            j += 2;


                            
                            for (int a = 0; a < Rows; a++)
                            {
                                for (int b = 0; b < Columns; b++)
                                {
                                    if (a == Agents[0].positionY && b == Agents[0].positionX)
                                    {
                                        Console.Write(" | 0");
                                    }
                                    else if (a == Agents[1].positionY && b == Agents[1].positionX)
                                    {
                                        Console.Write(" | 1");
                                    }
                                    else if (a == Agents[2].positionY && b == Agents[2].positionX)
                                    {
                                        Console.Write(" | 2");
                                    }
                                    else if (a == Agents[3].positionY && b == Agents[3].positionX)
                                    {
                                        Console.Write(" | 3");
                                    }
                                    else if (a == startY && b == startX)
                                    {
                                        Console.Write(" | S");
                                    }
                                    else if (a == targetY && b == targetX)
                                    {
                                        Console.Write(" | F");
                                    }
                                    else if (maze[a, b].Walls == true)
                                    {
                                        Console.Write(" | X");
                                    }
                                    else
                                    {
                                        Console.Write(" |  ");
                                    }
                                }
                                Console.Write(" |");
                                Console.WriteLine();
                                Console.Write("  ");
                                for (int k = 0; k < Columns; k++)
                                {
                                    Console.Write("----");
                                }
                                Console.WriteLine();
                            }

                        }
                        if (finish == true)
                        {
                            break;
                        }

                        Agents[i].Set_Fitness(maze[targetY, targetX], maze[startY, startX]);
                        tFitness += Agents[i].fitness;
                    }
                    if (finish == true)
                    {
                        break;
                    }

                    Population[] Crossover = new Population[4];



                    for (int d = 0; d < Agents.Length; d++)
                    {
                        Agents[d].fPercentage = (Agents[d].fitness / tFitness) * 100;
                    }
                    do
                    {
                        for (int e = 0; e < 4; e++)
                        {
                            int x = r.Next(1, 101);
                            if (x >= 0 && x <= Agents[0].fPercentage)
                            {
                                Crossover[e] = Agents[0];
                            }
                            else if (x > Agents[0].fPercentage && x <= (Agents[0].fPercentage + Agents[1].fPercentage))
                            {
                                Crossover[e] = Agents[1];
                            }
                            else if (x > (Agents[0].fPercentage + Agents[1].fPercentage) && x <= (Agents[0].fPercentage + Agents[1].fPercentage + Agents[2].fPercentage))
                            {
                                Crossover[e] = Agents[2];
                            }
                            else if (x > (Agents[0].fPercentage + Agents[1].fPercentage + Agents[2].fPercentage) && x <= 100)
                            {
                                Crossover[e] = Agents[3];
                            }
                        }
                        if (Crossover[0].chromosome == Crossover[1].chromosome)
                        {
                            Crossover[0] = null;
                            Crossover[1] = null;
                        }
                        if (Crossover[2].chromosome == Crossover[3].chromosome)
                        {
                            Crossover[2] = null;
                            Crossover[3] = null;
                        }
                        if (Crossover[0] != null && Crossover[1] != null && Crossover[2] != null && Crossover[3] != null)
                        {
                            if (((Crossover[0].chromosome == Crossover[2].chromosome) && (Crossover[1].chromosome == Crossover[3].chromosome)) || ((Crossover[0].chromosome == Crossover[3].chromosome) && (Crossover[1].chromosome == Crossover[2].chromosome)))
                            {
                                Crossover[0] = null;
                                Crossover[1] = null;
                                Crossover[2] = null;
                                Crossover[3] = null;
                            }
                        }
                    } while (Crossover[0] == null || Crossover[1] == null || Crossover[2] == null || Crossover[3] == null);
                    int crossover = r.Next(0, 11);
                    Console.WriteLine();
                    string[] chromosomeHold = new string[bitsMutated / 2];
                    string[] chromosomeHold2 = new string[bitsMutated / 2];
                    string[] chromosomeHold3 = new string[bitsMutated / 2];
                    string[] chromosomeHold4 = new string[bitsMutated / 2];
                    string[] child = new string[bitsMutated];
                    string[] child2 = new string[bitsMutated];
                    string[] child3 = new string[bitsMutated];
                    string[] child4 = new string[bitsMutated];



                    if (crossover <= 7)
                    {
                        Console.WriteLine();
                      
                        for (int f = 0; f < bitsMutated; f++)
                        {
                            if (f == bitsMutated / 2)
                            {
                                Console.Write(" ");
                            }
                            Console.Write(Crossover[0].chromosome[f]);
                        }
   
                      
                        for (int g = 0; g < bitsMutated; g++)
                        {
                            if (g == bitsMutated / 2)
                            {
                                Console.Write(" ");
                            }
                            Console.Write(Crossover[1].chromosome[g]);
                        }
                      
                        Console.WriteLine();
                        for (int i = 0; i < (bitsMutated / 2); i++)
                        {   chromosomeHold[i] = Crossover[0].chromosome[i];
                            chromosomeHold2[i] = Crossover[1].chromosome[i + (bitsMutated / 2)];
                            chromosomeHold3[i] = Crossover[1].chromosome[i];
                            chromosomeHold4[i] = Crossover[0].chromosome[i + (bitsMutated / 2)]; 
                        }

                        chromosomeHold.CopyTo(child, 0);
                        chromosomeHold2.CopyTo(child, (bitsMutated / 2));
                        chromosomeHold3.CopyTo(child2, 0);
                        chromosomeHold4.CopyTo(child2, (bitsMutated / 2));
                       
                        for (int h = 0; h < bitsMutated; h++)
                        {
                            if (h == bitsMutated / 2)
                            {
                                Console.Write(" ");
                            }
                            Console.Write(child[h]);
                        }

                        for (int p = 0; p < bitsMutated; p++)
                        {
                            if (p == bitsMutated / 2)
                            {
                                Console.Write(" ");
                            }
                            Console.Write(child2[p]);
                        }
                        Console.WriteLine();

                    }
                    else
                    {

                        for (int j = 0; j < bitsMutated; j++)
                        {
                            if (j == bitsMutated / 2)
                            {
                                Console.Write(" ");
                            }
                            Console.Write(Crossover[0].chromosome[j]);
                        }
                        child = Crossover[0].chromosome;

                        for (int k = 0; k < bitsMutated; k++)
                        {
                            if (k == bitsMutated / 2)
                            {
                                Console.Write(" ");
                            }
                            Console.Write(Crossover[1].chromosome[k]);
                        }
                        child2 = Crossover[1].chromosome;
                        Console.WriteLine();
                    }
                    crossover = r.Next(1, 11);



                    if (crossover <= 7)
                    {

                        for (int l = 0; l < bitsMutated; l++)
                        {
                            if (l == bitsMutated / 2)
                            {
                                Console.Write(" ");
                            }
                            Console.Write(Crossover[2].chromosome[l]);
                        }
                        for (int m = 0; m < bitsMutated; m++)
                        {
                            if (m == bitsMutated / 2)
                            {
                                Console.Write(" ");
                            }
                            Console.Write(Crossover[3].chromosome[m]);
                        }
                        Console.WriteLine();

                        int n = 0;
                        while (n < (bitsMutated / 2))
                        {
                            chromosomeHold[n] = Crossover[2].chromosome[n];
                            chromosomeHold2[n] = Crossover[3].chromosome[n + (bitsMutated / 2)];
                            chromosomeHold3[n] = Crossover[3].chromosome[n];
                            chromosomeHold4[n] = Crossover[2].chromosome[n + (bitsMutated / 2)];
                            n++;
                        }

                        chromosomeHold.CopyTo(child3, 0);
                        chromosomeHold2.CopyTo(child3, (bitsMutated / 2));
                        chromosomeHold3.CopyTo(child4, 0);
                        chromosomeHold4.CopyTo(child4, (bitsMutated / 2));

                        for (int o = 0; o < bitsMutated; o++)
                        {
                            if (o == bitsMutated / 2)
                            {
                                Console.Write(" ");
                            }
                            Console.Write(child3[o]);
                        }

                        for (int p = 0; p < bitsMutated; p++)
                        {
                            if (p == bitsMutated / 2)
                            {
                                Console.Write(" ");
                            }
                            Console.Write(child4[p]);
                        }
                        Console.WriteLine();
                    }
                    else
                    {

                        for (int q = 0; q < bitsMutated; q++)
                        {
                            if (q == bitsMutated / 2)
                            {
                                Console.Write(" ");
                            }
                            Console.Write(Crossover[2].chromosome[q]);
                        }
                        child3 = Crossover[2].chromosome;

                        for (int s = 0; s < bitsMutated; s++)
                        {
                            if (s == bitsMutated / 2)
                            {
                                Console.Write(" ");
                            }
                            Console.Write(Crossover[3].chromosome[s]);
                        }
                        child4 = Crossover[3].chromosome;
                        Console.WriteLine();
                    }
                    Agents[0].chromosome = child;
                    Agents[1].chromosome = child2;
                    Agents[2].chromosome = child3;
                    Agents[3].chromosome = child4;
                    Console.WriteLine();



                    for (int s = 0; s < 4; s++)
                    {
                        for (int j = 0; j < bitsMutated; j++)
                        {
                            int mutation = r.Next(0, bitsMutated);
                            if (mutation == 0)
                            {
                                if (Agents[s].chromosome[j] == "1")
                                {
                                    Agents[s].chromosome[j] = "0";
                                }
                                else
                                {
                                    Agents[s].chromosome[j] = "1";
                                }
                                Console.WriteLine("Chromosome " + s + " has mutated");
                            }
                        }
                    }



                    for (int t = 0; t < Agents.Length; t++)
                    {
                        Agents[t].fitness = 0;
                        Agents[t].fPercentage = 0;
                        Agents[t].positionX = startX;
                        Agents[t].positionY = startY;
                        Console.WriteLine();
                        Console.WriteLine(t + ": ");



                        for (int p = 0; p < bitsMutated; p++)
                        {
                            if (p == bitsMutated / 2)
                            {
                                Console.Write(" ");
                            }
                            Console.Write(Agents[t].chromosome[p]);
                        }
                        Console.WriteLine();
                    }
                    Console.WriteLine();
                }
                   stopWatch.Stop();
                TimeSpan tElapsed = stopWatch.Elapsed;
                Console.WriteLine("Winning chromosome: ");






                for (int u = 0; u < bitsMutated; u++)
                {
                    Console.Write(cFitness[u]);
                }
            Console.Write("");
            Console.WriteLine("Time Elapsed: " + tElapsed);
            Console.ReadLine();
            
            
        }
    }
}