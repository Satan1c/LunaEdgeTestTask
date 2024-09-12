using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace Api.Extensions;

public static class JwtGenerator
{
	private static string[]             s_validAudiences;
	private static string               s_validIssuer;
	private static byte[]               s_keyBytes;
	private static SymmetricSecurityKey s_issuerSigningKey;

	public static void Initialize(WebApplicationBuilder builder)
	{
		s_validAudiences = builder.Configuration.GetSection("Authentication:Schemes:JwtBearer:ValidAudiences").Get<string[]>()!;
		s_validIssuer    = builder.Configuration["Authentication:Schemes:JwtBearer:ValidIssuer"]!;

		s_keyBytes         = Convert.FromBase64String(builder.Configuration["Authentication:Schemes:JwtBearer:Key"]!);
		s_issuerSigningKey = new SymmetricSecurityKey(s_keyBytes);
	}

	public static IServiceCollection AddAuth(this IServiceCollection serviceCollection, WebApplicationBuilder builder)
	{
		Initialize(builder);

		serviceCollection.AddAntiforgery()
						 .AddCors()
						 .AddAuthorization(
										   options =>
										   {
											   options.FallbackPolicy = new AuthorizationPolicyBuilder()
																		.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
																		.RequireAuthenticatedUser()
																		.Build();
										   }
										  )
						 .AddAuthentication(
											options =>
											{
												options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
												options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
												options.DefaultScheme             = JwtBearerDefaults.AuthenticationScheme;
											}
										   )
						 .AddJwtBearer(
									   JwtBearerDefaults.AuthenticationScheme,
									   options => { options.TokenValidationParameters = BearerValidationParameters(); }
									  );

		return serviceCollection;
	}

	private static TokenValidationParameters BearerValidationParameters()
		=> new()
		{
			ValidateIssuer           = true,
			ValidateIssuerSigningKey = true,
			ValidateAudience         = true,
			ValidateLifetime         = true,
			ClockSkew                = TimeSpan.Zero,
			ValidAudiences           = s_validAudiences,
			ValidIssuer              = s_validIssuer,
			RequireSignedTokens      = true,
			IssuerSigningKey         = s_issuerSigningKey
		};

	public static string HashPassword(string password)
		=> Encoding.UTF8.GetString(HMACSHA256.HashData(s_keyBytes, Encoding.UTF8.GetBytes(password)));

	public static string GenerateToken(string userId)
	{
		//JwtRegisteredClaimNames.
		var tokenHandler = new JwtSecurityTokenHandler();

		var tokenDescriptor = new SecurityTokenDescriptor
		{
			Subject            = new ClaimsIdentity([new Claim(JwtRegisteredClaimNames.Jti, userId)]),
			IssuedAt           = DateTime.UtcNow,
			Expires            = DateTime.UtcNow.AddDays(14),
			SigningCredentials = new SigningCredentials(s_issuerSigningKey, SecurityAlgorithms.HmacSha256),
			Audience           = s_validAudiences.First(),
			Issuer             = s_validIssuer
		};

		var token = tokenHandler.CreateToken(tokenDescriptor);
		return tokenHandler.WriteToken(token);
	}
}
