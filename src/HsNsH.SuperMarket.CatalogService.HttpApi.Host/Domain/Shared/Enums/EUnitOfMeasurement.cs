using System.ComponentModel;

namespace HsNsH.SuperMarket.CatalogService.Domain.Shared.Enums;

public enum EUnitOfMeasurement : byte
{
    [Description("UN")]
    Unity = 1,

    [Description("MG")]
    Milligram = 2,

    [Description("G")]
    Gram = 3,

    [Description("KG")]
    Kilogram = 4,

    [Description("L")]
    Liter = 5
}