using SindyPetshop.Domain.Entities;

namespace SindyPetshop.Domain.Interfaces;

// Calcula el costo de envío según el método de entrega elegido.
// La dirección se recibe como parámetro para dejar la puerta abierta a una futura
// implementación por distancia real (necesita geocodificación de Direccion, todavía
// sin resolver - Direccion hoy solo tiene Calle/Ciudad como texto libre, sin lat/lng).
public interface ICostoEnvioService
{
    decimal Calcular(MetodoEntrega metodoEntrega, Direccion? direccion);
}