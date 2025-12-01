using coolgym_webapi.Contexts.Equipments.Domain.Constants;
using coolgym_webapi.Contexts.Equipments.Domain.Exceptions;

namespace coolgym_webapi.Contexts.Equipments.Domain.Model.ValueObjects;

public record Location
{
    public Location(string name, string address)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw InvalidLocationException.EmptyName();

        if (name.Length > EquipmentDomainConstants.MaxLocationNameLength)
            throw InvalidLocationException.NameTooLong(name.Length);

        if (string.IsNullOrWhiteSpace(address))
            throw InvalidLocationException.EmptyAddress();

        if (address.Length > EquipmentDomainConstants.MaxLocationAddressLength)
            throw InvalidLocationException.AddressTooLong(address.Length);

        Name = name.Trim();
        Address = address.Trim();
    }

    public string Name { get; init; }
    public string Address { get; init; }
}