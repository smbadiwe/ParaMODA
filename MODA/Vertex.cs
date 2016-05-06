using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODA
{
    public class Vertex
    {
        private int name;
        private List<int> neighbors;
        private List<int> neighbors_degrees;

        public Vertex() : this(0)
        {
        }

        public Vertex(int Name) : this(Name, new List<int>())
        {
        }

        public Vertex(int Name, List<int> Neighbors)
        {
            name = Name;
            neighbors = Neighbors;
        }

        public void SetNeighborsDegree(List<int> neighbors_degree)
        {
            neighbors_degrees = new List<int>();
            for (int i = 0; i < neighbors_degree.Count; i++)
            {
                neighbors_degrees.Add(neighbors_degree[i]);
            }
            SortNeighborsDegree();
        }

        public List<int> GetNeighborsDegrees()
        {
            return neighbors_degrees;
        }

        private void SortNeighborsDegree()
        {
            int temp = 0;

            for (int i = 0; i < neighbors_degrees.Count; i++)
            {
                for (int j = i + 1; j < neighbors_degrees.Count; j++)
                {
                    if (neighbors_degrees[i] < neighbors_degrees[j])
                    {
                        temp = neighbors_degrees[i];
                        neighbors_degrees[i] = neighbors_degrees[j];
                        neighbors_degrees[j] = temp;
                    }
                }
            }
        }

        public bool DeleteNeighbor(int neighbor, int neighborDegree)
        {
            if (neighbors.Contains(neighbor))
            {
                neighbors.Remove(neighbor);
                neighbors_degrees.Remove(neighborDegree);
                return true;
            }
            return false;
        }

        public void UpdateNeighborList(int degree_of_neighbor)
        {
            neighbors_degrees.Remove(degree_of_neighbor + 1);
            neighbors_degrees.Add(degree_of_neighbor);
            SortNeighborsDegree();
        }

        public int GetDegree()
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

        public void Delete()
        {
            neighbors.Clear();
            neighbors_degrees.Clear();
        }
    }
}
