using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUpgradeSystem : MonoBehaviour
{
    [System.Serializable]
    public class Stone
    {
        public string name;
        public int exp;
        public int gold;

        public Stone(string name, int exp, int gold)
        {
            this.name = name;
            this.exp = exp;
            this.gold = gold;
        }
    }

    public List<Stone> stones;

    public int currentLevel = 1;

    public Text smallText;
    public Text mediumText;
    public Text largeText;
    public Text hugeText;
    public Text totalGoldText;

    public Text requireExpText;
    public Image expFill;

    int selectedMethod = 0;

    void Start()
    {
        stones = new List<Stone>()
        {
            new Stone("소", 3, 8),
            new Stone("중", 5, 12),
            new Stone("대", 12, 30),
            new Stone("특대", 20, 45)
        };

        UpdateRequireExpUI();
        ClearResultUI();
    }

    int GetNeedExp(int level)
    {
        return 8 * level * level;
    }

    Dictionary<string, int> InitResult()
    {
        return new Dictionary<string, int>()
        {
            { "소", 0 },
            { "중", 0 },
            { "대", 0 },
            { "특대", 0 }
        };
    }

    Dictionary<string, int> BruteForce(int targetExp)
    {
        Dictionary<string, int> best = null;
        int bestGold = int.MaxValue;

        int max = 40;

        for (int a = 0; a < max; a++)
            for (int b = 0; b < max; b++)
                for (int c = 0; c < max; c++)
                    for (int d = 0; d < max; d++)
                    {
                        int exp = a * 3 + b * 5 + c * 12 + d * 20;
                        if (exp < targetExp) continue;

                        int gold = a * 8 + b * 12 + c * 30 + d * 45;

                        if (gold < bestGold)
                        {
                            bestGold = gold;
                            best = new Dictionary<string, int>()
                            {
                                { "소", a },
                                { "중", b },
                                { "대", c },
                                { "특대", d }
                            };
                        }
                    }

        return best;
    }

    Dictionary<string, int> GreedyMinWaste(int targetExp)
    {
        var result = InitResult();
        int exp = 0;

        stones.Sort((a, b) => a.exp.CompareTo(b.exp));

        foreach (var s in stones)
        {
            while (exp + s.exp <= targetExp)
            {
                exp += s.exp;
                result[s.name]++;
            }
        }

        if (exp < targetExp) result["소"]++;

        return result;
    }

    Dictionary<string, int> GreedyEfficiency(int targetExp)
    {
        var result = InitResult();
        int exp = 0;

        stones.Sort((a, b) => (b.exp / (float)b.gold).CompareTo(a.exp / (float)a.gold));

        foreach (var s in stones)
        {
            while (exp < targetExp)
            {
                exp += s.exp;
                result[s.name]++;
            }
        }

        return result;
    }

    Dictionary<string, int> GreedyBig(int targetExp)
    {
        var result = InitResult();
        int exp = 0;

        stones.Sort((a, b) => b.exp.CompareTo(a.exp));

        foreach (var s in stones)
        {
            while (exp + s.exp <= targetExp)
            {
                exp += s.exp;
                result[s.name]++;
            }
        }

        if (exp < targetExp) result["소"]++;

        return result;
    }

    void UpdateRequireExpUI()
    {
        int need = GetNeedExp(currentLevel);
        requireExpText.text = "필요 경험치 " + 0 + "/" + need;
        expFill.fillAmount = 0f;
    }

    void ClearResultUI()
    {
        smallText.text = "강화석 소 x 0";
        mediumText.text = "강화석 중 x 0";
        largeText.text = "강화석 대 x 0";
        hugeText.text = "강화석 특대 x 0";
        totalGoldText.text = "총 가격 0 gold";
    }

    void PrintResult(Dictionary<string, int> r)
    {
        int exp =
            r["소"] * 3 +
            r["중"] * 5 +
            r["대"] * 12 +
            r["특대"] * 20;

        int gold =
            r["소"] * 8 +
            r["중"] * 12 +
            r["대"] * 30 +
            r["특대"] * 45;

        smallText.text = "강화석 소 x " + r["소"];
        mediumText.text = "강화석 중 x " + r["중"];
        largeText.text = "강화석 대 x " + r["대"];
        hugeText.text = "강화석 특대 x " + r["특대"];

        totalGoldText.text = "총 가격 " + gold + " gold";

        int need = GetNeedExp(currentLevel);

        requireExpText.text = "필요 경험치 " + exp + "/" + need;
        expFill.fillAmount = Mathf.Clamp01(exp / (float)need);
    }

    public void UseBruteForce() { selectedMethod = 0; }
    public void UseMinWaste() { selectedMethod = 1; }
    public void UseEfficiency() { selectedMethod = 2; }
    public void UseBigFirst() { selectedMethod = 3; }

    public void OnUpgradeButton()
    {
        int need = GetNeedExp(currentLevel);

        Dictionary<string, int> result = null;

        if (selectedMethod == 0) result = BruteForce(need);
        else if (selectedMethod == 1) result = GreedyMinWaste(need);
        else if (selectedMethod == 2) result = GreedyEfficiency(need);
        else if (selectedMethod == 3) result = GreedyBig(need);

        PrintResult(result);
    }
}
