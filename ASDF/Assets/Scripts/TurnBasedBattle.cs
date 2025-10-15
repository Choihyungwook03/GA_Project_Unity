using System.Collections.Generic;
using UnityEngine;

public class TurnBasedBattle : MonoBehaviour
{
    private class Unit
    {
        public string name;
        public float speed;
        public float cooldown;

        public Unit(string name, float speed)
        {
            this.name = name;
            this.speed = speed;
            this.cooldown = 0f;
        }
    }

    private SimplePriorityQueue<Unit> queue = new SimplePriorityQueue<Unit>();
    private List<Unit> allUnits = new List<Unit>();
    private int turnCount = 0;

    void Start()
    {
        allUnits.Add(new Unit("전사", 5));
        allUnits.Add(new Unit("마법사", 7));
        allUnits.Add(new Unit("궁수", 10));
        allUnits.Add(new Unit("도적", 12));

        foreach (var u in allUnits)
            queue.Enqueue(u, u.cooldown);

        Debug.Log("=== 턴제 전투 시작 ===");
        PrintNextOrder();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            NextTurn();
        }
    }

    void NextTurn()
    {
        if (queue.Count == 0) return;

        turnCount++;

        var current = queue.Dequeue();

        Debug.Log($"[ {turnCount}턴 ] {current.name}의 턴입니다!");

        current.cooldown += 100f / current.speed;

        queue.Enqueue(current, current.cooldown);

        PrintNextOrder();
    }

    void PrintNextOrder()
    {
        var temp = queue.GetHeapCopy();
        temp.Sort((a, b) =>
        {
            int cmp = a.Item2.CompareTo(b.Item2);
            if (cmp != 0) return cmp;
            return b.Item1.speed.CompareTo(a.Item1.speed);
        });

        string order = "다음 턴 순서: ";
        foreach (var (unit, _) in temp)
            order += $"{unit.name} ";
        Debug.Log(order);
    }
}
