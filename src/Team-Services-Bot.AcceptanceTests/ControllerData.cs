// ———————————————————————————————
// <copyright file="ControllerData.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains the data used in controller tests.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot.AcceptanceTests
{
    using System;
    using System.Net.Http;
    using System.Web.Http;

    /// <summary>
    /// State for controller tests
    /// </summary>
    public class ControllerData : IDisposable
    {
        private bool disposedValue = false; // To detect redundant calls

        public HttpConfiguration HttpConfiguration { get; set; }

        public MessagesController Controller { get; set; }

        public HttpResponseMessage Response { get; set; }

        /// <summary>
        /// Dispose of the disposable objects.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    this.Controller?.Dispose();
                    this.Response?.Dispose();
                }

                this.disposedValue = true;
            }
        }
    }
}
