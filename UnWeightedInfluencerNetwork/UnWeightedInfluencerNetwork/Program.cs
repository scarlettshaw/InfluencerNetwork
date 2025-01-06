using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Xml.Linq;
class InfluencerList
{
    LinkedList<int>[] adjacencyList;//using an adjacency list
    Tuple<double, int> bestscore = new Tuple<double, int>(0, 0);//stores the best score that is checked and updated by influencer score comparing each bin
    Tuple<double, int> worstscore = new Tuple<double, int>(1000, 1000);//stores worst influencer score

    public InfluencerList(int vertices)//in the adjacency list there will be an entry for each node
    {
        adjacencyList = new LinkedList<int>[vertices];
        for (int i = 0; i < adjacencyList.Length; ++i)
        {
            adjacencyList[i] = new LinkedList<int>();
        }
    }

    public void updateBestScore(Tuple<double, int> nbestscore)//checks if the best score is beaten by the newest influencer score

    {

        if (nbestscore.Item1 > this.bestscore.Item1)

            this.bestscore = nbestscore;


    }


    public void updateWorstScore(Tuple<double, int> nworstscore)//checks if the worst score is better than the newest influencer score
    {

        if (nworstscore.Item1 < this.worstscore.Item1)

            this.worstscore = nworstscore;


    }

    public Tuple<double, int> getBestScore() { return this.bestscore; }
    //getters for the best and worst score
    public Tuple<double, int> getWorstScore() { return this.worstscore; }

    public void addEdge(int srcVertex, int destVertex)//adds a connection to a nodes entry in the adjacency list
    {
        adjacencyList[srcVertex].AddLast((destVertex));
        //for the source nodes adjancency list entry we add it's connected node and the weight of that connection
    }

    public int getNumberOfNodes()
    {
        return adjacencyList.Length;//returns how many nodes in adjacency list
    }
    public LinkedList<int>[] getAdjList()
    {
        return adjacencyList;//returns adjacency list
    }
    public int[] BFS(LinkedList<int>[] adjacencylist, int nodes, int j)
    {
        bool[] visited = new bool[nodes + 1];//keeps track of whether each node has been visited(true/false)
        int[] distances = new int[nodes + 1];//initializes the distances array
        for (int i = 0; i <= nodes; i++)
        {
            distances[i] = int.MaxValue;
        }
        Queue<int> queue = new Queue<int>();//queue to process each node as it comes
        queue.Enqueue(j);//enqueues source node
        visited[j] = true;//source node is visited
        distances[j] = 0;//distance of 0
        while (queue.Count != 0)
        {
            int currentVertex = queue.Dequeue();//dequeue current node
            Console.Write(currentVertex + " ");
            foreach (int edge in adjacencylist[currentVertex])//looks at the relevant linked list in the adjancency list 
            {
                {

                    if (!visited[edge])//if edge in this list has not been visited
                    {
                        queue.Enqueue(edge);//enqueues to process in future iterations
                        visited[edge] = true;//edge is set as visited
                        distances[edge] = distances[currentVertex] + 1;//updates the shortest distance of this node from the source node
                    }
                }
            }

        }

        return distances;
    }
    public void printGraph()//prints adjacency list and it's nodes with it's connections
    {
        int i = 0;
        foreach (LinkedList<int> list in adjacencyList)//searches through
        {
            Console.Write("adjacencyList[" + i + "] -> "); ;//the adjacency list for "user" i
            foreach (int edge in list)
                Console.Write(edge + ") -> ");//connected to user without weight
            ++i;
            Console.WriteLine("Null"); ;//signals end of linked list
        }
    }



    public double influencerscore(int[] distances, int node)
    {

        double top = getNumberOfNodes() - 2; //extra minus one accounts for the first adjacency list as i don't have a user zero. i want it to be readable so 
        //it is better to have my list for 1 map correctly. instead of my list 0 being for the user 1
        double bottom = 0;
        foreach (int distance in distances)//for each distance in the distance array
        {
            if (distance != int.MaxValue) //if the connection is there
                bottom += distance;//add the amount of edges to the destination to my bottom total
        }
        return top / bottom;
    }





}


public class graphMain
{
    public static void Main()
    {
        int nodes = 9;//count of nodes in my program(including the 0)
        InfluencerList influencer = new InfluencerList(nodes);//create a new influencer list with 11 entries

        int s, d;//stores source node, destination node
        string fileName = "C:\\Users\\Scarz\\source\\repos\\BFS\\BFS\\TextFile1.txt";
        if (File.Exists(fileName))
        {
            StreamReader sr = new StreamReader(File.OpenRead(fileName));
            while (!sr.EndOfStream)//while i have not reached the end of the file
            {
                string[] edge = sr.ReadLine().Split(' ');//read line, and split each variable at the space
                s = Convert.ToInt32(edge[0]);//first read is always the source node
                d = Convert.ToInt32(edge[1]);//destination node

                influencer.addEdge(s, d);//add the edge to the source nodes adjacency list
            }
        }

        influencer.printGraph();//print the adjacency list with all connections in a linked list format

        for (int j = 1; j < influencer.getNumberOfNodes(); j++)//for each influencer a distance array is processed
        {


            int[] distances = influencer.BFS(influencer.getAdjList(), nodes, j);//create a distance array from the source node j

            for (int i = 1; i < influencer.getNumberOfNodes(); i++)//for each influencer we print out it's connection to this source node and the distance from it
            {



                if (distances[i] < int.MaxValue)//checks for connection between these nodes
                {
                    Console.WriteLine("Node:" + i + " to " + j + "distance " + distances[i]);
                }
                else
                {
                    Console.WriteLine("Node:" + i + " to " + j + " been visited: " + distances[i]);
                }


                if (i == influencer.getNumberOfNodes() - 1)//if we have reached the last connection possible
                {
                    //we calculate the influence score and sprint it, we check against the stored best and worst scores and update them if necessary
                    Console.WriteLine("Influencer score " + influencer.influencerscore(distances, i));
                    Tuple<double, int> nbestscore = new Tuple<double, int>(influencer.influencerscore(distances, i), j);
                    influencer.updateBestScore(nbestscore);

                    Tuple<double, int> nworstscore = new Tuple<double, int>(influencer.influencerscore(distances, i), j);
                    influencer.updateWorstScore(nworstscore);
                }
            }

        }
        //prints out the best and worst performing nodes
        Console.WriteLine("Node " + influencer.getBestScore().Item2 + " has the best influencer score of " + influencer.getBestScore().Item1);
        Console.WriteLine("Node " + influencer.getWorstScore().Item2 + " has the worst influencer score of " + influencer.getWorstScore().Item1);
    }
}
