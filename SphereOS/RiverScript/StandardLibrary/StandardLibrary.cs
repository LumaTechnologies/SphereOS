using RiverScript.VM;
using SphereOS;

namespace RiverScript.StandardLibrary
{
    internal static partial class StandardLibrary
    {
        internal static void LoadStandardLibrary(Interpreter interpreter)
        {
            // IO
            interpreter.DefineVariable("print", Stdlib_print, scope: null);
            interpreter.DefineVariable("read", Stdlib_read, scope: null);

            // Types
            interpreter.DefineVariable("str", Stdlib_str, scope: null);
            interpreter.DefineVariable("num", Stdlib_num, scope: null);
            interpreter.DefineVariable("typeof", Stdlib_typeof, scope: null);

            // Maths
            interpreter.DefineVariable("floor", Stdlib_floor, scope: null);
            interpreter.DefineVariable("round", Stdlib_round, scope: null);
            interpreter.DefineVariable("ceil", Stdlib_ceil, scope: null);

#if SPHEREOS
            interpreter.DefineVariable("_ENV", new VMString("RiverScript (SphereOS)"), scope: null);
            interpreter.DefineVariable("_SPHEREOS", new VMString(Kernel.Version), scope: null);
#else
            interpreter.DefineVariable("_ENV", new VMString("RiverScript"), scope: null);
#endif
        }
    }
}
