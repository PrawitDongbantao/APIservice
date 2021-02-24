using System;
using System.Linq;
using APIservice.Models;
using APIservice.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace APIservice.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        public IAuthRepository AuthRepository { get; }
        public AuthController(IAuthRepository authRepository)
        {
            this.AuthRepository = authRepository;

        }

        [HttpPost("login")] // ทำ sub part
        public IActionResult Login([FromBody] User model)
        {
            try
            {
                (User users, string token) = AuthRepository.Login(model); //ประกาศ paramiter / type ไปอีก

                if (users == null)
                {
                    return Unauthorized(new { message = "username not found", token = String.Empty }); // ถ้า login ไม่ผ่านก็ ส่งไปว่าไม่ผ่าน 
                }
                if (String.IsNullOrEmpty(token))
                {
                    return Unauthorized(new { message = "password incorrect", token = String.Empty }); // ถ้า password ผิด 
                }
                return Ok(new { message = "login successfully", token = token });
            }
            catch (Exception error)
            {
                return StatusCode(500, new { message = error }); // ถ้า catch ก็ ส่ง error code 500 
                //return BadRequest(); // 
            }
        }

        [HttpPost("[action]")] // [acction] ->   คือ function นึง ใน web API หมายความขอ ขอใช้ชื่อ path เดียวกันกับ ชื่อ ฟังก์ชั่น
        public IActionResult Register([FromBody] User user)
        {
            try
            {
                AuthRepository.Register(user);
                return Ok(new { message = "register successfully" });
            }
            catch (Exception error)
            {
                return StatusCode(500, new { message = error });
            }
        }

    }
}