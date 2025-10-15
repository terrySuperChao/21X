//牌堆
using System.Collections.Generic;


public class PlayPokerMgr : Singleton<PlayPokerMgr>
{
    
    private enum PlayState {
        none,
        play,
        end,
        death
    }

    private class PlayerState {
        public IUser user;
        public PlayState state;
    }

    private class SortItem{
        public int index;
        public int point;
        public SortItem(int idx, int pt) {
            index = idx;
            point = pt;
        }
    }

    private List<PlayerState> _players = new List<PlayerState>();
    private IGameFlow _gameFlow;

    public void setGameFlow(IGameFlow gameFlow) {
        _gameFlow = gameFlow;
    }

    public void addPlayer(IUser user) {
        PlayerState ps = new PlayerState();
        ps.user = user;
        ps.state = PlayState.none;
        _players.Add(ps);
    }

    public IPoker getDealPoker() {
        int number = PokerPileMgr.Instance.getRemainCards();
        if (number == 0)
        {
            PokerPileMgr.Instance.shuffle();
            GameMessage.Instance.addMsg(GameConst.SHUFFLEPOKER, number);
        }
        else if (number == 1) {
            GameMessage.Instance.addMsg(GameConst.SHUFFLEPOKER, number);
        }
        return PokerPileMgr.Instance.dealPoker();
    }

    public void startPlayPoker() {
        _gameFlow.gameBegin(new GameBeginPara(getPlayers()));
      
        for (int i = 0; i < 2; i++) {
            for (int j = 0; j < _players.Count; j++){
                IUser user = _players[j].user;
                IPoker poker = getDealPoker();
                poker.setBack(i == 0 && user.isNpc());
                HandPokerMgr.Instance.addHandPoker(user, poker);

                int number = HandPokerMgr.Instance.getHandPokerPoint(user, true);
                GameMessage.Instance.addMsg(GameConst.DEALPOKER, user, poker, number);
            }
        }
        _gameFlow.handPokerAfter(new HandPokerAfterPara(getPlayers()));

        IUser player = null;
        for (int j = 0; j < _players.Count; j++)
        {
            if (j == 0)
            {
                player = _players[j].user;
                _players[j].state = PlayState.play;
            }
            else {
                _players[j].state = PlayState.none;
            }
        }

        if (player != null) {
            GameMessage.Instance.addMsg(GameConst.PLAYERACTION, player);
        }
    }

    public void dealPokerAction() {
        int index = getPlayingIndex();
        if (index == -1) {
            gameSettle();
            return;
        }

        IUser user = _players[index].user;
        IPoker poker = getDealPoker();
        HandPokerMgr.Instance.addHandPoker(user, poker);

        int number = HandPokerMgr.Instance.getHandPokerPoint(user, true);
        GameMessage.Instance.addMsg(GameConst.DEALPOKER, user, poker, number);

         _gameFlow.dealPokerAfter(new DealPokerAfterPara(getPlayers(),user));
      
        //最终的分数
        int number2 = HandPokerMgr.Instance.getHandPokerPoint(user, false);
        if (number2 <= 21)
        {
            _players[index].state = PlayState.none;
            nextPlayer(index);
        }
        else
        {
            _players[index].state = PlayState.death;
            if (getPlayerStateNumber() == 0){
                gameSettle();
            }
            else {
                nextPlayer(index);
            }  
        }
    }

    public List<IUser> getPlayers() {
        List<IUser> list = new List<IUser>();
        for (int i = 0; i < _players.Count; i++) {
            list.Add(_players[i].user);
        }
        return list;
    }

    public void clearPlayer() {
        _players.Clear();
    }

    public void stopDealPokerAction() {
        int index = getPlayingIndex();
        if (index == -1)
        {
            gameSettle();
            return;
        }

        _players[index].state = PlayState.end;
        GameMessage.Instance.addMsg(GameConst.STOPDEALPOKER, _players[index].user);
        
        if (getPlayerStateNumber() == 0)
        {
            gameSettle();
        }
        else {
            nextPlayer(index);
        }
    }

    private void gameSettle() {
        int maxPoint = 99;//最大值
        List<SortItem> list = new List<SortItem>();
        for (int i = 0; i < _players.Count; i++) {
            if (_players[i].state == PlayState.end || _players[i].state == PlayState.none) {
                int number = HandPokerMgr.Instance.getHandPokerPoint(_players[i].user,false);
                if(number <= 21)
                {
                    if (number == 21 && HandPokerMgr.Instance.isBlackJack(_players[i].user))
                    {
                        number = maxPoint;
                    }
                    list.Add(new SortItem(i,number));                    
                } 
            }
        }

        list.Sort((x, y)=> { return x.point < y.point ? 1 : -1; });

        int index = -1;
        IUser user = null;  
        bool isBack = false;
        if (list.Count == 1 || (list.Count == 2 && list[0].point > list[1].point))
        {
            index = list[0].index;
            isBack = list[0].point == maxPoint;
            user = _players[index].user;
        }

        for (int i = 0; i < _players.Count; i++)
        {
            if (index == i)
            {
                _players[i].user.addWins();
            }
            _players[i].user.addPlay();
        }
        GameMessage.Instance.addMsg(GameConst.GAMESETTLE, user);

        bool isGameOver = _gameFlow.gameSettle(new GameSettlePara(getPlayers(), index, isBack));

        GameMessage.Instance.addMsg(isGameOver ? GameConst.GAMEOVER : GameConst.GAMENEXTROUND);
    }

    private void nextPlayer(int index) {
        int count = _players.Count;
        for (int j = index + 1; j <= index + count; j++)
        {
            int idx = j % count;
            if (_players[idx].state == PlayState.none)
            {
                _players[idx].state = PlayState.play;
                GameMessage.Instance.addMsg(GameConst.PLAYERACTION, _players[idx].user);
                break;
            }
        }
    }

    private int getPlayerStateNumber() {
        int number = 0;
        for (int i = 0; i < _players.Count; i++)
        {
            if (_players[i].state == PlayState.none)
            {
                number++;
            }
        }
        return number;
    }

    public int getPlayingIndex() {
        int index = -1;
        for (int i = 0; i < _players.Count; i++)
        {
            if (_players[i].state == PlayState.play)
            {
                index = i;
                break;
            }
        }
        return index;
    }

    public void dealNumberPoker(IUser user,int dealNumber)
    {
        for (int i = 0; i < dealNumber; i++)
        {
            IPoker poker = getDealPoker();
            HandPokerMgr.Instance.addHandPoker(user, poker);

            int number = HandPokerMgr.Instance.getHandPokerPoint(user, true);
            GameMessage.Instance.addMsg(GameConst.DEALPOKER, user, poker, number);
        }
    }
}
