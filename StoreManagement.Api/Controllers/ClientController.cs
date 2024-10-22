﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreManagement.Application.Interfaces;
using StoreManagement.Bases;
using StoreManagement.Bases.Domain.Model;
using StoreManagement.Domain.Dtos.Client;

namespace StoreManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class ClientController : ApiControllersBase
    {
        private readonly IClientService _clientService;
        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }
        [HttpPost]

        public async Task<IActionResult> AddClient([FromBody] AddClientDto addClientDto)
        {
            var client = await _clientService.AddClient(addClientDto);
            return Ok(client);
        }
        [HttpGet]

        public async Task<IActionResult> GetClient(Guid id)
        {
            var client = await _clientService.GetClient(id);
            return Ok(client);
        }

        [HttpGet("GetAll")]
        //[Authorize(nameof(UserRole.Cashier))]

        public async Task<IActionResult> GetClients([FromQuery] GetAllClientsFilter clientsFilter)
        {
            var clients = await _clientService.GetClients(clientsFilter);
            return Ok(clients);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateClient(UpdateClientDto clientDto, Guid id)
        {
            var updatedClient = await _clientService.UpdateClient(clientDto, id);
            return Ok(updatedClient);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteClient(Guid id)
        {
            var result = await _clientService.DeleteClient(id);
            return Ok(result);
        }
    }
}
