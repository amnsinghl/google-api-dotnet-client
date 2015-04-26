﻿/*
Copyright 2013 Google Inc

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Util.Store;

namespace Google.Apis.Auth.OAuth2
{
    /// <summary>A helper utility to manage the authorization code flow.</summary>
    public class GoogleWebAuthorizationBroker
    {
        public const string ACCESS_TYPE_OFFLINE = "offline";
        public const string ACCESS_TYPE_ONLINE = "online";

        /// <summary>Asynchronously authorizes the specified user.</summary>
        /// <remarks>
        /// It uses <see cref="Google.Apis.Util.Store.StorageDataStore"/> as the flow's data store by default.
        /// </remarks>
        /// <param name="clientSecrets">The client secrets.</param>
        /// <param name="scopes">
        /// The scopes which indicate the Google API access your application is requesting.
        /// </param>
        /// <param name="user">The user to authorize.</param>
        /// <param name="taskCancellationToken">Cancellation token to cancel an operation.</param>
        /// <param name="accessType">To specify access type of the oauth token</param>
        /// <returns>User credential.</returns>
        public static async Task<UserCredential> AuthorizeAsync(ClientSecrets clientSecrets,
            IEnumerable<string> scopes, string user, CancellationToken taskCancellationToken, string accessType = null)
        {
            var initializer = new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = clientSecrets,
            };

            return await AuthorizeAsyncCore(initializer, scopes, user, taskCancellationToken, accessType).ConfigureAwait(false);
        }

        /// <summary>Asynchronously authorizes the specified user.</summary>
        /// <remarks>
        /// It uses <see cref="Google.Apis.Util.Store.StorageDataStore"/> as the flow's data store by default.
        /// </remarks>
        /// <param name="clientSecretsStream">
        /// The client secrets stream. The AuthorizationCodeFlow constructor is responsible for disposing the stream.
        /// Note that it's more secured to use the authenticate method which gets <see cref="ClientSecrets"/>, and
        /// hard code your secrets in your app rather than storing the secrets in a separate unsecured file.
        /// </param>
        /// <param name="scopes">
        /// The scopes which indicate the Google API access your application is requesting.
        /// </param>
        /// <param name="user">The user to authorize.</param>
        /// <param name="taskCancellationToken">Cancellation token to cancel an operation.</param>
        /// <returns>User credential.</returns>
        public static async Task<UserCredential> AuthorizeAsync(Stream clientSecretsStream,
            IEnumerable<string> scopes, string user, CancellationToken taskCancellationToken)
        {
            var initializer = new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecretsStream = clientSecretsStream,
            };

            return await AuthorizeAsyncCore(initializer, scopes, user, taskCancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously reauthorizes the user. This method should be called if the users want to authorize after 
        /// they revoked the token.
        /// </summary>
        /// <param name="userCredential">The current user credential. Its <see cref="UserCredential.Token"/> will be
        /// updated. </param>
        /// <param name="taskCancellationToken">Cancellation token to cancel an operation.</param>
        public static async Task ReauthorizeAsync(UserCredential userCredential,
            CancellationToken taskCancellationToken)
        {
            var installedApp = new AuthorizationCodeWPInstalledApp(userCredential.Flow);
            // Create an authorization code installed app instance and authorize the user.
            UserCredential newUserCredential = await installedApp.AuthorizeAsync(userCredential.UserId,
                taskCancellationToken).ConfigureAwait(false);
            userCredential.Token = newUserCredential.Token;
        }

        /// <summary>The core logic for asynchronously authorizing the specified user.</summary>
        /// <param name="initializer">The authorization code initializer.</param>
        /// <param name="scopes">
        /// The scopes which indicate the Google API access your application is requesting.
        /// </param>
        /// <param name="user">The user to authenticate.</param>
        /// <param name="taskCancellationToken">Cancellation token to cancel an operation.</param>
        /// <param name="accessType">To specify access type of the oauth token</param>
        /// <returns>User credential.</returns>
        private static async Task<UserCredential> AuthorizeAsyncCore(
            GoogleAuthorizationCodeFlow.Initializer initializer, IEnumerable<string> scopes, string user,
            CancellationToken taskCancellationToken, string accessType = null)
        {
            initializer.Scopes = scopes;
            initializer.DataStore = new StorageDataStore();

            var installedApp = new AuthorizationCodeWPInstalledApp(new GoogleAuthorizationCodeFlow(initializer, accessType));
            return await installedApp.AuthorizeAsync(user, taskCancellationToken).ConfigureAwait(false);
        }
    }
}
