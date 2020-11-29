using System.ComponentModel;

namespace SII5.Models
{
    public enum MemoryType
    {
        [Description("Оперативная память")]
        RAM = 0,
        [Description("Графическая память")]
        GraphicMemory = 1,
        [Description("Память микроконтроллеров")]
        MicrocontrollerRAM = 2,
        [Description("Кэш-память")]
        Cache = 3,
        [Description("Вторичная память")]
        SecondaryMemory = 4,
    }
}
