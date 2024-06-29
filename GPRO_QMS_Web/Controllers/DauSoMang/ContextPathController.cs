using QMS_System.Data;
using QMS_System.Data.BLL;
using QMS_Website.App_Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace QMS_Website.Controllers
{
    public class ContextPathController : ApiController
    {
        [HttpPost]
        [HttpGet]
        public async Task<HttpResponseMessage> path(string mo_id, string sender, string serviceId, string message, string @operator, string commandCode, string key, string username)
        {
            var respReturn = new HttpResponseMessage(HttpStatusCode.OK);
            string user = "bvdktv",
                pass = "bvdktv@!3$H7",
                code = user + "|" + mo_id + "|" + pass,
                md5 = GPRO.Ultilities.GlobalFunction.EncryptMD5(code);
            if (key != md5)
                respReturn.Content = new StringContent("4");
            else
            {
                string sms = "";
                var resp = BLLRegisterOnline.Instance.Register(AppGlobal.Connectionstring, message, sender,8);
                if (resp.IsSuccess)
                {
                    //respReturn.Content = new StringContent("0-Thanh cong");
                    sms = (string)resp.Data;
                }
                else
                {
                    //respReturn.Content = new StringContent(a);
                    sms = resp.Errors.FirstOrDefault().Message;
                }
                respReturn.Content = new StringContent("0");
                //gọi lại api
                await CallMT(mo_id, sender, serviceId, sms, @operator, commandCode, key, username);
                // respReturn.Content = new StringContent(ms);
            }
            return respReturn;
        }

        public async Task<string> CallMT(string mo_id, string sender, string serviceId, string message, string _operator, string commandCode, string key, string username)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string url = String.Format("http://service.anhducmedia.vn:6688/service/ahp/mt6x88?mo_id={0}&serviceId={1}&sender={2}&receiver={3}&message={4}&requestTime={5}&operator={6}&commandCode={7}&contentType={8}&messageType={9}&messageIndex={10}&userName={11}&key={12}",
                        mo_id, serviceId, sender, sender, message, DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"), _operator, commandCode, 0, 0, 1, username, key);
                    HttpResponseMessage httpResp = await client.GetAsync(url);
                    string respBody = httpResp.Content.ReadAsStringAsync().Result;
                    return respBody;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return "";
        }

        [HttpGet]
        [HttpPost]
        public string RegisterOnline(string name, string phone, string date)
        {
            var model = new Q_RegisterOnline();
            model.CreatedDate = DateTime.Now;
            model.Name = name;
            model.PhoneNumber = phone;
            model.RegisterDate = date;
            model.ServiceId = 8;
            var resp = BLLRegisterOnline.Instance.Register(AppGlobal.Connectionstring, model);
            if (resp.IsSuccess)
            {
                return resp.Data;
            }
            else
                return resp.Errors != null && resp.Errors.Count > 0 ? resp.Errors[0].Message : "Lỗi thực thi.";

        }
    }

}