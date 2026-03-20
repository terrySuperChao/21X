using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FightCardView : MonoBehaviour
{
    public Button stopPokerBtn;
    public Button dealPokerBtn;

    public GameObject cardPrefab;

    public GameObject userAsset;
    public GameObject npcAsset;

    public GameObject effectImage;
    public GameObject attackImage;

    public Transform rootTransform;

    // Start is called before the first frame update
    void Start()
    {
        EventDispatcher.Instance.on(GameConst.DEALCARD, this.dealCard);
        EventDispatcher.Instance.on(GameConst.CANDIDACYCARD, this.candidacyCard);
        EventDispatcher.Instance.on(GameConst.OKSELECTCARD, this.okSelectCard);
        EventDispatcher.Instance.on(GameConst.CANCELSELECTCARD, this.cancelSelectCard);
        EventDispatcher.Instance.on(GameConst.DEALPOKER, this.dealPoker);
        EventDispatcher.Instance.on(GameConst.TURNPLAYER, this.turnPlayer);
        EventDispatcher.Instance.on(GameConst.STOPDEALPOKER, this.stopDealPoker);
        EventDispatcher.Instance.on(GameConst.WAITOPERATOR, this.waitOperator);
        EventDispatcher.Instance.on(GameConst.TOTALPOKERPOINT, this.totalPokerPoint);
        EventDispatcher.Instance.on(GameConst.FIGHTFLOWSTATE, this.fightFlowState);
        EventDispatcher.Instance.on(GameConst.SHUFFLEPOKER, this.shufflePoker);
        EventDispatcher.Instance.on(GameConst.GAMESETTLE, this.gameSettle);
        EventDispatcher.Instance.on(GameConst.GAMEOVER, this.gameOver);
        EventDispatcher.Instance.on(GameConst.ADDPOKERVALUE, this.addPokerValue);
        EventDispatcher.Instance.on(GameConst.ADDCARDVALUE, this.addCardValue);
        EventDispatcher.Instance.on(GameConst.COMMONATTACK, this.commonAttack);
        EventDispatcher.Instance.on(GameConst.FLYFONT, this.flyFont);
        EventDispatcher.Instance.on(GameConst.REFACTORING, this.refactoring);
        EventDispatcher.Instance.on(GameConst.REHANDPOKER, this.reHandPoker);
        EventDispatcher.Instance.on(GameConst.CLEARHANDPOKER, this.clearHandPoker);
        EventDispatcher.Instance.on(GameConst.GAMECLEAR, this.gameClear);
        EventDispatcher.Instance.on(GameConst.SHOWTIPS, this.onShowTips);
        EventDispatcher.Instance.on(GameConst.EXIT_PAGE, this.exitPageHandle);

        FightPokerMgr.Instance.init();
        FightPokerMgr.Instance.runFlow();

        this.setBtnInteractable(false);

        Invoke("initUserInfo", 0.05f);
        Invoke("delayMessageComplete", 0.5f);
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.off(GameConst.DEALCARD, this.dealCard);
        EventDispatcher.Instance.off(GameConst.CANDIDACYCARD, this.candidacyCard);
        EventDispatcher.Instance.off(GameConst.OKSELECTCARD, this.okSelectCard);
        EventDispatcher.Instance.off(GameConst.CANCELSELECTCARD, this.cancelSelectCard);
        EventDispatcher.Instance.off(GameConst.DEALPOKER, this.dealPoker);
        EventDispatcher.Instance.off(GameConst.TURNPLAYER, this.turnPlayer);
        EventDispatcher.Instance.off(GameConst.STOPDEALPOKER, this.stopDealPoker);
        EventDispatcher.Instance.off(GameConst.WAITOPERATOR, this.waitOperator);
        EventDispatcher.Instance.off(GameConst.TOTALPOKERPOINT, this.totalPokerPoint);
        EventDispatcher.Instance.off(GameConst.FIGHTFLOWSTATE, this.fightFlowState);
        EventDispatcher.Instance.off(GameConst.SHUFFLEPOKER, this.shufflePoker);
        EventDispatcher.Instance.off(GameConst.GAMESETTLE, this.gameSettle);
        EventDispatcher.Instance.off(GameConst.GAMEOVER, this.gameOver);
        EventDispatcher.Instance.off(GameConst.ADDPOKERVALUE, this.addPokerValue);
        EventDispatcher.Instance.off(GameConst.ADDCARDVALUE, this.addCardValue);
        EventDispatcher.Instance.off(GameConst.COMMONATTACK, this.commonAttack);
        EventDispatcher.Instance.off(GameConst.FLYFONT, this.flyFont);
        EventDispatcher.Instance.off(GameConst.REFACTORING, this.refactoring);
        EventDispatcher.Instance.off(GameConst.REHANDPOKER, this.reHandPoker);
        EventDispatcher.Instance.off(GameConst.CLEARHANDPOKER, this.clearHandPoker);
        EventDispatcher.Instance.off(GameConst.GAMECLEAR, this.gameClear);
        EventDispatcher.Instance.off(GameConst.SHOWTIPS, this.onShowTips);
        EventDispatcher.Instance.off(GameConst.EXIT_PAGE, this.exitPageHandle);
    }
    public void exitPageHandle(params System.Object[] obj)
    {
        string pageName = Enum.GetName(typeof(PageIndex), GameDataMgr.Instance.getPageIndex());
        UIMgr.Instance.showView(pageName);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void initUserInfo()
    {
        Transform poker = this.rootTransform.Find("Poker");
        List<IUser> players = FightPokerMgr.Instance.getPlayers();
        foreach (IUser user in players) {
            if (user.isNpc()){
                this.npcAsset.GetComponent<PlayerAsset>().initUserInfo(user, poker.position);
            }else {
                this.userAsset.GetComponent<PlayerAsset>().initUserInfo(user, poker.position);
            }
        }
    }

    private void fightFlowState(params System.Object[] obj) {
        FightFlowState state = (FightFlowState)obj[0];
        GameReqMgr.Instance.requestSaveFightFlowState(state);
        GameMessage.Instance.setHandleMessageComplete();
    }

    // ------》》》》 1发卡牌
    public void dealCard(params System.Object[] obj)
    {
        Invoke("delayMessageComplete", 0.5f);
    }

    // ------》》》》 1.1 选择卡牌
    public void candidacyCard(params System.Object[] obj)
    {
        ICandidacyCardPara para = (ICandidacyCardPara)obj[0];
        UIMgr.Instance.showTips("SelectCardView", obj[0]);

    }

    public void okSelectCard(params System.Object[] obj)
    {
        StartCoroutine(okSelectCardHandle(obj));
    }

    private IEnumerator okSelectCardHandle(params System.Object[] obj)
    {
        ISelectCardPara para = (ISelectCardPara)obj[0];
        ICard card = para.getCard();

        GameObject cardObject = Instantiate(this.cardPrefab, this.rootTransform);
        cardObject.GetComponent<Card>().loadCard(card);
        cardObject.GetComponent<Card>().showNameText(false);
        cardObject.transform.position = para.getPosition();

        PlayerAsset asset = this.userAsset.GetComponent<PlayerAsset>();
        iTween.MoveTo(cardObject, asset.getCardPosition(card), 0.5f);
        iTween.ScaleTo(cardObject, new Vector3(0.7f, 0.7f, 0.7f), 0.5f);
        yield return new WaitForSeconds(0.6f);
        Destroy(cardObject);
        asset.addCard(card);

        Invoke("delayMessageComplete", 0.1f);
    }

    public void cancelSelectCard(params System.Object[] obj)
    {
        GameMessage.Instance.setHandleMessageComplete();
    }


    // ------》》》》 2发扑克牌
    private void dealPoker(params System.Object[] obj)
    {
        Invoke("delayMessageComplete", 0.6f);
    }

    // ------》》》》 3轮到操作者
    private void turnPlayer(params System.Object[] obj)
    {
        IUser user = (IUser)obj[0];
        this.setBtnInteractable(!user.isNpc());
        Invoke("delayMessageComplete", 0.1f);
    }

    // ------》》》》 4 等待操作
    private void waitOperator(params System.Object[] obj) {
        StartCoroutine(waitOperatorHandle(obj));
    }

    private IEnumerator waitOperatorHandle(params System.Object[] obj)
    {
        IUser user = (IUser)obj[0];
        if (user.isNpc())
        {
            yield return new WaitForSeconds(RandomMgr.Instance.getRangeInt(1, 3));
            GameReqMgr.Instance.requestNpcOperator(user);
        }
        else
        {
            yield return new WaitForSeconds(0.1f);
            this.setBtnInteractable(true);
        }
        GameMessage.Instance.setHandleMessageComplete();
    }

    // ------》》》》 5 停牌
    private void stopDealPoker(params System.Object[] obj)
    {
        GameMessage.Instance.setHandleMessageComplete();
    }


    // ------》》》》 6 统计点数
    private void totalPokerPoint(params System.Object[] obj)
    {
        GameMessage.Instance.setHandleMessageComplete();
    }

    public void addPokerValue(params System.Object[] obj)
    {
        GameMessage.Instance.setHandleMessageComplete();
    }

    public void addCardValue(params System.Object[] obj)
    {
        GameMessage.Instance.setHandleMessageComplete();
    }

    public void flyFont(params System.Object[] obj)
    {
        GameMessage.Instance.setHandleMessageComplete();
    }

    private void gameSettle(params System.Object[] obj)
    {
        StartCoroutine(gameSettleHandle(obj));
    }

    private IEnumerator gameSettleHandle(params System.Object[] obj)
    {
        setBtnInteractable(false);
        yield return new WaitForSeconds(0.5f);

        EventDispatcher.Instance.emit(GameConst.FLIPPOKER);
        yield return new WaitForSeconds(0.5f);

        this.userAsset.GetComponent<PlayerAsset>().showUserState();

        IUser user = (IUser)obj[0];
        string tipsText = "";
        if (user == null)
        {
            tipsText = "本回合平局";
        }
        else
        {
            if (user.isNpc())
            {
                tipsText = "本回合NPC获胜";
            }
            else
            {
                tipsText = "本回合玩家获胜";
            }
        }
        UIMgr.Instance.showTips("TipsView", tipsText);

        Invoke("delayMessageComplete", 0.5f);
    }

    private void gameOver(params System.Object[] obj)
    {
        GameReqMgr.Instance.requestSavePlayerInfo();
        GameReqMgr.Instance.requestExitPage();
        EventDispatcher.Instance.emit("returnToLobby");
        GameMessage.Instance.setHandleMessageComplete();
    }

    public void onDealPokerClick() {
        this.setBtnInteractable(false);
        EventDispatcher.Instance.emit(GameConst.HIDE_REFACTORING);
        GameReqMgr.Instance.requestSaveFightFlowState(FightFlowState.dealPoker);
    }

    public void onStopPokerClick() {
        this.setBtnInteractable(false);
        EventDispatcher.Instance.emit(GameConst.HIDE_REFACTORING);
        GameReqMgr.Instance.requestSaveFightFlowState(FightFlowState.stopDealPoker);
    }

    private void setBtnInteractable(bool able) {
        stopPokerBtn.interactable = dealPokerBtn.interactable = able;
    }

    private void shufflePoker(params System.Object[] obj) {
        Invoke("delayMessageComplete", 0.1f);
    }

    public void commonAttack(params System.Object[] obj)
    {
        StartCoroutine(commonAttackHandle(obj));
    }

    private IEnumerator commonAttackHandle(params System.Object[] obj)
    {
        IUICommonPara para = (IUICommonPara)obj[0];
        Vector3 npcPositon = this.npcAsset.GetComponent<PlayerAsset>().getHeadPosition();
        Vector3 userPositon = this.userAsset.GetComponent<PlayerAsset>().getHeadPosition();
        if (para.getUser().isNpc())
        {
            this.attackImage.transform.position = npcPositon;
            this.effectImage.transform.position = userPositon;
            iTween.MoveTo(this.attackImage, userPositon, 1.0f);
        }
        else
        {
            this.attackImage.transform.position = userPositon;
            this.effectImage.transform.position = npcPositon;
            iTween.MoveTo(this.attackImage, npcPositon, 1.0f);
        }

        this.attackImage.SetActive(true);
        yield return new WaitForSeconds(1.1f);
        this.attackImage.SetActive(false);
        this.effectImage.SetActive(true);
        this.effectImage.transform.Find("effectText").GetComponent<Text>().text = "-" + para.getValue();
        yield return new WaitForSeconds(0.5f);
        effectImage.SetActive(false);
        
        GameMessage.Instance.setHandleMessageComplete();
    }

    public void refactoring(params System.Object[] obj)
    {
        IRefactoringPara para = (IRefactoringPara)obj[0];
        if (para.getUser().isNpc()){
            FightPokerMgr.Instance.reDealHandPoker(para.getUser());
        }else {
            EventDispatcher.Instance.emit(GameConst.RUN_REFACTORING, para);
        }
        GameMessage.Instance.setHandleMessageComplete();
    }

    public void reHandPoker(params System.Object[] obj) {
        IReHandPokerPara para = (IReHandPokerPara)obj[0];
        FightPokerMgr.Instance.reDealHandPoker(para.getUser(), para.getSuit());
    }
    
    public void clearHandPoker(params System.Object[] obj)
    {
        GameMessage.Instance.setHandleMessageComplete();
    }

    public void gameClear(params System.Object[] obj)
    {
        FightPokerMgr.Instance.clear();
        Invoke("delayMessageComplete", 1.6f);
    }

    private void onShowTips(params System.Object[] obj)
    {
        UIMgr.Instance.showTips("TipsView", obj[0]);
    }

    private void delayMessageComplete() {
        GameMessage.Instance.setHandleMessageComplete();
    }
    public void onPopClick()
    {
        UIMgr.Instance.showView("PopView");
    }
}
