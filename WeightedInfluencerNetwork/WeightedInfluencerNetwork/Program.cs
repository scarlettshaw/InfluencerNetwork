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
    LinkedList<Tuple<int, int>>[] adjacencyList;//using an adjacency list
    Tuple<double, int> bestscore = new Tuple<double, int>(0, 0);//stores the best score that is checked and updated by influencer score comparing each bin
    Tuple<double, int> worstscore = new Tuple<double, int>(1000, 1000);//stores worst influencer score

    public InfluencerList(int vertices)//in the adjacency list there will be an entry for each node
    {
        adjacencyList = new LinkedList<Tuple<int, int>>[vertices];
        for (int i = 0; i < adjacencyList.Length; ++i)
        {
            adjacencyList[i] = new LinkedList<Tuple<int, int>>();
        }
    }

    public void updateBestScore(Tuple<double, int> nbestscore)
    {

        if (nbestscore.Item1 > this.bestscore.Item1)//checks if the best score is beaten by the newest influencer score

            this.bestscore = nbestscore;


    }


    public void updateWorstScore(Tuple<double, int> nworstscore)//checks if the worst score is better than the newest influencer score
    {

        if (nworstscore.Item1 < this.worstscore.Item1)

            this.worstscore = nworstscore;


    }

    public Tuple<double, int> getBestScore() { return this.bestscore; }//getters for the best and worst score
    public Tuple<double, int> getWorstScore() { return this.worstscore; }


    public void addEdge(int srcVertex, int destVertex, int weight)//adds a connection to a nodes entry in the adjacency list
    {
        adjacencyList[srcVertex].AddLast(new Tuple<int, int>(destVertex, weight));
        //for the source nodes adjancency list entry we add it's connected node and the weight of that connection
    }

    public int getNumberOfNodes()
    {
        return adjacencyList.Length;//returns how many nodes in adjacency list
    }

    public void printGraph()//prints adjacency list and it's nodes with it's connections
    {
        int i = 0;
        foreach (LinkedList<Tuple<int, int>> list in adjacencyList)//searches through
        {
            Console.Write("adjacencyList[" + i + "] -> ");//the adjancency list for "user" i
            foreach (Tuple<int, int> edge in list)
                Console.Write(edge.Item1 + "(" + edge.Item2 + ") -> ");//connected to user with weight x
            ++i;
            Console.WriteLine("Null");//signals end of linked list
        }
    }
    public int[] dijkstra(int src)//dijkstras algorithm
    {
        int n = getNumberOfNodes();//gets number of nodes in my graph
        int[] distance = new int[n];//creates an array of distances according to the number of nodes
        for (int i = 0; i < n; i++)//for each index in distance
            distance[i] = 1000;//the distance is a large value
        distance[src] = 0;//distance to itself is always 0
        SortedSet<Tuple<int, int>> pq = new SortedSet<Tuple<int, int>>();//sorted list to store weights in order aka smallest to largest
        pq.Add(new Tuple<int, int>(src, 0));//add the source node with it's weight of 0


        while (pq.Count > 0)//while the sorted list is not 0
        {
            Tuple<int, int> current = pq.First(); //a tuple called current stores the current source node in the sorted set
            pq.Remove(current);//remove this current element form the list
            LinkedList<Tuple<int, int>> neighbors = adjacencyList[current.Item1];//linked list to store the connections of the current source node, it finds the first connected item in it's respective 
            //adjacency list entry.

            foreach (Tuple<int, int> neighbor in neighbors)//for each connection in this neighbours linked list
            {
                if (distance[current.Item1] + neighbor.Item2 < distance[neighbor.Item1])
                //if the shortest distance to the current node from the source node and the weight of the connection the the current node added is less than the current shortest distance between the two
                {
                    distance[neighbor.Item1] = distance[current.Item1] + neighbor.Item2;//we update the shortest found distance between the two
                    pq.Add(neighbor);//add the connection to the queue to process when it's weight is the smallest
                }

            }
        }
        return distance;
    }

    public double influencerscore(int[] dist, int node)
    {

        double top = getNumberOfNodes() - 2; //extra minus one accounts for the first adjacency list as i don't have a user zero. i want it to be readable so 
        //it is better to have my list for user 1 appear correctly. instead of my list 0 being for the user 1
        double bottom = 0;
        for (int i = 1; i < getNumberOfNodes(); i++)//for each distance in the distance array
        {
            if (dist[i] != 1000)//if the connection is there
            {
                bottom += dist[i];//add the connection to my bottom total
            }

        }
        return top / bottom;
    }





}


public class graphMain
{
    public static void Main()
    {
        int nodes = 11;//count of nodes in my program(including the 0)
        InfluencerList influencer = new InfluencerList(nodes);//create a new influencer list with 11 entries

        int s, d, w;//stores source node, destination node and the weight of the connection
        string fileName = "C:\\Users\\Scarz\\source\\repos\\WeightedInfluencerNetwork\\WeightedInfluencerNetwork\\TextFile1.txt";//stores the name of my file(WILL HAVE TO UPDATE FOR OTHER USERS)
        if (File.Exists(fileName))//if my file does exist
        {
            StreamReader sr = new StreamReader(File.OpenRead(fileName));//open file
            while (!sr.EndOfStream)//while i have not reached the end of the file
            {
                string[] edge = sr.ReadLine().Split(' ');//read line, and split each variable at the space
                s = Convert.ToInt32(edge[0]);//first read is always the source node
                d = Convert.ToInt32(edge[1]);//destination node
                w = Convert.ToInt32(edge[2]);//weight 
                influencer.addEdge(s, d, w);//add the edge to the source nodes adjancency list
                //repeated until all connections have been added
            }
        }

        influencer.printGraph();//print the adjacency list with all connections in a linked list format

        for (int j = 1; j < influencer.getNumberOfNodes(); j++)//for each influencer a distance array is processed
        {


            int[] dist = influencer.dijkstra(j);//pass the source node and create an array of distances

            for (int i = 1; i < influencer.getNumberOfNodes(); i++)//for each influencer we print out it's connection to this source node and the distance from it
            {


                if (dist[i] != 1000)//checks for connection between these nodes
                {
                    Console.WriteLine("Node:{0}   Shortest_Distance from " + j + " : {1}", i, dist[i]);
                }
                else
                {
                    Console.WriteLine("Node:{0}   Shortest_Distance from " + j + " : no connection", i);
                }


                if (i == influencer.getNumberOfNodes() - 1)//if we have reached the last connection possible
                {
                    //we calculate the influence score and sprint it, we check against the stored best and worst scores and update them if neccessary
                    Console.WriteLine("Influencer score " + influencer.influencerscore(dist, i));
                    Tuple<double, int> nbestscore = new Tuple<double, int>(influencer.influencerscore(dist, i), j);
                    influencer.updateBestScore(nbestscore);
                    Tuple<double, int> nworstscore = new Tuple<double, int>(influencer.influencerscore(dist, i), j);
                    influencer.updateWorstScore(nworstscore);
                }
            }

        }

        Console.WriteLine("Node " + influencer.getBestScore().Item2 + " has the best influencer score of " + influencer.getBestScore().Item1);
        Console.WriteLine("Node " + influencer.getWorstScore().Item2 + " has the worst influencer score of " + influencer.getWorstScore().Item1);
        //prints out the best and worst performing nodes
    }
}
