using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MvcBoard.Models;
using Dapper;
using System.Web.Security;
using System.Data;

namespace MvcBoard.Controllers
{
    public class AccountController : Controller
    {
        /// <summary>
        /// 로그인
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Login model)
        {
            if (ModelState.IsValid)
            {
                using (IDbConnection db = new SqlConnection(DapperLib.Config.DBConnStrTest()))
                {
                    string sqlQuery = "SELECT * FROM userdb";
                    var user = db.Query(sqlQuery).FirstOrDefault(u => u.UserId.Equals(model.UserId) &&
                                                                      u.UserPassword.Equals(model.UserPassword));
                    if (user != null)
                    {
                        FormsAuthentication.SetAuthCookie(model.UserId, false);
                        // 로그인 성공시
                        return RedirectToAction("Index", "Board");
                    }
                    // 로그인 실패시
                    ModelState.AddModelError(string.Empty, "ID 혹은 비밀번호가 옳바르지 않습니다");
                }
            }
            return View(model);
        }

        /// <summary>
        /// 회원 가입
        /// </summary>
        /// <returns></returns>
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(User model)
        {
            if (ModelState.IsValid)
            {
                using (IDbConnection db = new SqlConnection(DapperLib.Config.DBConnStrTest()))
                {
                    string sqlQuery = "Insert Into userdb (UserId, UserPassword, UserName, RegisterDate) Values(@UserId, @UserPassword, @UserName, GETDATE())";
                    var rowsAffected = db.Execute(sqlQuery, new { UserId = model.UserId, UserPassword = model.UserPassword, UserName = model.UserName });
                }
                return RedirectToAction("Index", "Board");
            }
            return View(model);
        }
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        // GET: User/UserEdit  
        public ActionResult UserEdit()
        {
            User _user = new User();
            using (IDbConnection db = new SqlConnection(DapperLib.Config.DBConnStrTest()))
            {
                string UserId = User.Identity.Name;
                _user = db.Query<User>("Select UserName, UserPassword From userdb " +
                                        "WHERE UserId = @UserId", new { UserId = UserId }).SingleOrDefault();
            }
            return View(_user);


        }

        // POST: User/UserEdit
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult UserEdit(User _user)
        {
            using (IDbConnection db = new SqlConnection(DapperLib.Config.DBConnStrTest()))
            {
                string UserId = User.Identity.Name;
                var username = db.QuerySingleOrDefault<string>("SELECT UserName " +
                                                               "From userdb WHERE UserId = @UserId", new { UserId = UserId });

                if (username != _user.UserName)
                {
                    string sqlQuery = "update userdb set UserName='" + _user.UserName +
                                      "',UserPassword='" + _user.UserPassword +
                                      "' where UserId= @UserId";
                    
                    string boardQuery = "update mvcboard set board_name='" + _user.UserName +
                                      "' where UserId= @UserId";

                    db.Execute(sqlQuery, new { UserId = UserId });
                    db.Execute(boardQuery, new { UserId = UserId });
                }

                else
                {
                    string sqlQuery = "update userdb set UserName='" + _user.UserName +
                                      "',UserPassword='" + _user.UserPassword +
                                      "' where UserId= @UserId";
                    db.Execute(sqlQuery, new { UserId = UserId });
                }

            }

            return RedirectToAction("Index", "Board");

        }

    }
}
