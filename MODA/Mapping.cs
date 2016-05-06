using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODA
{
    public class Mapping
    {
        List<Map> mapping;
        List<int> domain;
        List<int> range;

        public Mapping() : this(new List<Map>())
        {
        }

        public Mapping(List<Map> maps)
        {
            mapping = new List<Map>();
            domain = new List<int>();
            range = new List<int>();
            for (int i = 0; i < maps.Count; i++)
            {
                mapping.Add(maps[i]);
                domain.Add(maps[i].Domain);
                range.Add(maps[i].Range);
            }
        }

        public List<int> GetRange()
        {
            return range;
        }

        public List<int> GetDomain()
        {
            return domain;
        }

        // add a function to avoid repeated domains

        public void clear()
        {
            mapping.Clear();
            domain.Clear();
            range.Clear();
        }

        public void AddMap(Map map)
        {
            mapping.Add(map);
            domain.Add(map.Domain);
            range.Add(map.Range);
        }

        public int get_length()
        {
            return mapping.Count;
        }
        
        public int FindMap(int item)
        {
            for (int i = 0; i < domain.Count; i++)
            {
                if (domain[i] == item) return i;
            }
            return -1;
        }

        public int GetRange(int domain)
        {
            return mapping[FindMap(domain)].Range;
        }

        public Map GetMap(int index)
        {
            return mapping[index];
        }

        public List<Map> GetMapping()
        {
            return mapping;
        }
    }
}
