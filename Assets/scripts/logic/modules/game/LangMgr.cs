using System;
using System.Collections.Generic;
using UnityEngine;
public class LanguageList
{
    public static string zh_hk = "zh-HK";
    public static string zh_mo = "zh-MO";
    public static string zh_cn = "zh-CN";
    public static string zh_tw = "zh-TW";
    public static string zh_sg = "zh-SG";
    public static string en_us = "en-US";
    public static string en_gb = "en-GB";
    public static string ms_bn = "ms-BN";
    public static string ms_my = "ms-MY";
    public static string ar_sa = "ar-SA";
    public static string vi_vn = "vi-VN";
    public static string th_th = "th-TH";
    public static string ko_kr = "ko-KR";
    public static string ja_jp = "ja-JP";
    public static string es_es = "es-ES";
    public static string id_id = "id-ID";
    public static string ur_pk = "ur-PK";
    public static string tr_tr = "tr-TR";
    public static string af_za = "af-ZA";
    public static string az_az = "az-AZ";
    public static string be_by = "be-BY";
    public static string bg_bg = "bg-BG";
    public static string bs_ba = "bs-BA";
    public static string ca_es = "ca-ES";
    public static string cs_cz = "cs-CZ";
    public static string cy_gb = "cy-GB";
    public static string da_dk = "da-DK";
    public static string de_de = "de-DE";
    public static string dv_mv = "dv-MV";
    public static string et_ee = "et-EE";
    public static string eu_es = "eu-ES";
    public static string fa_ir = "fa-IR";
    public static string fi_fi = "fi-FI";
    public static string fo_fo = "fo-FO";
    public static string fr_fr = "fr-FR";
    public static string gl_es = "gl-ES";
    public static string gu_in = "gu-IN";
    public static string he_il = "he-IL";
    public static string hi_in = "hi-IN";
    public static string hr_hr = "hr-HR";
    public static string hu_hu = "hu-HU";
    public static string hy_am = "hy-AM";
    public static string is_is = "is-IS";
    public static string it_it = "it-IT";
    public static string ka_ge = "ka-GE";
    public static string kk_kz = "kk-KZ";
    public static string kn_in = "kn-IN";
    public static string kok_in = "kok-IN";
    public static string ky_kg = "ky-KG";
    public static string lt_lt = "lt-LT";
    public static string lv_lv = "lv-LV";
    public static string mi_nz = "mi-NZ";
    public static string mk_mk = "mk-MK";
    public static string mn_mn = "mn-MN";
    public static string mr_in = "mr-IN";
    public static string mt_mt = "mt-MT";
    public static string nb_no = "nb-NO";
    public static string nl_nl = "nl-NL";
    public static string nn_no = "nn-NO";
    public static string ns_za = "ns-ZA";
    public static string pa_in = "pa-IN";
    public static string pl_pl = "pl-PL";
    public static string pt_pt = "pt-PT";
    public static string qu_bo = "qu-BO";
    public static string ro_ro = "ro-RO";
    public static string ru_ru = "ru-RU";
    public static string sa_in = "sa-IN";
    public static string se_se = "se-SE";
    public static string sk_sk = "sk-SK";
    public static string sl_si = "sl-SI";
    public static string sq_al = "sq-AL";
    public static string sr_ba = "sr-BA";
    public static string sv_se = "sv-SE";
    public static string sw_ke = "sw-KE";
    public static string syr_sy = "syr-SY";
    public static string ta_in = "ta-IN";
    public static string te_in = "te-IN";
    public static string tl_ph = "tl-PH";
    public static string tn_za = "tn-ZA";
    public static string tt_ru = "tt-RU";
    public static string uk_ua = "uk-UA";
    public static string uz_uz = "uz-UZ";
    public static string xh_za = "xh-ZA";
    public static string zu_za = "zu-ZA";
    public static string ar_ae = "ar-AE";
    public static string ar_bh = "ar-BH";
    public static string ar_dz = "ar-DZ";
    public static string ar_eg = "ar-EG";
    public static string ar_iq = "ar-IQ";
    public static string ar_jo = "ar-JO";
    public static string ar_kw = "ar-KW";
    public static string ar_lb = "ar-LB";
    public static string ar_ly = "ar-LY";
    public static string ar_ma = "ar-MA";
    public static string ar_om = "ar-OM";
    public static string ar_qa = "ar-QA";
    public static string ar_sy = "ar-SY";
    public static string ar_tn = "ar-TN";
    public static string ar_ye = "ar-YE";
}

public class LangMgr : Singleton<LangMgr>
{
    private string _defLanguage = LanguageList.en_us;
    private string _curLanguageStr = LanguageList.en_us;
    private Dictionary<string, LangInfo> _configDic = null;
    public void init(string path)
    {
        this._configDic = JsonMgr.Instance.readObject<Dictionary<string, LangInfo>>(path);
    }
    private bool isLanguageContains(string key)
    {
        foreach (var value in this._configDic.Values) {
            if (this.getText(value, key) == "")
            {
                return false;
            }
            else {
                return true;
            }
        }
        return false;
    }

    private string getText(LangInfo info,string key) {
        if (info == null) return "";

        if (key == LanguageList.zh_cn)
        {
            return info.zh_CN;
        }
        else if (key == LanguageList.zh_sg)
        {
            return info.zh_SG;
        }
        else if (key == LanguageList.zh_tw)
        {
            return info.zh_TW;
        }
        else if (key == LanguageList.zh_hk)
        {
            return info.zh_HK;
        }
        else if (key == LanguageList.zh_mo)
        {
            return info.zh_MO;
        }
        else if (key == LanguageList.en_us)
        {
            return info.en_US;
        }
        else if (key == LanguageList.en_gb)
        {
            return info.zh_CN;
        }
        else if (key == LanguageList.ar_sa)
        {
            return info.ar_SA;
        }
        else if (key == LanguageList.vi_vn)
        {
            return info.vi_VN;
        }
        else if (key == LanguageList.id_id)
        {
            return info.id_ID;
        }
        else if (key == LanguageList.th_th)
        {
            return info.th_TH;
        }
        return "";           
    }
    public void setCurLanguage(string curlanguage)
    {
        if (!this.isLanguageContains(curlanguage))
        {
            this._curLanguageStr = this._defLanguage;
        }
        else
        {
            this._curLanguageStr = curlanguage;
        }
    }

    public string getText(string key) {
        return this.getText(this._configDic[key], this._curLanguageStr);
    }
}
