// ———————————————————————————————
// <copyright file="EncryptionExtensionsTests.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains tests for the encryption extensions.
// </summary>
// ———————————————————————————————
namespace Vsar.TSBot.UnitTests.Extensions
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [ExcludeFromCodeCoverage]
    [TestClass]
    [TestCategory("Unit")]
    public class EncryptionExtensionsTests
    {
        [TestMethod]
        public void EncryptDecrypt()
        {
            var token = new OAuthToken
            {
                AccessToken = "12345",
                AppSecret = "67890",
                AuthorizeUrl = new Uri("https://authorize.url"),
                CreatedOn = DateTime.Now,
                ExpiresIn = 900,
                RefreshToken = "1234567890",
                TokenType = "jwt-bearer"
            };

            var key = Guid.NewGuid();

            var encrypted = token.Encrypt(key.ToString());
            var decrypted = encrypted.Decrypt<OAuthToken>(key.ToString());
            decrypted.ShouldBeEquivalentTo(token);
        }
    }
}