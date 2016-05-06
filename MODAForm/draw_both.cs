using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Motif1
{
    class draw_both
    {
         Graphics pallete;
        int size;
        int node_number;
        List<bool> adj_matrix;
        List<double> node_x;
        List<double> node_y;

        List<int> domain;
        List<int> range;
        
        public draw_both(Graphics grfx,int Size,int Node_number,List<bool> Adj_Mtrx,int Node_number2,List<int> Domain,List<int> Range)
        {
            pallete = grfx;
            size = Size;
            node_number = Node_number;
            adj_matrix = new List<bool>();
            domain = new List<int>();
            range = new List<int>();
            int i = 0;

            for (i = 0; i < Adj_Mtrx.Count; i++)
                adj_matrix.Add(Adj_Mtrx[i]);

            for (i = 0; i < Domain.Count; i++)
            {
                domain.Add(Domain[i]);
                range.Add(Range[i]);
            }

        }
        public void draw(int pen_size,Color line_color,Color label_color,Color line_color2,Color label_color2)
        {
            double delta =Math.PI*2/node_number;
            double teta = 0;
            node_x = new List<double>();
            node_y = new List<double>();

            Font fnt = new Font("Arial", 8, FontStyle.Bold);
            
            int x0 = size / 2;
            int y0 = size / 2;

            double x = 0;
            double y = 0;

            double length = size / 2 - 14;
            Pen pen=new Pen(line_color,pen_size);
            

            for (int i = 0; i < node_number; i++)
            {
                teta += delta;
                if (teta > Math.PI * 2)
                    teta -= Math.PI * 2;
                x = x0 + length * Math.Cos(teta);
                y = y0 + length * Math.Sin(teta);

                pallete.DrawEllipse(pen, (int)x - 2, (int)y - 2, 4, 4);
                pallete.DrawString(i.ToString(), fnt, new SolidBrush(label_color), (int)x - 10, (int)y);
                node_x.Add(x);
                node_y.Add(y);
                
            }
            draw_all_edge(line_color, 1);
            draw_mapped_edge(line_color2,pen_size,label_color2);
                

        }
        void draw_mapped_edge(Color clr,int size,Color label_color)
        {
            Pen pen = new Pen(clr, size + 1);
//            pen.DashStyle = DashStyle.Dash;
            
            int f1;
            int f2;
            Font fnt = new Font("Arial", 8, FontStyle.Bold);
            for (int i = 0; i < node_number; i++)
                for (int j = i + 1; j < node_number; j++)
                    if (is_link_between(i, j))
                    {
                        f1 = find_range(i, domain, range);
                        f2 = find_range(j, domain, range);
                        if (f1 == -1 || f2 == -1)
                            return;
                        pallete.DrawLine(pen, (int)node_x[f1], (int)node_y[f1],
                            (int)node_x[f2], (int)node_y[f2]);

                       // pallete.DrawString(i.ToString(), fnt, new SolidBrush(label_color), (int)node_x[f1] - 10, (int)node_y[f1]);
                        //pallete.DrawString(j.ToString(), fnt, new SolidBrush(label_color), (int)node_x[f2] - 10, (int)node_y[f2]);

                    }

        }

        int find_range(int domain, List<int> domain_list, List<int> range_list)
        {
            for (int i = 0; i < domain_list.Count; i++)
                if (domain_list[i] == domain)
                    return range[i];
            return -1;
        }

        void draw_all_edge(Color clr,int size)
        {
            Pen pen=new Pen(clr,size);
            for (int i = 0; i < node_number; i++)
                for (int j = i + 1; j < node_number; j++)
                    if (is_link_between(i, j))
                    {
                       // if (!domain.Contains(i) || !domain.Contains(j))
                            pallete.DrawLine(pen, (int)node_x[i], (int)node_y[i],
                                (int)node_x[j], (int)node_y[j]);
                    }

        }
        bool is_link_between(int node1, int node2)
        {
            int tmp_n1 = node1;
            int tmp_n2 = node2;
            int index = (tmp_n1 * (2 * node_number - 1 - tmp_n1) / 2) +
                 (tmp_n2 - tmp_n1 - 1);
            return (adj_matrix[index]);

        }


    }
}
