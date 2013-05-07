using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CBTools.MTrand;

namespace cribEXP
{
        public class deck
        {
            internal int[] cards;
            internal int nxtCard;
            public deck()
            {
                cards = new int[52];
                reset();
            }
            public void reset()
            {
                for (int i = 0; i < 52; i++)
                    cards[i] = i;
                nxtCard = 0;
            }
            public void shuffle()
            {
                KnuthShuffle();
                nxtCard = 0;
            }
            void swap(ref int x, ref int y)
            {
                int tmp = x;
                x = y;
                y = tmp;
            }
            void KnuthShuffle() 
            { 
                int rand;
                for(int i=51;i>=0;i--)
                {
                    rand=(int)(MTrand.genrand()*52);
                    swap(ref cards[i], ref cards[rand]);
                }
            }
            public int[] nextCards(int cnt)
            {
                var rv = new int[cnt];
                Array.Copy(cards, nxtCard, rv, 0, cnt);
                nxtCard += cnt;
                return rv;
            }
            public int nextCard()
            {
                return cards[nxtCard++];
            }
        }
        public struct card
        {
            private int mId;
            public int id { get { return mId; } }
            public int suit { get { return mId/13+1; } }
            public int face { get { return mId%13+1; } }
            public int value { get { return Math.Min(face, 10); } }
            public card(int card)
            {
                mId = card;
            }
            public override string ToString()
            {
                string rv;
                if (id == 52 || id == 53)
                    return "Jk";

                switch (face)
                {
                    case 11:
                        rv="J";
                        break;
                    case 12:
                        rv="Q";
                        break;
                    case 13:
                        rv="K";
                        break;
                    default:
                        rv=face.ToString();
                        break;
                }
                switch (suit)
                {
                    case 1:
                        rv+="♣";
                        break;
                    case 2:
                        rv+="♦";
                        break;
                    case 3:
                        rv+="♥";
                        break;
                    case 4:
                        rv+="♠";
                        break;
                }
                return rv;
            }
        }
        public class hand
        {
            public card[] cards;
            public hand(int[] cards)
            {
                this.cards = new card[cards.Length];
                for (int i = 0; i < cards.Length; i++)
                {
                    this.cards[i] = new card(cards[i]);
                }
            }
            public hand(card[] cards)
            {
                this.cards = cards;
            }
            public card this[int index] { get { return cards[index]; } }
            public int Count { get { return cards.Length; } }
        }
        public class CribbageHand : hand
        {
            public CribbageHand(int[] cards) : base(cards) {}

            private static card[] cA = new card[5];
            public static int score(hand h, card turn, bool crib)
            {           
                int score = 0;
                h.cards.CopyTo(cA, 0);
                cA[4] = turn;
                Array.Sort<card>(cA, (x, y) => { if (x.face != y.face)
                                                    return x.face-y.face;
                                                 return x.suit-y.suit;
                                               } );

                for (int i = 0; i < h.Count; i++)
                {
                    bool flush = (i == 0);
                    if (h[i].face == 11 && h[i].suit == turn.suit) score++; // his nibs
                    if (h[i].face == turn.face) score += 2; // pair
                    for (int j = i+1; j < h.Count; j++)
                    {
                        if (h[i].face == h[j].face)
                            score += 2; // pair
                        // flushes
                        if (flush && h[i].suit != h[j].suit)
                            flush = false;
                    }
                    if (flush)
                        if (h[i].suit == turn.suit)
                            score += 5;
                        else if (!crib)
                            score += 4;
                }

                // Straights - count the runs&repeats at the same time.
                int last = cA[0].face;
                int repeats = 1;
                int combos = 1;
                int run = 1;
                for (int i = 1; i < cA.Length; i++)
                {
                    if (cA[i].face == last) 
                    {
                        repeats++;
                    }
                    else if (cA[i].face == last+1)
                    {
                        combos *= repeats;
                        repeats=1;
                        run++;
                    }
                    else if (run >= 3)
                    {
                        break;
                    }
                    else
                    {
                        repeats = 1;
                        combos = 1;
                        run = 1;
                    }
                    last = cA[i].face;
                }
                if (run >= 3)
                    score += run*combos;

                // 15 counts - use the bits of an int to generate all the combos
                for (int i = 1; i <= 0x1F; i++)
                {
                    int sum = 0;
                    int tmp = i;
                    int j = 0;
                    while (tmp > 0)
                    {
                        if ((tmp & 0x1) == 0x1)
                            sum += cA[j].value;
                        tmp >>= 1;
                        j++;
                    }
                    if (sum == 15)
                        score += 2;
                }
                return score;
            }
        }
    class Program
    {
        //public class hand
        //{
        //    int[] cards;
        //    public hand(int[] cards)
        //    {
        //        digestCards(cards);
        //    }
        //    private void digestCards(int[] cards)
        //    {
        //        var suits = new sint[] { (sint)(-1), -1, -1, -1 };
        //        var suitidx = (sint)0;
        //        this.cards = (int[])cards.Clone();
        //        Array.Sort(this.cards, (x, y) => ((x%13)+(x/4)*.2).CompareTo((y%13)+(y/13)*.2));

