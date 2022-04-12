using System.Diagnostics;
using Age.Data;
using Age.Helper;
using Age.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Age.Controllers;

public class HomeController : Controller
{
    private readonly IAgeServices IAge;

    public HomeController(IAgeServices _IAgeServices)
    {
        this.IAge = _IAgeServices;
    }

    public async Task<IActionResult> Index()
    {
        var msg1 = "";
        var msg2 = "";
        var qY = await this.IAge.取得年度設定();
        if (qY == null)
        {
            msg2 = "本年度尚未開放預約";
        }
        else
        {
            msg1 = $"本年度開放預約期間為：{qY.開放日期.ToString("yyyy.MM.dd HH:mm")} - {qY.截止日期.ToString("yyyy.MM.dd HH:mm")}";
            var _now = DateTime.Now;

            if (_now < qY.開放日期)
            {
                msg2 = "目前日期尚未至開放日期";
            }
            else if (_now > qY.截止日期)
            {
                msg2 = "目前日期已超過截止日期";
            }
            else
            {
                var q1 = await this.IAge.已預約人數();
                if (q1 >= qY.開放人數)
                {
                    msg2 = $"本年度預約人數已額滿";
                }
            }
        }

        ViewData["msg1"] = msg1;
        ViewData["msg2"] = msg2;
        return View();
    }

    [HttpGet]
    public IActionResult Page()
    {
        #region 安全驗證碼

        var sec = SecurityModule.CreateRecaptchaString();
        var security1 = sec[0];
        var answer1 = sec[1];
        // var answer1 = sec[2];

        // Console.WriteLine(answer1);
        #endregion
        ViewData["security1"] = security1;
        ViewData["answer1"] = answer1;
        var dto = new dtoInfo();
        return View(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Page(dtoInfo dto)
    {
        var d = dto;
        var q = await this.IAge.檢查身分證是否重複(dto.txtIR_ID);
        if (q != 0)
        {
            var msg = $"您的身份證號:{dto.txtIR_ID},已取過號碼,取號碼:{q}";
            return RedirectToAction("Privacy", "Home", new { msg = msg });
        }
        else
        {

            var q1 = await this.IAge.取號(dto);
            if (q1.msg != "")
            {
                //  網路取號已額滿
                return RedirectToAction("Privacy", "Home", new { msg = "本年度開放人數已滿" });
            }


            TempData["取號碼"] = q1.取號碼;
            TempData["姓名"] = q1.姓名;
            TempData["補登記時間"] = q1.補登記時間;
            TempData["報到時間"] = q1.報到時間;
            return RedirectToAction("Privacy", "Home", new { msg = "" });
        }
    }

    [HttpGet]
    public IActionResult Search()
    {
        var dto = new dtoInfo2();
        return View(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Search(dtoInfo2 dto)
    {

        var msg = "";
        var qY = await this.IAge.取得年度設定();
        var q1 = await this.IAge.已預約人數();
        var q = await this.IAge.檢查身分證是否重複(dto.txtBox);
        if (q != 0)
        {
            msg = $"您的身份證號:{dto.txtBox},已取過號碼,取號碼:{q}";
            return RedirectToAction("Privacy", "Home", new { msg = msg });
        }
        else
        {
            var cnt = qY.開放人數 - q1;
            msg = $"您的身份證號:{dto.txtBox},尚未取號,目前還有 {cnt} 名額數";
        }
        return RedirectToAction("Privacy", "Home", new { msg = msg });
    }

    public IActionResult Privacy(string msg = "")
    {
        var dto = new dtoReturn1();
        if (msg != "")
        {
            dto.msg = msg;
        }
        else
        {
            dto.msg = "";
            dto.取號碼 = int.Parse(TempData["取號碼"].ToString());
            dto.報到時間 = TempData["報到時間"].ToString();
            dto.姓名 = TempData["姓名"].ToString();
            dto.補登記時間 = TempData["補登記時間"].ToString();
        }

        return View(dto);
    }

}
