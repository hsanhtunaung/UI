using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using UI.Models;

namespace I.Controllers
{
    //[Authorize]
    public class eVoucherController : Controller
    {
        #region Declaration
        string fileName;
        string apiURL;
        private IConfiguration _configuration;

        public eVoucherController(IConfiguration configuration)
        {
            _configuration = configuration;
            apiURL = _configuration.GetValue<string>("APIUrl:apiurl");
        }

        #endregion

        // GET: eVoucherController
        public ActionResult Index()
        {
            var jwt = Request.Cookies["jwtCookie"];

            List<TblEVoucher> lst = new List<TblEVoucher>();
            try
            {
                var context = new ProgramTestContext();
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

                HttpClient client = new HttpClient(clientHandler);
                //for Jwt Header
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
                //
                client.BaseAddress = new Uri(apiURL);
                System.Net.ServicePointManager.ServerCertificateValidationCallback = (senderX, certificate, chain, sslPolicyErrors) => { return true; };
                HttpResponseMessage Res = client.GetAsync(apiURL + string.Format("API/eVoucher/Get")).Result;
                if (Res.IsSuccessStatusCode)
                {
                    var pResponse = Res.Content.ReadAsStringAsync().Result;
                    lst = JsonConvert.DeserializeObject<List<TblEVoucher>>(pResponse);
                }              
                return View(lst);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message.ToString();
                return View(lst);
            }
        }

        // GET: eVoucherController/Details/5
        public ActionResult Details(int id)
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            TblEVoucher eVoucher = new TblEVoucher();
            HttpClient client = new HttpClient(clientHandler);
            client.BaseAddress = new Uri(apiURL);
            System.Net.ServicePointManager.ServerCertificateValidationCallback = (senderX, certificate, chain, sslPolicyErrors) => { return true; };
            HttpResponseMessage Res = client.GetAsync(apiURL + string.Format("API/eVoucher/GetByID" + "?id=" + id)).Result;
            if (Res.IsSuccessStatusCode)
            {
                var pResponse = Res.Content.ReadAsStringAsync().Result;
                eVoucher = JsonConvert.DeserializeObject<TblEVoucher>(pResponse);
            }
            return View(eVoucher);
        }

