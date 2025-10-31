namespace coolgym_webapi.Contexts.Equipments.Domain.Model.ValueObjects;

public record Location
{
    public string Name { get; init; }
    public string Address { get; init; }

    public Location(string name, string address)
    {
        // Validación de Name
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre de la ubicación no puede estar vacío.", nameof(name));
        
        if (name.Length > 100)
            throw new ArgumentException("El nombre de la ubicación no puede exceder 100 caracteres.", nameof(name));
        
        // Validación de Address
        if (string.IsNullOrWhiteSpace(address))
            throw new ArgumentException("La dirección no puede estar vacía.", nameof(address));
        
        if (address.Length > 200)
            throw new ArgumentException("La dirección no puede exceder 200 caracteres.", nameof(address));

        Name = name.Trim();
        Address = address.Trim();
    }
}