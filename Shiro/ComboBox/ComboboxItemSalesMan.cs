
using Shiro.Class;

namespace Shiro.ComboBox
{
    internal class ComboboxItemSalesMan
    {
        public string Text { private get; set; }
        public SalesMan Value { get; set; }

        public override string ToString()
        {
            return Value.NAME;
        }
    }
}
