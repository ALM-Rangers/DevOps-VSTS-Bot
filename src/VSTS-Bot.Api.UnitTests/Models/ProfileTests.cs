// ———————————————————————————————
// <copyright file="ProfileTests.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains tests for the profile model.
// </summary>
// ———————————————————————————————
namespace Vsar.TSBot.UnitTests.Models
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    [TestCategory("Unit")]
    [ExcludeFromCodeCoverage]
    public class ProfileTests
    {
        [TestMethod]
        public void DisplayName()
        {
            var target = new Profile
            {
                Id = new Guid("67A5E965-0B59-4E31-A21B-ED9032C0E6B4"),
                TokenEncrypted = "90wXyNupQ+GFhWTa3P0UQdcHoFudTH5X8yAcAXkYj+6zbbVEb0Ja995dKacOlg6nl5n8fHpHiCWMlf2fIlIymhFfPrYImqrOFRe35W1htnNeWxfGzuW3qmRjDfdMoBK/mSn28jMQcHtyZvxDPEubStp+xutbmh6x95lEEbkOZbhiVp8f751jbVgOCX57/QRHwn18uN85+fh1Pqc3L07Vo3/EPJYL9ciRgC9BP/HhqzFyXgAXgOyR5Xm52wr/UtfcBgxAvmn++fiUD8E7pc5lvmyH/bHtF/X8wDUmltP5UUTO7wvxbGFfUAID+ln25DiH",
                DisplayNameEncrypted = "mBfXWXCzx5S/U1aoLy7tzXn4/uqLsjH3rgxnkNDvT7IIKlUJwgcPd5Te5Dd1xjle"
            };

            target.DisplayName.Should().Be("\"me\"");
            target.Token.AccessToken.Should().Be("x25onorum4neacdjmvzvaxjeosik7qxo7fbnn6lebefeday7fxmq");
        }
    }
}