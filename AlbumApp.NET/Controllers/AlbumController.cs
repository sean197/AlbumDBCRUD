using AlbumApp.NET.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AlbumApp.NET.Controllers
{
    public class AlbumController : Controller
    {
        // GET: Album
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ViewAll()
        {
            return View(GetAllAlbums());
        }

        IEnumerable<Album> GetAllAlbums()
        {
            using (DBModel db = new DBModel())
            {
                return db.Albums.ToList<Album>();
            }
        }

        public ActionResult AddOrEdit(int id = 0)
        {
            Album amb = new Album();
            if(id != 0)
            {
                using (DBModel db = new DBModel())
                {
                    amb = db.Albums.Where(x => x.AlbumID == id).FirstOrDefault<Album>();
                }
            }
            return View(amb);
        }

        [HttpPost]
        public ActionResult AddOrEdit(Album alb)
        {
            try
            {
                if (alb.ImageUpload != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(alb.ImageUpload.FileName);
                    string extension = Path.GetExtension(alb.ImageUpload.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    alb.ImagePath = "~/AppFiles/Images/" + fileName;
                    alb.ImageUpload.SaveAs(Path.Combine(Server.MapPath("~/AppFiles/Images/"), fileName));
                }
                using (DBModel db = new DBModel())
                {
                    if(alb.AlbumID == 0)
                    {
                        db.Albums.Add(alb);
                        db.SaveChanges();
                    }
                    else
                    {
                        db.Entry(alb).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }

                return Json(new { success = true, html = GlobalClass.RenderRazorViewToString(this, "ViewAll", GetAllAlbums()), message = "Submitted Successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = ex.Message}, JsonRequestBehavior.AllowGet); 

            }
        }

        public ActionResult Delete(int id)
        {
            try
            {
                using (DBModel db = new DBModel())
                {
                    Album alb = db.Albums.Where(x => x.AlbumID == id).FirstOrDefault<Album>();
                    db.Albums.Remove(alb);
                    db.SaveChanges();
                }
                return Json(new { success = true, html = GlobalClass.RenderRazorViewToString(this, "ViewAll", GetAllAlbums()), message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}