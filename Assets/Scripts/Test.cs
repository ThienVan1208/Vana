// using Firebase.Auth;
// using Unity.Jobs;
// using UnityEngine.Jobs;
// using Unity.Burst;
// using UnityEngine;
// using Unity.Mathematics;
// using Unity.Collections;
// using UnityEngine.UI;
// public enum TestType
// {
//     MainThread,
//     MultiThread
// }
// [BurstCompile]
// struct RunTest : IJob
// {
//     public int threshold;
//     public NativeArray<long> a;
//     public void Execute()
//     {
//         for (int i = 0; i < threshold; i++)
//         {
//             for (int j = 0; j < threshold; j++)
//             {
//                 for (int k = 0; k < threshold; k++)
//                 {
//                     for (int l = 0; l < threshold; l++)
//                     {
//                         a[0]++;
//                         // Debug.Log(a++);
//                     }
//                 }
//             }
//         }
//     }
// }
// [BurstCompile]
// public class Test : MonoBehaviour
// {
//     public int threshold = 70;
//     public long a = -99999999999;
//     public TestType type;
//     private NativeArray<long> nativeA;
//     private void Awake()
//     {
//         nativeA = new NativeArray<long>(1, Allocator.Persistent);
//     }
//     private void Update()
//     {
//         if (type == TestType.MultiThread)
//         {
//             RunTest rt = new RunTest
//             {
//                 threshold = threshold,
//                 a = nativeA
//             };
//             JobHandle jobHandle = rt.Schedule();
//             jobHandle.Complete();
//             a = nativeA[0];
//         }
//         else
//         {
//             for (int i = 0; i < threshold; i++)
//         {
//             for (int j = 0; j < threshold; j++)
//             {
//                 for (int k = 0; k < threshold; k++)
//                 {
//                     for (int l = 0; l < threshold; l++)
//                     {
//                         a++;
//                         // Debug.Log(a++);
//                     }
//                 }
//             }
//         }
//         }

//     }
//     private void OnDestroy()
//     {
//         nativeA.Dispose(); // Clean up to avoid memory leaks
//     }
// }












using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;
using UnityEngine;

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

    private void Awake()
    {
        nativeA = new NativeArray<long>(1, Allocator.Persistent);
        nativeA[0] = a;
        temp = new NativeArray<long>(threshold, Allocator.TempJob);
        isInitialized = true;
        Debug.Log($"Test instance created on {gameObject.name}.");
    }

    private void Update()
    {
        if (!isInitialized) return;

        if (type == TestType.MultiThread)
        {
            RunTestParallel rt = new RunTestParallel
            {
                threshold = threshold,
                a = nativeA,
                temp = temp
            };
            jobHandle = rt.Schedule(threshold, 1, jobHandle);


            jobHandle.Complete();
            for (int i = 0; i < temp.Length; i++)
            {
                nativeA[0] += temp[i];
            }
            temp.Dispose();
            temp = new NativeArray<long>(threshold, Allocator.TempJob); // Recreate for next frame
            a = nativeA[0];
        }
        else
        {
            for (int i = 0; i < threshold; i++)
            {
                for (int j = 0; j < threshold; j++)
                {
                    for (int k = 0; k < threshold; k++)
                    {
                        for (int l = 0; l < threshold; l++)
                        {
                            a++;
                        }
                    }
                }
            }
        }
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