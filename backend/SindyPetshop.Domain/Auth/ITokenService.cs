using SindyPetshop.Domain.Entities;

namespace SindyPetshop.Domain.Interfaces;

public interface ITokenService
{
    string GenerarToken(Cliente cliente);
}