// ———————————————————————————————
// <copyright file="DocumentClientExtensions.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Provide extensions for the document client.
// </summary>
// ———————————————————————————————
namespace Vsar.TSBot
{
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;

    /// <summary>
    /// Provides extensions for the document client.
    /// </summary>
    public static class DocumentClientExtensions
    {
        /// <summary>
        /// Creates the database and collection if it does not exist.
        /// </summary>
        /// <param name="documentClient">The document client.</param>
        /// <param name="databaseId">The id of the database.</param>
        /// <param name="collectionId">The id of the collection.</param>
        public static void CreateCollectionIfDoesNotExist(this IDocumentClient documentClient, string databaseId, string collectionId)
        {
            documentClient.CreateDatabaseIfNotExistsAsync(databaseId).Wait();
            documentClient.CreateCollectionIfNotExistsAsync(databaseId, collectionId).Wait();
        }

        private static async Task CreateDatabaseIfNotExistsAsync(this IDocumentClient documentClient, string databaseId)
        {
            try
            {
                await documentClient.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(databaseId));
            }
            catch (DocumentClientException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    await documentClient.CreateDatabaseAsync(new Database { Id = databaseId });
                }
                else
                {
                    throw;
                }
            }
        }

        private static async Task CreateCollectionIfNotExistsAsync(this IDocumentClient documentClient, string databaseId, string collectionId)
        {
            try
            {
                await documentClient.ReadDocumentCollectionAsync(
                    UriFactory.CreateDocumentCollectionUri(databaseId, collectionId));
            }
            catch (DocumentClientException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    await documentClient.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(databaseId),
                        new DocumentCollection { Id = collectionId });
                }
                else
                {
                    throw;
                }
            }
        }
    }
}