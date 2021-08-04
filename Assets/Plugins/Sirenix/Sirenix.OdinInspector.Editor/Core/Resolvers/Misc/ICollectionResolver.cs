#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="ICollectionResolver.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="ICollectionResolver.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor
{
    using System;

    public interface ICollectionResolver : IApplyableResolver, IRefreshableResolver
    {
        bool IsReadOnly { get; }

        Type ElementType { get; }

        int MaxCollectionLength { get; }

        void QueueRemove(object[] values);

        void QueueRemove(object value, int selectionIndex);

        void QueueAdd(object[] values);

        void QueueAdd(object value, int selectionIndex);

        void QueueClear();

        bool CheckHasLengthConflict();

        void EnqueueChange(Action action);
    }
}
#endif