using SindyPetshop.Domain.Entities;
using SindyPetshop.Domain.Interfaces;

namespace SindyPetshop.Infrastructure.Services;

// Implementación provisoria: tarifa fija para cualquier Envío, sin distinguir distancia.
// Retiro siempre $0. Cuando Sindy confirme la fórmula real por tramos (y se resuelva
// cómo geocodificar las direcciones para calcular km reales, incluyendo si hay que
// rechazar envíos fuera de un radio de cobertura), se reemplaza esta clase por una que
// sí calcule por distancia - el resto del checkout no se entera del cambio, solo se
// actualiza el registro de ICostoEnvioService en Program.cs.
public class TarifaPlanaCostoEnvioService : ICostoEnvioService
{
    private readonly decimal _tarifaPlana;

    public TarifaPlanaCostoEnvioService(decimal tarifaPlana)
    {
        _tarifaPlana = tarifaPlana;
    }

    public decimal Calcular(MetodoEntrega metodoEntrega, Direccion? direccion)
    {
        return metodoEntrega == MetodoEntrega.Envio ? _tarifaPlana : 0m;
    }
}