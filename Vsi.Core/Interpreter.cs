using System.Collections.Generic;

namespace Vsi.Core
{
    public class Interpreter
    {
        public Ast Ast { get; set; }
        public Dictionary<string, object?> Env { get; set; }

        public Interpreter(Ast ast, Dictionary<string, object?>? env = null)
        {
            Ast = ast;
            Env = env ?? new Dictionary<string, object?>();
        }

        public override string ToString()
        {
            string envStr = Env.Count == 0 ? "{}" : "{...}";
            return $"Interpreter({Ast}, {envStr})";
        }

        public void Interpret()
        {
            Ast.Eval(Env);
        }
    }
}
