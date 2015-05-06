
using Shiro.Class;

namespace Shiro.ComboBox
{
    internal sealed class ComboboxItemCustomer
    {
        public string Text { private get; set; }
        public Customer Value { get; set; }

        public override string ToString()
        {
            return Value.Name;
        }
    }
}