        // GET: eVoucherController/Create
        public ActionResult Create()
        {
            PopulatePayment();
            PopulateBuyType();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection, IFormFile file, TblEVoucher eVoucher)
        {
            try
            {
                TblEVoucher model = new TblEVoucher();
                bool uploadSuccess = UploadFile(file);
                if (uploadSuccess)
                {
                    eVoucher.Image = fileName;
                    string PaymentMethodID = collection["DDLPaymentMethod"];
                    string buytypeid = collection["DDLTypes"];
                     eVoucher.PaymentMethodId =Convert.ToInt32(PaymentMethodID);

                    if (eVoucher.PaymentMethodId == 1)//10%discount
                    {
                        eVoucher.Amount = (eVoucher.Amount - (eVoucher.Amount * 10 / 100)) * eVoucher.Quantity;
                    }
                    else
                    {
                        eVoucher.Amount = eVoucher.Amount * eVoucher.Quantity;
                    }
                    eVoucher.BuyTypeId = Convert.ToInt32(buytypeid);
                    var context = new ProgramTestContext();                   
                    HttpClientHandler clientHandler = new HttpClientHandler();
                    clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

                    HttpClient client = new HttpClient(clientHandler);                   
                    var myContent = JsonConvert.SerializeObject(eVoucher);
                    var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
                    var byteContent = new ByteArrayContent(buffer);
                    byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var Res = client.PostAsync(apiURL + string.Format("API/eVoucher/post"), byteContent).Result;
                    if (Res.IsSuccessStatusCode)
                    {
                        var pResponse = Res.Content.ReadAsStringAsync().Result;
                        return RedirectToAction("Index");
                    }
                    else
                    { 
                    ViewBag.Message = "Cannot Insert data";
                    PopulatePayment();
                    PopulateBuyType();
                    }

                }
                else
                {
                    ViewBag.Message = "Uplodad Image";
                    PopulatePayment();
                    PopulateBuyType();
                    return View();
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Failed to create eVoucher";
                PopulatePayment();
                PopulateBuyType();
                return View();
            }
        }

        // GET: eVoucherController/Edit/5
        public ActionResult Edit(int id)
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            TblEVoucher eVoucher = new TblEVoucher();
            HttpClient client = new HttpClient(clientHandler);
            client.BaseAddress = new Uri(apiURL);
            System.Net.ServicePointManager.ServerCertificateValidationCallback = (senderX, certificate, chain, sslPolicyErrors) => { return true; };
            HttpResponseMessage Res = client.GetAsync(apiURL + string.Format("API/eVoucher/GetByID" + "?id=" + id)).Result;
            if (Res.IsSuccessStatusCode)
            {
                var pResponse = Res.Content.ReadAsStringAsync().Result;
                eVoucher = JsonConvert.DeserializeObject<TblEVoucher>(pResponse);
            }
            if (eVoucher.InActive == false)
            {
                ViewBag.InActive = 0;
            }
            else
                ViewBag.InActive = 1;
           
            var contexts = new ProgramTestContext();
            var lst = contexts.TblPaymentMethods.ToList();            

            SelectList objmodeldata = new SelectList(lst, "Id", "MethodName",eVoucher.PaymentMethodId);
            ViewBag.lstPaymetMethod = objmodeldata;

            var lstType = contexts.TblBuyTypes.ToList();
            SelectList objmodeldatas = new SelectList(lstType, "Id", "TypeName",eVoucher.BuyTypeId);
            ViewBag.lstBuyType = objmodeldatas;

            return View(eVoucher);
        }

        // POST: eVoucherController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection, TblEVoucher eVoucher)
        {
            try
            {
                string PaymentMethodID = collection["DDLPaymentMethod"];
                string buytypeid = collection["DDLTypes"];
                eVoucher.PaymentMethodId = Convert.ToInt32(PaymentMethodID);

                if (eVoucher.PaymentMethodId == 1)//10%discount
                {
                    eVoucher.Amount = (eVoucher.Amount - (eVoucher.Amount * 10 / 100)) * eVoucher.Quantity;
                }
                else
                {
                    eVoucher.Amount = eVoucher.Amount * eVoucher.Quantity;
                }
                eVoucher.BuyTypeId = Convert.ToInt32(buytypeid);
                bool inActive;
                string InActiveValue = collection["InActive"];
                if (InActiveValue == "on")
                {
                    inActive = true;
                    eVoucher.InActive = inActive;
                }
                else
                {
                    inActive = false;
                    eVoucher.InActive = inActive;
                }

                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

                HttpClient client = new HttpClient(clientHandler);
                var myContent = JsonConvert.SerializeObject(eVoucher);
                var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var Res = client.PutAsync(apiURL + string.Format("API/eVoucher/put"), byteContent).Result;
                if (Res.IsSuccessStatusCode)
                {
                    var pResponse = Res.Content.ReadAsStringAsync().Result;
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Message = "Cannot Update data";
                    PopulatePayment();
                    PopulateBuyType();
                    return View();
                }
            }
            catch(Exception ex)
            {
                return View();
            }
        }

        // GET: eVoucherController/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

                HttpClient client = new HttpClient(clientHandler);
                client.BaseAddress = new Uri(apiURL);
                System.Net.ServicePointManager.ServerCertificateValidationCallback = (senderX, certificate, chain, sslPolicyErrors) => { return true; };

                HttpResponseMessage Res = client.DeleteAsync(apiURL + string.Format("API/eVoucher/Delete" + "?id=" + Convert.ToInt32(id))).Result;

                if (Res.IsSuccessStatusCode)
                {

                    return RedirectToAction("Index");

                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {

                return RedirectToAction("Index");
            }
        }

