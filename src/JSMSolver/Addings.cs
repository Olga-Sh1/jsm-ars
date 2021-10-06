using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace JSMSolver
{
    /// <summary>Усиления предиката сходства</summary>
    [Flags]
    public enum Addings
    {
        [Description("Простой")]
        Simple = 0,
        [Description("Запрет на контрпримеры")]
        Contr = 1,
        [Description("Метод различия")]
        Difference = 4,
        [Description("Метод сходства-различия")]
        DiffSimilarity = 8,
        [Description("Упрощение метода различия")]
        SimpleDiffSimilarity = 2,
        //[Description("Взвешенный запрет на контр-пр.")]
        //WeightedContr = 16
        //[Description("Единство причины")]
        //Unicity = 8
    }
}
