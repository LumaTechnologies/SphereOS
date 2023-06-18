using Cosmos.System;

namespace SphereOS.Shell
{
    /// <summary>
    /// Describes the result of a ReadLineEx operation. 
    /// </summary>
    public struct ReadLineExResult
    {
        public ReadLineExResult(string input) : this()
        {
            Input = input;
        }

        public ReadLineExResult(ConsoleKeyEx cancelKey)
        {
            Cancelled = true;
            CancelKey = cancelKey;
        }

        /// <summary>
        /// The user input to ReadLineEx, if it was not cancelled.
        /// </summary>
        public string Input = string.Empty;

        /// <summary>
        /// If the ReadLineEx was cancelled by a cancel key.
        /// </summary>
        public bool Cancelled = false;

        /// <summary>
        /// The key that was used to cancel the ReadLineEx.
        /// This will be <see cref="ConsoleKeyEx.NoName"/> if the operation was not cancelled by the user.
        /// </summary>
        public ConsoleKeyEx CancelKey = ConsoleKeyEx.NoName;
    }
}
