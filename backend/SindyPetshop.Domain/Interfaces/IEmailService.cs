namespace SindyPetshop.Domain.Interfaces;


public interface IEmailService
{
    Task EnviarConfirmacionPedidoAsync(
        string destinatarioEmail,
        string destinatarioNombre,
        int pedidoId,
        decimal total,
        Guid trackingToken,
        string? linkPago);

    Task EnviarActivacionCuentaAsync(
        string destinatarioEmail,
        string destinatarioNombre,
        Guid tokenActivacion);
}