using MyHomeRamen.Domain.Common.Table;

namespace MyHomeRamen.Domain.Reservations.Tables;

internal static class TableValidator
{
    internal static void Validate(Table table)
    {
        CheckTableNumber(table);
        CheckSeats(table);
    }

    private static void CheckTableNumber(Table table)
    {
        if (table.TableNumber < TableConstants.MinTableNumber)
        {
            throw TableErrors.TableNumberTooSmall();
        }
    }

    private static void CheckSeats(Table table)
    {
        if (table.MinNumberOfSeats < TableConstants.MinSeats)
        {
            throw TableErrors.MinSeatsTooSmall();
        }

        if (table.MaxNumberOfSeats > TableConstants.MaxSeats)
        {
            throw TableErrors.MaxSeatsTooLarge();
        }

        if (table.MaxNumberOfSeats < table.MinNumberOfSeats)
        {
            throw TableErrors.MaxSeatsSmallerThanMinSeats();
        }
    }
}
