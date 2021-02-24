using APIservice.Models;
using APIservice.Repositories.Interfaces;
using APIservice.Entity;
using APIservice.Installers;
using System.Linq;
using System;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Cryptography;

namespace APIservice.Repositories.Classes
{
    public class AuthRepository : IAuthRepository
    {
        private readonly CMPOSContext _context;
        private readonly JwtSettings jwtSettings;
        public AuthRepository(CMPOSContext context, JwtSettings jwtSettings)   //jwtSettings เราทำอีนี้ เป็น service ไปแล้ว (อีกนัย คือประกาศ static ไปแล้ว)
        {
            this.jwtSettings = jwtSettings;
            _context = context;
        }
        public (User, string) Login(User user)
        {
            var result = _context.Users.SingleOrDefault(u => u.Username == user.Username); //SingleOrDefault error ส่งค่า null ....... =>  อีนี้ เรีย แรมด้า
            var token = String.Empty;
            if (result != null && VerifyPassword(result.Password, user.Password)) // function  การตรวจสอบ password ของเขาเอง ใช้วิธีนี้
            {
                token = BuildToken(result);
            }
            return (result, token);
        }
        public void Register(User user)
        {
            user.Password = CreatePasswordHash(user.Password);
            _context.Users.Add(user);
            _context.SaveChanges();
        }
        public bool VerifyPassword(string hash, string password) //วิธีการตรวจ คือเอา password mี่ user กรอกมา ไป  hash แล้ว มาเทียบกับ database ไม่มีการ ถอดรหัสจนเหลือแค่ password
        {
            var parts = hash.Split('.', 2);

            if (parts.Length != 2) // ถ้า Length 2 คือไม่ม่ . ก็ return เลย 
            {
                return false;
            }

            var salt = Convert.FromBase64String(parts[0]);
            var passwordHash = parts[1];

            // วิธีการเทียบ password แบบ hash function ตาม MicroSoft เลย
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return passwordHash == hashed;
        }
        private string BuildToken(User user) // fuction สร้าง token
        {
            // key is case-sensitive
            var claims = new[] {   ///// claims = ข้อมูลที่บรรจุใน token
                new Claim(JwtRegisteredClaimNames.Sub, "For Testing"), // subject : ของ Jwt
                new Claim("id", user.Id.ToString()),
                new Claim("username", user.Username),
                new Claim(ClaimTypes.Role, user.Position),
                new Claim("email", "peawit.dongbantao@th.panasonic.com") // เพื่มมาเอง 
          
                // new เพิ่มก็ได้ ถ้าต้องการ เก็บข้อมูลใน token เพิ่มเติม ระวังเรื่อง standard ด้วย
                // ถ้าเก็บไว้ใน token เราจะสามารถนำ fronend ดึงมาใช้ได้โดยไม่ต้อง ติดต่อ กฟะฟิฟหำ
            };

            var expires = DateTime.Now.AddDays(Convert.ToDouble(jwtSettings.Expire)); // เวลาในการคงอยู่ของ token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); // การเข้ารหัส อยู่ตรงนี้ จะเปลี่ยนอะไรก็ เปลี่ยน

            var token = new JwtSecurityToken(
                issuer: jwtSettings.Issuer,
                audience: jwtSettings.Audience,
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        private string CreatePasswordHash(string password)
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
            return $"{Convert.ToBase64String(salt)}.{hashed}";
        }
    }
}