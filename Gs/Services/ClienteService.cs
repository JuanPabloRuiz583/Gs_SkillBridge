using System;
using System.Collections.Generic;
using System.Linq;

using Gs.Data;
using Gs.Dtos;
using Gs.Models;

namespace Gs.Services
{
    public class ClienteService : IClienteService
    {
        private readonly AppDbContext _context;

        public ClienteService(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retorna todos os clientes cadastrados.
        /// </summary>
        public IEnumerable<Cliente> GetAll()
        {
            return _context.Clientes.ToList();
        }

        /// <summary>
        /// Busca um cliente por ID.
        /// </summary>
        public Cliente? GetById(long id)
        {
            return _context.Clientes.Find(id);
        }

        /// <summary>
        /// Cria um novo cliente, validando e-mail duplicado.
        /// </summary>
        public (Cliente? cliente, string? error) Create(ClienteDTO clienteDto)
        {
            if (clienteDto == null)
                return (null, "Dados do cliente são obrigatórios");

            var email = clienteDto.Email?.Trim();

            if (string.IsNullOrWhiteSpace(email))
                return (null, "Email é obrigatório");

            // Normaliza e-mail para comparação
            email = email.ToLowerInvariant();

            var clienteExistente = _context.Clientes
                .FirstOrDefault(c => c.Email != null && c.Email.ToLower() == email);

            if (clienteExistente != null)
                return (null, "um cliente com esse email ja existe");

            var cliente = new Cliente
            {
                Nome = clienteDto.Nome,
                Email = email,
                Senha = clienteDto.Senha,
                ProfissaoAtual = clienteDto.ProfissaoAtual,
                Competencias = clienteDto.Competencias
            };

            _context.Clientes.Add(cliente);
            _context.SaveChanges();

            return (cliente, null);
        }

        /// <summary>
        /// Atualiza os dados de um cliente existente.
        /// </summary>
        public (Cliente? cliente, string? error) Update(long id, ClienteDTO clienteDto)
        {
            if (clienteDto == null)
                return (null, "Dados do cliente são obrigatórios");

            if (id != clienteDto.Id)
                return (null, "ID do corpo não corresponde ao da URL");

            var cliente = _context.Clientes.Find(id);
            if (cliente == null)
                return (null, "Cliente não encontrado");

            // Atualizações simples (mantém o existente se vier nulo)
            if (!string.IsNullOrWhiteSpace(clienteDto.Nome))
                cliente.Nome = clienteDto.Nome;

            if (!string.IsNullOrWhiteSpace(clienteDto.Email))
                cliente.Email = clienteDto.Email.Trim().ToLowerInvariant();

            if (!string.IsNullOrWhiteSpace(clienteDto.Senha))
                cliente.Senha = clienteDto.Senha;

            if (!string.IsNullOrWhiteSpace(clienteDto.ProfissaoAtual))
                cliente.ProfissaoAtual = clienteDto.ProfissaoAtual;

            if (!string.IsNullOrWhiteSpace(clienteDto.Competencias))
                cliente.Competencias = clienteDto.Competencias;

            _context.Clientes.Update(cliente);
            _context.SaveChanges();

            return (cliente, null);
        }

        /// <summary>
        /// Remove um cliente pelo ID.
        /// </summary>
        public bool Delete(long id)
        {
            var cliente = _context.Clientes.Find(id);
            if (cliente == null)
                return false;

            _context.Clientes.Remove(cliente);
            _context.SaveChanges();
            return true;
        }

        /// <summary>
        /// Autentica um cliente por e-mail e senha.
        /// </summary>
        public Cliente? Authenticate(string email, string senha)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
                return null;

            var normalizedEmail = email.Trim().ToLowerInvariant();

            return _context.Clientes
                .FirstOrDefault(c =>
                    c.Email != null &&
                    c.Email.ToLower() == normalizedEmail &&
                    c.Senha == senha);
        }
    }
}
