using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Data;
using WebApplication1.DataAccess.Interfaces;
using WebApplication1.Models.Domain;

namespace WebApplication1.DataAccess.Repositories
{
    public class BlogsRepository : IBlogsRepository
    {
        private readonly ApplicationDbContext _context;
        public BlogsRepository(ApplicationDbContext context )
        {
            _context = context;
        }

        public void AddBlog(Blog b)
        {
            _context.Blogs.Add(b);
            _context.SaveChanges();
        }

        public void DeleteBlog(int id)
        {
            throw new NotImplementedException();
        }

        public Blog GetBlog(int id)
        {
            return _context.Blogs.SingleOrDefault(x => x.BlogId == id);
        }

        public IQueryable<Blog> GetBlogs()
        {
            throw new NotImplementedException();
        }
    }
}
