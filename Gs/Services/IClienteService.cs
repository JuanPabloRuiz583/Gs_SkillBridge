using System.Collections.Generic;

using Gs.Dtos.Request;
using Gs.Models;

namespace Gs.Services
{
    public interface IClienteService
    {
        /// <summary>
        /// Retorna todos os clientes cadastrados.
        /// </summary>
        IEnumerable<Cliente> GetAll();

        /// <summary>
        /// Busca um cliente por ID.
        /// </summary>
        Cliente? GetById(long id);

        /// <summary>
        /// Cria um novo cliente a partir do DTO de request
        /// e retorna a entidade criada ou uma mensagem de erro.
        /// </summary>
        (Cliente? cliente, string? error) Create(ClienteRequestDTO clienteDto);

        /// <summary>
        /// Atualiza os dados de um cliente existente.
        /// </summary>
        (Cliente? cliente, string? error) Update(long id, ClienteRequestDTO clienteDto);

        /// <summary>
        /// Remove um cliente pelo ID.
        /// </summary>
        bool Delete(long id);

        /// <summary>
        /// Autentica um cliente por e-mail e senha.
        /// </summary>
        Cliente? Authenticate(string email, string senha);
    }
}
