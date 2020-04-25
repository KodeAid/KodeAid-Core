// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace AutoMapper
{
    public static class MappingExpressionExtensions
    {
        /// <summary>
        /// Ignore all source members which have a null value.
        /// </summary>
        public static void IgnoreAllNullMembers<TSource, TDestination>(this IMappingExpression<TSource, TDestination> mappingExpression)
            => mappingExpression.ForAllMembers(a => a.MapFrom<IgnoreNullSourceValues<TSource, TDestination>, object>(a.DestinationMember.Name));

        /// <summary>
        /// Ignore all source members not previously configured which have a null value.
        /// </summary>
        public static void IgnoreAllOtherNullMembers<TSource, TDestination>(this IMappingExpression<TSource, TDestination> mappingExpression)
            => mappingExpression.ForAllOtherMembers(a => a.MapFrom<IgnoreNullSourceValues<TSource, TDestination>, object>(a.DestinationMember.Name));

        private class IgnoreNullSourceValues<TSource, TDestination> : IMemberValueResolver<TSource, TDestination, object, object>
        {
            public object Resolve(TSource source, TDestination destination, object sourceMember, object destinationMember, ResolutionContext context)
                => sourceMember ?? destinationMember;
        }
    }
}
