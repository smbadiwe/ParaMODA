using System;
using System.Collections.Generic;
using System.Text;


namespace Motif1
{
    class Mapping
    {
        List<Map> mapping;
        List<int> domain;
        List<int> range;

        public Mapping()
        {
            mapping = new List<Map>();
            domain = new List<int>();
            range = new List<int>();
        }

        public List<int> getRange()
        {
            return range;
        }

        void setRange()
        {
            for (int i = 0; i < mapping.Count; i++)
                range.Add(mapping[i].get_range());
        }

        void setDomain()
        {
            for (int i = 0; i < mapping.Count; i++)
                domain.Add(mapping[i].get_domain());
        }

        public List<int> getDomain()
        {
            return domain;
        }

        public Mapping(List<Map> maps)
        {
               mapping=new List<Map>();
               for (int i = 0; i < maps.Count; i++)
                   mapping.Add(maps[i]);
               range = new List<int>();
               domain = new List<int>();
               setDomain();
               setRange();
        }
        // add a function to avoid repeated domains

        public void clear()
        {
            mapping.Clear();
        }
        public void AddMap(Map map)
        {
            mapping.Add(map);
            domain.Add(map.get_domain());
            range.Add(map.get_range());
        }

        public int get_length()
        {
            return (int)mapping.Count;
        }

        

        public  void set_mapping(List<Map> maps)
        {
            mapping=new List<Map>();
            domain.Clear();
            range.Clear();
            for (int i = 0; i < maps.Count; i++)
            {
                domain.Add(maps[i].get_domain());
                range.Add(maps[i].get_range());
                mapping.Add(maps[i]);
            }
        }
        public int findMap(int item)
        {
            for (int i = 0; i < domain.Count; i++)
                if (domain[i] == item)
                    return i;
            return -1;
            
        }

        public int getRange(int domain)
        {
            return mapping[findMap(domain)].get_range();
        }

        public Map get_map(int index)
        {
            return mapping[index];
        }
        public List<Map> get_mapping()
        {
            return mapping;
        }
    }
}
