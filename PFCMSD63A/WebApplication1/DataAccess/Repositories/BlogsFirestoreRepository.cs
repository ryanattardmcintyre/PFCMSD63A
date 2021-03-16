using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.DataAccess.Interfaces;
using WebApplication1.Models.Domain;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Microsoft.Extensions.Configuration;

namespace WebApplication1.DataAccess.Repositories
{
    public class BlogsFirestoreRepository : IBlogsRepository
    {
        FirestoreDb db;
        public BlogsFirestoreRepository(IConfiguration config)
        {
            var projId = config.GetSection("AppSettings").GetSection("ProjectId").Value;
            db = FirestoreDb.Create(projId); //we are creating a reference to the db
        }

        public void AddBlog(Blog b)
        {
            DocumentReference docRef = db.Collection("blogs").Document();
            Dictionary<string, object> blog = new Dictionary<string, object>
            {
                { "Id", b.BlogId }, //we don't have auto generated ids (recommended Ids should be guids)
                { "Url", b.Url },
                { "Title", b.Title }
            };
            docRef.SetAsync(blog).Wait();
        }

        public void DeleteBlog(int id)
        {
            Query allBlogsQuery = db.Collection("blogs").WhereEqualTo("Id", id);
            Task<QuerySnapshot> t = allBlogsQuery.GetSnapshotAsync();
            t.Wait();

            QuerySnapshot allBlogsQuerySnapshot = t.Result; //contains documents representing my blogs
            DocumentSnapshot documentSnapshot = allBlogsQuerySnapshot.Documents[0];

            DocumentReference cityRef = documentSnapshot.Reference;
            cityRef.DeleteAsync().Wait();
        }

        public Blog GetBlog(int id)
        {
            Query allBlogsQuery = db.Collection("blogs").WhereEqualTo("Id", id);
            Task<QuerySnapshot> t = allBlogsQuery.GetSnapshotAsync();
            t.Wait();

            QuerySnapshot allBlogsQuerySnapshot = t.Result; //contains documents representing my blogs
            DocumentSnapshot documentSnapshot = allBlogsQuerySnapshot.Documents[0];
           
            Dictionary<string, object> blog = documentSnapshot.ToDictionary();
            Blog myBlog = new Blog();
            myBlog.BlogId = blog.ContainsKey("Id") ? Convert.ToInt32(blog["Id"].ToString()) : 0;
            myBlog.Title = blog.ContainsKey("Title") ? blog["Title"].ToString() : "";
            myBlog.Url = blog.ContainsKey("Url") ? blog["Url"].ToString() : "";

            return myBlog;
        }

        public IQueryable<Blog> GetBlogs()
        {
            Query allBlogsQuery = db.Collection("blogs");
            Task<QuerySnapshot> t = allBlogsQuery.GetSnapshotAsync();
            t.Wait();

            QuerySnapshot allCitiesQuerySnapshot = t.Result; //contains documents representing my blogs
            List<Blog> myBlogs = new List<Blog>();
            foreach (DocumentSnapshot documentSnapshot in allCitiesQuerySnapshot.Documents)
            {
                Dictionary<string, object> blog = documentSnapshot.ToDictionary();
                Blog myBlog = new Blog();
                myBlog.BlogId = blog.ContainsKey("Id")? Convert.ToInt32(blog["Id"].ToString()): 0;
                myBlog.Title = blog.ContainsKey("Title") ? blog["Title"].ToString() : "";
                myBlog.Url = blog.ContainsKey("Url") ? blog["Url"].ToString() : "";

                myBlogs.Add(myBlog);
            }

            return myBlogs.AsQueryable();
        }

        public void UpdateBlog(Blog b)
        {
            Query allBlogsQuery = db.Collection("blogs").WhereEqualTo("Id", b.BlogId);
            Task<QuerySnapshot> t = allBlogsQuery.GetSnapshotAsync();
            t.Wait();

            QuerySnapshot allBlogsQuerySnapshot = t.Result; //contains documents representing my blogs
            DocumentSnapshot documentSnapshot = allBlogsQuerySnapshot.Documents[0];

            DocumentReference docRef = documentSnapshot.Reference;
            Dictionary<string, object> blog = new Dictionary<string, object>
            {
                { "Id", b.BlogId }, //we don't have auto generated ids (recommended Ids should be guids)
                { "Url", b.Url },
                { "Title", b.Title }
            };

            docRef.SetAsync(blog).Wait();

        }
    }
}
