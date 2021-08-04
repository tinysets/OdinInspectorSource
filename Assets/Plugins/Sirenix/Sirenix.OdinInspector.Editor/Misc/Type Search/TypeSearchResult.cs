#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="TypeSearchResult.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="TypeSearchResult.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor.TypeSearch
{
    using System;

    public struct TypeSearchResult
    {
        public TypeSearchInfo MatchedInfo;
        public Type MatchedType;
        public Type[] MatchedTargets;
        public TypeMatchRule MatchedRule;
    }
}
#endif