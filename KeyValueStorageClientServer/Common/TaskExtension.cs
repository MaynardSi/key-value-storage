﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace Common
{
    public static class TaskExtension
    {
        // https://devblogs.microsoft.com/pfxteam/how-do-i-cancel-non-cancelable-async-operations/
        public static async Task<T> WithCancellation<T>(this Task<T> task, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();
            using (cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).TrySetResult(true), tcs))
                if (task != await Task.WhenAny(task, tcs.Task))
                    throw new OperationCanceledException(cancellationToken);
            return await task;
        }
    }
}