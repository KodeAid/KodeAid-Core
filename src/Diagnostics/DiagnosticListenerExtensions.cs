// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.Collections.Generic;

namespace System.Diagnostics
{
    public static class DiagnosticListenerExtensions
    {
        /// <summary>
        /// Creates a new <see cref="Activity"/> with the given <paramref name="operationName"/>.
        /// No <see cref="Activity"/> is created if <paramref name="operationName"/> is not enabled on <paramref name="diagnosticListener"/>.
        /// </summary>
        /// <param name="diagnosticListener">The <see cref="DiagnosticListener"/> to invoke on.</param>
        /// <param name="operationName">Name of the <see cref="Activity"/>'s operation.</param>
        /// <param name="parentId">Optional parent ID of the new child activity.</param>
        /// <param name="tags">Tags of the new activity.</param>
        /// <param name="baggage">Baggage of the new activity.</param>
        /// <returns>The newly created but unstarted <see cref="Activity"/>, or null if <paramref name="operationName"/> is not enabled on <paramref name="diagnosticListener"/>.</returns>
        public static Activity CreateActivity(this DiagnosticListener diagnosticListener, string operationName, string parentId = null, IEnumerable<KeyValuePair<string, string>> tags = null, IEnumerable<KeyValuePair<string, string>> baggage = null)
        {
            if (!diagnosticListener.IsEnabled(operationName))
            {
                return null;
            }

            var activity = new Activity(operationName);

            if (parentId != null)
            {
                activity.SetParentId(parentId);
            }

            if (tags != null)
            {
                foreach (var t in tags)
                {
                    activity.AddTag(t.Key, t.Value);
                }
            }

            if (baggage != null)
            {
                foreach (var b in baggage)
                {
                    activity.AddTag(b.Key, b.Value);
                }
            }

            return activity;
        }

        /// <summary>
        /// Creates a new <see cref="Activity"/> with the given <paramref name="operationName"/> and then starts it passing <paramref name="startArgs"/> through to <see cref="DiagnosticSource.StartActivity(Activity, object)"/>.
        /// No <see cref="Activity"/> is created or started if <paramref name="operationName"/> is not enabled on <paramref name="diagnosticListener"/>.
        /// </summary>
        /// <param name="diagnosticListener">The <see cref="DiagnosticListener"/> to invoke on.</param>
        /// <param name="operationName">Name of the <see cref="Activity"/>'s operation.</param>
        /// <param name="parentId">Optional parent ID of the new child activity.</param>
        /// <param name="tags">Tags of the new activity.</param>
        /// <param name="baggage">Baggage of the new activity.</param>
        /// <param name="startArgs">Arguments passed through to <see cref="DiagnosticSource.StartActivity(Activity, object)"/>.</param>
        /// <returns>The newly created and started <see cref="Activity"/>, or null if <paramref name="operationName"/> is not enabled on <paramref name="diagnosticListener"/>.</returns>
        public static Activity StartActivity(this DiagnosticListener diagnosticListener, string operationName, string parentId = null, IEnumerable<KeyValuePair<string, string>> tags = null, IEnumerable<KeyValuePair<string, string>> baggage = null, object startArgs = null)
        {
            var activity = diagnosticListener.CreateActivity(operationName, parentId, tags, baggage);

            if (activity == null)
            {
                return null;
            }

            return diagnosticListener.StartActivity(activity, startArgs);
        }
    }
}
