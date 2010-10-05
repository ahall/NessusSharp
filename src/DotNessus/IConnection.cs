using System;
using System.Collections.Generic;

namespace DotNessus
{
    /// <summary>
    /// Represents a connection to a Nessus XMLrpc backend.
    /// </summary>
    public interface IConnection
    {
        /// <summary>
        /// Creates a policy on the backend.
        /// </summary>
        /// <param name="policy">
        /// A <see cref="Policy"/>
        /// </param>
        void CreatePolicy(Policy policy);

        /// <summary>
        /// Returns all available policies.
        /// </summary>
        /// <returns>
        /// A <see cref="IList<Policy>"/> representing all available policies.
        /// </returns>
        List<Policy> ListPolicies();

        /// <summary>
        /// Deletes an existing policy. Aftewards the id has been set to INVALID_ID.
        /// </summary>
        /// <param name="policy">
        /// A <see cref="Policy"/>
        /// </param>
        void DeletePolicy(Policy policy);
    }
}

