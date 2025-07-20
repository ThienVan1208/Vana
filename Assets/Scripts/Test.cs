
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;
using UnityEngine;
using System;
using UnityEngine.Events;

public enum TestType
{
    MainThread,
    MultiThread
}

[BurstCompile]
struct RunTestParallel : IJobParallelFor
{
    public int threshold;
    public NativeArray<long> a;
    public NativeArray<long> temp; // Per-thread accumulator

    public void Execute(int index)
    {
        long localSum = 0;
        for (int j = 0; j < threshold; j++)
        {
            for (int k = 0; k < threshold; k++)
            {
                for (int l = 0; l < threshold; l++)
                {
                    localSum++;
                }
            }
        }
        temp[index] = localSum; // Store per-thread result
    }
}

public class Test : MonoBehaviour
{
    public int threshold = 10; // Reduced for testing
    public long a = -99999999999;
    public TestType type;
    private NativeArray<long> nativeA;
    private JobHandle jobHandle;
    private bool isInitialized;
    private NativeArray<long> temp; // Moved temp here for clarity
    private UnityEvent testAction;

    private void Awake()
    {
        // nativeA = new NativeArray<long>(1, Allocator.Persistent);
        // nativeA[0] = a;
        // temp = new NativeArray<long>(threshold, Allocator.TempJob);
        // isInitialized = true;
        // Debug.Log($"Test instance created on {gameObject.name}.");
        
    }

    private void Update()
    {
        // if (!isInitialized) return;

        // if (type == TestType.MultiThread)
        // {
        //     RunTestParallel rt = new RunTestParallel
        //     {
        //         threshold = threshold,
        //         a = nativeA,
        //         temp = temp
        //     };
        //     jobHandle = rt.Schedule(threshold, 1, jobHandle);


        //     jobHandle.Complete();
        //     for (int i = 0; i < temp.Length; i++)
        //     {
        //         nativeA[0] += temp[i];
        //     }
        //     temp.Dispose();
        //     temp = new NativeArray<long>(threshold, Allocator.TempJob); // Recreate for next frame
        //     a = nativeA[0];
        // }
        // else
        // {
        //     for (int i = 0; i < threshold; i++)
        //     {
        //         for (int j = 0; j < threshold; j++)
        //         {
        //             for (int k = 0; k < threshold; k++)
        //             {
        //                 for (int l = 0; l < threshold; l++)
        //                 {
        //                     a++;
        //                 }
        //             }
        //         }
        //     }
        // }
    }

    private void hehe()
    {
        Debug.Log("hehe");
    }
    public void SubcribeAction()
    {
        // testAction += hehe;
        testAction.AddListener(hehe);
    }
    public void UnsubcribeAction()
    {
        // testAction -= hehe;
        testAction.RemoveListener(hehe);
    }
    public void RaiseAction()
    {
        testAction.Invoke();
    }

    private void OnDestroy()
    {
        if (isInitialized && nativeA.IsCreated)
        {
            jobHandle.Complete();
            nativeA.Dispose();
            if (temp.IsCreated) temp.Dispose();
            Debug.Log($"Test instance on {gameObject.name} disposed NativeArray.");
        }
    }

    private void OnApplicationQuit()
    {
        if (isInitialized && nativeA.IsCreated)
        {
            jobHandle.Complete();
            nativeA.Dispose();
            if (temp.IsCreated) temp.Dispose();
            Debug.Log($"Test instance on {gameObject.name} disposed NativeArray on quit.");
        }
    }
}