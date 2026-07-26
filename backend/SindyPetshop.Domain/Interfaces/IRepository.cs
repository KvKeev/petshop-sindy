namespace SindyPetshop.Domain.Interfaces;

// Contrato genérico: operaciones básicas que cualquier entidad va a necesitar
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    void Update(T entity);
    void Remove(T entity);
    Task SaveChangesAsync();
}