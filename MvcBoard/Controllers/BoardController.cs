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
using System.IO;
using System.Runtime.CompilerServices;

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
                string identityName = User.Identity.Name;
                var username = db.QuerySingleOrDefault<string>("SELECT UserName " +
                                                               "From userdb WHERE UserId = @identityName", new { identityName = identityName });
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
                            boards = boardData.Where(x => x.board_name.Contains(keyword)).ToList();
                            totalCount = boards.Count();
                            break;

                    }
                }
                boards = boards.Skip((pageNum - 1) * maxListCount)
                               .Take(maxListCount).ToList();


                ViewBag.Page = pageNum;
                ViewBag.TotalCount = totalCount;
                ViewBag.MaxListCount = maxListCount;
                ViewBag.SearchKind = searchKind;
                ViewBag.Keyword = keyword;
                Session["UserName"] = username;
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
                    string fileQuery = @"
                    SELECT FileTitle 
                    FROM mvcboard 
                    WHERE board_postNo = @id";

                    string fullQuery = @"
                    DECLARE @board_count INT 
                    SELECT @board_count = (board_readCount + 1) FROM mvcboard WHERE board_postNo = @id

                    UPDATE mvcboard SET board_readCount = @board_count WHERE board_postNo = @id

                    SELECT
                        board_postNo, board_subject, board_content, board_name, board_writeTime, board_readCount, FileTitle, FileContent
                    FROM
                        mvcboard
                    WHERE
                        board_postNo = @id
                ";

                    string boardQuery = @"
                    DECLARE @board_count INT 
                    SELECT @board_count = (board_readCount + 1) FROM mvcboard WHERE board_postNo = @id

                    UPDATE mvcboard SET board_readCount = @board_count WHERE board_postNo = @id

                    SELECT
                        board_postNo, board_subject, board_content, board_name, board_writeTime, board_readCount
                    FROM
                        mvcboard
                    WHERE
                        board_postNo = @id
                ";


                    if (db.Query(fileQuery, new { @id = id }) != null)
                    {
                        _board = db.Query<Board>(fullQuery, new { @id = id }).SingleOrDefault();

                        return View(_board);
                    }

                    else
                    {
                        _board = db.Query<Board>(boardQuery, new { @id = id }).SingleOrDefault();

                        return View(_board);
                    }
                }
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
        public ActionResult Create(Board _board)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(DapperLib.Config.DBConnStrTest()))
                {
                    if (_board.upfiles != null)
                    {
                        string FileTitle = Path.GetFileNameWithoutExtension(_board.upfiles.FileName);

                        string FileExtension = Path.GetExtension(_board.upfiles.FileName);

                        FileTitle = DateTime.Now.ToString("yyyyMMdd")+ "-" +FileTitle.Trim() + FileExtension;

                        string UploadPath = ConfigurationManager.AppSettings["FilePath"].ToString();

                        _board.FileContent = UploadPath + FileTitle;

                        _board.upfiles.SaveAs(_board.FileContent);

                        string sqlQuery = "Insert Into mvcboard (board_name, board_subject, board_content, " +
                                                                "board_writeTime, FileTitle, FileContent, UserId) " +
                                                                "Values(@board_name, @board_subject, @board_content, GETDATE(), @FileTitle, @FileContent, @UserId)";
                        var rowsAffected = db.Execute(sqlQuery, new
                        {
                            board_name = Session["UserName"],
                            board_subject = _board.board_subject,
                            board_content = _board.board_content,
                            FileTitle = FileTitle,
                            FileContent = _board.FileContent,
                            UserId = User.Identity.Name
                        });


                    }
                    else
                    {
                        string sqlQuery = "Insert Into mvcboard (board_name, board_subject, board_content, " +
                                                                "board_writeTime, UserId) " +
                                                                "Values(@board_name, @board_subject, @board_content, GETDATE(), @UserId)";
                        var rowsAffected = db.Execute(sqlQuery, new
                        {
                            board_name = Session["UserName"],
                            board_subject = _board.board_subject,
                            board_content = _board.board_content,
                            UserId = User.Identity.Name
                        });
                    }

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
        [ValidateInput(false)]
        public ActionResult Edit(Board _board)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(DapperLib.Config.DBConnStrTest()))
                {
                    if (_board.upfiles != null)
                    {
                        Stream str = _board.upfiles.InputStream;
                        BinaryReader Br = new BinaryReader(str);
                        Byte[] FileDet = Br.ReadBytes((Int32)str.Length);
                        string sqlQuery = "update mvcboard set board_subject='" + _board.board_subject +
                                          "',board_content='" + _board.board_content +
                                          "',FileTitle='" + _board.upfiles.FileName +
                                          "',FileContent='" + FileDet +
                                          "' where board_postNo=" + _board.board_postNo;


                        db.Execute(sqlQuery);

                    }

                    else
                    {
                        string sqlQuery = "update mvcboard set board_subject='" + _board.board_subject +
                                          "',board_content='" + _board.board_content +
                                          "' where board_postNo=" + _board.board_postNo;
                        int rowsAffected = db.Execute(sqlQuery);
                    }
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


        [HttpGet]
        public FileResult DownLoadFile(int id)
        {
            using (IDbConnection db = new SqlConnection(DapperLib.Config.DBConnStrTest()))
            {
                string sqlQuery = "SELECT board_postNo, FileTitle, FileContent From mvcboard WHERE board_postNo = @id";
                List<Board> ObjFiles = db.Query<Board>(sqlQuery, new { @id = id }).ToList();

                var FileById = (from FC in ObjFiles
                                where FC.board_postNo.Equals(id)
                                select new { FC.FileTitle, FC.FileContent }).ToList().FirstOrDefault();

                return File(FileById.FileContent, "application/pdf", FileById.FileTitle);
            }

        }




        #region Database related operations  
        private void SaveFileDetails(FileDetailsModel objDet)
        {
            using (IDbConnection db = new SqlConnection(DapperLib.Config.DBConnStrTest()))
            {
                DynamicParameters Parm = new DynamicParameters();
                Parm.Add("@FileName", objDet.FileName);
                Parm.Add("@FileContent", objDet.FileContent);
                db.Execute("AddFile", Parm, commandType: System.Data.CommandType.StoredProcedure);
            }
        }
        #endregion
    }
}

// hello