        //        for (int i = 0; i < 12; i++)
        //        {
        //            var suit = this.cards[i] / 13;
        //            if (suits[suit] < 0)
        //            {
        //                suits[suit] = suitidx++;
        //            }
        //            this.cards[i] = (int)(this.cards[i]%13 + 13*suits[suit]);
        //        }
        //    }

        //    public override int GetHashCode()
        //    {
        //        long sum = 0;
        //        for (int i = 0; i < 6; i++)
        //        {
        //            sum += (cards[i]+cards[i+6])*(int)Math.Pow(52, i);
        //        }
        //        return sum.GetHashCode();
        //    }
        //    public override bool Equals(object obj)
        //    {
        //        var h = (hand)obj;
        //        for (int i = 0; i < 12; i++)
        //        {
        //            if (h.cards[i] != this.cards[i])
        //                return false;
        //        }
        //        return true;
        //    }
        //}
        static bool handsPossible(int[] h1, int[] h2)
        {
            int i = 0, j = 0;
            int cnt = 0;
            while (i < h1.Length)
            {
                cnt = 0;
                int item = h1[i];
                while (i < h1.Length && h1[i] == item) { i++; cnt++; }

                while (j < h2.Length && item >  h2[j]) { j++;        }
                while (j < h2.Length && item == h2[j]) { j++; cnt++; }
                if (cnt > 4)
                    return false;
            }
            return true;
        }
        public static card[] leftovers(card[] hand)
        {
            var rv = new card[52-hand.Length];
            int idx =0;
            for (int i = 0; i < 52; i++)
            {
                if(!hand.Any<card>(x => i == x.id))
                    rv[idx++] = new card(i);
            }
            return rv;
        }
        public static int[][] discardDist(card[] hand)
        {
            var cntA = new int[15][];
            int cnti = 0;
            var cards = new card[4];
            var rem = leftovers(hand);
            for (int i = 0; i < hand.Length; i++) // first discard
                for (int j = i+1; j < hand.Length; j++, cnti++) // second discard
                {
                    cntA[cnti] = new int[30];
                    int l = 0;
                    for (int k = 0; k < hand.Length; k++)
                    {
                        if (k == i || k == j) continue;
                        cards[l++] = hand[k];
                    }
                    var h = new hand(cards);
                    for (int k = 0; k < rem.Length; k++)
                    {
                        int score = CribbageHand.score(h, rem[k], false);
                        cntA[cnti][score]++;
                    }
                }
            return cntA;
        }
        public static int[] discardDist(int[] hand, deck deck)
        {
            var cards = new int[4];
            var cntA = new int[30];
            for (int i = 0; i < hand.Length; i++) // first discard
                for (int j = i+1; j < hand.Length; j++) // second discard
                {
                    int l = 0;
                    for (int k = 0; k < hand.Length; k++)
                    {
                        if (k == i || k == j) continue;
                        cards[l++] = hand[k];
                    }
                    var h = new hand(cards);
                    for (int k = deck.nxtCard; k < deck.cards.Length; k++)
                    {
                        int score = CribbageHand.score(h, new card(deck.cards[k]), false);
                        cntA[score]++;
                    }
                }
            return cntA;
        }
        static void Main(string[] args)
        {
            System.Windows.Forms.Application.Run(new Form1());
            //Console.WriteLine("Hand value = {0}", CribbageHand.score(new hand(hand), new card(4), false));

            //do {
            //    //for (int i = 0; i < hands.Length; i++)
            //    //{
            //    //    Console.Write("{0},", hands[i]);
            //    //} 
            //    //    Console.WriteLine();
            //    cnt++;
            //} while(incHand(hand));

            //var myHand = new int[] { 1, 1, 1, 1 };

            //do { 
            //    var opHand = new int[] { 1, 1, 1, 1 };
            //    do {
            //        if (handsPossible(myHand, opHand))
            //        {
            //            RunSim(myHand, opHand);
            //            cnt++;
            //        }
            //    } while (incHand(opHand));
            //} while (incHand(myHand));

            

            //int cnt = 0, cnt2 = 0;
            //var hh = new HashSet<hand>();// Dictionary<hand, hand>();
            //var cards = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
            //do {
            //    var h = new hand(cards);
            //    if (!hh.Contains(h))
            //    {
            //        hh.Add(h);
            //        cnt2++;
            //    }
            //    cnt++;
            //} while (incHand(cards));
            //Console.WriteLine("Valid 13-card combos: {0}  Took {1} s", cnt, DateTime.Now.Subtract(ts).TotalSeconds);

            //var cards2 = new card[] { new card(0, 1), 
            //                          new card(0, 2), 
            //                          new card(0, 3), 
            //                          new card(0, 4), 
            //                          new card(1, 0), 
            //                          new card(1, 1) };
            //do {
            //    cnt++;
            //} while (incHand2(cards2) != 0);
            //Console.WriteLine("Valid 6-card hands (suits identical): {0}", cnt);
        }

