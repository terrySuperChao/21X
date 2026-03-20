//ÅÆ¶Ñ
using System.Collections.Generic;


public class BarrierPokerPile:BasePokerPile
{
    public override List<IPoker> preShuffle(List<IPoker> allPoker)
    {
        List<IPoker> publicPoker = new List<IPoker>(); //¹«¹²ÅÆ
        while (true)
        {
            PokerSuit suit = PokerSuit.spade;
            int j = RandomMgr.Instance.getRangeInt(0, 750);
            if (j <= 400) //ºÚÌÒ
            {
                suit = PokerSuit.spade;
            }
            else if (j <= 500)//ºìÌÒ
            {
                suit = PokerSuit.heart;
            }
            else if (j <= 600)//Ã·»¨
            {
                suit = PokerSuit.club;
            }
            else if (j <= 750)//·½¿é
            {
                suit = PokerSuit.diamond;
            }

            List<IPoker> randomPoker = new List< IPoker >();
            for (int i = 0; i < allPoker.Count; i++)
            {
                if (suit == (PokerSuit)allPoker[i].getSuit())
                {
                    randomPoker.Add(allPoker[i]);
                }
            }

            if (randomPoker.Count > 0) {
                int m = RandomMgr.Instance.getRangeInt(0, randomPoker.Count);
                publicPoker.Add(randomPoker[m]);
                allPoker.Remove(randomPoker[m]);
            }

            if (allPoker.Count == 0 || publicPoker.Count == 3) {
                break;
            }
        }
        return publicPoker;
    }
    
}
