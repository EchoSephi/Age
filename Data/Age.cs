using System.ComponentModel.DataAnnotations;
using Age.Helper;
using Age.Models;
using Microsoft.EntityFrameworkCore;

namespace Age.Data
{
    public class AgeGetNumber
    {
        [Key]
        public Guid Guid { get; set; }
        public string 姓名 { get; set; }
        public string 身份證號 { get; set; }
        public string 電話 { get; set; }
        public string 手機 { get; set; }
        public string EMAIL { get; set; }
        public int 取號碼 { get; set; }
        public DateTime 取號日期 { get; set; }
        public DateTime UpdateTime { get; set; }
        public int deleted { get; set; }
    }

    public class AgeYearSet
    {
        [Key]
        public Guid Guid { get; set; }
        public int 執行年度 { get; set; }
        public DateTime 開放日期 { get; set; }
        public DateTime 截止日期 { get; set; }
        public int 起始號 { get; set; }
        public int 開放人數 { get; set; }
        public DateTime UpdateTime { get; set; }
        public int deleted { get; set; }
    }

    public class AgeYearDaySet
    {
        [Key]
        public Guid Guid { get; set; }
        public Guid AgeYearSetGuid { get; set; }
        public int Sort { get; set; }
        public int 人數 { get; set; }
        public string 報到時間 { get; set; }
        public string 補登記時間 { get; set; }
    }

    public class dtoReturn1
    {
        public string 姓名 { get; set; }
        public int 取號碼 { get; set; }
        public string 報到時間 { get; set; }
        public string 補登記時間 { get; set; }
        public string msg { get; set; }
    }

    public interface IAgeServices
    {
        Task<int> 檢查身分證是否重複(string 身份證號);
        Task<dtoReturn1> 取號(dtoInfo dto);
        Task<int> 已預約人數(int year = 0);
        Task<AgeYearSet> 取得年度設定(int year = 0);
    }

    public class AgeServices : IAgeServices
    {

        private IConfiguration IConf { get; }
        private string _dapperconn { get; set; }
        public AgeServices(IConfiguration IConfiguration, AgeContext TgContext)
        {
            this.IConf = IConfiguration;
            if (AgeContext.ConnS == null || AgeContext.ConnS == "")
            {
                this._dapperconn = UStore.GetUStore(this.IConf["ConnectionStrings:Root"], "Root");
            }
            else
            {
                this._dapperconn = AgeContext.ConnS;
            }
        }

        // 檢查身分證+年份是否重複
        public async Task<int> 檢查身分證是否重複(string 身份證號)
        {
            var _now = DateTime.Now.Year;
            using (var context = new AgeContext())
            {
                var q = await (from p in context.AgeGetNumber
                               where p.身份證號 == 身份證號
                               && p.取號日期.Year == _now
                               && p.deleted == 0
                               select p).FirstOrDefaultAsync();
                if (q == null)
                {
                    return 0;
                }
                return q.取號碼;
            }
        }

        // 已預約人數
        public async Task<int> 已預約人數(int year = 0)
        {
            var _now = DateTime.Now;
            if (year == 0)
            {
                year = _now.Year;
            }
            using (var context = new AgeContext())
            {
                var q = await (from p in context.AgeGetNumber
                               where p.取號日期.Year == year
                               && p.deleted == 0
                               // orderby p.取號碼 descending
                               select p).ToListAsync();
                if (q.FirstOrDefault() == null)
                {
                    return 0;
                }
                return q.Count();
            }

        }

        // 取得年度設定
        public async Task<AgeYearSet> 取得年度設定(int year = 0)
        {
            var _now = DateTime.Now;
            if (year == 0)
            {
                year = _now.Year;
            }
            using (var context = new AgeContext())
            {
                var qY = await (from p in context.AgeYearSet
                                where p.執行年度 == year && p.deleted == 0
                                select p).FirstOrDefaultAsync();
                return qY;
            }
        }

        // 取號
        public async Task<dtoReturn1> 取號(dtoInfo dto)
        {
            var _now = DateTime.Now;
            var dtoR = new dtoReturn1();
            using (var context = new AgeContext())
            {
                var qY = await (from p in context.AgeYearSet
                                where p.執行年度 == _now.Year && p.deleted == 0
                                select p).FirstOrDefaultAsync();

                var q1 = await (from p in context.AgeGetNumber
                                where p.取號日期.Year == _now.Year
                                && p.deleted == 0
                                orderby p.取號碼 descending
                                select p).ToListAsync();

                var 取號碼 = 0;
                if (q1.FirstOrDefault() == null)
                {
                    if (qY == null)
                    {
                        取號碼 = 1;
                    }
                    else
                    {
                        取號碼 = qY.起始號;
                    }
                }
                else
                {
                    if (q1.Count() + 1 > qY.開放人數)
                    {
                        // 已額滿
                        dtoR.取號碼 = 0;
                        dtoR.msg = "本年度開放人數已滿";
                        return dtoR;
                    }
                    取號碼 = q1.FirstOrDefault().取號碼 + 1;
                }

                var q = new AgeGetNumber();
                q.Guid = Guid.NewGuid();
                q.姓名 = dto.txtName;
                q.身份證號 = dto.txtIR_ID;
                q.電話 = NullToEmpty(dto.txtPD_Tel);
                q.手機 = NullToEmpty(dto.txtPD_Cellphone);
                q.EMAIL = NullToEmpty(dto.txtEmail);
                q.取號碼 = 取號碼;
                q.取號日期 = _now;
                q.UpdateTime = _now;
                q.deleted = 0;

                await context.AgeGetNumber.AddAsync(q);
                await context.SaveChangesAsync();

                var q2 = await 取得報到時間(q.取號碼);
                dtoR.msg = "";
                dtoR.姓名 = q.姓名;
                dtoR.取號碼 = q.取號碼;
                dtoR.報到時間 = q2.報到時間;
                dtoR.補登記時間 = q2.補登記時間;
                return dtoR;
            }
        }

        public async Task<AgeYearDaySet> 取得報到時間(int 取號碼)
        {
            var _now = DateTime.Now;
            using (var context = new AgeContext())
            {
                var q = await (from p in context.AgeYearSet
                               where p.執行年度 == _now.Year && p.deleted == 0
                               select p).FirstOrDefaultAsync();

                var num = 取號碼 - q.起始號 + 1;

                var q2 = await (from p in context.AgeYearDaySet
                                where p.AgeYearSetGuid == q.Guid
                                orderby p.Sort ascending
                                select p).ToListAsync();
                foreach (var p in q2)
                {
                    if (num <= p.人數)
                    {
                        return p;
                    }
                }
                return null;
            }
        }

        private string NullToEmpty(string str)
        {
            if (str == null)
            {
                return string.Empty;
            }
            else
            {
                return str;
            }
        }

    }

}