using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Charting=System.Windows.Forms.DataVisualization.Charting;

namespace cribEXP
{
    public partial class Form1 : Form
    {
        PictureBox[] cardPics;
        PictureBox[] selCards;
        Charting.Chart[] charts;
        public Form1()
        {
            InitializeComponent();
            int x = 0;
            selCards = new PictureBox[6];
            cardPics = new PictureBox[54];
            for (int i = 0; i < 54; i++)
            {
                cardPics[i] = new PictureBox();
                cardPics[i].Name = "c"+i;
                cardPics[i].Size = new Size(75, 108);
                cardPics[i].Image = (Bitmap)Resource1.ResourceManager.GetObject("c"+i.ToString());
                cardPics[i].SizeMode = PictureBoxSizeMode.Zoom;
                cardPics[i].Location = new Point(x, 15);
                cardPics[i].BorderStyle = BorderStyle.FixedSingle;
                cardPics[i].Click += new EventHandler(Card_Click);
                cardPics[i].Tag = new card(i);
                cardTableau.Controls.Add(cardPics[i]);
                cardPics[i].BringToFront();
                x += 15;
            }
            cardPics[52].Visible = false;
            cardPics[53].Visible = false;

            charts = new Charting.Chart[15];
            for (int i = 0; i < 15; i++)
            {
                charts[i] = new Charting.Chart();
                charts[i].Dock = DockStyle.Fill;
                charts[i].ChartAreas.Add("main");
                charts[i].ChartAreas[0].AxisX.LabelAutoFitStyle = System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.DecreaseFont;
                tableLayoutPanel1.Controls.Add(charts[i], i/4, i%4);
            }
        }

        void Card_Click(object sender, EventArgs e)
        {
            var c = (PictureBox)sender;
            int idx = Array.IndexOf<PictureBox>(selCards, c);
            if (idx >= 0)
            {
                c.Location = new Point(c.Location.X, 15);
                for (int i = idx; i < selCards.Length-1; i++)
                    selCards[i]=selCards[i+1];
                selCards[selCards.Length-1] = null;
            }
            else
            {
                idx = Array.IndexOf<PictureBox>(selCards, null);
                if (idx >= 0)
                {
                    c.Location = new Point(c.Location.X, 0);
                    selCards[idx] = c;
                }
            }

            if (Array.IndexOf<PictureBox>(selCards, null) >= 0)
            {
                clearGraphs();
            }
            else // 6 cards have been selected so run analysis of the discards
            {
                runAnalysis();
            }
        }
        private void clearGraphs()
        {
            foreach (var chart in charts) chart.Series.Clear();
        }
        private void runAnalysis()
        {
            clearGraphs();
            var cards = Array.ConvertAll<PictureBox, card>(selCards, p => (card)p.Tag);
            Array.Sort<card>(cards, (x,y) => x.id.CompareTo(y.id));
            var dists = Program.discardDist(cards);
            for (int i = 0; i < dists.Length; i++)
            {
                int lim = 5;
                for (int j = 5; j > 0; j--, lim+=j)
                {
                    if (i < lim)
                    {
                        charts[i].Series.Add(String.Format("Discard {0},{1}", cards[5-j], cards[i-(lim-j)+(5-j)+1]));
                        break;
                    }
                }
                for (int j = 0; j < dists[i].Length; j++)
                {
                    charts[i].Series[0].Points.AddXY(j, dists[i][j]);
                    charts[i].Series[0].Points[j].AxisLabel = j.ToString();
                }
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            
            //DateTime ts = DateTime.Now;
            //var deck = new deck();
            //deck.shuffle();
            //var hand = deck.nextCards(6);
            //var dist = Program.discardDist(hand, deck);
            //int sumprod = 0, cnt = 0, min = int.MaxValue, max = 0;
            //for (int i = 0; i < dist.Length; i++) { 
            //    if (dist[i] > 0)
            //    {
            //        min = Math.Min(i, min);
            //        max = i;
            //    }

            //    sumprod += i*dist[i]; 
            //    cnt += dist[i]; 
            //}
            //var avg = ((double)sumprod / cnt);
            //var mdnIdx = (cnt+1)/2.0;
            //var lastHit = -1;
            //double median = -1;
            //cnt = 0;
            //for (int i = 0; i < dist.Length; i++)
            //{
            //    if (dist[i] > 0) lastHit = i;
            //    cnt += dist[i];
            //    if (median < 0 && cnt >= mdnIdx)
            //    {
            //        if ((cnt-dist[i]+1)-mdnIdx == .5)
            //            median = (i+lastHit)/2.0;
            //        else
            //            median = i;
            //    }
            //}
            //Console.WriteLine("Min = {0,3}  Max = {1,3}  Median = {2:0.0} Avg = {3:0.00}", min, max, median, avg);
            //chart1.ChartAreas[0].AxisX.Interval = 1;
            //chart1.ChartAreas[0].AxisX.LabelAutoFitStyle = System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.DecreaseFont;

            //for (int i = 0; i < dist.Length; i++)
            //{
            
            //    chart1.Series[0].Points.AddXY(i, dist[i]/(double)cnt);
            //    chart1.Series[0].Points[i].AxisLabel = i.ToString();
                //if (dist[i] > 0)
                //    Console.Write("{0,3} pts : {1,6:.00%}", i, dist[i]/(double)cnt);
                //else
                //    Console.Write("{0,3} pts : ", i);
                //for (int j = 0; j < dist[i]; j++)
                //    Console.Write("+");
                //Console.WriteLine();
            //}
        }


    }
}
