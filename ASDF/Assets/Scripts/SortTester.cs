using System.Diagnostics; // Stopwatch
using UnityEngine;
using UnityEngine.UI;

public class SortTester : MonoBehaviour
{
    public Text resultText; // UI Text 연결

    private int[] dataOrigin;

    void Start()
    {
        // 실행할 때마다 새로운 랜덤 데이터 1만 개 생성
        dataOrigin = new int[10000];
        System.Random rand = new System.Random();
        for (int i = 0; i < dataOrigin.Length; i++)
        {
            dataOrigin[i] = rand.Next(0, 10000);
        }
    }

    public void OnSelectionSort()
    {
        int[] data = (int[])dataOrigin.Clone();
        Stopwatch sw = new Stopwatch();
        sw.Start();

        SelectionSort(data);

        sw.Stop();
        resultText.text = $"선택 정렬: {sw.Elapsed.TotalMilliseconds:F4} ms";
    }

    public void OnBubbleSort()
    {
        int[] data = (int[])dataOrigin.Clone();
        Stopwatch sw = new Stopwatch();
        sw.Start();

        BubbleSort(data);

        sw.Stop();
        resultText.text = $"버블 정렬: {sw.Elapsed.TotalMilliseconds:F4} ms";
    }

    public void OnQuickSort()
    {
        int[] data = (int[])dataOrigin.Clone();
        Stopwatch sw = new Stopwatch();
        sw.Start();

        QuickSort(data, 0, data.Length - 1);

        sw.Stop();
        resultText.text = $"퀵 정렬: {sw.Elapsed.TotalMilliseconds:F4} ms";
    }

    // ---------------- 정렬 알고리즘 ----------------

    void SelectionSort(int[] arr)
    {
        for (int i = 0; i < arr.Length - 1; i++)
        {
            int min = i;
            for (int j = i + 1; j < arr.Length; j++)
            {
                if (arr[j] < arr[min])
                    min = j;
            }
            int temp = arr[min];
            arr[min] = arr[i];
            arr[i] = temp;
        }
    }

    void BubbleSort(int[] arr)
    {
        for (int i = 0; i < arr.Length - 1; i++)
        {
            for (int j = 0; j < arr.Length - i - 1; j++)
            {
                if (arr[j] > arr[j + 1])
                {
                    int temp = arr[j];
                    arr[j] = arr[j + 1];
                    arr[j + 1] = temp;
                }
            }
        }
    }

    void QuickSort(int[] arr, int left, int right)
    {
        int i = left, j = right;
        int pivot = arr[(left + right) / 2];

        while (i <= j)
        {
            while (arr[i] < pivot) i++;
            while (arr[j] > pivot) j--;
            if (i <= j)
            {
                int tmp = arr[i];
                arr[i] = arr[j];
                arr[j] = tmp;
                i++; j--;
            }
        }

        if (left < j) QuickSort(arr, left, j);
        if (i < right) QuickSort(arr, i, right);
    }
}
