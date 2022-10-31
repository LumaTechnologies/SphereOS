using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThumbOS.Commands
{
    /// <summary>
    /// Describes whether a command executed successfully.
    /// </summary>
    internal enum ReturnCode
    {
        /// <summary>
        /// The command completed successfully.
        /// </summary>
        Success,

        /// <summary>
        /// The command was provided with invalid arguments.
        /// </summary>
        Invalid,

        /// <summary>
        /// The specified file, directory, or device was not found.
        /// </summary>
        NotFound,

        /// <summary>
        /// The command failed.
        /// </summary>
        Failure,

        /// <summary>
        /// The user is unauthorised to execute this command.
        /// </summary>
        Unauthorised
    }
}
