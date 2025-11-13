using Gs.Dtos;
using Gs.Models;

namespace Gs.Services
{
    public interface IClienteService
    {
        IEnumerable<Cliente> GetAll();
        Cliente GetById(long id);
        (Cliente cliente, string error) Create(ClienteDTO clienteDto);
        (Cliente cliente, string error) Update(long id, ClienteDTO clienteDto);
        bool Delete(long id);
        Cliente Authenticate(string email, string senha);
    }
}
