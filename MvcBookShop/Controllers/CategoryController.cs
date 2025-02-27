using System;
using System.Data;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MvcBookShop.Models;
using MvcBookShop.PrimaveraWebServices;
using System.Text.Encodings.Web;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Http;


namespace MvcBookShop.Controllers
{

    public class CategoryController : Controller
    {

        public IActionResult Index(string category)
        {
            ViewData["username"] = HttpContext.Session.GetString("username");
            ViewData["CategoryTitle"] = category;

            try
            {

                dynamic books = WebServicesManager.Instance.WS05_GetSetOfBooksInCategory(category);

                int retries = 0;
                if (books == null && retries < 3)
                {
                    System.Threading.Thread.Sleep(1000);
                    retries++;
                    books = WebServicesManager.Instance.WS05_GetSetOfBooksInCategory(category);
                }

                if (books == null)
                {
                    ViewData["Books"] = new List<Book> { };
                    ViewData["Error"] = "There was an error retrieving the books of this category";
                }

                var bookList = new List<Book> { };
                foreach (dynamic x in books.DataSet.Table)
                {
                    bookList.Add(new Book() { ID = x.Artigo, Title = x.Descricao, Price = x.PVP1, Author = x.CDU_Autor, Cover = @"./images/books/" + x.Artigo + ".jpg" });
                }

                ViewData["Books"] = bookList;
                ViewData["Error"] = "";

            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught.", e);
            }

            return View();

        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]

        public IActionResult Error()
        {
            return View(
                new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                }
            );
        }
    }
}