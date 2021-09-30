using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using UrlService.Models;

namespace UrlService.Infrastructure
{

    public interface ITokenFactory
    {
        TokenResult Generate(User user);
    }


    public class JwtTokenFactory : ITokenFactory
    {
        private IConfiguration _configuration;

        public JwtTokenFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public TokenResult Generate(User user)
        {
            var claims = new[]
            {
                new Claim("UserName", user.UserName),
                new Claim("Id", user.Id),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AuthSettings:Key"]));

            var expireDate = DateTime.Now.AddDays(1);
            var token = new JwtSecurityToken(
                issuer: _configuration["AuthSettings:Issuer"],
                audience: _configuration["AuthSettings:Audience"],
                claims: claims,
                expires: expireDate,
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            string tokenAsString = new JwtSecurityTokenHandler().WriteToken(token);
            return new TokenResult(tokenAsString, expireDate);
        }

    }

    public interface ITokenResult
    {

    }

    public class TokenResult : ITokenResult
    {
        public TokenResult(string token, DateTime expireDate)
        {
            this.Token = token;
            this.ExpireDate = expireDate;

        }
        public string Token { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}