        // POST: eVoucherController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public ActionResult LogIn()
        {
            return View();
        }
        [HttpPost]
        public ActionResult LogIn(UserInfo _userData)
        {
            if (_userData != null && _userData.Email != null && _userData.Password != null)
            {
                var context = new ProgramTestContext();
                var user = context.UserInfo.Where(a => a.Email == _userData.Email && a.Password == _userData.Password).FirstOrDefault();
                if (user != null)
                {
                    string token = GenerateJSONWebToken();
                    SetJWTCookie(token);
                   
                }

                //var user = await GetUser(_userData.Email, _userData.Password);
            }
            return RedirectToAction("Index");
        }


        #region Populate Data
        private void PopulatePayment()
        {
            List<TblPaymentMethod> lst = new List<TblPaymentMethod>();
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

            // Pass the handler to httpclient(from you are calling api)
            HttpClient client = new HttpClient(clientHandler);
            //Passing service base url
            client.BaseAddress = new Uri(apiURL);
            System.Net.ServicePointManager.ServerCertificateValidationCallback = (senderX, certificate, chain, sslPolicyErrors) => { return true; };
            //Sending request to find web api REST service resource GetAllEmployees using HttpClient
            HttpResponseMessage Res = client.GetAsync(apiURL + string.Format("API/eVoucher/GetPaymentMethod")).Result;
            //Checking the response is successful or not which is sent using HttpClient
            if (Res.IsSuccessStatusCode)
            {

                var pResponse = Res.Content.ReadAsStringAsync().Result;
                lst = JsonConvert.DeserializeObject<List<TblPaymentMethod>>(pResponse);
            }

            SelectList objmodeldata = new SelectList(lst, "Id", "MethodName");
            ViewBag.lstPaymetMethod = objmodeldata;
        }

        private void PopulateBuyType()
        {
            List<TblBuyType> lst = new List<TblBuyType>();
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

            HttpClient client = new HttpClient(clientHandler);
            client.BaseAddress = new Uri(apiURL);
            System.Net.ServicePointManager.ServerCertificateValidationCallback = (senderX, certificate, chain, sslPolicyErrors) => { return true; };
            HttpResponseMessage Res = client.GetAsync(apiURL + string.Format("API/eVoucher/GetBuyType")).Result;
            if (Res.IsSuccessStatusCode)
            {
                var pResponse = Res.Content.ReadAsStringAsync().Result;
                lst = JsonConvert.DeserializeObject<List<TblBuyType>>(pResponse);
            }
            SelectList objmodeldata = new SelectList(lst, "Id", "TypeName");
            ViewBag.lstBuyType = objmodeldata;
        }

        #endregion


        #region Method

        private string GenerateJSONWebToken()
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("MynameisJamesBond007"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "hsan",
                audience: "hsan",
                expires: DateTime.Now.AddHours(3),
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private void SetJWTCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddHours(3),
            };
            Response.Cookies.Append("jwtCookie", token, cookieOptions);
        }
        public bool UploadFile(IFormFile file)
        {
            try
            {
                bool isCopied = false;
                //1 check if the file length is greater than 0 bytes 
                if (file != null && file.Length > 0)
                {
                    fileName = file.FileName;
                    //2 Get the extension of the file
                    string extension = Path.GetExtension(fileName);
                    //3 check the file extension as png
                    if (extension == ".png" || extension == ".jpg")
                    {
                        //4 set the path where file will be copied
                        string filePath = Path.GetFullPath(
                            Path.Combine(Directory.GetCurrentDirectory(),
                                                        "UploadedFiles"));
                        //5 copy the file to the path
                        using (var fileStream = new FileStream(
                            Path.Combine(filePath, fileName),
                                           FileMode.Create))
                        {
                            file.CopyToAsync(fileStream);
                            isCopied = true;
                        }
                    }
                    else
                    {
                        throw new Exception("File must be either .png or .JPG");
                    }
                }
                else
                {
                    return false;
                }
                return isCopied;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #endregion
    }
}
