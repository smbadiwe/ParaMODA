using System;
using System.Collections.Generic;
using System.Text;

namespace Motif1
{
    class Vertex
    {
        int name;
        List<int> neighbors;
        List<int> neighbors_degrees;

        public Vertex(int Name, List<int> Neighbors)
        {
            name = Name;
            neighbors = Neighbors;
        }

        public Vertex(int Name)
        {
            name = Name;

        }

        public void setNeighborsDegree(List<int> neighbors_degree)
        {
            neighbors_degrees = new List<int>();
            for (int i = 0; i < neighbors_degree.Count; i++)
                neighbors_degrees.Add(neighbors_degree[i]);

            sortNeighborsDegree();
        }

        public List<int> getNeighborsDegrees()
        {
            return neighbors_degrees;
        }

        void sortNeighborsDegree()
        {
            int temp = 0;

            for (int i = 0; i < neighbors_degrees.Count; i++)
                for (int j = i + 1; j < neighbors_degrees.Count; j++)
                    if (neighbors_degrees[i] < neighbors_degrees[j])
                    {
                        temp = neighbors_degrees[i];
                        neighbors_degrees[i] = neighbors_degrees[j];
                        neighbors_degrees[j] = temp;
                    }

        }

        public bool deleteNeighbor(int neighbor, int neighborDegree)
        {
            if (neighbors.Contains(neighbor))
            {
                neighbors.Remove(neighbor);
                neighbors_degrees.Remove(neighborDegree);
                return true;
            }
            else
                return false;
        }

        public void updateNeighborList(int degree_of_neighbor)
        {
            neighbors_degrees.Remove(degree_of_neighbor + 1);
            neighbors_degrees.Add(degree_of_neighbor);
            sortNeighborsDegree();
        }

        public Vertex()
        {
            name = 0;
        }

        public int getDegree()
        {
            return neighbors.Count;
        }

        public int getName()
        {
            return name;
        }

        public List<int> getNeighbors()
        {
            return neighbors;
        }

        public void delete()
        {
            neighbors.Clear();
            neighbors_degrees.Clear();
        }


    }
}
