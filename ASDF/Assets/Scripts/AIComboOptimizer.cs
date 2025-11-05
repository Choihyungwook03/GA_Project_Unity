using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

[System.Serializable]
public class CardType
{
    public string name;
    public int damage;
    public int cost;
    public int count; 

    public CardType(string name, int damage, int cost, int count)
    {
        this.name = name;
        this.damage = damage;
        this.cost = cost;
        this.count = count;
    }
}

public class AIComboOptimizer : MonoBehaviour
{
    public Button startButton;

    public int maxCost = 15;

    List<CardType> cardTypes;

    Coroutine runningRoutine;
    long iteration = 0;
    int yieldInterval = 1000;

    int bestDamage = 0;
    List<string> bestSequence = new List<string>();

    void Start()
    {
        startButton.onClick.AddListener(OnStartButtonClicked);

        cardTypes = new List<CardType>()
        {
            new CardType("Quick Shot", 6, 2, 2),
            new CardType("Heavy Shot", 8, 3, 2),
            new CardType("Multi Shot", 16, 5, 1),
            new CardType("Triple Shot", 24, 7, 1)
        };
    }

    public void OnStartButtonClicked()
    {
        if (runningRoutine != null)
        {
            Debug.Log("[AI] 이미 실행 중입니다.");
            return;
        }
        runningRoutine = StartCoroutine(OptimizeRoutine());
    }

    IEnumerator OptimizeRoutine()
    {
        Debug.Log("[AI] 최적화 시뮬레이션 시작");

        Stopwatch sw = new Stopwatch();
        sw.Start();

        iteration = 0;
        bestDamage = 0;
        bestSequence.Clear();

        int[] counts = new int[cardTypes.Count];
        for (int i = 0; i < cardTypes.Count; i++) counts[i] = cardTypes[i].count;

        List<string> currentSeq = new List<string>();

        IEnumerator Recurse(int currentCost, int currentDamage)
        {
            if (currentDamage > bestDamage)
            {
                bestDamage = currentDamage;
                bestSequence = new List<string>(currentSeq);
            }

            for (int i = 0; i < cardTypes.Count; i++)
            {
                var ct = cardTypes[i];

                if (counts[i] <= 0) continue;
                if (currentCost + ct.cost > maxCost) continue;

                counts[i]--;
                currentSeq.Add(ct.name);
                currentDamage += ct.damage;
                currentCost += ct.cost;

                iteration++;
                if (iteration % yieldInterval == 0)
                {
                    yield return null;
                }

                var sub = Recurse(currentCost, currentDamage);
                while (sub.MoveNext()) yield return sub.Current;

                currentSeq.RemoveAt(currentSeq.Count - 1);
                counts[i]++;
                currentDamage -= ct.damage;
                currentCost -= ct.cost;
            }

            yield break;
        }

        var root = Recurse(0, 0);
        while (root.MoveNext()) yield return root.Current;

        sw.Stop();

        Debug.Log($"[AI] 탐색 완료. 시도 수={iteration} 최적 데미지={bestDamage} 소요={sw.Elapsed.TotalSeconds:F3}초");
        Debug.Log("[AI] 최적 시퀀스 (카드 순서): " + (bestSequence.Count == 0 ? "(빈 시퀀스)" : string.Join(" -> ", bestSequence)));

        runningRoutine = null;
    }
}
