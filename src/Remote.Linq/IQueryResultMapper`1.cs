﻿// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using System.Linq.Expressions;

    public interface IQueryResultMapper<in TSource>
    {
        TResult? MapResult<TResult>(TSource? source, Expression expression);
    }
}