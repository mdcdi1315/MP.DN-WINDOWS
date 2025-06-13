

namespace MP.ExceptionSystem
{
    public sealed class VariableNotFoundException : BaseException
    {
        private System.String name;

        public VariableNotFoundException(System.String name) : base($"The requested variable does not exist. \nVariable name: {name}") { this.name = name; }

        public System.String VariableName => name;
    }
}