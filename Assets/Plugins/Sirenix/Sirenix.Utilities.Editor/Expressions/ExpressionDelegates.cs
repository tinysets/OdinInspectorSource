#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="ExpressionDelegates.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="ExpressionDelegates.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.Utilities.Editor.Expressions
{
    internal delegate void ExpressionAction();
    internal delegate void ExpressionAction<T1>(T1 arg1);
    internal delegate void ExpressionAction<T1, T2>(T1 arg1, T2 arg2);
    internal delegate void ExpressionAction<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3);
    internal delegate void ExpressionAction<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    internal delegate void ExpressionAction<T1, T2, T3, T4, T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    internal delegate void ExpressionAction<T1, T2, T3, T4, T5, T6>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
    internal delegate void ExpressionAction<T1, T2, T3, T4, T5, T6, T7>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);
    internal delegate void ExpressionAction<T1, T2, T3, T4, T5, T6, T7, T8>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);
    internal delegate void ExpressionAction<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9);
    internal delegate TResult ExpressionFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg);
    internal delegate TResult ExpressionFunc<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);
    internal delegate TResult ExpressionFunc<T1, T2, T3, T4, T5, T6, T7, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);
    internal delegate TResult ExpressionFunc<T1, T2, T3, T4, T5, T6, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
    internal delegate TResult ExpressionFunc<T1, T2, T3, T4, T5, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    internal delegate TResult ExpressionFunc<T1, T2, T3, T4, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    internal delegate TResult ExpressionFunc<T1, T2, T3, TResult>(T1 arg1, T2 arg2, T3 arg3);
    internal delegate TResult ExpressionFunc<T1, T2, TResult>(T1 arg1, T2 arg2);
    internal delegate TResult ExpressionFunc<T1, TResult>(T1 arg1);
    internal delegate TResult ExpressionFunc<TResult>();
}
#endif