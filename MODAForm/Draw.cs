using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Motif1
{
    class Draw
    {
        Graphics pallete;
        int size;
        int node_number;
        List<bool> adj_matrix;
        List<double> node_x;
        List<double> node_y;
        public Draw(Graphics grfx,int Size,int Node_number,List<bool> Adj_Mtrx)
        {
            pallete = grfx;
            size = Size;
            node_number = Node_number;
            adj_matrix = new List<bool>();
            for (int i = 0; i < Adj_Mtrx.Count; i++)
                adj_matrix.Add(Adj_Mtrx[i]);
        }
        public void draw(int pen_size,Color line_color,Color label_color)
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
            draw_all_edge(line_color, pen_size);
                

        }

        void draw_all_edge(Color clr,int size)
        {
            Pen pen=new Pen(clr,size);
            for (int i = 0; i < node_number; i++)
                for (int j = i + 1; j < node_number; j++)
                    if (is_link_between(i, j))
                        pallete.DrawLine(pen, (int)node_x[i], (int)node_y[i],
                            (int)node_x[j], (int)node_y[j]);

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