        // Goes back to make sure that this increment is not going to push our end card
        // over 13
        static bool extendCards(int[] cards, int i)
        {
            int rptCnt = 0;
            for (int j = i-1; j >= 0 && cards[j] == cards[i]; j++)
                rptCnt++;
            while (i < cards.Length-1)
            {
                if (rptCnt == 4)
                {
                    cards[i+1] = (int)(cards[i]+1);
                    rptCnt = 0;
                }
                else
                {
                    cards[i+1] = cards[i];
                    rptCnt++;
                }
                if (cards[i+1] > 13) return false;
                i++;
            }
            return true;
        }
        static bool incHand(int[] cards)
        {
            int max = cards.Length-1;
            int i = max;
            while (i >= 0 
                   && (++cards[i] > 13
                       || !extendCards(cards, i)))
            {
                i--;
            }
            return (i >= 0);
            //if (i < 0) return false;
            //int rptCnt = 0;
            //for (int j = i-1; j >= 0 && cards[j] == cards[i]; j++)
            //    rptCnt++;

            //while (i < max)
            //{
            //    if (rptCnt == 4)
            //    {
            //        cards[i+1] = (int)(cards[i]+1);
            //        rptCnt = 0;
            //    }
            //    else
            //    {
            //        cards[i+1] = cards[i];
            //        rptCnt++;
            //    }
            //    i++;
            //}

            //return (cards[max] <= 13);

            //int max = cards.Length-1;
            //int i = max;
            //while (i >= 0 && ++cards[i] > 13)
            //    i--;
            //if (i < 0) return false;
            //while (i < max)
            //{
            //    cards[i+1] = cards[i];
            //    i++;
            //}
            //return true;
        }
        //static bool incHand(int[] cards)
        //{
        //    int max = cards.Length-1;
        //    int i = max;
        //    while (i >= 0 && ++cards[i] == 52-(max-i))
        //        i--;
        //    if (i < 0) return false;
        //    while (i < max)
        //    {
        //        cards[i+1] = (int)(cards[i]+1);
        //        i++;
        //    }
        //    return true;
        //}
        //static int incHand2(card[] cards)
        //{
        //    int i = 5;
        //    while (i >= 0 && (++cards[i].face == 13 || 
        //        (i >= 4 &&                         // Check for > 4 of the same card
        //         cards[i]   == cards[i-1] &&
        //         cards[i-1] == cards[i-2] &&
        //         cards[i-2] == cards[i-3] &&
        //         cards[i-3] == cards[i-4])))
        //    {
        //        i--;               
        //    }
        //    if (i < 0) return 0;

        //    return cnt;
        //}
    }
}
