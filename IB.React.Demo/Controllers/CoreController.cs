using System;
using System.Collections.Generic;
using System.Linq;
using IB.React.Core.Auth;
using IB.React.Core.Database.Services;
using IB.React.Core.Model;
using IB.React.Core.Model.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IB.React.Demo.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class CoreController : Controller
	{
		private ICodeService codeService;
		
		public CoreController(ICodeService codeService)
		{
			this.codeService = codeService;
		}

		/// <summary>
		/// 공통 코드를 조회합니다.
		/// </summary>
		/// <param name="groupId">공통 코드 그룹 ID</param>
		/// <returns></returns>
		[HttpGet]
		[Authorize(Roles = "User,Admin")]
		[Route("GetCommonCodes")]
		public IActionResult GetCommonCodes(string groupId)
		{
			try
			{
				return new JsonResult(new CommonResponse<List<CodeModel>>()
				{
					Success = true,
					Data = codeService.GetCodes(groupId).ToList(),
				});
			}
			catch (Exception e)
			{
				return new JsonResult(new CommonResponse<List<CodeModel>>()
				{
					Success = false,
					Data = null,
					Message = e.Message
				});
			}
		}
	}
}