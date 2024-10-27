// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


#if NET8_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using AutoMapper.Configuration;
#endif

namespace AutoMapper
{
    public static class MappingExpressionExtensions
    {
#if NET8_0_OR_GREATER
        [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "get_TypeMapActions")]
        private static extern List<Action<TypeMap>> GetTypeMapActions(TypeMapConfiguration typeMapConfiguration);
#endif

        ///// <summary>
        ///// Skip all mappings where the source member resolves to a null value.
        ///// </summary>
        ///// <remarks>
        ///// This leaves the destination member untouched when the source member resolves to a null value.
        ///// </remarks>
        //public static IMappingExpression<TSource, TDestination> SkipMemberMappingWhenSourceValueResolvesToNull<TSource, TDestination>(this IMappingExpression<TSource, TDestination> mappingExpression)
        //{
        //    mappingExpression.ForAllMembers(o => o.Condition((_, _, s, d, c) => s is not null));
        //    return mappingExpression;
        //}

        /// <summary>
        /// Ignore all source members which have a null value.
        /// </summary>
        public static IMappingExpression<TSource, TDestination> IgnoreAllNullMembers<TSource, TDestination>(this IMappingExpression<TSource, TDestination> mappingExpression)
        {
            mappingExpression.ForAllMembers(a => a.MapFrom<IgnoreNullSourceValues<TSource, TDestination>, object>(a.DestinationMember.Name));
            return mappingExpression;
        }

        /// <summary>
        /// Ignore all source members not previously configured which have a null value.
        /// </summary>
        public static void IgnoreAllOtherNullMembers<TSource, TDestination>(this IMappingExpression<TSource, TDestination> mappingExpression)
            => mappingExpression.ForAllOtherMembers(a => a.MapFrom<IgnoreNullSourceValues<TSource, TDestination>, object>(a.DestinationMember.Name));

#if NET8_0_OR_GREATER
        public static void ForAllOtherMembers<TSource, TDestination>(this IMappingExpression<TSource, TDestination> expression, Action<IMemberConfigurationExpression<TSource, TDestination, object>> memberOptions)
        {
            var typeMapConfiguration = (TypeMapConfiguration)expression;
            var typeMapActions = GetTypeMapActions(typeMapConfiguration);

            typeMapActions.Add(typeMap =>
            {
                var destinationTypeDetails = typeMap.DestinationTypeDetails;

                foreach (var accessor in destinationTypeDetails.WriteAccessors.Where(m => typeMapConfiguration.GetDestinationMemberConfiguration(m) == null))
                {
                    expression.ForMember(accessor.Name, memberOptions);
                }
            });
        }
#endif

        private class IgnoreNullSourceValues<TSource, TDestination> : IMemberValueResolver<TSource, TDestination, object, object>
        {
            public object Resolve(TSource source, TDestination destination, object sourceMember, object destinationMember, ResolutionContext context)
                => sourceMember ?? destinationMember;
        }
    }
}
