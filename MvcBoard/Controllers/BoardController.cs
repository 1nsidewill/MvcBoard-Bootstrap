using MvcBoard.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Dapper;

namespace MvcBoard.Controllers
{
    [Authorize]
    public class BoardController : Controller
    {
        // GET: Board
        public ActionResult Index()
        {
            List<Board> boardList = new List<Board>();
            using (IDbConnection db = new SqlConnection(DapperLib.Config.DBConnStrTest()))
            {
                var boardData = db.Query<Board>("Select * From mvcboard " +
                                                "ORDER BY board_postNo DESC");


                int maxListCount = 10;
                int pageNum = 1;
                string keyword = Request.QueryString["keyword"] ?? string.Empty;
                string searchKind = Request.QueryString["searchKind"] ?? string.Empty; int totalCount = 0;

                if (Request.QueryString["page"] != null)
                    pageNum = Convert.ToInt32(Request.QueryString["page"]);

                var boards = new List<Board>();


                if (string.IsNullOrEmpty(keyword))
                {
                    boards = boardData.ToList();
                    totalCount = boards.Count();
                }
                else
                {
                    switch (searchKind)
                    {
                        case "subject":
                            boards = boardData.Where(x => x.board_subject.Contains(keyword)).ToList();
                            totalCount = boards.Count();
                            break;
                        case "content":
                            boards = boardData.Where(x => x.board_content.Contains(keyword)).ToList();
                            totalCount = boards.Count();
                            break;
                        case "name":
                            boards = boardData.Where(x => x.UserId.Contains(keyword)).ToList();
                            totalCount = boards.Count();
                            break;

                    }
                }
                boards = boards.Skip((pageNum - 1) * maxListCount)
                               .Take(maxListCount).ToList();

                ViewBag.Page = pageNum;
                ViewBag.TotalCount = boardData.Count();
                ViewBag.MaxListCount = maxListCount;
                ViewBag.SearchKind = searchKind;
                ViewBag.Keyword = keyword;

                return View(boards);
            }
        }





        // GET: Board/Details/5
        public ActionResult Details(int? id)
        {
            try
            {
                Board _board = new Board();
                using (IDbConnection db = new SqlConnection(DapperLib.Config.DBConnStrTest()))
                {
                    string test = @"
                    DECLARE @board_count INT 
                    SELECT @board_count = (board_readCount + 1) FROM mvcboard WHERE board_postNo = @id

                    UPDATE mvcboard SET board_readCount = @board_count WHERE board_postNo = @id

                    SELECT
                        *
                    FROM
                        mvcboard
                    WHERE
                        board_postNo = @id


                ";

                    _board = db.Query<Board>(test, new { @id = id }).SingleOrDefault();


                }

                return View(_board);
            }
            catch
            {
                return RedirectToAction("Error");
            }

        }

        // GET: Board/Create
        // GET: Board/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Board/Create
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(User _user, Board _board)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(DapperLib.Config.DBConnStrTest()))
                {
                    string sqlQuery = "Insert Into mvcboard (board_name, board_subject, board_content, board_writeTime) Values(@board_name, @board_subject, @board_content, GETDATE())";
                    var rowsAffected = db.Execute(sqlQuery, new { board_name = _user.UserName, board_subject = _board.board_subject, board_content = _board.board_content });
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Error");
            }

        }

        // GET: Board/Edit/5  
        public ActionResult Edit(int id)
        {
            try
            {
                Board _board = new Board();
                using (IDbConnection db = new SqlConnection(DapperLib.Config.DBConnStrTest()))
                {
                    _board = db.Query<Board>("Select * From mvcboard " +
                                           "WHERE board_postNo =" + id, new { id }).SingleOrDefault();
                }
                return View(_board);
            }
            catch
            {
                return RedirectToAction("Error");
            }

        }

        // POST: Board/Edit/5  
        [HttpPost]
        public ActionResult Edit(Board _board)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(DapperLib.Config.DBConnStrTest()))
                {

                    string sqlQuery = "update mvcboard set board_name='" + _board.UserId + "',board_subject='" + _board.board_subject + "',board_content='" + _board.board_content + "' where board_postNo=" + _board.board_postNo;

                    int rowsAffected = db.Execute(sqlQuery);
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Error");
            }

        }

        // GET: Board/Delete/5  
        public ActionResult Delete(int id)
        {
            Board _board = new Board();
            using (IDbConnection db = new SqlConnection(DapperLib.Config.DBConnStrTest()))
            {
                _board = db.Query<Board>("Select * From mvcboard " +
                                       "WHERE board_postNo =" + id, new { id }).SingleOrDefault();
            }
            return View(_board);
        }

        // POST: Board/Delete/5  
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            using (IDbConnection db = new SqlConnection(DapperLib.Config.DBConnStrTest()))
            {
                string sqlQuery = "Delete From mvcboard WHERE board_postNo = " + id;

                int rowsAffected = db.Execute(sqlQuery);


            }
            return RedirectToAction("Index");
        }
        public ActionResult Error()
        {
            return View();
        }
    }
}