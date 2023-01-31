namespace SphereOS.Commands
{
    /// <summary>
    /// Describes whether a command executed successfully.
    /// </summary>
    internal enum ReturnCode : byte
    {
        /// <summary>
        /// The command completed successfully.
        /// </summary>
        Success = 0,

        /// <summary>
        /// The command failed.
        /// </summary>
        Failure = 1,

        /// <summary>
        /// The specified file, directory, or device was not found.
        /// </summary>
        NotFound = 2,

        /// <summary>
        /// The command was provided with invalid arguments.
        /// </summary>
        Invalid = 3,

        /// <summary>
        /// The user is unauthorised to execute this command.
        /// </summary>
        Unauthorised = 4,

        /// <summary>
        /// The command was aborted.
        /// </summary>
        Aborted = 5
    }
}
