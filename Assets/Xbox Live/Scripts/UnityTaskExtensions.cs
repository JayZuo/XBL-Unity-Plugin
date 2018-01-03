// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#if ENABLE_WINMD_SUPPORT
using System.Collections;
using System.Threading.Tasks;

using UnityEngine;

public static class UnityTaskExtensions
{
    public static IEnumerator AsCoroutine(this Task task)
    {
        yield return new WaitUntil(() => task.IsCompleted || task.IsFaulted || task.IsCanceled);
    }
}
#endif
