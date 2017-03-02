﻿// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.ExpressionVisitors.ExpressionTranslator
{
    using Remote.Linq;
    using Remote.Linq.Expressions;
    using Shouldly;
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using Xunit;

    public class When_subquery_expression_use_same_parameter_name
    {
        interface IValue
        {
            int Value { get; }
        }

        class A : IValue
        {
            public int Value { get; set; }

            public override bool Equals(object obj) => Value == (obj as A)?.Value;

            public override int GetHashCode() => Value;
        }

        class B : IValue
        {
            public int Value { get; set; }

            public override bool Equals(object obj) => Value == (obj as B)?.Value;

            public override int GetHashCode() => Value;
        }

        [Fact]
        public void Parameter_expression_should_be_resolved_by_instance_rather_then_by_name()
        {
            IQueryable<A> localQueryable1 = new[]
            {
                new A { Value = 1 },
                new A { Value = 2 },
                new A { Value = 3 },
                new A { Value = 4 },
            }.AsQueryable();

            IQueryable<B> localQueryable2 = new[]
            {
                new B { Value = 1 },
                new B { Value = 2 },
                new B { Value = 3 },
                new B { Value = 4 },
            }.AsQueryable();

            Func<Type, IQueryable> queryableProvider = t =>
            {
                if (t == typeof(A)) return localQueryable1;
                if (t == typeof(B)) return localQueryable2;
                return null;
            };

            IQueryable<A> remoteQueryable1 = RemoteQueryable.Create<A>(x => x.Execute(queryableProvider: queryableProvider));
            IQueryable<B> remoteQueryable2 = RemoteQueryable.Create<B>(x => x.Execute(queryableProvider: queryableProvider));
            
            A[] localResult = BuildQuery(localQueryable1, localQueryable2).ToArray();
            A[] remoteResult = BuildQuery(remoteQueryable1, remoteQueryable2).ToArray();

            remoteResult.SequenceEqual(localResult).ShouldBeTrue();
        }

        [Fact]
        public void Parameter_expression_should_be_resolved_by_instance_rather_then_by_name2()
        {
            IQueryable<A> localQueryable = new[]
            {
                new A { Value = 1 },
                new A { Value = 2 },
                new A { Value = 3 },
                new A { Value = 4 },
            }.AsQueryable();

            IQueryable<A> remoteQueryable = 
                RemoteQueryable.Create<A>(x => x.Execute(queryableProvider: t => localQueryable));

            A[] localResult = BuildQuery(localQueryable, localQueryable).ToArray();
            A[] remoteResult = BuildQuery(remoteQueryable, remoteQueryable).ToArray();

            remoteResult.SequenceEqual(localResult).ShouldBeTrue();
        }

        private static IQueryable<T1> BuildQuery<T1,T2>(IQueryable<T1> queriable1, IQueryable<T2> queriable2)
            where T1: IValue
            where T2: IValue
        {
            Expression<Func<T2, bool>> subfilter = 
                x => x.Value % 2 == 0;

            Expression<Func<T1, bool>> outerfilter = 
                x => queriable2.Where(subfilter).Where(d => d.Value == x.Value).Any();

            return queriable1.Where(outerfilter);
        }
    }
}