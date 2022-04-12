using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Age.Helper
{
    public class JwtHelpers
    {
        private readonly IConfiguration IConf;
        public JwtHelpers(IConfiguration IConfiguration)
        {
            this.IConf = IConfiguration;
        }
        public string GenerateToken_3min(string issuer, string signKey)
        {
            // List<Claim> list = new List<Claim> ();
            // list.Add (new Claim ("sub", UserId));
            // list.Add (new Claim ("jti", Guid.NewGuid ().ToString ()));

            var claims = new[] {
                new Claim (ClaimTypes.Name, "Age"),
                    new Claim (ClaimTypes.Sid, "6EC340E5-6D2B-4E14-A767-18ACB6CC9726"),
                    new Claim("DateOfJoing", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),    
            };

            ClaimsIdentity subject = new ClaimsIdentity(claims);
            // SymmetricSecurityKey key = new SymmetricSecurityKey (Encoding.UTF8.GetBytes (signKey));
            // SigningCredentials signingCredentials = new SigningCredentials (key, "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signKey));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = issuer,
                Subject = subject,
                Expires = DateTime.Now.AddMinutes(5),
                SigningCredentials = signingCredentials
            };
            // JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler ();
            // SecurityToken token = jwtSecurityTokenHandler.CreateToken (tokenDescriptor);
            // return jwtSecurityTokenHandler.WriteToken (token);
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var serializeToken = tokenHandler.WriteToken(securityToken);
            return serializeToken;
        }

    }
}
