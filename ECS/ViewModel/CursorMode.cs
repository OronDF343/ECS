using System.ComponentModel;

namespace ECS.ViewModel
{
    public enum CursorMode
    {
        [Description("Arrange items")]
        ArrangeItems,

        [Description("Connect elements")]
        ConnectToNode,

        [Description("Add resistors")]
        AddResistor,

        [Description("Add voltage sources")]
        AddVoltageSource,

        [Description("Add nodes")]
        AddNode,

        [Description("Add reference nodes")]
        AddRefNode,

        [Description("Add switches")]
        AddSwitch
    }
}
