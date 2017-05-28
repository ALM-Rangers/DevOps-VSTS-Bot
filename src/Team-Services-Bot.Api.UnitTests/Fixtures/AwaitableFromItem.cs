// ———————————————————————————————
// <copyright file="AwaitableFromItem.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Represents an implementation for IAwaitable.
// </summary>
// ———————————————————————————————
namespace Vsar.TSBot.UnitTests
{
    using System;
    using System.Runtime.CompilerServices;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Internals.Fibers;

    public sealed class AwaitableFromItem<T> : IAwaitable<T>, IAwaiter<T>
    {
        private readonly T item;

        public AwaitableFromItem(T item)
        {
            this.item = item;
        }

        bool IAwaiter<T>.IsCompleted => true;

        T IAwaiter<T>.GetResult()
        {
            return this.item;
        }

        void INotifyCompletion.OnCompleted(Action continuation)
        {
            throw new NotImplementedException();
        }

        IAwaiter<T> IAwaitable<T>.GetAwaiter()
        {
            return this;
        }
    }
}