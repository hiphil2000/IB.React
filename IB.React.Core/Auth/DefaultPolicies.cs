using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace IB.React.Core.Auth
{
	/// <summary>
	/// 기본 인증 정책 클래스입니다.
	/// </summary>
	public class DefaultPolicies
	{
		public const string Admin = "Admin";
		public const string User = "User";

		public static AuthorizationPolicy AdminPolicy()
		{
			return new AuthorizationPolicyBuilder()
				.RequireAuthenticatedUser()
				.RequireClaim(ClaimTypes.Role, Admin)
				.Build();
		}

		public static AuthorizationPolicy UserPolicy()
		{
			return new AuthorizationPolicyBuilder()
				.RequireAuthenticatedUser()
				.RequireClaim(ClaimTypes.Role, User)
				.Build();
		}

		/// <summary>
		/// 기본 정책을 설정합니다.
		/// </summary>
		/// <param name="configure"></param>
		public static void ConfigureDefaults(AuthorizationOptions configure)
		{
			configure.AddPolicy(Admin, AdminPolicy());
			configure.AddPolicy(User, UserPolicy());
		}
	}
}