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
        private bool disposed; // To detect redundant calls to Disposable()

        ~ControllerData()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Gets or sets <see cref="HttpConfiguration"/> for the controller.
        /// </summary>
        public HttpConfiguration HttpConfiguration { get; set; }

        /// <summary>
        /// Gets or sets <see cref="MessagesController"/> for the controller.
        /// </summary>
        public MessagesController Controller { get; set; }

        /// <summary>
        /// Gets or sets <see cref="HttpResponseMessage"/> for the controller.
        /// </summary>
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

        /// <summary>
        /// Implements IDisposable pattern. Disposes all managed and unmanaged objects if not disposed already.
        /// </summary>
        /// <param name="disposing">True if managed objects should be disposed and false otherwise.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.Controller?.Dispose();
                    this.Response?.Dispose();
                }

                this.disposed = true;
            }
        }
    }
}
