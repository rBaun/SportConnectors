﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.BusinessLogic;
using Api.DAL.Entities;
using Api.DataTransferObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Authorize]
    [EnableCors("allowOrigin")]
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase {

        private readonly PlayerLogic _playerLogic;

        public PlayerController(PlayerLogic playerLogic) {
            _playerLogic = playerLogic;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Register([FromBody] Player entity) {

            Player player = _playerLogic.Create(entity);

            return Ok(player);
        }

        [Route("[action]")]
        public IActionResult GetById([FromBody] Player entity) {
            Player player = _playerLogic.GetById(entity.Id);

            return Ok(player);
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult Update([FromBody] Player entity) {

            var player = _playerLogic.Update(entity);

            return Ok(player);
        }


        [HttpPost]
        [Route("[action]")]
        public IActionResult SearchPlayers([FromBody] SearchCriteriaForPlayer request) {
            var firsTime = DateTime.Now;
            List<Player> list = _playerLogic.HandleSearchAlgorithm(request);
            var afterTime = DateTime.Now;

            List<DateTime> lol = new List<DateTime>();
            lol.Add(firsTime);
            lol.Add(afterTime);

            
            return Ok(list);
        }
    }
}
