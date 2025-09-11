//牌堆
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;

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
    private IGameSettle _gameSettle;

    public void setGameSettle(IGameSettle gameSettle) {
        _gameSettle = gameSettle;
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
            GameCtrl.Instance.addMsg(GameConst.SHUFFLEPOKER, number);
        }
        else if (number == 1) {
            GameCtrl.Instance.addMsg(GameConst.SHUFFLEPOKER, number);
        }
        return PokerPileMgr.Instance.dealPoker();
    }

    public void startPlayPoker() {
        for (int i = 0; i < 2; i++) {
            for (int j = 0; j < _players.Count; j++){
                IUser user = _players[j].user;
                IPoker poker = getDealPoker();
                poker.setBack(i == 0 && user.isNpc());
                HandPokerMgr.Instance.addHandPoker(user, poker);

                int number = HandPokerMgr.Instance.getHandPokerPoint(user, true);
                GameCtrl.Instance.addMsg(GameConst.DEALPOKER, user, poker, number);
            }
        }

        for (int j = 0; j < _players.Count; j++)
        {
            if (j == 0)
            {
                _players[j].state = PlayState.play;
                GameCtrl.Instance.addMsg(GameConst.PLAYERACTION, _players[j].user);
            }
            else {
                _players[j].state = PlayState.none;
            }
        }
    }

    public void dealPoker() {
        int index = -1;
        for (int i = 0; i < _players.Count; i++) {
            if (_players[i].state == PlayState.play) {
                index = i;
                break;
            }
        }

        if (index == -1) {
            gameOver();
            return;
        }

        IUser user = _players[index].user;
        IPoker poker = getDealPoker();
        HandPokerMgr.Instance.addHandPoker(user, poker);

        int number = HandPokerMgr.Instance.getHandPokerPoint(user, true);
        GameCtrl.Instance.addMsg(GameConst.DEALPOKER, user, poker, number);

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
            gameOver();
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

    public void stopDealPoker() {
        int number = 0;
        for (int i = 0; i < _players.Count; i++)
        {
            if (_players[i].state == PlayState.none)
            {
                number++;
            }
        }

        for (int i = 0; i < _players.Count; i++) {
            if (_players[i].state == PlayState.play)
            {
                _players[i].state = PlayState.end;
                GameCtrl.Instance.addMsg(GameConst.STOPDEALPOKER, _players[i].user);
                nextPlayer(i);
                break;
            }
        }

        if (number == 0) {
            gameOver();
        }
    }

    private void gameOver() {
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
        if (list.Count == 1 || list[0].point > list[1].point)
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

        _gameSettle?.gameSettle(new GameSettlePara(this.getPlayers(), index, isBack));

        GameCtrl.Instance.addMsg(GameConst.GAMEOVER, user);
    }

    private void nextPlayer(int index) {
        int count = _players.Count;
        for (int j = index + 1; j <= index + count; j++)
        {
            int idx = j % count;
            if (_players[idx].state == PlayState.none)
            {
                _players[idx].state = PlayState.play;
                GameCtrl.Instance.addMsg(GameConst.PLAYERACTION, _players[idx].user);
                break;
            }
        }
    }
}
