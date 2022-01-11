using System;
using System.Configuration;
using System.Data.Common;
using System.Text;
using IB.React.Core.Auth;
using IB.React.Core.Database.Services;
using IB.React.Core.Model.Settings;
using JWT.Extensions.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace IB.React.Core
{
	public static class IbHelper
	{
		public static void AddIbSettings(this IServiceCollection services, IConfiguration configuration)
		{
			// DB Provider 등록
			DbProviderFactories.RegisterFactory("System.Data.SqlClient", System.Data.SqlClient.SqlClientFactory.Instance);
			
			// Jwt설정 바인드
			services.Configure<JwtSettings>(opt => configuration.GetSection(JwtSettings.Key).Bind(opt));
			
			// 서비스 추가
			services.AddTransient<IUserService, UserService>();
			services.AddTransient<ITokenService, TokenService>();
			services.AddTransient<ICodeService, CodeService>();
			services.AddSingleton<IJwtService, JwtService>();	

			// JWT 인증 사용
			services.AddAuthentication(options =>
			{
				options.DefaultScheme = JwtHandler.SchemeName;
				options.AddScheme<JwtHandler>(JwtHandler.SchemeName, JwtHandler.SchemeName);
				
			});
			
			// 기본 인증 정책 사용
			services.AddAuthorization(DefaultPolicies.ConfigureDefaults);
		}
	}
}