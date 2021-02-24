using System;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace APIservice.Installers
{
    public class JWTInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = new JwtSettings();
            configuration.Bind(nameof(jwtSettings), jwtSettings); //เอา JWT setting มา magging กับ class
            services.AddSingleton(jwtSettings); // Singleton คือ การกระกาศ object คนละตัว แต่เป็นตัวเดียวกัน เพียง 1 object

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
             {
                 options.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuer = true,
                     ValidIssuer = jwtSettings.Issuer,
                     ValidateAudience = true,
                     ValidAudience = jwtSettings.Audience,
                     ValidateLifetime = true,
                     ClockSkew = TimeSpan.Zero, // disable delay when token is expire
                     ValidateIssuerSigningKey = true,
                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)) // อาจเปลี่ยน encode ก็ได้แต่อาจจะช้า
                 };
             });
        }
    }

    public class JwtSettings
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Expire { get; set; }
    }
}